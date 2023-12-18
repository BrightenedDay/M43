using System.Runtime.InteropServices;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using MediaToolkit;
using MediaToolkit.Model;
using Microsoft.Win32;
using YoutubeExplode.Videos;

namespace M43
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            int res = 0;
            try
            {
                var o = Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "AppsUseLightTheme", -1);

                if (o != null)
                    res = (int)o;
                else
                    res = 3;
            }
            catch { }

            if (res != 1 && res != 3)
                UseImmersiveDarkMode(Handle, true);
            else
            {

                BackColor = SystemColors.Control;
                urlBox.BackColor = Color.White;
                mp4Button.BackColor = Color.White;
                mp3Button.BackColor = Color.White;
            }
        }

        #region Window Color

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        private static bool UseImmersiveDarkMode(IntPtr handle, bool enabled)
        {
            if (IsWindows10OrGreater(17763))
            {
                var attribute = DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;
                if (IsWindows10OrGreater(18985))
                {
                    attribute = DWMWA_USE_IMMERSIVE_DARK_MODE;
                }

                int useImmersiveDarkMode = enabled ? 1 : 0;
                return DwmSetWindowAttribute(handle, attribute, ref useImmersiveDarkMode, sizeof(int)) == 0;
            }

            return false;
        }

        private static bool IsWindows10OrGreater(int build = -1)
        {
            return Environment.OSVersion.Version.Major >= 10 && Environment.OSVersion.Version.Build >= build;
        }

        #endregion

        private bool running = false;
        string downloadsPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads\\";

        string convertsafe(string value) { return value.Replace('\\', ' ').Replace('/', ' ').Replace('|', ' ').Replace('"', ' ').Replace('*', ' ').Replace('?', ' '); }

        private async void mp4Button_Click(object sender, EventArgs e)
        {
            if (running) return;
            try
            {
                running = true;
                var videoUrl = urlBox.Text;

                if (string.IsNullOrEmpty(videoUrl))
                {
                    running = false;
                    return;
                }

                var youtube = new YoutubeClient();
                Video? video;

                try
                {
                    video = await youtube.Videos.GetAsync(videoUrl);
                }
                catch
                {
                    running = false;
                    return;
                }


                progressBar.Visible = true;

                Thread thread = new Thread(async () =>
                {
                    var title = "(Video) " + video.Title;
                    title = convertsafe(title);

                    var streamInfoSet = await youtube.Videos.Streams.GetManifestAsync(videoUrl);
                    var videoStreamInfo = streamInfoSet.GetMuxedStreams().TryGetWithHighestVideoQuality();

                    if (videoStreamInfo == null)
                    {
                        DisableProgressBar();
                        running = false;
                        return;
                    }

                    var videoStream = await youtube.Videos.Streams.GetAsync(videoStreamInfo);

                    var filePath = downloadsPath + title + ".mp4";
                    filePath = GenerateUniqueFilePath(filePath);

                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        var buffer = new byte[4096];
                        int bytesRead;

                        while ((bytesRead = await videoStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                        }
                    }

                    DisableProgressBar();
                    running = false;
                });
                thread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while downloading MP4: " + ex.Message);
                progressBar.Visible = false;
                running = false;
            }
        }

        private async void mp3Button_Click(object sender, EventArgs e)
        {
            if (running) return;
            try
            {
                running = true;
                var videoUrl = urlBox.Text;

                if (string.IsNullOrEmpty(videoUrl))
                {
                    running = false;
                    return;
                }

                var youtube = new YoutubeClient();
                Video? video;

                try
                {
                    video = await youtube.Videos.GetAsync(videoUrl);
                }
                catch
                {
                    running = false;
                    return;
                }

                progressBar.Visible = true;

                Thread thread = new Thread(async () =>
                {
                    var title = "(Audio) " + video.Title;
                    title = convertsafe(title);

                    var manifest = await youtube.Videos.Streams.GetManifestAsync(videoUrl);

                    var info = manifest.GetAudioOnlyStreams().TryGetWithHighestBitrate();

                    if (info == null) { DisableProgressBar(); running = false; return; }


                    var audioStream = await youtube.Videos.Streams.GetAsync(info);

                    var filePath = downloadsPath + "audio_download.temp";
                    filePath = GenerateUniqueFilePath(filePath);

                    await youtube.Videos.Streams.DownloadAsync(info, filePath);

                    if (hideTemp.Checked)
                        File.SetAttributes(filePath, FileAttributes.Normal | FileAttributes.Hidden);

                    var fileIn = new MediaFile { Filename = filePath };
                    var fileOut = new MediaFile { Filename = GenerateUniqueFilePath(downloadsPath + title + ".mp3") };

                    using (var engine = new Engine())
                    {
                        engine.GetMetadata(fileIn);

                        engine.Convert(fileIn, fileOut);
                    }

                    File.Delete(filePath);

                    DisableProgressBar();
                    running = false;
                });
                thread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while downloading MP3: " + ex.Message);
                progressBar.Visible = false;
                running = false;
            }
        }

        private void DisableProgressBar()
        {
            if (InvokeRequired)
                Invoke(new Action(() => { progressBar.Visible = false; }));
            else
                progressBar.Visible = true;
        }

        private string GenerateUniqueFilePath(string originalFilePath)
        {
            string? directory = Path.GetDirectoryName(originalFilePath);

            if (directory == null) return originalFilePath;

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFilePath);
            string fileExtension = Path.GetExtension(originalFilePath);

            int counter = 1;
            string newFilePath = originalFilePath;

            while (File.Exists(newFilePath))
            {
                string newFileName = $"{fileNameWithoutExtension} ({counter}){fileExtension}";
                newFilePath = Path.Combine(directory, newFileName);
                counter++;
            }

            return newFilePath;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ToolTip toolTip1 = new ToolTip();

            toolTip1.AutoPopDelay = 10000;
            toolTip1.InitialDelay = 1000;
            toolTip1.ReshowDelay = 500;
            toolTip1.ShowAlways = true;

            toolTip1.SetToolTip(hideTemp, "(Doesn't affect download)\nHides the temporary file that gets generated\n(this file gets deleted after it is finished using it).");
        }
    }
}