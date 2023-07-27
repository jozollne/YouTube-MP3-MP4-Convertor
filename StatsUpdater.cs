using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExplode.Common;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;
using static System.Net.WebRequestMethods;

namespace Youtube_Videos_Herrunterladen
{
    internal class StatsUpdater
    {
        private readonly Utilityclass utilityclass;
        private readonly Main main;
        private readonly Label titleLb;
        private readonly Label durationLb;
        private readonly Label mp4SizeLb;
        private readonly Label mp3SizeLb;
        private readonly PictureBox thumbnailPicBox; 
        private readonly Label channelLb;
        private readonly Label idLb;
        private readonly Label uploadDateLb;
        private readonly ComboBox mp4QualityComboBox;

        public StatsUpdater(Utilityclass utilityclass, Main main, Label titleLb, Label durationLb, Label mp4SizeLb, Label mp3SizeLb, PictureBox thumbnailPicBox, Label channelLb, Label idLb, Label uploadDateLb, ComboBox mp4QualityComboBox)
        {
            this.titleLb = titleLb;
            this.durationLb = durationLb;
            this.mp4SizeLb = mp4SizeLb;
            this.mp3SizeLb = mp3SizeLb;
            this.thumbnailPicBox = thumbnailPicBox;
            this.channelLb = channelLb;
            this.idLb = idLb;
            this.uploadDateLb = uploadDateLb;
            this.mp4QualityComboBox = mp4QualityComboBox;
            this.main = main;
            this.utilityclass = utilityclass;
        }

        // Asynchronous method to update the video statistics
        public async Task UpdateVideoStatsAsync()
        {
            // Initialization of a variable to store the current video ID

            try
            {
                VideoAndAudioSizeUpdater videoAndAudioSizeUpdater = new VideoAndAudioSizeUpdater(utilityclass, main, mp4QualityComboBox, mp4SizeLb, mp3SizeLb);
                videoAndAudioSizeUpdater.UpdateVideoAndAudioSize();

                // Updating the corresponding Labels in the main window with the video information
                titleLb.Text = "Titel: \"" + main.stream.Title + "\"";
                idLb.Text = "ID: " + main.stream.Id;
                channelLb.Text = $"Kanal: {main.stream.Author}";
                DateTimeOffset uploadDateOffset = main.stream.UploadDate;
                DateTime uploadDate = uploadDateOffset.DateTime;
                string uploadDateString = uploadDate.ToShortDateString();
                uploadDateLb.Text = $"Hochgeladen: {uploadDateString}";
                durationLb.Text = $"Dauer: {main.stream.Duration}";
                try
                {
                    var thumbnails = main.stream.Thumbnails.OrderByDescending(t => t.Resolution.Width * t.Resolution.Height);

                    // Get the thumbnail
                    foreach (var thumbnail in thumbnails)
                    {
                        try
                        {
                            using (var client = new WebClient())
                            {
                                var data = await client.DownloadDataTaskAsync(thumbnail.Url);

                                using (var mem = new MemoryStream(data))
                                {
                                    var tempImage = Image.FromStream(mem);
                                    // Create a new image from the tempImage
                                    main.image = new Bitmap(tempImage);
                                    // Display image in PictureBox
                                    thumbnailPicBox.Image = main.image;
                                }

                                // If we got here, it means we've successfully downloaded and processed a thumbnail
                                // So we break the loop and stop trying
                                break;
                            }
                        }
                        catch { }
                    }
                }
                catch (Exception ex)
                {
                    // Handle any errors that might have occurred
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
            catch { }
        }
    }
}
