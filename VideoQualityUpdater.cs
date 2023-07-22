using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode.Videos.Streams;

namespace Youtube_Videos_Herrunterladen
{
    internal class VideoQualityUpdater
    {
        /*public VideoQualityUpdater() 
        {
            var uniqueStreamInfos = streamManifest.GetVideoOnlyStreams()
                .GroupBy(s => s.VideoQuality)
                .Select(g => g.First())
                .OrderByDescending(s => s.VideoQuality);

            // Display the avalible Video streams in the ComboBox
            foreach (var streamInfo in uniqueStreamInfos)
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
        }*/
    }
}
