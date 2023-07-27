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

namespace Youtube_Videos_Herrunterladen
{
    public partial class Main : Form
    {
        readonly Utilityclass utilityclass;
        public YoutubeClient youtube = new YoutubeClient();  // Create a new YoutubeClient to interact with the YouTube service
        public static string username = Environment.UserName;  // Get the username of the current user
        public string selectedFolderPath = $@"C:\Users\{username}\Downloads\"; // Set the default download location to the current user's Downloads folder
        public string tempFolderPath = $@"C:\Users\{username}\AppData\Local\Temp";
        public InfoForm infoForm = new InfoForm();  // Define the infoForm outside the method.
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

        public Main()
        {
            InitializeComponent();  // Initialize the components of the Form

            utilityclass = new Utilityclass(this, mp4QualityComboBox, linkBox, idLb, uploadDateLb, mp4SizeLb, mp3SizeLb, thumbnailPicBox, titleLb, durationLb, channelLb);

            historyBox.ScrollBars = ScrollBars.Both;

            // Configure showInfoFormBt Button to be transparent
            showInfoFormBt.FlatAppearance.BorderSize = 0;
            showInfoFormBt.BackColor = Color.Transparent;
            showInfoFormBt.FlatAppearance.BorderSize = 0;
            showInfoFormBt.FlatAppearance.MouseDownBackColor = Color.Transparent;
            showInfoFormBt.FlatAppearance.MouseOverBackColor = Color.Transparent;
            showInfoFormBt.FlatStyle = FlatStyle.Flat;
            showInfoFormBt.UseVisualStyleBackColor = false;  // Ensure the Button is always transparent

            linkBox.TextChanged += LinkBox_TextChanged;  // Register the text changed event handler for the linkBox control
            mp4QualityComboBox.TextChanged += ComboBox_TextChanged;

            UsbManager usbManager = new UsbManager(this, usbSticksPanel, subLb1);  // Instantiate UsbManager to manage USB devices
            timer.Tick += new EventHandler(usbManager.UpdateUsbLabels);  // Register the Tick event to update the USB labels
            timer.Start();  // Start the timer
            subLb1.Text = "Speicherort: " + selectedFolderPath;  // Set the initial text of subLb1 to the selected folder path
        }

        // This method handles the click event for the DownloadMp3Bt Button
        // It starts the process of downloading audio from YouTube in an asynchronous manner
        private async void DownloadMp3Bt_Click(object sender, EventArgs e)
        {
            ToggleControls(false);
            AudioDownloader audioDownloader = new AudioDownloader(utilityclass, this, selectedFolderPath, currentSizeLb, progressBar, historyBox, tempFolderPath);  // Instantiate AudioDownloader to download audio from YouTube
            await audioDownloader.DownloadAudioAsync();  // Asynchronously download the audio file
            linkBox.Text = "";
            ToggleControls(true);
        }
        // This method handles the click event for the DownloadMp4Bt Button
        // It starts the process of downloading video from YouTube in an asynchronous manner
        private async void DownloadMp4Bt_Click(object sender, EventArgs e)
        {
            ToggleControls(false);
            VideoDownloader VideoDownloader = new VideoDownloader(utilityclass, this, currentSizeLb, progressBar, selectedFolderPath, tempFolderPath, historyBox, mp4QualityComboBox);  // Instantiate VideoDownloader to download video from YouTube
            await VideoDownloader.DownloadVideoAsync();  // Asynchronously download the video file
            linkBox.Text = "";
            ToggleControls(true);
        }

        private async void LinkBox_TextChanged(object sender, EventArgs e)
        {
            // When a new link i enterd, diably controls becuase youtube explode needs to load all the informarion
            ToggleControls(false);
            await utilityclass.GetStreamInfos();
            mp4QualityComboBox.Items.Clear();
            StatsUpdater statsUpdater = new StatsUpdater(utilityclass, this, titleLb, durationLb, mp4SizeLb, mp3SizeLb, thumbnailPicBox, channelLb, idLb, uploadDateLb, mp4QualityComboBox);
            await statsUpdater.UpdateVideoStatsAsync();  // Asynchronously update the video stats
            ToggleControls(true);

        }
        private async void ComboBox_TextChanged(object sender, EventArgs e)
        {
            await utilityclass.GetStreamInfos();
            VideoAndAudioSizeUpdater videoAndAudioSizeUpdater = new VideoAndAudioSizeUpdater(utilityclass, this, mp4QualityComboBox, mp4SizeLb, mp3SizeLb);
            videoAndAudioSizeUpdater.UpdateVideoAndAudioSize();
        }

        // This method toggles the Enabled status of various controls on the form
        public void ToggleControls(bool isEnabled)
        {
            linkBox.ReadOnly = !isEnabled;  // Toggle the ReadOnly status of the TextBox
            downloadMp4Bt.Enabled = isEnabled;  // Toggle the Enabled status of the Download Mp4 Button
            downloadMp3Bt.Enabled = isEnabled;  // Toggle the Enabled status of the Download Mp3 Button
            selectFolderButton.Enabled = isEnabled;  // Toggle the Enabled status of the Select Folder Button
            timer.Enabled = isEnabled;  // Toggle the Enabled status of the Timer
            mp4QualityComboBox.Enabled = isEnabled;

            foreach (Label label in usbSticksPanel.Controls.OfType<Label>())  // Toggle the Enabled status of the Labels in the usbSticksPanel
            {
                label.Enabled = isEnabled;  // Toggle the Enabled status of each Label
            }
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

        // This method handles the click event for the showInfoFormBt Button
        // It shows the infoForm as a separate window
        private void ShowInfoFormBt_Click(object sender, EventArgs e)
        {
            infoForm.Show();
        }
    }
}
//Wenn die audio convertiert wird, hällt das ganze programm wegen prozess.waitforexit an. Das ändern :)
//Thumbnail in den Hintergrung
//Das & Zeichen beei Streams aus playlisten wegmachen, so das der ganze link kompiert werden kann.
//Klasse für alle Methoden erstellen?
//Timer für die USB Sticks ausbauen und richtige lösung suchen
//Ganze Playlists aufeinmal downloaden