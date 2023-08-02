using System; 
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;  
using Youtube_Videos_Herrunterladen;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;
using System.Drawing.Imaging;
using System.Collections.Generic;
using Youtube_Videos_Herrunterladen.Properties;
using System.Runtime.InteropServices;

namespace Youtube_Videos_Herrunterladen
{
    public partial class MainForm : Form
    {
        private readonly Utilityclass utilityclass;
        public YoutubeClient youtube = new YoutubeClient();  // Create a new YoutubeClient to interact with the YouTube service
        public static string username = Environment.UserName;  // Get the username of the current user
        public string selectedFolderPath = $@"C:\Users\{username}\Downloads\"; // Set the default download location to the current user's Downloads folder
        public string tempFolderPath = $@"C:\Users\{username}\AppData\Local\Temp";
        public InfoForm infoForm = new InfoForm();  // Define the infoForm outside the method.
        public HistoryForm historyForm;
        public Uri uri;
        public string streamId;
        public Video stream;
        public StreamManifest streamManifest;
        public IStreamInfo audioStreamInfo;
        public IStreamInfo videoStreamInfo;
        public Image image;
        readonly System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer() // Create a timer to trigger events at regular intervals
        {
            Interval = 500 // Set the interval of the timer to 500ms (0.5 seconds)
        };

        public MainForm()
        {
            InitializeComponent();  
            // Create an instance of the Utulityclass
            utilityclass = new Utilityclass(this, mp4QualityComboBox, linkBox, idLb, uploadDateLb, mp4SizeLb, mp3SizeLb, titleLb, durationLb, channelLb, downloadSpeedLb);

            // Create an instance of the HistoryForm
            historyForm = new HistoryForm(this, utilityclass);

            // Fokus auf das Hauptformular setzen, um zu verhindern, dass sich der Fokus automatisch auf die linkBox TextBox legt
            this.Activated += (sender, e) => utilityclass.BringToFrontWithoutFocus(historyForm, this); // Abonniere das Activated Event

            ToggleControlsSecurity(false);

            // Add evry label to the pic box so they get transparent
            this.Controls.Add(mainShadow);
            this.BackColor = Color.FromArgb(0xDA, 0x0F, 0x0F);
            mainShadow.Controls.Add(channelLb);
            mainShadow.Controls.Add(currentSizeLb);
            mainShadow.Controls.Add(downloadSpeedLb);
            mainShadow.Controls.Add(durationLb);
            mainShadow.Controls.Add(idLb);
            mainShadow.Controls.Add(infoLb);
            mainShadow.Controls.Add(historyBt);
            mainShadow.Controls.Add(mp3SizeLb);
            mainShadow.Controls.Add(mp4QualityLb);
            mainShadow.Controls.Add(mp4SizeLb);
            mainShadow.Controls.Add(showInfoFormBt);
            mainShadow.Controls.Add(subLb1);
            mainShadow.Controls.Add(subLb2);
            mainShadow.Controls.Add(titleLb);
            mainShadow.Controls.Add(uploadDateLb);
            mainShadow.Controls.Add(usbSticksPanel);
            
            // Updates the stats
            linkBox.TextChanged += LinkBox_TextChanged;  // Register the text changed event handler for the linkBox control
            mp4QualityComboBox.TextChanged += ComboBox_TextChanged;

            // Create an instance of the Utulityclass
            utilityclass = new Utilityclass(this, mp4QualityComboBox, linkBox, idLb, uploadDateLb, mp4SizeLb, mp3SizeLb, titleLb, durationLb, channelLb, downloadSpeedLb);

            // Looks for USB Sticks evry second 
            UsbManager usbManager = new UsbManager(this, usbSticksPanel, subLb1);  // Instantiate UsbManager to manage USB devices
            timer.Tick += new EventHandler(usbManager.UpdateUsbLabels);  // Register the Tick event to update the USB labels
            timer.Start();  // Start the timer
            subLb1.Text = "Speicherort: " + selectedFolderPath;  // Set the initial text of subLb1 to the selected folder path
        }

        // This method handles the click event for the DownloadMp3Bt Button
        // It starts the process of downloading audio from YouTube in an asynchronous manner
        private async void DownloadMp3Bt_Click(object sender, EventArgs e)
        {
            ToggleControlsDownload(false);
            AudioDownloader audioDownloader = new AudioDownloader(utilityclass, this, selectedFolderPath, currentSizeLb, progressBar, tempFolderPath, downloadSpeedLb);  // Instantiate AudioDownloader to download audio from YouTube
            await audioDownloader.DownloadAudioAsync();  // Asynchronously download the audio file
            linkBox.Text = "Link: (z.B. https://www.youtube.com/watch?v=6WRLynWxVKg)";
            linkBox.ForeColor = Color.Gray;
            BackgroundImage = Resources.defaultBackground;
            ToggleControlsDownload(true);
            ToggleControlsSecurity(false);
        }
        // This method handles the click event for the DownloadMp4Bt Button
        // It starts the process of downloading video from YouTube in an asynchronous manner
        private async void DownloadMp4Bt_Click(object sender, EventArgs e)
        {
            ToggleControlsDownload(false);
            VideoDownloader VideoDownloader = new VideoDownloader(utilityclass, this, currentSizeLb, progressBar, selectedFolderPath, tempFolderPath, mp4QualityComboBox, downloadSpeedLb);  // Instantiate VideoDownloader to download video from YouTube
            await VideoDownloader.DownloadVideoAsync();  // Asynchronously download the video file
            linkBox.Text = "Link: (z.B. https://www.youtube.com/watch?v=6WRLynWxVKg)";
            linkBox.ForeColor = Color.Gray;
            BackgroundImage = Resources.defaultBackground;
            ToggleControlsDownload(true);
            ToggleControlsSecurity(false);
        }

