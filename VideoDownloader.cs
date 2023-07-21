using NReco.VideoConverter;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using YoutubeExplode.Videos.Streams;
using Forms = System.Windows.Forms;

namespace downloader
{
    internal class VideoDownloader
    {
        private Main main;
        private TextBox linkBox;
        private Label currentSizeLb;
        private ProgressBar progressBar;
        private string selectedFolderPath;
        private string tempFolderPath;
        private TextBox historyBox;

        public VideoDownloader(Main main, TextBox linkBox, Label currentSizeLb, ProgressBar progressBar, string selectedFolderPath, string tempFolderPath, TextBox historyBox)
        {
            this.linkBox = linkBox;
            this.currentSizeLb = currentSizeLb;
            this.progressBar = progressBar;
            this.selectedFolderPath = selectedFolderPath;
            this.tempFolderPath = tempFolderPath;
            this.historyBox = historyBox;
            this.main = main;  // Setzen des Main-Objekts
        }

        public async Task DownloadVideoAsync()
        {
            try
            {
                var videoUrl = linkBox.Text;

                var uri = new Uri(videoUrl);
                var videoId = uri.Query.TrimStart('?').Split('&')[0].Substring(2);
                var video = await main.youtube.Videos.GetAsync(videoId);

                main.infoForm.infoBox.Text = $"Download von Video mit ID {videoId} wird gestartet...\r\n";

                main.infoForm.infoBox.AppendText("Stream-Informationen werden abgerufen...\r\n");
                var streamManifest = await main.youtube.Videos.Streams.GetManifestAsync(videoId);
                var videoStreamInfo = streamManifest.GetVideoStreams().GetWithHighestVideoQuality();
                var audioStreamInfo = streamManifest.GetAudioOnlyStreams().TryGetWithHighestBitrate();

                if (videoStreamInfo != null && audioStreamInfo != null)
                {
                    main.infoForm.infoBox.AppendText("Stream-Informationen erfolgreich abgerufen...\r\n");

                    long videoBytes = videoStreamInfo.Size.Bytes;
                    long audioBytes = audioStreamInfo.Size.Bytes;
                    long totalBytes = videoBytes + audioBytes;

                    string videoSize = main.FormatBytes(videoBytes);
                    string audioSize = main.FormatBytes(audioBytes);
                    string totalSize = main.FormatBytes(totalBytes);

                    string videoTitle = video.Title;
                    historyBox.Text += video.Title + ".mp4";

                    foreach (char c in Path.GetInvalidFileNameChars())
                    {
                        videoTitle = videoTitle.Replace(c, '_');
                    }

                    var tempVideoFilePath = Path.Combine(tempFolderPath, $@"{videoTitle}-tempVideo.mp4");
                    var tempAudioFilePath = Path.Combine(tempFolderPath, $@"{videoTitle}-tempAudio.mp3");
                    var rawAudioFilePath = Path.Combine(tempFolderPath, $@"{videoTitle}-rawAudio.mp3");

                    if (File.Exists(tempVideoFilePath)) File.Delete(tempVideoFilePath);
                    if (File.Exists(tempAudioFilePath)) File.Delete(tempAudioFilePath);
                    if (File.Exists(rawAudioFilePath)) File.Delete(rawAudioFilePath);


                    main.infoForm.infoBox.AppendText("Video-Download startet...\r\n");

                    var videoProgress = new Progress<double>(p => UpdateProgress(p, totalSize, totalBytes, true));
                    await main.youtube.Videos.Streams.DownloadAsync(videoStreamInfo, tempVideoFilePath, progress: videoProgress);

                    main.infoForm.infoBox.AppendText("Video-Download erfolgreich...\r\n");

                    main.infoForm.infoBox.AppendText("Audio-Download startet...\r\n");

                    var audioProgress = new Progress<double>(p => UpdateProgress(p, totalSize, totalBytes, false));
                    await main.youtube.Videos.Streams.DownloadAsync(audioStreamInfo, rawAudioFilePath, progress: audioProgress);

                    main.infoForm.infoBox.AppendText("Audio-Download erfolgreich...\r\n");

                    main.infoForm.infoBox.AppendText("Audio-Datei wird konvertiert...\r\n");

                    main.GetMp3FormatAndConvert(rawAudioFilePath, tempAudioFilePath);

                    main.infoForm.infoBox.AppendText("Audio-Datei erfolgreich konvertiert...\r\n");

                    main.infoForm.infoBox.AppendText("Video und Audio werden zusammengefügt...\r\n");

                    var FinalVideoFilePath = Path.Combine(selectedFolderPath, $@"{videoTitle}.mp4");

                    if (File.Exists(FinalVideoFilePath)) File.Delete(FinalVideoFilePath);

                    await MergeAsync(tempVideoFilePath, tempAudioFilePath, FinalVideoFilePath);

                    main.infoForm.infoBox.AppendText("Video und Audio erfolgreich zusammengefügt...\r\n");

                    UpdateProgress(1, totalSize, totalBytes, false);

                    File.Delete(tempVideoFilePath);
                    File.Delete(tempAudioFilePath);
                    File.Delete(rawAudioFilePath);

                    main.infoForm.infoBox.AppendText("Download und Zusammenführung erfolgreich abgeschlossen!\r\n");
                    progressBar.Value = 100;
                    historyBox.Text += " - Erfolgreich - " + Environment.NewLine;
                }
                else
                {
                    MessageBox.Show("Das Video enthält keinen verfügbaren Audio- oder Video-Stream.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    main.infoForm.infoBox.AppendText("Das Video enthält keinen verfügbaren Audio- oder Video-Stream.");
                    historyBox.Text += " - Fehler - " + Environment.NewLine;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ein Fehler ist aufgetreten: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                main.infoForm.infoBox.AppendText($"Ein Fehler ist aufgetreten: {ex.Message}");
                historyBox.Text += " - Fehler - " + Environment.NewLine;
            }
        }

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

        private void UpdateProgress(double progress, string totalSize, long totalBytes, bool isVideo)
        {
            double currentProgress = isVideo ? progress / 2 : 0.5 + progress / 2;
            long currentSize = (long)(totalBytes * currentProgress);

            string text = $"Herruntergeladen: {main.FormatBytes(currentSize)} / Größe: {totalSize} | Gesamtfortschritt: {currentProgress * 100:n2}%";

            if (currentSizeLb.InvokeRequired)  // If access to the UI element is needed
            {
                currentSizeLb.Invoke((MethodInvoker)delegate { currentSizeLb.Text = text; });
                progressBar.Invoke((MethodInvoker)delegate { progressBar.Value = (int)(currentProgress * 100); });
            }
            else
            {
                currentSizeLb.Text = text;
                progressBar.Value = (int)(currentProgress * 100);
            }
        }
    }
}
