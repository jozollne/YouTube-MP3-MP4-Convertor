using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExplode.Videos.Streams;
using ATL;

namespace Youtube_Videos_Herrunterladen
{
    internal class VideoDownloader
    {
        private readonly Main main;
        private readonly Label currentSizeLb;
        private readonly ProgressBar progressBar;
        private readonly string selectedFolderPath;
        private readonly string tempFolderPath;
        private readonly TextBox historyBox;
        private readonly ComboBox mp4QualityComboBox;
        private readonly Utilityclass utilityclass;

        public VideoDownloader(Utilityclass utilityclass, Main main, Label currentSizeLb, ProgressBar progressBar, string selectedFolderPath, string tempFolderPath, TextBox historyBox, ComboBox mp4QualityComboBox)
        {
            this.currentSizeLb = currentSizeLb;
            this.progressBar = progressBar;
            this.selectedFolderPath = selectedFolderPath;
            this.tempFolderPath = tempFolderPath;
            this.historyBox = historyBox;
            this.mp4QualityComboBox = mp4QualityComboBox;
            this.main = main;
            this.utilityclass = utilityclass;
        }

        // Method to download video asynchronously
        public async Task DownloadVideoAsync()
        {
            try
            {
                main.infoForm.infoBox.Text = $"Download von Video mit ID {main.streamId} wird gestartet...\r\n";
                main.infoForm.infoBox.AppendText("Stream-Informationen werden abgerufen...\r\n");

                if (main.videoStreamInfo != null && main.audioStreamInfo != null)  // Check if both video and audio stream info are not null
                {
                    main.infoForm.infoBox.AppendText("Stream-Informationen erfolgreich abgerufen...\r\n");

                    long videoBytes = main.videoStreamInfo.Size.Bytes;  // Get the total size of the video stream in bytes
                    long audioBytes = main.audioStreamInfo.Size.Bytes;  // Get the total size of the audio stream in bytes
                    long totalBytes = videoBytes + audioBytes;  // Calculate the total size of video and audio streams

                    string videoSize = utilityclass.FormatBytes(videoBytes);  // Convert the video size to a human-readable format
                    string audioSize = utilityclass.FormatBytes(audioBytes);  // Convert the audio size to a human-readable format
                    string totalSize = utilityclass.FormatBytes(totalBytes);  // Convert the total size to a human-readable format

                    string videoTitle = main.stream.Title + " " + mp4QualityComboBox.Text;  // Get the video title
                    historyBox.Text += main.stream.Title + " " + mp4QualityComboBox.Text + ".mp4";  // Append the video title to the history box

                    foreach (char c in Path.GetInvalidFileNameChars())  // Replace invalid characters in the video title with '_'
                    {
                        videoTitle = videoTitle.Replace(c, '_');
                    }

                    var tempVideoFilePath = Path.Combine(tempFolderPath, $@"{videoTitle}-tempVideo.mp4");  // Combine the temporary folder path and the video title to form the temporary video file path
                    var tempAudioFilePath = Path.Combine(tempFolderPath, $@"{videoTitle}-tempAudio.mp3");  // Combine the temporary folder path and the video title to form the temporary audio file path
                    var rawAudioFilePath = Path.Combine(tempFolderPath, $@"{videoTitle}-rawAudio.mp3");  // Combine the temporary folder path and the video title to form the raw audio file path

                    // Check and delete existing temporary and raw audio files
                    if (File.Exists(tempVideoFilePath)) File.Delete(tempVideoFilePath);
                    if (File.Exists(tempAudioFilePath)) File.Delete(tempAudioFilePath);
                    if (File.Exists(rawAudioFilePath)) File.Delete(rawAudioFilePath);


                    // Download the video stream asynchronously and update the progress for video download
                    main.infoForm.infoBox.AppendText("Video-Download gestartet...\r\n");
                    var videoProgress = new Progress<double>(p => UpdateProgress(p, totalSize, totalBytes, true));
                    await main.youtube.Videos.Streams.DownloadAsync(main.videoStreamInfo, tempVideoFilePath, progress: videoProgress);
                    main.infoForm.infoBox.AppendText("Video-Download erfolgreich...\r\n");

                    // Download the audio stream asynchronously and update the progress for audio download
                    main.infoForm.infoBox.AppendText("Audio-Download gestartet...\r\n");
                    var audioProgress = new Progress<double>(p => UpdateProgress(p, totalSize, totalBytes, false));
                    await main.youtube.Videos.Streams.DownloadAsync(main.audioStreamInfo, rawAudioFilePath, progress: audioProgress);
                    main.infoForm.infoBox.AppendText("Audio-Download erfolgreich...\r\n");

                    // Convert the raw audio file to a final audio file
                    main.infoForm.infoBox.AppendText("Audio-Datei wird konvertiert...\r\n");
                    utilityclass.GetMp3FormatAndConvert(rawAudioFilePath, tempAudioFilePath);

                    var FinalVideoFilePath = Path.Combine(selectedFolderPath, $@"{videoTitle}.mp4");  // Combine the selected folder path and the video title to form the final video file path

                    if (File.Exists(FinalVideoFilePath)) File.Delete(FinalVideoFilePath);  // Delete the final video file if it already exists

                    // Merge the temporary video and audio files to form the final video file
                    main.infoForm.infoBox.AppendText("Video und Audio werden zusammengefügt...\r\n");
                    await MergeAsync(tempVideoFilePath, tempAudioFilePath, FinalVideoFilePath);
                    main.infoForm.infoBox.AppendText("Video und Audio erfolgreich zusammengefügt...\r\n");


                    // Delete the temporary files
                    File.Delete(tempVideoFilePath);  
                    File.Delete(tempAudioFilePath); 
                    File.Delete(rawAudioFilePath);  

                    progressBar.Value = 100;  // Set the progress bar value to 100%
                    UpdateProgress(1, totalSize, totalBytes, false);  // Update the progress to 100% after merging
                    historyBox.Text += " - Erfolgreich - " + Environment.NewLine;  // Append "Erfolgreich" to the history box
                    main.infoForm.infoBox.AppendText("Download und Zusammenführung erfolgreich abgeschlossen!\r\n");
                }
                else  // If either video or audio stream info is null
                {
                    MessageBox.Show("Das Video enthält keinen verfügbaren Audio- oder Video-Stream.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);  // Show an error message
                    main.infoForm.infoBox.AppendText("Das Video enthält keinen verfügbaren Audio- oder Video-Stream.");  // Append error message to the info box
                    historyBox.Text += " - Fehler - " + Environment.NewLine;  // Append "Fehler" to the history box
                }
            }
            catch (Exception ex)  // Catch any exception
            {
                MessageBox.Show($"Ein Fehler ist aufgetreten: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);  // Show the error message
                main.infoForm.infoBox.AppendText($"Ein Fehler ist aufgetreten: {ex.Message}");  // Append the error message to the info box
                historyBox.Text += " - Fehler - " + Environment.NewLine;  // Append "Fehler" to the history box
            }
        }

        // Method to merge video and audio files asynchronously using FFmpeg
        private async Task MergeAsync(string videoFilePath, string audioFilePath, string outputFilePath)
        {
            var inputArgument = string.Format("-i \"{0}\" -i \"{1}\" -c copy", videoFilePath, audioFilePath);  // Define the input argument for FFmpeg
            var outputArgument = string.Format("\"{0}\"", outputFilePath);  // Define the output argument for FFmpeg

            var startInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",  // Set the file name for FFmpeg
                Arguments = $"{inputArgument} {outputArgument}",  // Set the arguments for FFmpeg
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            var process = new Process { StartInfo = startInfo };  // Create a new process for FFmpeg

            process.Start();  // Start the FFmpeg process

            await process.StandardOutput.ReadToEndAsync();
            await process.StandardError.ReadToEndAsync();

            process.WaitForExit();  // Wait for the FFmpeg process to exit
        }

        // Method to update the progress of the download
        private void UpdateProgress(double progress, string totalSize, long totalBytes, bool isVideo)
        {
            double currentProgress = isVideo ? progress / 2 : 0.5 + progress / 2;  // Calculate the current progress
            long currentSize = (long)(totalBytes * currentProgress);  // Calculate the current size based on the progress

            string text = $"Herruntergeladen: {utilityclass.FormatBytes(currentSize)} / Größe: {totalSize} | Gesamtfortschritt: {currentProgress * 100:n2}%";  // Format the progress into a text format

            if (currentSizeLb.InvokeRequired)  // If access to the UI element is needed
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
