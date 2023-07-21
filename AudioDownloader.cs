using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Youtube_Videos_Herrunterladen;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;
using Forms = System.Windows.Forms;  // Alias the System.Windows.Forms namespace to 'Forms' for better readability


namespace downloader
{
    internal class AudioDownloader
    {
        private TextBox linkBox;  // TextBox for the video URL
        private Label currentSizeLb;  // Label for displaying the current download size
        private ProgressBar progressBar;  // ProgressBar for showing the download progress
        private string selectedFolderPath;  // The selected path to save the downloaded file
        private string tempFolderPath;
        private TextBox historyBox;
        private Main main;  // Referenz auf das Main-Objekt

        public AudioDownloader(Main main, TextBox linkBox, string selectedFolderPath, Label currentSizeLb, ProgressBar progressBar, TextBox historyBox, string tempFolderPath)
        {
            this.main = main;  // Setzen des Main-Objekts
            this.linkBox = linkBox;  // Setzen des TextBox
            this.selectedFolderPath = selectedFolderPath;  // Setzen des ausgewählten Ordnerpfades
            this.currentSizeLb = currentSizeLb;  // Setzen des Labels
            this.progressBar = progressBar;  // Setzen des ProgressBars
            this.historyBox = historyBox;
            this.tempFolderPath = tempFolderPath;
        }


        public async Task DownloadAudioAsync()  // Method to download audio asynchronously
        {
            try
            {
                var videoURL = linkBox.Text;  // Get the video URL from the TextBox
                var uri = new Uri(videoURL);  // Parse the video URL into a Uri object
                var audioId = uri.Query.TrimStart('?').Split('&')[0].Substring(2);  // Extract the audio ID from the query string of the URL

                main.infoForm.infoBox.Text = $"Download von Audio mit ID {audioId} wird gestartet...\r\n";

                main.infoForm.infoBox.AppendText("Stream-Informationen werden abgerufen...\r\n");
                var audio = await main.youtube.Videos.GetAsync(audioId);  // Get the audio information from YouTube
                var audioStreamManifest = await main.youtube.Videos.Streams.GetManifestAsync(audioId);  // Get the audio stream manifest from YouTube
                var audioStreamInfo = audioStreamManifest.GetAudioOnlyStreams().TryGetWithHighestBitrate();  // Get the audio stream info with the highest bitrate

                if (audioStreamInfo != null)  // Check if the audio stream info is not null
                {
                    main.infoForm.infoBox.AppendText("Stream-Informationen erfolgreich abgerufen...\r\n");

                    long audioBytes = audioStreamInfo.Size.Bytes;  // Get the total size of the audio stream in bytes

                    string audioSize = main.FormatBytes(audioBytes);  // Convert the audio size to a human-readable format

                    string audioTitle = audio.Title;  // save video title

                    historyBox.Text += audio.Title + ".mp3";

                    foreach (char c in Path.GetInvalidFileNameChars()) // Read an save evry char in "c"
                    {
                        audioTitle = audioTitle.Replace(c, '_'); // Replace evry invalid symbol like "\" with an "_"
                    }

                    var rawAudioFilePath = Path.Combine(tempFolderPath, $"{audioTitle}-temp.mp3");  // Combine the selected folder path and the audio title to form the audio file path
                    var finalAudioFilePath = Path.Combine(selectedFolderPath, $"{audioTitle}.mp3");  // Combine the selected folder path and the audio title to form the audio file path

                    if (File.Exists(finalAudioFilePath)) File.Delete(finalAudioFilePath);  // If the audio file already exists, delete it

                    main.infoForm.infoBox.AppendText("Audio-Download startet...\r\n");
                    var audioProgress = new Progress<double>(p => UpdateProgress(p, audioSize, audioBytes));  // Create a progress object for the audio
                    await main.youtube.Videos.Streams.DownloadAsync(audioStreamInfo, rawAudioFilePath, progress: audioProgress);  // Download the audio stream asynchronously and update the progress
                    main.infoForm.infoBox.AppendText("Audio-Download erfolgreich...\r\n");

                    main.infoForm.infoBox.AppendText("Das Format des Audiostreams wird gesucht" + Environment.NewLine);
                    main.GetMp3FormatAndConvert(rawAudioFilePath, finalAudioFilePath);

                    main.infoForm.infoBox.AppendText("Metadaten werden gesetzt" + Environment.NewLine);
                    SetMetaData(finalAudioFilePath, audio);
                    main.infoForm.infoBox.AppendText("Metadaten gesetzt" + Environment.NewLine);
                    
                    File.Delete(rawAudioFilePath);
                    main.infoForm.infoBox.AppendText("Download und die Konvertierung erfolgreich abgeschlossen!\r\n");
                    progressBar.Value = 100;  // Set the progress bar value to 100%
                    historyBox.Text += " - Erfolgreich - " + Environment.NewLine;

                }
                else  // If the audio stream info is null
                {
                    MessageBox.Show("Das Video enthält keinen verfügbaren Audio-Stream.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);  // Show an error message
                    main.infoForm.infoBox.Text = "Das Video enthält keinen verfügbaren Audio-Stream" + Environment.NewLine;
                    historyBox.Text += " - Fehler - " + Environment.NewLine;
                }
            }
            catch (Exception ex)  // Catch any exception
            {
                MessageBox.Show($"Ein Fehler ist aufgetreten: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);  // Show the error message
                main.infoForm.infoBox.Text = "Ein Fehler ist aufgetreten: " + ex.Message + Environment.NewLine;
                historyBox.Text += " - Fehler - " + Environment.NewLine;
            }
        }

        private void UpdateProgress(double progress, string audioSize, long audioBytes)  // Method to update the progress
        {
            double currentProgress = progress;  // Calculate the current progress
            long currentSize = (long)(audioBytes * currentProgress);  // Calculate the current size based on the progress
            string text = $"Herruntergeladen: {main.FormatBytes(currentSize)} / Größe: {audioSize} | Gesamtfortschritt: {currentProgress * 100:n2}%";  // Format the progress into a text format

            if (currentSizeLb.InvokeRequired)  // Check if access to the UI element is required
            {
                currentSizeLb.Invoke((MethodInvoker)delegate { currentSizeLb.Text = text; });  // Update the current size label (invoked)
                progressBar.Invoke((MethodInvoker)delegate { progressBar.Value = (int)(currentProgress * 100); });  // Update the progress bar (invoked)
            }
            else
            {
                currentSizeLb.Text = text;  // Update the current size label
                progressBar.Value = (int)(currentProgress * 100);  // Update the progress bar
            }
        }

        private void SetMetaData(string finalAudioFilePath, Video audio)
        {
            // Das File-Objekt erstellen
            TagLib.File tagFile = TagLib.File.Create(finalAudioFilePath);

            // Metadaten setzen
            tagFile.Tag.Title = audio.Title;
            tagFile.Tag.Performers = new[] { $"{audio.Author.ToString()}" };
            tagFile.Tag.Year = (uint)audio.UploadDate.Year;

            // Änderungen speichern
            tagFile.Save();
        }

    }
}
