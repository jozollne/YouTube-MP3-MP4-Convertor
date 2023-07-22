using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExplode.Videos.Streams;

namespace Youtube_Videos_Herrunterladen
{
    internal class Utilityclass
    {
        private readonly Main main;
        private readonly ComboBox mp4QualityComboBox;
        private readonly TextBox linkBox;
        private readonly Label idLb;
        private readonly Label uploadDateLb;
        private readonly Label mp4SizeLb;
        private readonly Label mp3SizeLb;
        private readonly PictureBox thumbnailPicBox;
        private readonly Label titleLb;
        private readonly Label durationLb;
        private readonly Label channelLb;


        public Utilityclass(Main main, ComboBox mp4QualityComboBox, TextBox linkBox, Label idLb, Label uploadDateLb, Label mp4SizeLb, Label mp3SizeLb, PictureBox thumbnailPicBox, Label titleLb, Label durationLb, Label channelLb) 
        {
            this.mp4QualityComboBox = mp4QualityComboBox;
            this.linkBox = linkBox;
            this.idLb = idLb;
            this.uploadDateLb = uploadDateLb;
            this.mp4SizeLb = mp4SizeLb;
            this.mp3SizeLb = mp3SizeLb;
            this.thumbnailPicBox = thumbnailPicBox;
            this.titleLb = titleLb;
            this.durationLb = durationLb;
            this.channelLb = channelLb;
            this.main = main;
        }
        public void GetMp3FormatAndConvert(string tempAudioFilePath, string finalAudioFilePath)
        {
            using (FileStream fs = new FileStream(tempAudioFilePath, FileMode.Open, FileAccess.Read))  // Öffnen der Datei im FileStream
            {
                // Lesen der ersten 12 Bytes der Datei
                byte[] bytes = new byte[12];
                fs.Read(bytes, 0, 12);
                string hex = BitConverter.ToString(bytes).Replace("-", "").ToUpper();
                string format;

                if (hex.StartsWith("494433") || hex.StartsWith("FFFB"))  // Überprüfen des Audio-Stream-Formats anhand der ersten Bytes der Datei
                    format = ".mp3"; // MP3-Format erkannt
                else if (hex.StartsWith("52494646"))
                    format = ".wav"; // WAV-Format erkannt
                else if (hex.StartsWith("4F676753"))
                    format = ".ogg"; // OGG-Format erkannt
                else if (hex.StartsWith("664C6143"))
                    format = ".flac"; // FLAC-Format erkannt
                else if (hex.StartsWith("0000001C667479704D3441"))
                    format = ".m4a"; // M4A-Format erkannt
                else if (hex.StartsWith("1A45DFA3"))
                    format = ".webm"; // WebM-Format erkannt
                else
                    format = "Dateiformat konnte nicht erkannt werden"; // Unbekanntes Format

                main.infoForm.infoBox.AppendText("Das Format des Audiostrerams ist: " + format + Environment.NewLine);  // Ausgabe des erkannten Formats in ein Info-Fenster

                if (format != ".mp3")  // Wenn das Format nicht MP3 ist, wird es in MP3 umgewandelt
                {
                    main.infoForm.infoBox.AppendText("Das Format des Audiostrems wird von: " + format + " in: .mp3 umgewandelt" + Environment.NewLine);  // Ausgabe der Umwandlungsinformation in das Info-Fenster

                    // Starten von ffmpeg.exe als Prozess mit den entsprechenden Argumenten zur Umwandlung in MP3
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            // Dateiname der ausführbaren Datei, die für die Audiokonvertierung verwendet wird (ffmpeg.exe)
                            FileName = "ffmpeg.exe",

                            // Argumente für den ffmpeg.exe-Befehl:
                            // -i "{tempAudioFilePath}": Eingabedatei, die überprüft und konvertiert werden soll (temporäre Audiodatei)
                            // -vn: "Video Null", schließt das Video vom Eingabestream aus (nur Audio wird bearbeitet)
                            // -ar 44100: Abtastrate für die Audioausgabe auf 44100 Hz (typische Abtastrate für Audiodateien)
                            // -ac 2: Anzahl der Audiokanäle auf 2 festlegen (stereo)
                            // -b:a 192k: Bitrate für die Audioausgabe auf 192 kbit/s setzen (bessere Audioqualität, aber größere Dateigröße)
                            // "{finalAudioFilePath}": Ausgabedatei, in die die konvertierte Audiodatei geschrieben wird (Pfad durch "finalAudioFilePath" angegeben)
                            Arguments = $"-i \"{tempAudioFilePath}\" -vn -ar 44100 -ac 2 -b:a 192k \"{finalAudioFilePath}\"",

                            // Verhindert die Verwendung der Shell zum Ausführen des Prozesses (Sicherheitsmaßnahme)
                            UseShellExecute = false,

                            // Leitet die Ausgabe des Prozesses (falls vorhanden) in den Standardausgabestream des Prozesses um
                            RedirectStandardOutput = true,

                            // Legt fest, dass kein neues Fenster erstellt werden soll, wenn der Prozess gestartet wird (läuft im Hintergrund)
                            CreateNoWindow = true

                        }
                    };
                    process.Start();
                    process.WaitForExit();

                    main.infoForm.infoBox.AppendText("Das Format wurde von: " + format + " in: .mp3 umgewandelt" + Environment.NewLine);  // Ausgabe der Erfolgsmeldung in das Info-Fenster
                }
            }
        }

        public string FormatBytes(long bytes)
        {
            const int scale = 1024;  // Define the scale for formatting the bytes
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };  // Define the suffixes for each byte scale

            int suffixIndex = 0;  // Initialize the suffix index
            decimal number = bytes;  // Store the byte count as a decimal for formatting

            while (number >= scale && suffixIndex < suffixes.Length - 1)  // While the number of bytes is greater than or equal to the scale and there is another suffix
            {
                number /= scale;  // Divide the number by the scale
                suffixIndex++;  // Increment the suffix index
            }

            return $"{number:n2} {suffixes[suffixIndex]}";  // Return the formatted number and corresponding suffix
        }

        public IStreamInfo GetMp4VideoSize(StreamManifest streamManifest)
        {
            string cleanedValue;

            if (mp4QualityComboBox.SelectedItem == null) // Sets 144p to the default quality when there is no quality selecet. aka at the first link
            {
                cleanedValue = "144";
            }
            else
            {
                string selectedValue = mp4QualityComboBox.Text;
                int index = selectedValue.IndexOf('p');
                cleanedValue = selectedValue.Substring(0, index);
            }

            var videoStreamInfo = streamManifest.GetVideoStreams()
                .Where(s => s.VideoResolution.Height == Convert.ToInt32(cleanedValue))
                .OrderByDescending(s => s.Bitrate)
                .FirstOrDefault();

            return videoStreamInfo;
        }

        public async Task GetStreamInfos()
        {
            try
            {
                main.uri = new Uri(linkBox.Text);
                main.streamId = main.uri.Query.TrimStart('?').Split('&')[0].Substring(2);
                main.stream = await main.youtube.Videos.GetAsync(main.streamId);
                main.streamManifest = await main.youtube.Videos.Streams.GetManifestAsync(main.streamId);
                main.audioStreamInfo = main.streamManifest.GetAudioOnlyStreams().TryGetWithHighestBitrate();
                main.videoStreamInfo = GetMp4VideoSize(main.streamManifest);
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
                main.uri = null;
                main.streamId = null;
                main.stream = null;
                main.streamManifest = null;
                main.audioStreamInfo = null;
                main.videoStreamInfo = null;
            }
        }
    }
}
