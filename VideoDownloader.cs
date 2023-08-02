using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExplode.Videos.Streams;
using ATL;
using System.Collections.Generic;
using TagLib.Mpeg;

namespace Youtube_Videos_Herrunterladen
{
    internal class VideoDownloader
    {
        // Additional fields to track download progress
        private long previousSize = 0;
        private DateTime previousUpdateTime = DateTime.Now;
        private DateTime nextSpeedUpdateTime = DateTime.Now.AddSeconds(0.25);  // Update interval for speed (0.25 seconds)
        private const int SpeedSampleCount = 10;  // The number of recent speeds to use for smoothing

        // Queue to store the last N speeds for smoothing
        private Queue<double> speeds = new Queue<double>();

        private readonly MainForm mainForm;
        private readonly Label currentSizeLb;
        private readonly ProgressBar progressBar;
        private readonly string selectedFolderPath;
        private readonly string tempFolderPath;
        private readonly ComboBox mp4QualityComboBox;
        private readonly Utilityclass utilityclass;
        private readonly Label downloadSpeedLb;

        public VideoDownloader(Utilityclass utilityclass, MainForm mainForm, Label currentSizeLb, ProgressBar progressBar, string selectedFolderPath, string tempFolderPath, ComboBox mp4QualityComboBox, Label downloadSpeedLb)
        {
            this.currentSizeLb = currentSizeLb;
            this.progressBar = progressBar;
            this.selectedFolderPath = selectedFolderPath;
            this.tempFolderPath = tempFolderPath;
            this.mp4QualityComboBox = mp4QualityComboBox;
            this.mainForm = mainForm;
            this.utilityclass = utilityclass;
            this.downloadSpeedLb = downloadSpeedLb;
        }