        private async void LinkBox_TextChanged(object sender, EventArgs e)
        {
            // When a new link i enterd, diably controls becuase youtube explode needs to load all the informarion
            await utilityclass.GetStreamInfos();
            mp4QualityComboBox.Items.Clear();
            StatsUpdater statsUpdater = new StatsUpdater(utilityclass, this, titleLb, durationLb, mp4SizeLb, mp3SizeLb, channelLb, idLb, uploadDateLb, mp4QualityComboBox);
            await statsUpdater.UpdateVideoStatsAsync();  // Asynchronously update the video stats

        }
        private async void ComboBox_TextChanged(object sender, EventArgs e)
        {
            await utilityclass.GetStreamInfos();
            VideoAndAudioSizeUpdater videoAndAudioSizeUpdater = new VideoAndAudioSizeUpdater(utilityclass, this, mp4QualityComboBox, mp4SizeLb, mp3SizeLb);
            videoAndAudioSizeUpdater.UpdateVideoAndAudioSize();
        }

        // This method toggles the Enabled status of various controls on the form
        private void ToggleControlsDownload(bool isEnabled)
        {
            linkBox.ReadOnly = !isEnabled;  // Toggle the ReadOnly status of the TextBox
            downloadMp4Bt.Enabled = isEnabled;  // Toggle the Enabled status of the Download Mp4 Button
            downloadMp3Bt.Enabled = isEnabled;  // Toggle the Enabled status of the Download Mp3 Button
            selectFolderBt.Enabled = isEnabled;  // Toggle the Enabled status of the Select Folder Button
            timer.Enabled = isEnabled;  // Toggle the Enabled status of the Timer
            mp4QualityComboBox.Enabled = isEnabled;

            foreach (Label label in usbSticksPanel.Controls.OfType<Label>())  // Toggle the Enabled status of the Labels in the usbSticksPanel
            {
                label.Enabled = isEnabled;  // Toggle the Enabled status of each Label
            }
        }

        public void ToggleControlsSecurity(bool isEnabled)
        {
            downloadMp4Bt.Enabled = isEnabled;  // Toggle the Enabled status of the Download Mp4 Button
            downloadMp3Bt.Enabled = isEnabled;  // Toggle the Enabled status of the Download Mp3 Button
            mp4QualityComboBox.Enabled = isEnabled;
        }

        // This method handles the click event for the selectFolderButton
        // It opens a FolderBrowserDialog to allow the user to select a download folder
        private void SelectFolderButton_Click(object sender, EventArgs e)
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

        public void LinkBox_Enter(object sender, EventArgs e)
        {
            // Wenn die TextBox den Fokus erhält, und der Text noch das Wasserzeichen ist, leeren wir den Text und setzen die Schriftfarbe auf Schwarz.
            if (linkBox.Text == "Link: (z.B. https://www.youtube.com/watch?v=6WRLynWxVKg)")
            {
                linkBox.Text = "";
                linkBox.ForeColor = Color.Black;
            }
        }

        public void LinkBox_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(linkBox.Text))
            {
                linkBox.Text = "Link: (z.B. https://www.youtube.com/watch?v=6WRLynWxVKg)";
                linkBox.ForeColor = Color.Gray;
            }
        }

        private void showInfoFormBt_Click(object sender, EventArgs e)
        {
            infoForm.Show();
        }

        private void historyBt_Click(object sender, EventArgs e)
        {
            // Überprüfen Sie den Sichtbarkeitszustand der HistoryForm
            if (historyForm.Visible)
            {
                // Wenn die Form sichtbar ist, machen Sie sie unsichtbar und deabonnieren Sie das Move-Ereignis der Hauptform
                historyForm.Hide();
                this.Move -= Main_Move;
            }
            else
            {
                historyForm.Show();
                // Wenn die Form unsichtbar ist, machen Sie sie sichtbar, platzieren Sie sie unter der Hauptform und abonnieren Sie das Move-Ereignis der Hauptform
                historyForm.Location = new Point(this.Location.X, this.Location.Y + this.Height);
                historyForm.Width = this.Width; // Setzen Sie die Breite der HistoryForm auf die Breite der Hauptform
                this.Move += Main_Move;
                this.Resize += Main_Move;
            }
        }

        private void Main_Move(object sender, EventArgs e)
        {
            if (historyForm.Visible)
            {
                // Aktualisieren Sie die Position der HistoryForm, wenn sich die Hauptform bewegt
                historyForm.Location = new Point(this.Location.X, this.Location.Y + this.Height);
                // Aktualisieren Sie die Breite der HistoryForm, wenn die MainForm ihre Größe ändert
                historyForm.Width = this.Width;
            }
        }
    }
}
//Wenn die audio convertiert wird, hällt das ganze programm wegen prozess.waitforexit an. Das ändern :)
//Timer für die USB Sticks ausbauen und richtige lösung suchen
//Ganze Playlists aufeinmal downloaden
//Update Progress in die Utilyty klasse einfügen
//Statsupdater in Utilityclass einfügen
//Form größe und layout an thumbnail anpassen