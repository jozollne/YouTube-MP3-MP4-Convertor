﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExplode.Videos.Streams;
using MediaToolkit;
using MediaToolkit.Model;
using static System.Net.WebRequestMethods;
using System.Drawing;
using System.Runtime.InteropServices;
using Youtube_Videos_Herrunterladen.Properties;

namespace Youtube_Videos_Herrunterladen
{
    internal class Utilityclass
    {
        private readonly MainForm mainForm;
        private readonly ComboBox mp4QualityComboBox;
        private readonly TextBox linkBox;
        private readonly Label idLb;
        private readonly Label uploadDateLb;
        private readonly Label mp4SizeLb;
        private readonly Label mp3SizeLb;
        private readonly Label titleLb;
        private readonly Label durationLb;
        private readonly Label channelLb;
        private readonly Label downloadSpeedLb;

        public Utilityclass(MainForm mainForm, ComboBox mp4QualityComboBox, TextBox linkBox, Label idLb, Label uploadDateLb, Label mp4SizeLb, Label mp3SizeLb, Label titleLb, Label durationLb, Label channelLb, Label downloadSpeedLb) 
        {
            this.mp4QualityComboBox = mp4QualityComboBox;
            this.linkBox = linkBox;
            this.idLb = idLb;
            this.uploadDateLb = uploadDateLb;
            this.mp4SizeLb = mp4SizeLb;
            this.mp3SizeLb = mp3SizeLb;
            this.titleLb = titleLb;
            this.durationLb = durationLb;
            this.channelLb = channelLb;
            this.mainForm = mainForm;
            this.downloadSpeedLb = downloadSpeedLb;
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

                mainForm.infoForm.infoBox.AppendText("Das Format des Audiostrerams ist: " + format + Environment.NewLine);  // Ausgabe des erkannten Formats in ein Info-Fenster

                if (format != ".mp3")  // Wenn das Format nicht MP3 ist, wird es in MP3 umgewandelt
                {
                    mainForm.infoForm.infoBox.AppendText("Das Format des Audiostrems wird von: " + format + " in: .mp3 umgewandelt" + Environment.NewLine);  // Ausgabe der Umwandlungsinformation in das Info-Fenster

                    var inputFile = new MediaFile { Filename = tempAudioFilePath};
                    var outputFile = new MediaFile { Filename = finalAudioFilePath };

                    using (var engine = new Engine())
                    {
                        engine.GetMetadata(inputFile);
                        engine.Convert(inputFile, outputFile);
                    }

                    mainForm.infoForm.infoBox.AppendText("Das Format wurde von: " + format + " in: .mp3 umgewandelt" + Environment.NewLine);  // Ausgabe der Erfolgsmeldung in das Info-Fenster
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

        public void AddHistoryLabel(string Title, string streamId)
        {
            var labels = mainForm.historyForm.Controls.OfType<Label>().ToList();
            int topPosition;

            if (labels.Count > 0)
            {
                var lastLabel = labels[labels.Count - 1]; // get the last label
                topPosition = lastLabel.Top + lastLabel.Height + 3; // calculate the new top position
            }
            else
            {
                topPosition = mainForm.historyForm.label1.Bottom + 3; // if it's the first label, position it under label1
            }

            Label label = new Label
            {
                Name = streamId,
                Text = Title,
                AutoSize = true,
                Margin = new Padding(3, 3, 3, 3),
                Top = topPosition,
                Left = mainForm.historyForm.label1.Left, // align the label with label1
                BackColor = Color.Transparent,
                ForeColor = Color.White
            };

            label.Click += (s, e) =>
            {
                linkBox.Text = "https://www.youtube.com/watch?v=" + streamId;
                mainForm.ActiveControl = linkBox;
                linkBox.ForeColor = Color.Black;
            };

            mainForm.historyForm.Controls.Add(label);

            mainForm.historyForm.Height = label.Bottom + 50; // adjust the form height based on the last label

            foreach (char c in Path.GetInvalidFileNameChars())
            {
                Title = Title.Replace(c, '_');
            }
        }

        // Deklarationen für die SetWindowPos Funktion
        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        public const uint SWP_NOSIZE = 0x0001;
        public const uint SWP_NOMOVE = 0x0002;
        public const uint SWP_NOACTIVATE = 0x0010;

        public void BringToFrontWithoutFocus(Form formToBringToFront, Form formToCheck)
        {
            if (formToCheck.Visible) // Wenn das zu überprüfende Formular sichtbar ist
            {
                // Bringt das zuerst genannte Formular nach vorne, ohne den Fokus zu ändern
                SetWindowPos(formToBringToFront.Handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
                SetWindowPos(formToBringToFront.Handle, HWND_NOTOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
            }
        }

        public async Task GetStreamInfos()
        {
            try
            {
                mainForm.uri = new Uri(linkBox.Text);
                mainForm.streamId = mainForm.uri.Query.TrimStart('?').Split('&')[0].Substring(2);
                mainForm.stream = await mainForm.youtube.Videos.GetAsync(mainForm.streamId);
                mainForm.streamManifest = await mainForm.youtube.Videos.Streams.GetManifestAsync(mainForm.streamId);
                mainForm.audioStreamInfo = mainForm.streamManifest.GetAudioOnlyStreams().TryGetWithHighestBitrate();
                mainForm.videoStreamInfo = GetMp4VideoSize(mainForm.streamManifest);
                mainForm.ToggleControlsSecurity(true);
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
                mainForm.uri = null;
                mainForm.streamId = null;
                mainForm.stream = null;
                mainForm.streamManifest = null;
                mainForm.audioStreamInfo = null;
                mainForm.videoStreamInfo = null;
                downloadSpeedLb.Text = "Geschwindigkeit: Kein Download";
                mainForm.ToggleControlsSecurity(false);
                mainForm.Width = 1241;
                mainForm.Height = 722;
                mainForm.BackgroundImage = Resources.defaultBackground;
            }
        }
    }
}