        // Method to download video asynchronously
        public async Task DownloadVideoAsync()
        {
            try
            {
                mainForm.infoForm.infoBox.Text = $"Download von Video mit ID {mainForm.streamId} wird gestartet...\r\n";
                mainForm.infoForm.infoBox.AppendText("Stream-Informationen werden abgerufen...\r\n");

                if (mainForm.videoStreamInfo != null && mainForm.audioStreamInfo != null)  // Check if both video and audio stream info are not null
                {
                    mainForm.infoForm.infoBox.AppendText("Stream-Informationen erfolgreich abgerufen...\r\n");

                    long videoBytes = mainForm.videoStreamInfo.Size.Bytes;  // Get the total size of the video stream in bytes
                    long audioBytes = mainForm.audioStreamInfo.Size.Bytes;  // Get the total size of the audio stream in bytes
                    long totalBytes = videoBytes + audioBytes;  // Calculate the total size of video and audio streams

                    string videoSize = utilityclass.FormatBytes(videoBytes);  // Convert the video size to a human-readable format
                    string audioSize = utilityclass.FormatBytes(audioBytes);  // Convert the audio size to a human-readable format
                    string totalSize = utilityclass.FormatBytes(totalBytes);  // Convert the total size to a human-readable format

                    string videoTitle = mainForm.stream.Title + " " + mp4QualityComboBox.Text;  // Get the video title

                    utilityclass.AddHistoryLabel(videoTitle + ".mp4", mainForm.stream.Id);

                    foreach (char c in Path.GetInvalidFileNameChars())  // Replace invalid characters in the video title with '_'
                    {
                        videoTitle = videoTitle.Replace(c, '_');
                    }

                    var tempVideoFilePath = Path.Combine(tempFolderPath, $@"{videoTitle}-tempVideo.mp4");  // Combine the temporary folder path and the video title to form the temporary video file path
                    var tempAudioFilePath = Path.Combine(tempFolderPath, $@"{videoTitle}-tempAudio.mp3");  // Combine the temporary folder path and the video title to form the temporary audio file path
                    var rawAudioFilePath = Path.Combine(tempFolderPath, $@"{videoTitle}-rawAudio.mp3");  // Combine the temporary folder path and the video title to form the raw audio file path

                    // Check and delete existing temporary and raw audio files
                    if (System.IO.File.Exists(tempVideoFilePath)) System.IO.File.Delete(tempVideoFilePath);
                    if (System.IO.File.Exists(tempAudioFilePath)) System.IO.File.Delete(tempAudioFilePath);
                    if (System.IO.File.Exists(rawAudioFilePath)) System.IO.File.Delete(rawAudioFilePath);


                    // Download the video stream asynchronously and update the progress for video download
                    mainForm.infoForm.infoBox.AppendText("Video-Download gestartet...\r\n");
                    var videoProgress = new Progress<double>(p => UpdateProgress(p, totalSize, totalBytes, true));
                    await mainForm.youtube.Videos.Streams.DownloadAsync(mainForm.videoStreamInfo, tempVideoFilePath, progress: videoProgress);
                    mainForm.infoForm.infoBox.AppendText("Video-Download erfolgreich...\r\n");

                    // Download the audio stream asynchronously and update the progress for audio download
                    mainForm.infoForm.infoBox.AppendText("Audio-Download gestartet...\r\n");
                    var audioProgress = new Progress<double>(p => UpdateProgress(p, totalSize, totalBytes, false));
                    await mainForm.youtube.Videos.Streams.DownloadAsync(mainForm.audioStreamInfo, rawAudioFilePath, progress: audioProgress);
                    mainForm.infoForm.infoBox.AppendText("Audio-Download erfolgreich...\r\n");

                    // Convert the raw audio file to a final audio file
                    mainForm.infoForm.infoBox.AppendText("Das Format des Audiostreams wird gesucht...\r\n");
                    utilityclass.GetMp3FormatAndConvert(rawAudioFilePath, tempAudioFilePath);

                    var FinalVideoFilePath = Path.Combine(selectedFolderPath, $@"{videoTitle}.mp4");  // Combine the selected folder path and the video title to form the final video file path

                    if (System.IO.File.Exists(FinalVideoFilePath)) System.IO.File.Delete(FinalVideoFilePath);  // Delete the final video file if it already exists

                    // Merge the temporary video and audio files to form the final video file
                    mainForm.infoForm.infoBox.AppendText("Video und Audio werden zusammengefügt...\r\n");
                    await MergeAsync(tempVideoFilePath, tempAudioFilePath, FinalVideoFilePath);
                    mainForm.infoForm.infoBox.AppendText("Video und Audio erfolgreich zusammengefügt...\r\n");


                    // Delete the temporary files
                    System.IO.File.Delete(tempVideoFilePath);
                    System.IO.File.Delete(tempAudioFilePath);
                    System.IO.File.Delete(rawAudioFilePath);  

                    progressBar.Value = 100;  // Set the progress bar value to 100%
                    UpdateProgress(1, totalSize, totalBytes, false);  // Update the progress to 100% after merging
                    mainForm.infoForm.infoBox.AppendText("Download und Zusammenführung erfolgreich abgeschlossen!\r\n");
                }
                else  // If either video or audio stream info is null
                {
                    MessageBox.Show("Das Video enthält keinen verfügbaren Audio- oder Video-Stream.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);  // Show an error message
                    mainForm.infoForm.infoBox.AppendText("Das Video enthält keinen verfügbaren Audio- oder Video-Stream.");  // Append error message to the info box
                }
            }
            catch (Exception ex)  // Catch any exception
            {
                MessageBox.Show($"Ein Fehler ist aufgetreten: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);  // Show the error message
                mainForm.infoForm.infoBox.AppendText($"Ein Fehler ist aufgetreten: {ex.Message}");  // Append the error message to the info box
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
        // Method to update the download progress
        private void UpdateProgress(double progress, string totalSize, long totalBytes, bool isVideo)
        {
            double currentProgress = isVideo ? progress / 2 : 0.5 + progress / 2;  // Calculating current progress
            long currentSize = (long)(totalBytes * currentProgress);  // Calculating current size based on progress

            // Regular progress update
            string text = $"Heruntergeladen: {utilityclass.FormatBytes(currentSize)} / Größe: {totalSize} | Gesamtfortschritt: {currentProgress * 100:n2}%";
            if (currentSizeLb.InvokeRequired)  // If UI element access is needed
            {
                currentSizeLb.Invoke((MethodInvoker)delegate { currentSizeLb.Text = text; });  // Updating the label for the current size (invoked)
                progressBar.Invoke((MethodInvoker)delegate { progressBar.Value = (int)(currentProgress * 100); });  // Updating the progress bar (invoked)
            }
            else
            {
                currentSizeLb.Text = text;  // Updating the label for the current size
                progressBar.Value = (int)(currentProgress * 100);  // Updating the progress bar
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
                if (downloadSpeedLb.InvokeRequired)  // If UI element access is needed
                {
                    downloadSpeedLb.Invoke((MethodInvoker)delegate { downloadSpeedLb.Text = $"Geschwindigkeit: {speedText}"; });  // Updating the label for download speed (invoked)
                }
                else
                {
                    downloadSpeedLb.Text = $"Geschwindigkeit: {speedText}";  // Updating the label for download speed
                }
            }
        }
    }
}
