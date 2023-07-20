using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;
using Forms = System.Windows.Forms;  // Alias the System.Windows.Forms namespace to 'Forms' for better readability

namespace downloader
{
    internal class AudioDownloader
    {
        private Main main = new Main();  // Instantiate a new Main object
        private TextBox linkBox;  // TextBox for the video URL
        private Label currentSizeLb;  // Label for displaying the current download size
        private ProgressBar progressBar;  // ProgressBar for showing the download progress
        private string selectedFolderPath;  // The selected path to save the downloaded file
        private Panel historyPanel;

        public AudioDownloader(TextBox linkBox, string selectedFolderPath, Label currentSizeLb, ProgressBar progressBar, Panel historyPanel)
        {
            this.linkBox = linkBox;  // Set the TextBox
            this.selectedFolderPath = selectedFolderPath;  // Set the selected folder path
            this.currentSizeLb = currentSizeLb;  // Set the Label
            this.progressBar = progressBar;  // Set the ProgressBar
            this.historyPanel = historyPanel;
        }

        public async Task DownloadAudioAsync()  // Method to download audio asynchronously
        {
            try
            {
                var videoURL = linkBox.Text;  // Get the video URL from the TextBox

                var uri = new Uri(videoURL);  // Parse the video URL into a Uri object
                var audioId = uri.Query.TrimStart('?').Split('&')[0].Substring(2);  // Extract the audio ID from the query string of the URL
                var audio = await main.youtube.Videos.GetAsync(audioId);  // Get the audio information from YouTube

                var audioStreamManifest = await main.youtube.Videos.Streams.GetManifestAsync(audioId);  // Get the audio stream manifest from YouTube

                var audioStreamInfo = audioStreamManifest.GetAudioOnlyStreams().TryGetWithHighestBitrate();  // Get the audio stream info with the highest bitrate

                if (audioStreamInfo != null)  // Check if the audio stream info is not null
                {
                    long audioBytes = audioStreamInfo.Size.Bytes;  // Get the total size of the audio stream in bytes

                    string audioSize = main.FormatBytes(audioBytes);  // Convert the audio size to a human-readable format

                    string audioTitle = audio.Title;  // save video title

                    UpdateHistory(audio.Title);

                    foreach (char c in Path.GetInvalidFileNameChars()) // Read an save evry char in "c"
                    {
                        audioTitle = audioTitle.Replace(c, '_'); // Replace evry invalid symbol like "\" with an "_"
                    }

                    var audioFilePath = Path.Combine(selectedFolderPath, $"{audioTitle}.mp3");  // Combine the selected folder path and the audio title to form the audio file path

                    if (File.Exists(audioFilePath))  // Check if the audio file already exists
                    {
                        File.Delete(audioFilePath);  // If the audio file already exists, delete it
                    }

                    var audioProgress = new Progress<double>(p => UpdateProgress(p, audioSize, audioBytes));  // Create a progress object for the audio

                    await main.youtube.Videos.Streams.DownloadAsync(audioStreamInfo, audioFilePath, progress: audioProgress);  // Download the audio stream asynchronously and update the progress

                    UpdateProgress(1, audioSize, audioBytes);  // Update the progress to 100%
                    progressBar.Value = 100;  // Set the progress bar value to 100%
                }
                else  // If the audio stream info is null
                {
                    MessageBox.Show("Das Video enthält keinen verfügbaren Audio-Stream.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);  // Show an error message
                }
            }
            catch (Exception ex)  // Catch any exception
            {
                MessageBox.Show($"Ein Fehler ist aufgetreten: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);  // Show the error message
            }
        }

        private void UpdateHistory(string audioTitel)
        {
            Forms.Label label = new Forms.Label
            {
                Text = $"{audioTitel}",  // Set the label text as "Hi"
                AutoSize = true,  // Enable auto-sizing of the label based on its content
                Margin = new Padding(3, 3, 3, 3),  // Set the margin around the label for better spacing
                Top = historyPanel.Controls.OfType<Forms.Label>().Count() * 25  // Set the vertical position of the label based on the number of existing labels
            };

            historyPanel.Controls.Add(label);  // Add the label to the panel's controls
        }

        private void UpdateProgress(double progress, string audioSize, long audioBytes)  // Method to update the progress
        {
            double currentProgress = progress;  // Calculate the current progress
            long currentSize = (long)(audioBytes * currentProgress);  // Calculate the current size based on the progress
            string text = $"Herruntergeladen: {main.FormatBytes(currentSize)} / Größe: {audioSize} ({currentProgress * 100:n2}%)";  // Format the progress into a text format

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
    }
}
