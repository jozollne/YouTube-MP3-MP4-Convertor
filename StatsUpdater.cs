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


                // Get the thumbnail
                try
                {
                    var thumbnail = main.stream.Thumbnails.OrderByDescending(t => t.Resolution.Width * t.Resolution.Height).FirstOrDefault();  // Get the highest resolution thumbnail

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
