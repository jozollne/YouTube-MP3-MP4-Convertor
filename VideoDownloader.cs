using NReco.VideoConverter;
using System;  // Import the System namespace for basic functionalities like classes, interfaces and value types
using System.Diagnostics;  // Import the System.Diagnostics namespace for process and system monitoring
using System.IO;  // Import the System.IO namespace for working with files and directories
using System.Linq;
using System.Threading;
using System.Threading.Tasks;  // Import the System.Threading.Tasks namespace for working with tasks and asynchronous operations
using System.Web;
using System.Windows.Forms;  // Import the System.Windows.Forms namespace for building Windows applications
using YoutubeExplode.Videos.Streams;  // Import the YoutubeExplode.Videos.Streams namespace for accessing Youtube video streams
using Forms = System.Windows.Forms;  // Alias the System.Windows.Forms namespace to 'Forms' for better readability



namespace downloader  // Define the namespace for this application
{
    internal class VideoDownloader  // Define the VideoDownloader class
    {
        private Main main = new Main();  // Instantiate a new Main object
        private TextBox linkBox;  // Define a TextBox for the video URL
        private Label currentSizeLb;  // Define a Label to display the current download size
        private ProgressBar progressBar;  // Define a ProgressBar to show the download progress
        private string selectedFolderPath;  // Define a string to hold the selected folder path
        private string tempFolderPath;
        private Panel historyPanel;

        //string cVideoFilePath;
        //string cAudioFilePath;
        //string cOpusAudioFilePath;

        // Define a constructor for the VideoDownloader class
        public VideoDownloader(TextBox linkBox, Label currentSizeLb, ProgressBar progressBar, string selectedFolderPath, string tempFolderPath, Panel historyPanel)
        {
            this.linkBox = linkBox;  // Set the TextBox
            this.currentSizeLb = currentSizeLb;  // Set the Label
            this.progressBar = progressBar;  // Set the ProgressBar
            this.selectedFolderPath = selectedFolderPath;  // Set the selected folder path
            this.tempFolderPath = tempFolderPath;
            this.historyPanel = historyPanel;
        }

