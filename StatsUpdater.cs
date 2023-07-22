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
    internal class StatsUpdater
    {
        private Main main;
        private TextBox linkBox;
        private Label titleLb;
        private Label durationLb;
        private Label mp4SizeLb;
        private Label mp3SizeLb;
        private Label mp4QualityLB;
        private PictureBox thumbnailPicBox; 
        private Label channelLb;
        private Label idLb;
        private Label uploadDateLb;
        private ComboBox mp4QualityComboBox;

        public StatsUpdater(Main main, TextBox linkBox, Label titleLb, Label durationLb, Label mp4SizeLb, Label mp3SizeLb, Label mp4QualityLB, PictureBox thumbnailPicBox, Label channelLb, Label idLb, Label uploadDateLb, ComboBox mp4QualityComboBox)
        {
            this.linkBox = linkBox;
            this.titleLb = titleLb;
            this.durationLb = durationLb;
            this.mp4SizeLb = mp4SizeLb;
            this.mp3SizeLb = mp3SizeLb;
            this.mp4QualityLB = mp4QualityLB;
            this.thumbnailPicBox = thumbnailPicBox;
            this.channelLb = channelLb;
            this.idLb = idLb;
            this.uploadDateLb = uploadDateLb;
            this.mp4QualityComboBox = mp4QualityComboBox;
            this.main = main;
        }

        // Asynchronous method to update the video statistics
        public async Task UpdateVideoStatsAsync()
        {
            // Initialization of a variable to store the current video ID

            try
            {
                // Getting video information using the YoutubeExplode library
                var videoUrl = linkBox.Text;
                var uri = new Uri(videoUrl);
                var videoId = uri.Query.TrimStart('?').Split('&')[0].Substring(2);
                var video = await main.youtube.Videos.GetAsync(videoId);
                var streamManifest = await main.youtube.Videos.Streams.GetManifestAsync(videoId);
                var videoStreamInfo = streamManifest.GetVideoOnlyStreams().GetWithHighestVideoQuality();
                var audioStreamInfo = streamManifest.GetAudioOnlyStreams().TryGetWithHighestBitrate();
                IStreamInfo videoStreamInfoForSize = main.GetMp4VideoSize(streamManifest);

                VideoAndAudioSizeUpdater videoAndAudioSizeUpdater = new VideoAndAudioSizeUpdater(main, mp4QualityComboBox, mp4SizeLb, mp3SizeLb, linkBox);
                await videoAndAudioSizeUpdater.UpdateVideoAndAudioSize();

                // Updating the corresponding Labels in the main window with the video information
                titleLb.Text = "Titel: \"" + video.Title + "\"";
                idLb.Text = "ID: " + video.Id;
                channelLb.Text = $"Kanal: {video.Author}";
                DateTimeOffset uploadDateOffset = video.UploadDate;
                DateTime uploadDate = uploadDateOffset.DateTime;
                string uploadDateString = uploadDate.ToShortDateString();
                uploadDateLb.Text = $"Hochgeladen: {uploadDateString}";
                durationLb.Text = $"Dauer: {video.Duration}";


                // Get the thumbnail
                try
                {
                    var thumbnail = video.Thumbnails.OrderByDescending(t => t.Resolution.Width * t.Resolution.Height).FirstOrDefault();  // Get the highest resolution thumbnail

                    if (thumbnail != null)
                    {
                        string thumbnailUrl = thumbnail.Url;  // Get the thumbnail URL

                        using (var httpClient = new HttpClient())  // Create a new HttpClient for HTTP communications
                        {
                            var thumbnailResponse = await httpClient.GetAsync(thumbnailUrl);  // Send a GET request to the thumbnail URL

                            if (thumbnailResponse.IsSuccessStatusCode)  // Check if the response was successful
                            {
                                var thumbnailStream = await thumbnailResponse.Content.ReadAsStreamAsync();  // Get the response content as a Stream

                                if (thumbnailPicBox.InvokeRequired)  // Check if the PictureBox control's InvokeRequired property is true
                                {
                                    thumbnailPicBox.Invoke(new MethodInvoker(delegate  // Use the Invoke method to update the PictureBox control
                                    {
                                        thumbnailPicBox.Image = Image.FromStream(thumbnailStream);  // Load the thumbnail image from the Stream
                                    }));
                                }
                                else
                                {
                                    thumbnailPicBox.Image = Image.FromStream(thumbnailStream);  // Load the thumbnail image from the Stream
                                }
                            }
                        }
                    }
                }
                catch
                {
                    PictureError();
                }
                
                
            }
            catch
            {
                // Clear evrythig when the link is empty
                titleLb.Text = "Titel: Kein Link";
                idLb.Text = "ID: Kein Link";
                channelLb.Text = "Kanal: Kein Link";
                uploadDateLb.Text = "Hochgeladen: Kein Link";
                durationLb.Text = "Dauer: 00:00:00";
                mp4SizeLb.Text = ".mp4 Größe: 0 MB";
                mp3SizeLb.Text = ".mp3 Größe: 0 MB";
                thumbnailPicBox.Image = null;
            }

        }

        void PictureError()
        {
            string errorLoadingPicture = "Das Thumbnail konte nicht geladen werden!"; // Declare a variable or a string

            Bitmap picture = new Bitmap(thumbnailPicBox.Width, thumbnailPicBox.Height); // Create a Bitmap image object

            using (Graphics g = Graphics.FromImage(picture)) // Create a Graphics object from the image
            {
                Font textFont = new Font("Calibri", 15); // Define the font and text color
                Brush textColor = Brushes.Black;

                SizeF textSize = g.MeasureString(errorLoadingPicture.ToString(), textFont); // Measure the size of the text

                float x = (thumbnailPicBox.Width - textSize.Width) / 2; // Calculate the position of the text to center it
                float y = (thumbnailPicBox.Height - textSize.Height) / 2;

                g.DrawString(errorLoadingPicture, textFont, textColor, new PointF(x, y)); // Draw the string on the image
            }

            thumbnailPicBox.Image = picture; // Display the image in the PictureBox
        }
    }
}
