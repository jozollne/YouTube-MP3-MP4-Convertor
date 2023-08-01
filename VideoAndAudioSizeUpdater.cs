using System;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExplode.Videos.Streams;
using static System.Net.WebRequestMethods;

namespace Youtube_Videos_Herrunterladen
{
    internal class VideoAndAudioSizeUpdater
    {
        private readonly MainForm mainForm;
        private readonly ComboBox mp4QualityComboBox;
        private readonly Label mp4SizeLb;
        private readonly Label mp3SizeLb;
        private readonly Utilityclass utilityclass;

        public VideoAndAudioSizeUpdater(Utilityclass utilityclass, MainForm mainForm,ComboBox mp4QualityComboBox, Label mp4SizeLb, Label mp3SizeLb)
        {
            this.mp4QualityComboBox = mp4QualityComboBox;
            this.mp4SizeLb = mp4SizeLb;
            this.mp3SizeLb = mp3SizeLb;
            this.mainForm = mainForm;
            this.utilityclass = utilityclass;
        }

        public void UpdateVideoAndAudioSize()
        {
            try
            {
                var uniqueVideoStreamInfo = mainForm.streamManifest.GetVideoOnlyStreams()
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
                long totalAudioBytes = mainForm.audioStreamInfo.Size.Bytes;
                string totalAudioSize = utilityclass.FormatBytes(totalAudioBytes);
                
                long totalVideoBytes = mainForm.videoStreamInfo.Size.Bytes;
                string totalVideoSize = utilityclass.FormatBytes(totalVideoBytes);
                mp4SizeLb.Text = $".mp4 Größe: {totalVideoSize}";
                mp3SizeLb.Text = $".mp3 Größe: {totalAudioSize}";
            }
            catch
            {

            }
        }
    }
}
