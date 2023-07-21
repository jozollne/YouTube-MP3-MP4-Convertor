using System;  // Import the System namespace
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;  // Import the System.Windows.Forms namespace for creating Windows user interface
using TagLib;
using Youtube_Videos_Herrunterladen;
using Youtube_Videos_Herrunterladen.Properties;
using YoutubeExplode;  // Import the YoutubeExplode library for interacting with YouTube

namespace downloader
{
    public partial class Main : Form
    {
        public YoutubeClient youtube = new YoutubeClient();  // Create a new YoutubeClient to interact with the YouTube service
        //CancellationTokenSource cts = new CancellationTokenSource();
        public static string username = Environment.UserName;  // Get the username of the current user
        public string selectedFolderPath = $@"C:\Users\{username}\Downloads\"; // Set the default download location to the current user's Downloads folder
        private string tempFolderPath = $@"C:\Users\{username}\AppData\Local\Temp";
        public InfoForm infoForm = new InfoForm();  // Definieren Sie das infoForm außerhalb der Methode.


        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer()
        {
            Interval = 500 // Set the interval of the timer to 500ms (0.5 seconds)
        };

        public Main()
        {
            InitializeComponent();  // Initialize the components of the Form

            historyBox.ScrollBars = ScrollBars.Both;

            showInfoFormBt.FlatAppearance.BorderSize = 0;
            showInfoFormBt.BackColor = Color.Transparent;
            showInfoFormBt.FlatAppearance.BorderSize = 0;
            showInfoFormBt.FlatAppearance.MouseDownBackColor = Color.Transparent;
            showInfoFormBt.FlatAppearance.MouseOverBackColor = Color.Transparent;
            showInfoFormBt.FlatStyle = FlatStyle.Flat;
            showInfoFormBt.UseVisualStyleBackColor = false;  // Stellen Sie sicher, dass der Button immer transparent ist

            linkBox.TextChanged += LinkBox_TextChanged;  // Register the text changed event handler for the linkBox control

            UsbManager usbManager = new UsbManager(this, usbSticksPanel, subLb1, selectedFolderPath);  // Instantiate UsbManager to manage USB devices
            timer.Tick += new EventHandler(usbManager.UpdateUsbLabels);  // Register the Tick event to update the USB labels
            timer.Start();  // Start the timer

            subLb1.Text = "Speicherort: " + selectedFolderPath;  // Set the initial text of subLb1 to the selected folder path
        }

        private async void LinkBox_TextChanged(object sender, EventArgs e)
        {
            StatsUpdater statsUpdater = new StatsUpdater(linkBox, titelLb, durationLb, mp4SizeLb, mp3SizeLb, mp4QualityLb, thumbnailPicBox, chanelLb, idLb, uploadDateLb);  // Instantiate StatsUpdater to manage YouTube video stats
            await statsUpdater.UpdateVideoStatsAsync();  // Asynchronously update the video stats
        }
        private void cancelDownloadBt_Click(object sender, EventArgs e)
        {
            //cts.Cancel();
        }

        private async void DownloadMp3Bt_Click(object sender, EventArgs e)
        {
            ToggleControls(false);
            AudioDownloader audioDownloader = new AudioDownloader(this, linkBox, selectedFolderPath, currentSizeLb, progressBar, historyBox, tempFolderPath);  // Instantiate AudioDownloader to download audio from YouTube
            await audioDownloader.DownloadAudioAsync();  // Asynchronously download the audio file
            linkBox.Text = "";
            ToggleControls(true);
        }


        private async void DownloadMp4Bt_Click(object sender, EventArgs e)
        {
            ToggleControls(false);
            VideoDownloader VideoDownloader = new VideoDownloader(this, linkBox, currentSizeLb, progressBar, selectedFolderPath, tempFolderPath, historyBox);  // Instantiate VideoDownloader to download video from YouTube
            await VideoDownloader.DownloadVideoAsync(/*cts.Token*/);  // Asynchronously download the video file
            linkBox.Text = "";
            ToggleControls(true);
        }

