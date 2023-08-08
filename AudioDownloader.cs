using ATL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TagLib.Mpeg;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace Youtube_Videos_Herrunterladen
{
    internal class AudioDownloader
    {
        // Additional fields to track download progress
        private long previousSize = 0;
        private DateTime previousUpdateTime = DateTime.Now;
        private DateTime nextSpeedUpdateTime = DateTime.Now.AddSeconds(0.25);  // Update interval for speed (0.25 seconds)
        private const int SpeedSampleCount = 5;  // The number of recent speeds to use for smoothing

        // Queue to store the last N speeds for smoothing
        private Queue<double> speeds = new Queue<double>();

        private readonly Label currentSizeLb; 
        private readonly ProgressBar progressBar; 
        private readonly string selectedFolderPath; 
        private readonly string tempFolderPath;
        private readonly MainForm mainForm;
        private readonly Utilityclass utilityclass;
        private readonly Label downloadSpeedLb;


        public AudioDownloader(Utilityclass utilityclass, MainForm mainForm, string selectedFolderPath, Label currentSizeLb, ProgressBar progressBar, string tempFolderPath, Label downloadSpeedLb)
        {
            this.selectedFolderPath = selectedFolderPath;  
            this.currentSizeLb = currentSizeLb; 
            this.progressBar = progressBar; 
            this.tempFolderPath = tempFolderPath;
            this.mainForm = mainForm;
            this.utilityclass = utilityclass;
            this.downloadSpeedLb = downloadSpeedLb;
        }

        // Method to download audio asynchronously
        public async Task DownloadAudioAsync()  
        {
            try
            {
                mainForm.infoForm.infoBox.Text = $"Download von Audio mit ID {mainForm.streamId} wird gestartet...\r\n";
                mainForm.infoForm.infoBox.AppendText("Stream-Informationen werden abgerufen...\r\n");

                if (mainForm.audioStreamInfo != null)  // Check if the audio stream info is not null
                {
                    mainForm.infoForm.infoBox.AppendText("Stream-Informationen erfolgreich abgerufen...\r\n");

                    long audioBytes = mainForm.audioStreamInfo.Size.Bytes;  // Get the total size of the audio stream in bytes
                    string audioSize = utilityclass.FormatBytes(audioBytes);  // Convert the audio size to a human-readable format
                    string audioTitle = mainForm.watchStream.Title;  // save video title

                    utilityclass.AddHistoryLabel(audioTitle + ".mp3", mainForm.streamId);

                    foreach (char c in Path.GetInvalidFileNameChars())  // Replace invalid characters in the video title with '_'
                    {
                        audioTitle = audioTitle.Replace(c, ' ');
                    }

                    var rawAudioFilePath = Path.Combine(tempFolderPath, $"{audioTitle}-temp.mp3");  // Combine the selected folder path and the audio title to form the audio file path
                    var finalAudioFilePath = Path.Combine(selectedFolderPath, $"{audioTitle}.mp3");  // Combine the selected folder path and the audio title to form the audio file path

                    if (System.IO.File.Exists(finalAudioFilePath)) System.IO.File.Delete(finalAudioFilePath);  // If the audio file already exists, delete it

                    mainForm.infoForm.infoBox.AppendText("Audio-Download gestartet...\r\n");
                    var audioProgress = new Progress<double>(p => UpdateProgress(p, audioSize, audioBytes));  // Create a progress object for the audio
                    await mainForm.youtube.Videos.Streams.DownloadAsync(mainForm.audioStreamInfo, rawAudioFilePath, progress: audioProgress);  // Download the audio stream asynchronously and update the progress
                    mainForm.infoForm.infoBox.AppendText("Audio-Download erfolgreich...\r\n");

                    // Convert the raw audio file to a final audio file
                    mainForm.infoForm.infoBox.AppendText("Das Format des Audiostreams wird gesucht" + Environment.NewLine);
                    utilityclass.GetMp3FormatAndConvert(rawAudioFilePath, finalAudioFilePath);

                    // Set the metadata for the audio file
                    mainForm.infoForm.infoBox.AppendText("Metadaten werden gesetzt" + Environment.NewLine);
                    SetMetaData(finalAudioFilePath, mainForm.watchStream);
                    mainForm.infoForm.infoBox.AppendText("Metadaten gesetzt" + Environment.NewLine);

                    System.IO.File.Delete(rawAudioFilePath);
                    
                    progressBar.Value = 100;  // Set the progress bar value to 100%
                    mainForm.infoForm.infoBox.AppendText("Download und die Konvertierung erfolgreich abgeschlossen!\r\n");
                }
                else  // If the audio stream info is null
                {
                    MessageBox.Show("Das Video enthält keinen verfügbaren Audio-Stream.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);  // Show an error message
                    mainForm.infoForm.infoBox.AppendText("Das Video enthält keinen verfügbaren Audio-Stream" + Environment.NewLine);
                }
            }
            catch (Exception ex)  // Catch any exception
            {
                MessageBox.Show($"Ein Fehler ist aufgetreten: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);  // Show the error message
                mainForm.infoForm.infoBox.AppendText("Ein Fehler ist aufgetreten: " + ex.Message + Environment.NewLine);
            }
        }

        // Method to update the progress
        private void UpdateProgress(double progress, string audioSize, long audioBytes)
        {
            double currentProgress = progress;  // Calculate the current progress
            long currentSize = (long)(audioBytes * currentProgress);  // Calculate the current size based on the progress

            // Regular progress update
            string text = $"Heruntergeladen: {utilityclass.FormatBytes(currentSize)} / Größe: {audioSize} | Gesamtfortschritt: {currentProgress * 100:n2}%";
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

            // Speed calculation and update
            DateTime currentTime = DateTime.Now;
            if (currentTime >= nextSpeedUpdateTime)
            {
                TimeSpan timeDiff = currentTime - previousUpdateTime;
                long sizeDiff = currentSize - previousSize;
                double speed = sizeDiff / timeDiff.TotalSeconds;  // Speed in bytes per second

                // Adding the speed to the queue and removing old values
                speeds.Enqueue(speed);
                while (speeds.Count > SpeedSampleCount)
                {
                    speeds.Dequeue();
                }

                // Calculating the average of the speeds
                double averageSpeed = speeds.Average();
                string speedText = $"{averageSpeed / (1024.0 * 1024.0):0.##} MB/s";  // Converting speed to MB/s

                // Storing the current state for the next speed update
                previousSize = currentSize;
                previousUpdateTime = currentTime;
                nextSpeedUpdateTime = currentTime.AddSeconds(0.25);  // We'll next update the speed one quarter of a second from now

                // Update speed text
                if (downloadSpeedLb.InvokeRequired)  // Check if access to the UI element is required
                {
                    downloadSpeedLb.Invoke((MethodInvoker)delegate { downloadSpeedLb.Text = $"Geschwindigkeit: {speedText}"; });  // Update the label for download speed (invoked)
                }
                else
                {
                    downloadSpeedLb.Text = $"Geschwindigkeit: {speedText}";  // Update the label for download speed
                }
            }
        }

        // Method to set metadata for the audio file
        private void SetMetaData(string finalAudioFilePath, Video audio)
        {
            Track theTrack = new Track(finalAudioFilePath);

            // Set metadata
            theTrack.Title = audio.Title;
            theTrack.Artist = $"{audio.Author}";
            theTrack.Year = audio.UploadDate.Year;

            // Convert Image to byte[]
            byte[] byteImage;
            using (MemoryStream ms = new MemoryStream())
            {
                mainForm.image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byteImage = ms.ToArray();
            } // close MemoryStream

            // Neues Coverbild hinzufügen
            PictureInfo picInfo = PictureInfo.fromBinaryData(byteImage);
            theTrack.EmbeddedPictures.Add(picInfo);

            // Änderungen speichern
            theTrack.Save();
        }

    }
}
