using System; 
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;  
using Youtube_Videos_Herrunterladen;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace downloader
{
    public partial class Main : Form
    {
        public YoutubeClient youtube = new YoutubeClient();  // Create a new YoutubeClient to interact with the YouTube service
        public static string username = Environment.UserName;  // Get the username of the current user
        public string selectedFolderPath = $@"C:\Users\{username}\Downloads\"; // Set the default download location to the current user's Downloads folder
        private string tempFolderPath = $@"C:\Users\{username}\AppData\Local\Temp";
        public InfoForm infoForm = new InfoForm();  // Define the infoForm outside the method.
        string videoUrl;

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer() // Create a timer to trigger events at regular intervals
        {
            Interval = 500 // Set the interval of the timer to 500ms (0.5 seconds)
        };

        public Main()
        {
            InitializeComponent();  // Initialize the components of the Form

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

            UsbManager usbManager = new UsbManager(this, usbSticksPanel, subLb1, selectedFolderPath);  // Instantiate UsbManager to manage USB devices
            timer.Tick += new EventHandler(usbManager.UpdateUsbLabels);  // Register the Tick event to update the USB labels
            timer.Start();  // Start the timer
            subLb1.Text = "Speicherort: " + selectedFolderPath;  // Set the initial text of subLb1 to the selected folder path
        }

        public async void getStreamInfos()
        {
            var videoUrl = linkBox.Text;
            var uri = new Uri(videoUrl);
            var videoId = uri.Query.TrimStart('?').Split('&')[0].Substring(2);
            var video = await youtube.Videos.GetAsync(videoId);
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoId);
            var audioStreamInfo = streamManifest.GetAudioOnlyStreams().TryGetWithHighestBitrate();
            IStreamInfo videoStreamInfo = GetMp4VideoSize(streamManifest);
        }

        // Instantiate StatsUpdater to manage YouTube video stats
        private async void LinkBox_TextChanged(object sender, EventArgs e)
        {
            mp4QualityComboBox.Items.Clear();
            StatsUpdater statsUpdater = new StatsUpdater(this, linkBox, titelLb, durationLb, mp4SizeLb, mp3SizeLb, mp4QualityLb, thumbnailPicBox, chanelLb, idLb, uploadDateLb, mp4QualityComboBox);
            await statsUpdater.UpdateVideoStatsAsync();  // Asynchronously update the video stats
        }

        private async void ComboBox_TextChanged(object sender, EventArgs e)
        {
            VideoAndAudioSizeUpdater videoAndAudioSizeUpdater = new VideoAndAudioSizeUpdater(this, mp4QualityComboBox, mp4SizeLb, mp3SizeLb, linkBox);
            await videoAndAudioSizeUpdater.UpdateVideoAndAudioSize();
        }

        // This method handles the click event for the DownloadMp3Bt Button
        // It starts the process of downloading audio from YouTube in an asynchronous manner
        private async void DownloadMp3Bt_Click(object sender, EventArgs e)
        {
            ToggleControls(false);
            AudioDownloader audioDownloader = new AudioDownloader(this, linkBox, selectedFolderPath, currentSizeLb, progressBar, historyBox, tempFolderPath);  // Instantiate AudioDownloader to download audio from YouTube
            await audioDownloader.DownloadAudioAsync();  // Asynchronously download the audio file
            linkBox.Text = "";
            ToggleControls(true);
        }

        // This method handles the click event for the DownloadMp4Bt Button
        // It starts the process of downloading video from YouTube in an asynchronous manner
        private async void DownloadMp4Bt_Click(object sender, EventArgs e)
        {
            ToggleControls(false);
            VideoDownloader VideoDownloader = new VideoDownloader(this, linkBox, currentSizeLb, progressBar, selectedFolderPath, tempFolderPath, historyBox, mp4QualityComboBox);  // Instantiate VideoDownloader to download video from YouTube
            await VideoDownloader.DownloadVideoAsync();  // Asynchronously download the video file
            linkBox.Text = "";
            ToggleControls(true);
        }

        public void SetSelectedFolderPath(string path)
        {
            selectedFolderPath = path;  // This method sets the selectedFolderPath to the given path
        }

        // This method toggles the Enabled status of various controls on the form
        public void ToggleControls(bool isEnabled)
        {
            linkBox.ReadOnly = !isEnabled;  // Toggle the ReadOnly status of the TextBox
            downloadMp4Bt.Enabled = isEnabled;  // Toggle the Enabled status of the Download Mp4 Button
            downloadMp3Bt.Enabled = isEnabled;  // Toggle the Enabled status of the Download Mp3 Button
            selectFolderButton.Enabled = isEnabled;  // Toggle the Enabled status of the Select Folder Button
            timer.Enabled = isEnabled;  // Toggle the Enabled status of the Timer

            foreach (Label label in usbSticksPanel.Controls.OfType<Label>())  // Toggle the Enabled status of the Labels in the usbSticksPanel
            {
                label.Enabled = isEnabled;  // Toggle the Enabled status of each Label
            }
        }

        // This method formats the given bytes into a human-readable format with appropriate suffixes
        public string FormatBytes(long bytes)
        {
            const int scale = 1024;  // Define the scale for formatting the bytes
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };  // Define the suffixes for each byte scale

            int suffixIndex = 0;  // Initialize the suffix index
            decimal number = bytes;  // Store the byte count as a decimal for formatting

            while (number >= scale && suffixIndex < suffixes.Length - 1)  // While the number of bytes is greater than or equal to the scale and there is another suffix
            {
                number /= scale;  // Divide the number by the scale
                suffixIndex++;  // Increment the suffix index
            }

            return $"{number:n2} {suffixes[suffixIndex]}";  // Return the formatted number and corresponding suffix
        }

        // This method handles the click event for the selectFolderButton
        // It opens a FolderBrowserDialog to allow the user to select a download folder
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

        // Diese Methode ruft das Format eines Audio-Streams aus einer Datei ab und wandelt es gegebenenfalls in MP3 um
        public void GetMp3FormatAndConvert(string tempAudioFilePath, string finalAudioFilePath)
        {
            using (FileStream fs = new FileStream(tempAudioFilePath, FileMode.Open, FileAccess.Read))  // Öffnen der Datei im FileStream
            {
                // Lesen der ersten 12 Bytes der Datei
                byte[] bytes = new byte[12];
                fs.Read(bytes, 0, 12);
                string hex = BitConverter.ToString(bytes).Replace("-", "").ToUpper();
                string format;

                if (hex.StartsWith("494433") || hex.StartsWith("FFFB"))  // Überprüfen des Audio-Stream-Formats anhand der ersten Bytes der Datei
                    format = ".mp3"; // MP3-Format erkannt
                else if (hex.StartsWith("52494646"))
                    format = ".wav"; // WAV-Format erkannt
                else if (hex.StartsWith("4F676753"))
                    format = ".ogg"; // OGG-Format erkannt
                else if (hex.StartsWith("664C6143"))
                    format = ".flac"; // FLAC-Format erkannt
                else if (hex.StartsWith("0000001C667479704D3441"))
                    format = ".m4a"; // M4A-Format erkannt
                else if (hex.StartsWith("1A45DFA3"))
                    format = ".webm"; // WebM-Format erkannt
                else
                    format = "Dateiformat konnte nicht erkannt werden"; // Unbekanntes Format

                infoForm.infoBox.AppendText("Das Format des Audiostrerams ist: " + format + Environment.NewLine);  // Ausgabe des erkannten Formats in ein Info-Fenster

                if (format != ".mp3")  // Wenn das Format nicht MP3 ist, wird es in MP3 umgewandelt
                {
                    infoForm.infoBox.AppendText("Das Format des Audiostrems wird von: " + format + " in: .mp3 umgewandelt" + Environment.NewLine);  // Ausgabe der Umwandlungsinformation in das Info-Fenster

                    // Starten von ffmpeg.exe als Prozess mit den entsprechenden Argumenten zur Umwandlung in MP3
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            // Dateiname der ausführbaren Datei, die für die Audiokonvertierung verwendet wird (ffmpeg.exe)
                            FileName = "ffmpeg.exe",

                            // Argumente für den ffmpeg.exe-Befehl:
                            // -i "{tempAudioFilePath}": Eingabedatei, die überprüft und konvertiert werden soll (temporäre Audiodatei)
                            // -vn: "Video Null", schließt das Video vom Eingabestream aus (nur Audio wird bearbeitet)
                            // -ar 44100: Abtastrate für die Audioausgabe auf 44100 Hz (typische Abtastrate für Audiodateien)
                            // -ac 2: Anzahl der Audiokanäle auf 2 festlegen (stereo)
                            // -b:a 192k: Bitrate für die Audioausgabe auf 192 kbit/s setzen (bessere Audioqualität, aber größere Dateigröße)
                            // "{finalAudioFilePath}": Ausgabedatei, in die die konvertierte Audiodatei geschrieben wird (Pfad durch "finalAudioFilePath" angegeben)
                            Arguments = $"-i \"{tempAudioFilePath}\" -vn -ar 44100 -ac 2 -b:a 192k \"{finalAudioFilePath}\"",

                            // Verhindert die Verwendung der Shell zum Ausführen des Prozesses (Sicherheitsmaßnahme)
                            UseShellExecute = false,

                            // Leitet die Ausgabe des Prozesses (falls vorhanden) in den Standardausgabestream des Prozesses um
                            RedirectStandardOutput = true,

                            // Legt fest, dass kein neues Fenster erstellt werden soll, wenn der Prozess gestartet wird (läuft im Hintergrund)
                            CreateNoWindow = true

                        }
                    };
                    process.Start();
                    process.WaitForExit();

                    infoForm.infoBox.AppendText("Das Format wurde von: " + format + " in: .mp3 umgewandelt" + Environment.NewLine);  // Ausgabe der Erfolgsmeldung in das Info-Fenster
                }
            }
        }

        // Method to the Infos like titel, size etc. from the video with the selected video quality
        public IStreamInfo GetMp4VideoSize(StreamManifest streamManifest)
        {
            string cleanedValue;

            if (mp4QualityComboBox.SelectedItem == null) // Sets 144p to the default quality when there is no quality selecet. aka at the first link
            {
                cleanedValue = "144";
            }
            else
            {
                string selectedValue = mp4QualityComboBox.Text;
                int index = selectedValue.IndexOf('p');
                cleanedValue = selectedValue.Substring(0, index);
            }

            var videoStreamInfo = streamManifest.GetVideoStreams()
                .Where(s => s.VideoResolution.Height == Convert.ToInt32(cleanedValue))
                .OrderByDescending(s => s.Bitrate)
                .FirstOrDefault();

            return videoStreamInfo;
        }

        // This method handles the click event for the showInfoFormBt Button
        // It shows the infoForm as a separate window
        private void showInfoFormBt_Click(object sender, EventArgs e)
        {
            infoForm.Show();
        }
    }
}
//quallität anpassbar machen
//Thumbnail in den Hintergrung
//Das & Zeichen beei Streams aus playlisten wegmachen, so das der ganze link kompiert werden kann.
//Klasse für alle Methoden erstellen?
//Timer für die USB Sticks ausbauen und richtige lösung suchen
//Ganze Playlists aufeinmal downloaden