        public void SetSelectedFolderPath(string path)
        {
            selectedFolderPath = path;  // Assign the given path to the selectedFolderPath variable
        }

        public void ToggleControls(bool isEnabled)
        {
            linkBox.ReadOnly = !isEnabled;  // Toggle the ReadOnly status of the TextBox
            downloadMp4Bt.Enabled = isEnabled;  // Toggle the Enabled status of the Download Mp4 Button
            downloadMp3Bt.Enabled = isEnabled;  // Toggle the Enabled status of the Download Mp3 Button
            selectFolderButton.Enabled = isEnabled;  // Toggle the Enabled status of the Select Folder Button
            timer.Enabled = isEnabled;  // Toggle the Enabled status of the Timer

            // Toggle the Enabled status of the Labels
            foreach (Label label in usbSticksPanel.Controls.OfType<Label>())
            {
                label.Enabled = isEnabled;  // Toggle the Enabled status of each Label
            }
        }



        public string FormatBytes(long bytes)
        {
            const int scale = 1024;  // Define the scale for formatting the bytes
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };  // Define the suffixes for each byte scale

            int suffixIndex = 0;  // Initialize the suffix index
            decimal number = bytes;  // Store the byte count as a decimal for formatting

            // While the number of bytes is greater than or equal to the scale and there is another suffix
            while (number >= scale && suffixIndex < suffixes.Length - 1)
            {
                number /= scale;  // Divide the number by the scale
                suffixIndex++;  // Increment the suffix index
            }

            return $"{number:n2} {suffixes[suffixIndex]}";  // Return the formatted number and corresponding suffix
        }

        private void selectFolderButton_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())  // Create a new FolderBrowserDialog
            {
                DialogResult result = fbd.ShowDialog();  // Show the FolderBrowserDialog and store the result

                // If the user clicked OK and the selected path is not null or whitespace
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    selectedFolderPath = fbd.SelectedPath;  // Update the selectedFolderPath to the selected path
                    subLb1.Text = "Speicherort: " + selectedFolderPath;  // Update the subLb1 text to the selected folder path
                }
            }
        }

        public void GetMp3FormatAndConvert(string tempAudioFilePath, string finalAudioFilePath)
        {
            using (FileStream fs = new FileStream(tempAudioFilePath, FileMode.Open, FileAccess.Read))
            {
                byte[] bytes = new byte[12];
                fs.Read(bytes, 0, 12);
                string hex = BitConverter.ToString(bytes).Replace("-", "").ToUpper();
                string format;

                if (hex.StartsWith("494433") || hex.StartsWith("FFFB"))
                    format = ".mp3";
                else if (hex.StartsWith("52494646"))
                    format = ".wav";
                else if (hex.StartsWith("4F676753"))
                    format = ".ogg";
                else if (hex.StartsWith("664C6143"))
                    format = ".flac";
                else if (hex.StartsWith("0000001C667479704D3441"))
                    format = ".m4a";
                else if (hex.StartsWith("1A45DFA3"))
                    format = ".webm";
                else
                    format = "Dateiformat konnte nicht erkannt werden";

                infoForm.infoBox.AppendText("Das Format des Audiostrerams ist: " + format + Environment.NewLine);


                if (format != ".mp3")
                {
                    infoForm.infoBox.AppendText("Das Format des Audiostrems wird von: " + format + " in: .mp3 umgewandelt" + Environment.NewLine);
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "ffmpeg.exe",
                            Arguments = $"-i \"{tempAudioFilePath}\" -vn -ar 44100 -ac 2 -b:a 192k \"{finalAudioFilePath}\"",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        }
                    };
                    process.Start();
                    process.WaitForExit();
                    infoForm.infoBox.AppendText("Das Format wurde von: " + format + " in: .mp3 umgewandelt" + Environment.NewLine);
                }
            }
        }

        private void showInfoFormBt_Click(object sender, EventArgs e)
        {
            infoForm.Show();
        }
    }
}

//quallität anpassbar machen
//Schauenn welche Convertierung des Audiostreams schneller ist. Die vom VideoDownloader.cs oder AudioDownloader.cs