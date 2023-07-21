using System;  // Import the System namespace
using System.Drawing;
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


        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer()
        {
            Interval = 500 // Set the interval of the timer to 500ms (0.5 seconds)
        };

        public Main()
        {
            InitializeComponent();  // Initialize the components of the Form

            button1.FlatAppearance.BorderSize = 0;
            button1.BackColor = Color.Transparent;
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatAppearance.MouseDownBackColor = Color.Transparent;
            button1.FlatAppearance.MouseOverBackColor = Color.Transparent;
            button1.FlatStyle = FlatStyle.Flat;
            button1.UseVisualStyleBackColor = false;  // Stellen Sie sicher, dass der Button immer transparent ist

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
            AudioDownloader audioDownloader = new AudioDownloader(linkBox, selectedFolderPath, currentSizeLb, progressBar, historyBox, tempFolderPath);  // Instantiate AudioDownloader to download audio from YouTube
            await audioDownloader.DownloadAudioAsync();  // Asynchronously download the audio file
            linkBox.Text = "";
            ToggleControls(true);
        }

        private async void DownloadMp4Bt_Click(object sender, EventArgs e)
        {
            ToggleControls(false);
            VideoDownloader VideoDownloader = new VideoDownloader(linkBox, currentSizeLb, progressBar, selectedFolderPath, tempFolderPath, historyBox);  // Instantiate VideoDownloader to download video from YouTube
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

        private void button1_Click_1(object sender, EventArgs e)
        {
            InfoForm infoForm = new InfoForm();
            infoForm.Show();
        }
    }
}

//60gb Video ffmpeg wurde nicht beenet und läuft nach 100% im Taskamanager weiter
//status meldung label mit fehlern und erflogreichen download nachrichten
//quallität anpassbar machen
