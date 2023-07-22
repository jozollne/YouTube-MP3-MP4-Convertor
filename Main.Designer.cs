﻿namespace downloader
{
    partial class Main
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.linkBox = new System.Windows.Forms.TextBox();
            this.downloadMp4Bt = new System.Windows.Forms.Button();
            this.subLb1 = new System.Windows.Forms.Label();
            this.linkLb = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.mp4SizeLb = new System.Windows.Forms.Label();
            this.currentSizeLb = new System.Windows.Forms.Label();
            this.titelLb = new System.Windows.Forms.Label();
            this.durationLb = new System.Windows.Forms.Label();
            this.usbSticksPanel = new System.Windows.Forms.Panel();
            this.subLb2 = new System.Windows.Forms.Label();
            this.selectFolderButton = new System.Windows.Forms.Button();
            this.downloadMp3Bt = new System.Windows.Forms.Button();
            this.mp3SizeLb = new System.Windows.Forms.Label();
            this.mp4QualityLb = new System.Windows.Forms.Label();
            this.thumbnailLb = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.chanelLb = new System.Windows.Forms.Label();
            this.idLb = new System.Windows.Forms.Label();
            this.uploadDateLb = new System.Windows.Forms.Label();
            this.historyBox = new System.Windows.Forms.TextBox();
            this.showInfoFormBt = new System.Windows.Forms.Button();
            this.thumbnailPicBox = new System.Windows.Forms.PictureBox();
            this.mp4QualityComboBox = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.thumbnailPicBox)).BeginInit();
            this.SuspendLayout();
            // 
            // linkBox
            // 
            this.linkBox.Font = new System.Drawing.Font("Arial Narrow", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkBox.Location = new System.Drawing.Point(20, 53);
            this.linkBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.linkBox.Name = "linkBox";
            this.linkBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.linkBox.Size = new System.Drawing.Size(845, 32);
            this.linkBox.TabIndex = 0;
            // 
            // downloadMp4Bt
            // 
            this.downloadMp4Bt.Location = new System.Drawing.Point(630, 642);
            this.downloadMp4Bt.Name = "downloadMp4Bt";
            this.downloadMp4Bt.Size = new System.Drawing.Size(211, 33);
            this.downloadMp4Bt.TabIndex = 3;
            this.downloadMp4Bt.Text = ".mp4 Herrunterladen";
            this.downloadMp4Bt.UseVisualStyleBackColor = true;
            this.downloadMp4Bt.Click += new System.EventHandler(this.DownloadMp4Bt_Click);
            // 
            // subLb1
            // 
            this.subLb1.AutoSize = true;
            this.subLb1.Font = new System.Drawing.Font("Calibri", 24F, System.Drawing.FontStyle.Bold);
            this.subLb1.Location = new System.Drawing.Point(13, 600);
            this.subLb1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.subLb1.Name = "subLb1";
            this.subLb1.Size = new System.Drawing.Size(178, 39);
            this.subLb1.TabIndex = 2;
            this.subLb1.Text = "Speicherort:";
            // 
            // linkLb
            // 
            this.linkLb.AutoSize = true;
            this.linkLb.Font = new System.Drawing.Font("Calibri", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLb.Location = new System.Drawing.Point(13, 9);
            this.linkLb.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkLb.Name = "linkLb";
            this.linkLb.Size = new System.Drawing.Size(866, 39);
            this.linkLb.TabIndex = 4;
            this.linkLb.Text = "Link: (z.B. https://www.youtube.com/watch?v=6WRLynWxVKg)";
            // 
            // progressBar
            // 
            this.progressBar.BackColor = System.Drawing.Color.White;
            this.progressBar.Location = new System.Drawing.Point(20, 93);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(845, 23);
            this.progressBar.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 24F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(13, 148);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 39);
            this.label1.TabIndex = 6;
            this.label1.Text = "Info:";
            // 
            // mp4SizeLb
            // 
            this.mp4SizeLb.AutoSize = true;
            this.mp4SizeLb.Location = new System.Drawing.Point(20, 347);
            this.mp4SizeLb.Name = "mp4SizeLb";
            this.mp4SizeLb.Size = new System.Drawing.Size(169, 26);
            this.mp4SizeLb.TabIndex = 7;
            this.mp4SizeLb.Text = ".mp4 Größe: 0 MB";
            // 
            // currentSizeLb
            // 
            this.currentSizeLb.AutoSize = true;
            this.currentSizeLb.Location = new System.Drawing.Point(15, 119);
            this.currentSizeLb.Name = "currentSizeLb";
            this.currentSizeLb.Size = new System.Drawing.Size(550, 26);
            this.currentSizeLb.TabIndex = 8;
            this.currentSizeLb.Text = "Herruntergeladen: 0 MB / Größe: 0 MB | Gesamtfortschritt: 0%";
            // 
            // titelLb
            // 
            this.titelLb.AutoSize = true;
            this.titelLb.Location = new System.Drawing.Point(20, 191);
            this.titelLb.Name = "titelLb";
            this.titelLb.Size = new System.Drawing.Size(137, 26);
            this.titelLb.TabIndex = 9;
            this.titelLb.Text = "Titel: Kein Link";
            // 
            // durationLb
            // 
            this.durationLb.AutoSize = true;
            this.durationLb.Location = new System.Drawing.Point(20, 295);
            this.durationLb.Name = "durationLb";
            this.durationLb.Size = new System.Drawing.Size(152, 26);
            this.durationLb.TabIndex = 10;
            this.durationLb.Text = "Dauer: 00:00:00";
            // 
            // usbSticksPanel
            // 
            this.usbSticksPanel.Location = new System.Drawing.Point(25, 478);
            this.usbSticksPanel.Name = "usbSticksPanel";
            this.usbSticksPanel.Size = new System.Drawing.Size(235, 120);
            this.usbSticksPanel.TabIndex = 11;
            // 
            // subLb2
            // 
            this.subLb2.AutoSize = true;
            this.subLb2.Font = new System.Drawing.Font("Calibri", 24F, System.Drawing.FontStyle.Bold);
            this.subLb2.Location = new System.Drawing.Point(18, 439);
            this.subLb2.Name = "subLb2";
            this.subLb2.Size = new System.Drawing.Size(165, 39);
            this.subLb2.TabIndex = 12;
            this.subLb2.Text = "USB-Sticks:";
            // 
            // selectFolderButton
            // 
            this.selectFolderButton.Location = new System.Drawing.Point(12, 642);
            this.selectFolderButton.Name = "selectFolderButton";
            this.selectFolderButton.Size = new System.Drawing.Size(183, 33);
            this.selectFolderButton.TabIndex = 1;
            this.selectFolderButton.Text = "Speicherort ändern";
            this.selectFolderButton.UseVisualStyleBackColor = true;
            this.selectFolderButton.Click += new System.EventHandler(this.selectFolderButton_Click);
            // 
            // downloadMp3Bt
            // 
            this.downloadMp3Bt.Location = new System.Drawing.Point(419, 642);
            this.downloadMp3Bt.Name = "downloadMp3Bt";
            this.downloadMp3Bt.Size = new System.Drawing.Size(205, 33);
            this.downloadMp3Bt.TabIndex = 2;
            this.downloadMp3Bt.Text = ".mp3 Herrunterladen";
            this.downloadMp3Bt.UseVisualStyleBackColor = true;
            this.downloadMp3Bt.Click += new System.EventHandler(this.DownloadMp3Bt_Click);
            // 
            // mp3SizeLb
            // 
            this.mp3SizeLb.AutoSize = true;
            this.mp3SizeLb.Location = new System.Drawing.Point(20, 321);
            this.mp3SizeLb.Name = "mp3SizeLb";
            this.mp3SizeLb.Size = new System.Drawing.Size(169, 26);
            this.mp3SizeLb.TabIndex = 15;
            this.mp3SizeLb.Text = ".mp3 Größe: 0 MB";
            // 
            // mp4QualityLb
            // 
            this.mp4QualityLb.AutoSize = true;
            this.mp4QualityLb.Location = new System.Drawing.Point(20, 373);
            this.mp4QualityLb.Name = "mp4QualityLb";
            this.mp4QualityLb.Size = new System.Drawing.Size(136, 26);
            this.mp4QualityLb.TabIndex = 16;
            this.mp4QualityLb.Text = ".mp4 Qualität:";
            // 
            // thumbnailLb
            // 
            this.thumbnailLb.AutoSize = true;
            this.thumbnailLb.BackColor = System.Drawing.Color.Transparent;
            this.thumbnailLb.Font = new System.Drawing.Font("Calibri", 24F, System.Drawing.FontStyle.Bold);
            this.thumbnailLb.Location = new System.Drawing.Point(323, 148);
            this.thumbnailLb.Name = "thumbnailLb";
            this.thumbnailLb.Size = new System.Drawing.Size(135, 39);
            this.thumbnailLb.TabIndex = 19;
            this.thumbnailLb.Text = "Titelbild:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Calibri", 24F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(1071, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 39);
            this.label2.TabIndex = 20;
            this.label2.Text = "Verlauf:";
            // 
            // chanelLb
            // 
            this.chanelLb.AutoSize = true;
            this.chanelLb.Location = new System.Drawing.Point(20, 243);
            this.chanelLb.Name = "chanelLb";
            this.chanelLb.Size = new System.Drawing.Size(147, 26);
            this.chanelLb.TabIndex = 22;
            this.chanelLb.Text = "Kanal: Kein Link";
            // 
            // idLb
            // 
            this.idLb.AutoSize = true;
            this.idLb.Location = new System.Drawing.Point(20, 217);
            this.idLb.Name = "idLb";
            this.idLb.Size = new System.Drawing.Size(118, 26);
            this.idLb.TabIndex = 23;
            this.idLb.Text = "ID: Kein Link";
            // 
            // uploadDateLb
            // 
            this.uploadDateLb.AutoSize = true;
            this.uploadDateLb.Location = new System.Drawing.Point(20, 269);
            this.uploadDateLb.Name = "uploadDateLb";
            this.uploadDateLb.Size = new System.Drawing.Size(211, 26);
            this.uploadDateLb.TabIndex = 24;
            this.uploadDateLb.Text = "Hochgeladen: Kein Link";
            // 
            // historyBox
            // 
            this.historyBox.Location = new System.Drawing.Point(872, 49);
            this.historyBox.Multiline = true;
            this.historyBox.Name = "historyBox";
            this.historyBox.ReadOnly = true;
            this.historyBox.Size = new System.Drawing.Size(502, 627);
            this.historyBox.TabIndex = 26;
            // 
            // showInfoFormBt
            // 
            this.showInfoFormBt.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.showInfoFormBt.Image = ((System.Drawing.Image)(resources.GetObject("showInfoFormBt.Image")));
            this.showInfoFormBt.Location = new System.Drawing.Point(834, 117);
            this.showInfoFormBt.Name = "showInfoFormBt";
            this.showInfoFormBt.Size = new System.Drawing.Size(31, 31);
            this.showInfoFormBt.TabIndex = 25;
            this.showInfoFormBt.UseVisualStyleBackColor = true;
            this.showInfoFormBt.Click += new System.EventHandler(this.showInfoFormBt_Click);
            // 
            // thumbnailPicBox
            // 
            this.thumbnailPicBox.Location = new System.Drawing.Point(330, 217);
            this.thumbnailPicBox.Name = "thumbnailPicBox";
            this.thumbnailPicBox.Size = new System.Drawing.Size(511, 332);
            this.thumbnailPicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.thumbnailPicBox.TabIndex = 18;
            this.thumbnailPicBox.TabStop = false;
            // 
            // mp4QualityComboBox
            // 
            this.mp4QualityComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mp4QualityComboBox.FormattingEnabled = true;
            this.mp4QualityComboBox.Location = new System.Drawing.Point(20, 402);
            this.mp4QualityComboBox.Name = "mp4QualityComboBox";
            this.mp4QualityComboBox.Size = new System.Drawing.Size(222, 34);
            this.mp4QualityComboBox.TabIndex = 27;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 26F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(1386, 688);
            this.Controls.Add(this.mp4QualityComboBox);
            this.Controls.Add(this.historyBox);
            this.Controls.Add(this.showInfoFormBt);
            this.Controls.Add(this.uploadDateLb);
            this.Controls.Add(this.idLb);
            this.Controls.Add(this.chanelLb);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.thumbnailLb);
            this.Controls.Add(this.mp4QualityLb);
            this.Controls.Add(this.mp3SizeLb);
            this.Controls.Add(this.downloadMp3Bt);
            this.Controls.Add(this.selectFolderButton);
            this.Controls.Add(this.subLb2);
            this.Controls.Add(this.usbSticksPanel);
            this.Controls.Add(this.durationLb);
            this.Controls.Add(this.titelLb);
            this.Controls.Add(this.currentSizeLb);
            this.Controls.Add(this.mp4SizeLb);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.linkLb);
            this.Controls.Add(this.subLb1);
            this.Controls.Add(this.downloadMp4Bt);
            this.Controls.Add(this.linkBox);
            this.Controls.Add(this.thumbnailPicBox);
            this.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Main";
            this.Text = "Youtube Videos Herrunterladen";
            ((System.ComponentModel.ISupportInitialize)(this.thumbnailPicBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox linkBox;
        private System.Windows.Forms.Button downloadMp4Bt;
        private System.Windows.Forms.Label subLb1;
        private System.Windows.Forms.Label linkLb;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label mp4SizeLb;
        private System.Windows.Forms.Label currentSizeLb;
        private System.Windows.Forms.Label titelLb;
        private System.Windows.Forms.Label durationLb;
        private System.Windows.Forms.Panel usbSticksPanel;
        private System.Windows.Forms.Label subLb2;
        private System.Windows.Forms.Button selectFolderButton;
        private System.Windows.Forms.Button downloadMp3Bt;
        private System.Windows.Forms.Label mp3SizeLb;
        private System.Windows.Forms.Label mp4QualityLb;
        private System.Windows.Forms.PictureBox thumbnailPicBox;
        private System.Windows.Forms.Label thumbnailLb;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label chanelLb;
        private System.Windows.Forms.Label idLb;
        private System.Windows.Forms.Label uploadDateLb;
        private System.Windows.Forms.Button showInfoFormBt;
        private System.Windows.Forms.TextBox historyBox;
        private System.Windows.Forms.ComboBox mp4QualityComboBox;
    }
}

