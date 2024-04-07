using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using System;
using System.IO;
using Felipecsl.GifImageViewLibrary;
using System.Threading;
using YoutubeExplode;
using Android.Views;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;
using Android.Content.PM;
using Environment = Android.OS.Environment;
using Stream = System.IO.Stream;
using Xamarin.Essentials;
using System.Text.RegularExpressions;

namespace M43_Mobile
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        private bool running = false;
        private readonly string downloadsPath = Environment.ExternalStorageDirectory.AbsolutePath + "/" + Environment.DirectoryDownloads + "/";

        private string ConvertSafe(string value) { return Regex.Replace(value.Replace('\\', ' ').Replace('/', ' ').Replace('|', ' ').Replace('"', ' ').Replace('*', ' ').Replace('?', ' ').Replace('~', ' ').Replace('%', ' ').Replace('&', ' ').Replace('^', ' ').Replace('#', ' ').Replace('+', ' ').Replace('-', ' ').Replace('@', ' ').Replace('=', ' '), "[^A-Za-zåäö0-9()]", " "); }

        private EditText urlBox;
        private Button videoButton;
        private Button audioButton;
        private GifImageView runIcon;
        private TextView errorLabel;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            runIcon = FindViewById<GifImageView>(Resource.Id.runningIcon);

            using (Stream stream = Resources.OpenRawResource(Resource.Drawable.running_icon))
            {
                byte[] bytes = ConvertByteArray(stream);
                runIcon.SetBytes(bytes);
            }

            errorLabel = FindViewById<TextView>(Resource.Id.errorLabel);

            urlBox = FindViewById<EditText>(Resource.Id.inputBox);

            videoButton = FindViewById<Button>(Resource.Id.videoButton);
            videoButton.Click += DownloadVideo;

            audioButton = FindViewById<Button>(Resource.Id.audioButton);
            audioButton.Click += DownloadAudio;
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        #region Download Animation

        private byte[] ConvertByteArray(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];

            using MemoryStream ms = new MemoryStream();
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                ms.Write(buffer, 0, read);
            return ms.ToArray();
        }

        private void DisableAnimationThreaded()
        {
            RunOnUiThread(() =>
            {
                StopLoadingAnimation();
            });
        }

        private void StartLoadingAnimation()
        {
            errorLabel.Visibility = ViewStates.Gone;
            FindViewById<ImageView>(Resource.Id.runningIcon).Visibility = ViewStates.Visible;
            runIcon.StartAnimation();
        }

        private void StopLoadingAnimation()
        {
            errorLabel.Visibility = ViewStates.Gone;
            runIcon.StopAnimation();
            FindViewById<ImageView>(Resource.Id.runningIcon).Visibility = ViewStates.Gone;
        }

        #endregion

        #region Download Methods

        private async void DownloadVideo(object sender, EventArgs e)
        {
            if (running) return;

            running = true;
            try
            {
                string videoUrl = urlBox.Text;

                if (string.IsNullOrEmpty(videoUrl))
                {
                    running = false;
                    return;
                }

                var youtube = new YoutubeClient();
                #nullable enable
                Video? video;
                #nullable disable


                try
                {
                    video = await youtube.Videos.GetAsync(videoUrl);
                }
                catch
                {
                    running = false;
                    errorLabel.Text = $"Couldn't find the video";
                    errorLabel.Visibility = ViewStates.Visible;
                    return;
                }

                StartLoadingAnimation();

                errorLabel.Text = $"Downloading Video";
                errorLabel.Visibility = ViewStates.Visible;

                Thread thread = new Thread(async () =>
                {
                    var title = "(Video) " + video.Title;
                    title = ConvertSafe(title);

                    var streamInfoSet = await youtube.Videos.Streams.GetManifestAsync(videoUrl);
                    var videoStreamInfo = streamInfoSet.GetMuxedStreams().TryGetWithHighestVideoQuality();

                    if (videoStreamInfo == null)
                    {
                        DisableAnimationThreaded();
                        running = false;
                        return;
                    }

                    var videoStream = await youtube.Videos.Streams.GetAsync(videoStreamInfo);

                    var filePath = downloadsPath + title + ".mp4";
                    filePath = GenerateUniqueFilePath(filePath);

                    using (var fileStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write))
                    {
                        var buffer = new byte[4096];
                        int bytesRead;

                        while ((bytesRead = await videoStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                        }
                    }

                    DisableAnimationThreaded();
                    running = false;

                    SetLabel($"Download Finished (Check your Downloads folder)");
                });
                thread.Start();
            }
            catch (Exception ex)
            {
                SetLabel($"Error while downloading MP4: {ex.Message}");
                StopLoadingAnimation();
                running = false;
            }
        }

        private async void DownloadAudio(object sender, EventArgs e)
        {
            if (running) return;

            running = true;

            try
            {
                string videoUrl = urlBox.Text;

                if (string.IsNullOrEmpty(videoUrl))
                {
                    running = false;
                    return;
                }

                var youtube = new YoutubeClient();
                #nullable enable
                Video? video;
                #nullable disable

                try
                {
                    video = await youtube.Videos.GetAsync(videoUrl);
                }
                catch
                {
                    running = false;
                    errorLabel.Text = $"Couldn't find the video";
                    errorLabel.Visibility = ViewStates.Visible;
                    return;
                }

                StartLoadingAnimation();

                errorLabel.Text = $"Downloading Audio";
                errorLabel.Visibility = ViewStates.Visible;

                Thread thread = new Thread(async () =>
                {
                    var title = "(Audio) " + video.Title;
                    title = ConvertSafe(title);

                    var manifest = await youtube.Videos.Streams.GetManifestAsync(videoUrl);

                    var info = manifest.GetAudioOnlyStreams().TryGetWithHighestBitrate();

                    if (info == null)
                    {
                        DisableAnimationThreaded();
                        running = false;
                        return;
                    }

                    var audioStream = await youtube.Videos.Streams.GetAsync(info);

                    var filePath = GenerateUniqueFilePath(downloadsPath + title + ".webm");

                    await youtube.Videos.Streams.DownloadAsync(info, filePath);

                    #region Conversion Removed

                    // End method here if we only want to download the file
                    //if (!convertCheckBox.Checked)
                    //{
                    //    DisableAnimationThreaded();
                    //    running = false;
                    //    SetLabel("Download Finished (Check your Downloads folder)");
                    //    return;
                    //}

                    //if (!File.Exists(filePath))
                    //{
                    //    SetLabel("Couldn't detect temporary audio cache");
                    //    DisableAnimationThreaded();
                    //    running = false;
                    //    return;
                    //}

                    // Convert to MP3 format

                    //SetLabel("Converting audio file");

                    //try
                    //{
                    //    FFmpeg.SetExecutablesPath(FileSystem.AppDataDirectory);
                    //    await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Android, FileSystem.AppDataDirectory);

                    //    await SetExecutePermsFFmpeg();
                    //}
                    //catch
                    //{
                    //    File.Delete(filePath);
                    //    DisableAnimationThreaded();
                    //    SetLabel("Couldn't get ffmpeg");
                    //    running = false;
                    //    return;
                    //}


                    //var e = FFmpeg.GetMediaInfo(filePath);

                    //var convPath = GenerateUniqueFilePath(downloadsPath + title + ".mp3");

                    //try
                    //{
                    //    var conversion = FFmpeg.Conversions.New();
                    //    conversion.OnDataReceived += (sender, e) =>
                    //    {
                    //        SetLabel("");
                    //    };

                    //    await conversion.Start($@"-i {filePath} -f libmp3lame {convPath}").ConfigureAwait(false);

                    //    if (File.Exists(convPath))
                    //    {
                    //        File.Delete(filePath);
                    //        DisableAnimationThreaded();
                    //        SetLabel($"Failed to convert (FFmpeg failed)");
                    //        running = false;
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    File.Delete(filePath);
                    //    DisableAnimationThreaded();
                    //    SetLabel($"Failed to convert ({ex})");
                    //    running = false;
                    //}


                    //File.Delete(filePath);

                    #endregion

                    DisableAnimationThreaded();
                    running = false;
                    SetLabel($"Download Finished (Check your Downloads folder)");
                });
                thread.Start();
            }
            catch (Exception ex)
            {
                errorLabel.Text = $"Error while downloading Audio: {ex.Message}";
                errorLabel.Visibility = ViewStates.Visible;
                StopLoadingAnimation();
                running = false;
            }
        }

        #endregion

        private string GenerateUniqueFilePath(string originalFilePath)
        {
            #nullable enable
            string? directory = Path.GetDirectoryName(originalFilePath);
            #nullable disable

            if (string.IsNullOrEmpty(directory)) return originalFilePath;

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFilePath);
            string fileExtension = Path.GetExtension(originalFilePath);

            int counter = 1;
            string newFilePath = originalFilePath;

            while (File.Exists(newFilePath))
            {
                string newFileName = $"{fileNameWithoutExtension} {counter}{fileExtension}";
                newFilePath = Path.Combine(directory, newFileName);
                counter++;
            }

            return newFilePath;
        }

        //private Task SetExecutePermsFFmpeg()
        //{
        //    var filePath = FileSystem.AppDataDirectory;

        //    Java.IO.File ffmpeg = new Java.IO.File(filePath + "/ffmpeg");
        //    Java.IO.File ffprobe = new Java.IO.File(filePath + "/ffprobe");

        //    ffmpeg.SetExecutable(true, false);
        //    ffprobe.SetExecutable(true, false);

        //    return Task.CompletedTask;
        //}

        private void SetLabel(string message)
        {
            RunOnUiThread(() =>
            {
                errorLabel.Text = message;
                errorLabel.Visibility = ViewStates.Visible;
            });
        }
    }
}