using System;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExplode.Videos.Streams;
using static System.Net.WebRequestMethods;

namespace downloader
{
    internal class VideoAndAudioSizeUpdater
    {
        private Main main;
        private ComboBox mp4QualityComboBox;
        private Label mp4SizeLb;
        private Label mp3SizeLb;
        private TextBox linkBox;

        public VideoAndAudioSizeUpdater(Main main, ComboBox mp4QualityComboBox, Label mp4SizeLb, Label mp3SizeLb, TextBox linkBox)
        {
            this.main = main;
            this.mp4QualityComboBox = mp4QualityComboBox;
            this.mp4SizeLb = mp4SizeLb;
            this.mp3SizeLb = mp3SizeLb;
            this.linkBox = linkBox;
        }

        public async Task UpdateVideoAndAudioSize()
        {
            try
            {
                // Getting video information using the YoutubeExplode library
                var videoUrl = linkBox.Text;
                var uri = new Uri(videoUrl);
                var videoId = uri.Query.TrimStart('?').Split('&')[0].Substring(2);
                var video = await main.youtube.Videos.GetAsync(videoId);
                var streamManifest = await main.youtube.Videos.Streams.GetManifestAsync(videoId);
                var audioStreamInfo = streamManifest.GetAudioOnlyStreams().TryGetWithHighestBitrate();
                IStreamInfo videoStreamInfo = main.GetMp4VideoSize(streamManifest);

                var uniqueVideoStreamInfo = streamManifest.GetVideoOnlyStreams()
                    .GroupBy(s => s.VideoQuality)
                    .Select(g => g.First())
                    .OrderByDescending(s => s.VideoQuality);

                // Display the avalible Video streams in the ComboBox
                foreach (var streamInfo in uniqueVideoStreamInfo)
                {
                    string qualityDescriptor = "";
                    string qualityLabel = Convert.ToString(streamInfo.VideoQuality);

                    if (qualityLabel.Contains("4320"))
                    {
                        qualityDescriptor = "8K";
                    }
                    else if (qualityLabel.Contains("2160"))
                    {
                        qualityDescriptor = "4K";
                    }
                    else if (qualityLabel.Contains("1440"))
                    {
                        qualityDescriptor = "2K/QHD";
                    }
                    else if (qualityLabel.Contains("1080"))
                    {
                        qualityDescriptor = "Full-HD";
                    }
                    else if (qualityLabel.Contains("720"))
                    {
                        qualityDescriptor = "HD";
                    }
                    else if (qualityLabel.Contains("480"))
                    {
                        qualityDescriptor = "SD";
                    }
                    else if (qualityLabel.Contains("360"))
                    {
                        qualityDescriptor = "nHD";
                    }
                    else if (qualityLabel.Contains("240"))
                    {
                        qualityDescriptor = "FQVGA";
                    }
                    else if (qualityLabel.Contains("144"))
                    {
                        qualityDescriptor = "Low";
                    }
                    if (!mp4QualityComboBox.Items.Contains($"{qualityLabel} ({qualityDescriptor})")) // Check if the5 video quality entry exists. If so dont add a new entry
                        mp4QualityComboBox.Items.Add($"{qualityLabel} ({qualityDescriptor})");
                    if (mp4QualityComboBox.SelectedIndex == -1)
                        mp4QualityComboBox.SelectedItem = "144p (Low)";  // Sets Low as default quality
                }

                // Display the Video and Audio size to the label
                long totalAudioBytes = audioStreamInfo.Size.Bytes;
                string totalAudioSize = main.FormatBytes(totalAudioBytes);
                
                long totalVideoBytes = videoStreamInfo.Size.Bytes;
                string totalVideoSize = main.FormatBytes(totalVideoBytes);
                mp4SizeLb.Text = $".mp4 Größe: {totalVideoSize}";
                mp3SizeLb.Text = $".mp3 Größe: {totalAudioSize}";
            }
            catch
            {
                mp4SizeLb.Text = $".mp4 Größe: 0 MB";
                mp3SizeLb.Text = $".mp3 Größe: 0 MB";
            }
        }
    }
}
