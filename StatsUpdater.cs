using System;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExplode.Videos.Streams;

namespace downloader
{
    internal class StatsUpdater
    {
        Main main = new Main();
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

        public StatsUpdater(TextBox linkBox, Label titleLb, Label durationLb, Label mp4SizeLb, Label mp3SizeLb, Label mp4QualityLB, PictureBox thumbnailPicBox, Label channelLb, Label idLb, Label uploadDateLb)
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
        }

        // Asynchronous method to update the video statistics
        public async Task UpdateVideoStatsAsync()
        {
            // Initialization of a variable to store the current video ID
            string currentVideoId = "";
            var videoUrl = linkBox.Text;

            try
            {
                var uri = new Uri(videoUrl);
                var videoId = uri.Query.TrimStart('?').Split('&')[0].Substring(2);

                if (currentVideoId != videoId)
                {
                    currentVideoId = videoId;

                    // Getting video information using the YoutubeExplode library
                    var video = await main.youtube.Videos.GetAsync(videoId);

                    // Getting stream information for video and audio
                    var streamManifest = await main.youtube.Videos.Streams.GetManifestAsync(videoId);
                    var videoStreamInfo = streamManifest.GetVideoOnlyStreams().GetWithHighestVideoQuality();
                    var audioStreamInfo = streamManifest.GetAudioOnlyStreams().TryGetWithHighestBitrate();

                    if (videoStreamInfo != null && audioStreamInfo != null)
                    {
                        // Calculating the size of video and audio streams in bytes and converting to a readable format
                        long totalVideoBytes = videoStreamInfo.Size.Bytes;
                        string totalVideoSize = main.FormatBytes(totalVideoBytes);

                        long totalAudioBytes = audioStreamInfo.Size.Bytes;
                        string totalAudioSize = main.FormatBytes(totalAudioBytes);

                        // Updating the corresponding Labels in the main window with the video information
                        titleLb.Text = "Title: \"" + video.Title + "\"";
                        idLb.Text = $"ID: {video.Id}";
                        channelLb.Text = $"Channel: {video.Author}";
                        DateTimeOffset uploadDateOffset = video.UploadDate;
                        DateTime uploadDate = uploadDateOffset.DateTime;
                        string uploadDateString = uploadDate.ToShortDateString();
                        uploadDateLb.Text = $"Uploaded: {uploadDateString}";
                        durationLb.Text = $"Duration: {video.Duration}";
                        mp4SizeLb.Text = $".mp4 Size: {totalVideoSize}";
                        mp3SizeLb.Text = $".mp3 Size: {totalAudioSize}";

                        // Display the video quality in the right format
                        string qualityLabel = Convert.ToString(videoStreamInfo.VideoQuality);
                        string qualityDescriptor = "";
                        if (qualityLabel.Contains("4320"))
                        {
                            qualityDescriptor = "8K";
                        }
                        else if (qualityLabel.Contains("2160"))
                        {
                            qualityDescriptor = "4K";
                        }
                        else if (qualityLabel.Contains("1080"))
                        {
                            qualityDescriptor = "HD";
                        }
                        else
                        {
                            qualityDescriptor = "SD";
                        }

                        mp4QualityLB.Text = $".mp4 Quality: {qualityLabel} ({qualityDescriptor})";

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
                }
            }
            catch
            {
                // Clear evrythig when the link is empty
                currentVideoId = null;
                videoUrl = null;
                titleLb.Text = "Title: No link";
                idLb.Text = "ID: No link";
                channelLb.Text = "Channel: No link";
                uploadDateLb.Text = "Uploaded: No link";
                durationLb.Text = "Duration: 00:00:00";
                mp4SizeLb.Text = ".mp4 Size: 0 MB";
                mp3SizeLb.Text = ".mp3 Size: 0 MB";
                mp4QualityLB.Text = ".mp4 Quality: No link";
                thumbnailPicBox.Image = null; 
            }

        }

        void PictureError()
        {
            string errorLoadingPicture = "The image could not be loaded!"; // Declare a variable or a string

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
