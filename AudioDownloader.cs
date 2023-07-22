using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace Youtube_Videos_Herrunterladen
{
    internal class AudioDownloader
    {
        private readonly Label currentSizeLb; 
        private readonly ProgressBar progressBar; 
        private readonly string selectedFolderPath; 
        private readonly string tempFolderPath;
        private readonly TextBox historyBox;
        private readonly Main main;
        private readonly Utilityclass utilityclass;


        public AudioDownloader(Utilityclass utilityclass, Main main, string selectedFolderPath, Label currentSizeLb, ProgressBar progressBar, TextBox historyBox, string tempFolderPath)
        {
            this.selectedFolderPath = selectedFolderPath;  
            this.currentSizeLb = currentSizeLb; 
            this.progressBar = progressBar; 
            this.historyBox = historyBox;
            this.tempFolderPath = tempFolderPath;
            this.main = main;
            this.utilityclass = utilityclass;
        }

        // Method to download audio asynchronously
        public async Task DownloadAudioAsync()  
        {
            try
            {
                main.infoForm.infoBox.Text = $"Download von Audio mit ID {main.streamId} wird gestartet...\r\n";
                main.infoForm.infoBox.AppendText("Stream-Informationen werden abgerufen...\r\n");

                if (main.audioStreamInfo != null)  // Check if the audio stream info is not null
                {
                    main.infoForm.infoBox.AppendText("Stream-Informationen erfolgreich abgerufen...\r\n");

                    long audioBytes = main.audioStreamInfo.Size.Bytes;  // Get the total size of the audio stream in bytes
                    string audioSize = utilityclass.FormatBytes(audioBytes);  // Convert the audio size to a human-readable format
                    string audioTitle = main.stream.Title;  // save video title

                    historyBox.Text += main.stream.Title + ".mp3";

                    foreach (char c in Path.GetInvalidFileNameChars()) // Read an save evry char in "c"
                    {
                        audioTitle = audioTitle.Replace(c, '_'); // Replace evry invalid symbol like "\" with an "_"
                    }

                    var rawAudioFilePath = Path.Combine(tempFolderPath, $"{audioTitle}-temp.mp3");  // Combine the selected folder path and the audio title to form the audio file path
                    var finalAudioFilePath = Path.Combine(selectedFolderPath, $"{audioTitle}.mp3");  // Combine the selected folder path and the audio title to form the audio file path

                    if (File.Exists(finalAudioFilePath)) File.Delete(finalAudioFilePath);  // If the audio file already exists, delete it

                    main.infoForm.infoBox.AppendText("Audio-Download gestartet...\r\n");
                    var audioProgress = new Progress<double>(p => UpdateProgress(p, audioSize, audioBytes));  // Create a progress object for the audio
                    await main.youtube.Videos.Streams.DownloadAsync(main.audioStreamInfo, rawAudioFilePath, progress: audioProgress);  // Download the audio stream asynchronously and update the progress
                    main.infoForm.infoBox.AppendText("Audio-Download erfolgreich...\r\n");

                    // Convert the raw audio file to a final audio file
                    main.infoForm.infoBox.AppendText("Das Format des Audiostreams wird gesucht" + Environment.NewLine);
                    utilityclass.GetMp3FormatAndConvert(rawAudioFilePath, finalAudioFilePath);

                    // Set the metadata for the audio file
                    main.infoForm.infoBox.AppendText("Metadaten werden gesetzt" + Environment.NewLine);
                    SetMetaData(finalAudioFilePath, main.stream);
                    main.infoForm.infoBox.AppendText("Metadaten gesetzt" + Environment.NewLine);

                    File.Delete(rawAudioFilePath);

                    progressBar.Value = 100;  // Set the progress bar value to 100%
                    historyBox.Text += " - Erfolgreich - " + Environment.NewLine;
                    main.infoForm.infoBox.AppendText("Download und die Konvertierung erfolgreich abgeschlossen!\r\n");
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

        // Method to update the progress
        private void UpdateProgress(double progress, string audioSize, long audioBytes)  // Method to update the progress
        {
            double currentProgress = progress;  // Calculate the current progress
            long currentSize = (long)(audioBytes * currentProgress);  // Calculate the current size based on the progress
            string text = $"Herruntergeladen: {utilityclass.FormatBytes(currentSize)} / Größe: {audioSize} | Gesamtfortschritt: {currentProgress * 100:n2}%";  // Format the progress into a text format

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

        // Method to set metadata for the audio file
        private void SetMetaData(string finalAudioFilePath, Video audio)
        {
            TagLib.File tagFile = TagLib.File.Create(finalAudioFilePath);  // Create the TagLib.File object

            // Set metadata
            tagFile.Tag.Title = audio.Title;
            tagFile.Tag.Performers = new[] { $"{audio.Author}" };
            tagFile.Tag.Year = (uint)audio.UploadDate.Year;

            tagFile.Save();  // Save changes
        }
    }
}
