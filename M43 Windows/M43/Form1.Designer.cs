namespace M43
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            urlBox = new TextBox();
            mp4Button = new Button();
            mp3Button = new Button();
            progressBar = new ProgressBar();
            hideTemp = new CheckBox();
            SuspendLayout();
            // 
            // urlBox
            // 
            urlBox.BackColor = Color.FromArgb(30, 30, 30);
            urlBox.BorderStyle = BorderStyle.FixedSingle;
            urlBox.Dock = DockStyle.Top;
            urlBox.Font = new Font("Microsoft Sans Serif", 7.20000029F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point);
            urlBox.ForeColor = SystemColors.WindowFrame;
            urlBox.Location = new Point(0, 0);
            urlBox.Name = "urlBox";
            urlBox.PlaceholderText = "https://www.youtube.com/";
            urlBox.Size = new Size(394, 21);
            urlBox.TabIndex = 0;
            // 
            // mp4Button
            // 
            mp4Button.BackColor = Color.FromArgb(30, 30, 30);
            mp4Button.FlatAppearance.BorderColor = Color.Black;
            mp4Button.FlatAppearance.BorderSize = 0;
            mp4Button.FlatStyle = FlatStyle.Flat;
            mp4Button.Font = new Font("Microsoft New Tai Lue", 13.8F, FontStyle.Regular, GraphicsUnit.Point);
            mp4Button.ForeColor = Color.Gray;
            mp4Button.Location = new Point(0, 36);
            mp4Button.Name = "mp4Button";
            mp4Button.Size = new Size(351, 45);
            mp4Button.TabIndex = 1;
            mp4Button.Text = "Download Video [ MP4 ]";
            mp4Button.TextAlign = ContentAlignment.MiddleLeft;
            mp4Button.UseVisualStyleBackColor = false;
            mp4Button.Click += mp4Button_Click;
            // 
            // mp3Button
            // 
            mp3Button.BackColor = Color.FromArgb(30, 30, 30);
            mp3Button.FlatAppearance.BorderColor = Color.Black;
            mp3Button.FlatAppearance.BorderSize = 0;
            mp3Button.FlatStyle = FlatStyle.Flat;
            mp3Button.Font = new Font("Microsoft New Tai Lue", 12F, FontStyle.Regular, GraphicsUnit.Point);
            mp3Button.ForeColor = Color.Gray;
            mp3Button.Location = new Point(0, 87);
            mp3Button.Name = "mp3Button";
            mp3Button.Size = new Size(247, 50);
            mp3Button.TabIndex = 2;
            mp3Button.Text = "Download Audio [ MP3 ]";
            mp3Button.TextAlign = ContentAlignment.MiddleLeft;
            mp3Button.UseVisualStyleBackColor = false;
            mp3Button.Click += mp3Button_Click;
            // 
            // progressBar
            // 
            progressBar.Dock = DockStyle.Bottom;
            progressBar.ForeColor = SystemColors.ControlDark;
            progressBar.Location = new Point(0, 145);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(394, 15);
            progressBar.Step = 1;
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.TabIndex = 0;
            progressBar.Visible = false;
            // 
            // hideTemp
            // 
            hideTemp.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            hideTemp.Checked = true;
            hideTemp.CheckState = CheckState.Checked;
            hideTemp.FlatAppearance.BorderSize = 0;
            hideTemp.ForeColor = Color.Gray;
            hideTemp.Location = new Point(253, 100);
            hideTemp.Name = "hideTemp";
            hideTemp.Size = new Size(141, 30);
            hideTemp.TabIndex = 3;
            hideTemp.Text = "Hide Temp file";
            hideTemp.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Desktop;
            ClientSize = new Size(394, 160);
            Controls.Add(hideTemp);
            Controls.Add(progressBar);
            Controls.Add(mp3Button);
            Controls.Add(mp4Button);
            Controls.Add(urlBox);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "M43";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox urlBox;
        private Button mp4Button;
        private Button mp3Button;
        private ProgressBar progressBar;
        private CheckBox hideTemp;
    }
}