        // Define an async method to download the video
        public async Task DownloadVideoAsync(/*CancellationToken ct*/)
        {
            try
            {
                var videoUrl = linkBox.Text;  // Get the video URL from the TextBox

                var uri = new Uri(videoUrl);  // Parse the video URL into a Uri object
                var videoId = uri.Query.TrimStart('?').Split('&')[0].Substring(2);  // Extract the video ID from the query string of the URL
                var video = await main.youtube.Videos.GetAsync(videoId);  // Get the video information from YouTube

                var streamManifest = await main.youtube.Videos.Streams.GetManifestAsync(videoId);  // Get the video stream manifest from YouTube

                var videoStreamInfo = streamManifest.GetVideoStreams().GetWithHighestVideoQuality();  // Get the video stream info with the highest video quality
                var audioStreamInfo = streamManifest.GetAudioOnlyStreams().TryGetWithHighestBitrate(); // Get the audio stream info with the highest bitrate

                if (videoStreamInfo != null && audioStreamInfo != null)  // Check if the video and audio stream info are not null
                {
                    long videoBytes = videoStreamInfo.Size.Bytes;  // Get the total size of the video stream in bytes
                    long audioBytes = audioStreamInfo.Size.Bytes;  // Get the total size of the audio stream in bytes
                    long totalBytes = videoBytes + audioBytes;  // Get the total size of the video and audio streams in bytes

                    string videoSize = main.FormatBytes(videoBytes);  // Convert the video size to a human-readable format
                    string audioSize = main.FormatBytes(audioBytes);  // Convert the audio size to a human-readable format
                    string totalSize = main.FormatBytes(totalBytes);  // Convert the total size to a human-readable format

                    string videoTitle = video.Title;  // save video title

                    UpdateHistory(videoTitle);

                    foreach (char c in Path.GetInvalidFileNameChars()) // Read an save evry char in "c"
                    {
                        videoTitle = videoTitle.Replace(c, '_'); // Replace evry invalid symbol like "\" with an "_"
                    }


                    var videoFilePath = Path.Combine(tempFolderPath, $@"{videoTitle}_tempVideo.mp4");  // Define the video file path
                    var audioFilePath = Path.Combine(tempFolderPath, $@"{videoTitle}_tempAudio.mp3"); // Define the temporary audio mp4 file path
                    var opusAudioFilePath = Path.Combine(tempFolderPath, $@"{videoTitle}_tempAudio.opus");  // Define the temporary opus audio file path

                    // Check if the video and audio files already exist, and if so, delete them
                    if (File.Exists(videoFilePath)) File.Delete(videoFilePath);
                    if (File.Exists(audioFilePath)) File.Delete(audioFilePath);
                    if (File.Exists(opusAudioFilePath)) File.Delete(opusAudioFilePath);

                    var videoProgress = new Progress<double>(p => UpdateProgress(p, totalSize, totalBytes, true));
                    var audioProgress = new Progress<double>(p => UpdateProgress(p, totalSize, totalBytes, false));

                    //cVideoFilePath = videoFilePath; // Copy the values so the values are klass wide avalible
                    //cAudioFilePath = audioFilePath;
                    //cOpusAudioFilePath = opusAudioFilePath;

                    //ct.ThrowIfCancellationRequested();
                    await main.youtube.Videos.Streams.DownloadAsync(videoStreamInfo, videoFilePath, progress: videoProgress);
                    //ct.ThrowIfCancellationRequested();
                    await main.youtube.Videos.Streams.DownloadAsync(audioStreamInfo, opusAudioFilePath, progress: audioProgress);

                    // The audio conversion step is critical because the audio streams downloaded from YouTube are typically in the Opus format.
                    // While Opus is efficient and high-quality, it's not as widely supported by media players as other formats, like MP3.
                    // By converting the audio to MP3, we ensure that the downloaded video will have audio that can be played back on a wide variety of devices and media players.
                    ConvertAudioFile(opusAudioFilePath, audioFilePath); 

                    var outputFilePath = Path.Combine(selectedFolderPath, $@"{videoTitle}.mp4");  // Define the output file path

                    // If the output file already exists, delete it
                    if (File.Exists(outputFilePath)) File.Delete(outputFilePath);

                    // Merge the video and audio files into one output file
                    await MergeAsync(videoFilePath, audioFilePath, outputFilePath);

                    // Update the download progress to 100%
                    UpdateProgress(1, totalSize, totalBytes, false);

                    File.Delete(videoFilePath);
                    File.Delete(audioFilePath);
                    File.Delete(opusAudioFilePath);

                    // Update the progress bar to 100%
                    progressBar.Value = 100;

                }
                else
                {
                    // Display an error message if there is no available video or audio stream
                    MessageBox.Show("Das Video enthält keinen verfügbaren Audio- oder Video-Stream.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            //catch (OperationCanceledException) // Catch, wenn der Benutzer den Download abbricht
            //{
            //    // Code, um auf den Abbruch zu reagieren (z. B. Temp-Dateien löschen, Benutzer informieren, etc.)
            //    CleanUp();
            //    MessageBox.Show("Download abgrebrochen!");
            //}
            catch (Exception ex)  // Catch any exceptions that occur during the download process
            {
                // Display an error message if an exception occurs
                MessageBox.Show($"Ein Fehler ist aufgetreten: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //private void CleanUp()
        //{
        //    File.Delete(cVideoFilePath);
        //    File.Delete(cAudioFilePath);
        //    File.Delete(cOpusAudioFilePath);
        //}

        // The reason for this block of code is to make sure that the application can find the FFmpeg executable when it's needed for converting audio files.
        // By including the application's directory in the PATH variable, we're telling the system where to look for the FFmpeg executable.
        // This is important because the FFmpeg converter will not be able to function without the FFmpeg executable.
        private void ConvertAudioFile(string opusAudioFilePath, string audioFilePath)
        {
            var applicationPath = AppDomain.CurrentDomain.BaseDirectory; // Get the base directory of the current application domain (where the application's executable resides)
            var ffmpegPath = Path.Combine(applicationPath, "ffmpeg.exe"); // Combine the application path with the name of the FFmpeg executable to create the full path to FFmpeg

            var pathVar = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process); // Get the current PATH environment variable for the current process
            if (!pathVar.Split(';').ToList().Contains(applicationPath)) // Check if the application path is not already included in the PATH environment variable
            {
                pathVar += $";{applicationPath}";  // Append the application path to the PATH variable, separated by a semicolon
                Environment.SetEnvironmentVariable("PATH", pathVar, EnvironmentVariableTarget.Process); // Set the updated PATH environment variable for the current process
            }

            var ffMpeg = new NReco.VideoConverter.FFMpegConverter(); // Create a new instance of the NReco FFmpeg converter
            ffMpeg.ConvertMedia(opusAudioFilePath, null, audioFilePath, "mp3", new ConvertSettings()); // Use the FFmpeg converter to convert the Opus audio file to MP3
        }

        // Define an async method to merge the video and audio files
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

            // Read the output and error streams of the FFmpeg process
            await process.StandardOutput.ReadToEndAsync();
            await process.StandardError.ReadToEndAsync();

            process.WaitForExit();  // Wait for the FFmpeg process to exit
        }

        // Define a method to update the download progress
        private void UpdateProgress(double progress, string totalSize, long totalBytes, bool isVideo)
        {
            // Calculate the current progress and size
            double currentProgress = isVideo ? progress / 2 : 0.5 + progress / 2;
            long currentSize = (long)(totalBytes * currentProgress);

            // Create a text string to display the download progress
            string text = $"Herruntergeladen: {main.FormatBytes(currentSize)} / Größe: {totalSize} ({currentProgress * 100:n2}%)";

            if (currentSizeLb.InvokeRequired)  // If access to the UI element is needed
            {
                // Update the text of the current size label and the value of the progress bar
                currentSizeLb.Invoke((MethodInvoker)delegate { currentSizeLb.Text = text; });
                progressBar.Invoke((MethodInvoker)delegate { progressBar.Value = (int)(currentProgress * 100); });
            }
            else
            {
                // Update the text of the current size label and the value of the progress bar
                currentSizeLb.Text = text;
                progressBar.Value = (int)(currentProgress * 100);
            }
        }

        private void UpdateHistory(string videoTitel)
        {
            Forms.Label label = new Forms.Label
            {
                Text = $"{videoTitel}.mp4",  // Set the label text as "Hi"
                AutoSize = true,  // Enable auto-sizing of the label based on its content
                Margin = new Padding(3, 3, 3, 3),  // Set the margin around the label for better spacing
                Top = historyPanel.Controls.OfType<Forms.Label>().Count() * 25  // Set the vertical position of the label based on the number of existing labels
            };

            historyPanel.Controls.Add(label);  // Add the label to the panel's controls
        }
    }
}
