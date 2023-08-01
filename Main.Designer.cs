namespace Youtube_Videos_Herrunterladen
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
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.infoLb = new System.Windows.Forms.Label();
            this.mp4SizeLb = new System.Windows.Forms.Label();
            this.currentSizeLb = new System.Windows.Forms.Label();
            this.titleLb = new System.Windows.Forms.Label();
            this.durationLb = new System.Windows.Forms.Label();
            this.usbSticksPanel = new System.Windows.Forms.Panel();
            this.subLb2 = new System.Windows.Forms.Label();
            this.selectFolderBt = new System.Windows.Forms.Button();
            this.downloadMp3Bt = new System.Windows.Forms.Button();
            this.mp3SizeLb = new System.Windows.Forms.Label();
            this.mp4QualityLb = new System.Windows.Forms.Label();
            this.historyLb = new System.Windows.Forms.Label();
            this.channelLb = new System.Windows.Forms.Label();
            this.idLb = new System.Windows.Forms.Label();
            this.uploadDateLb = new System.Windows.Forms.Label();
            this.showInfoFormBt = new System.Windows.Forms.Button();
            this.thumbnailPicBox = new System.Windows.Forms.PictureBox();
            this.mp4QualityComboBox = new System.Windows.Forms.ComboBox();
            this.downloadSpeedLb = new System.Windows.Forms.Label();
            this.shadowLeft = new System.Windows.Forms.PictureBox();
            this.historyPanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.thumbnailPicBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.shadowLeft)).BeginInit();
            this.SuspendLayout();
            // 
            // linkBox
            // 
            this.linkBox.Font = new System.Drawing.Font("Arial Narrow", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkBox.ForeColor = System.Drawing.Color.Gray;
            this.linkBox.Location = new System.Drawing.Point(20, 53);
            this.linkBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.linkBox.Name = "linkBox";
            this.linkBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.linkBox.Size = new System.Drawing.Size(1082, 32);
            this.linkBox.TabIndex = 0;
            this.linkBox.Text = "Link: (z.B. https://www.youtube.com/watch?v=6WRLynWxVKg)";
            this.linkBox.Enter += new System.EventHandler(this.LinkBox_Enter);
            this.linkBox.Leave += new System.EventHandler(this.LinkBox_Leave);
            // 
            // downloadMp4Bt
            // 
            this.downloadMp4Bt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.downloadMp4Bt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.downloadMp4Bt.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.downloadMp4Bt.ForeColor = System.Drawing.Color.White;
            this.downloadMp4Bt.Location = new System.Drawing.Point(891, 642);
            this.downloadMp4Bt.Name = "downloadMp4Bt";
            this.downloadMp4Bt.Size = new System.Drawing.Size(211, 33);
            this.downloadMp4Bt.TabIndex = 3;
            this.downloadMp4Bt.Text = ".mp4 Herrunterladen";
            this.downloadMp4Bt.UseVisualStyleBackColor = false;
            this.downloadMp4Bt.Click += new System.EventHandler(this.DownloadMp4Bt_Click);
            // 
            // subLb1
            // 
            this.subLb1.AutoSize = true;
            this.subLb1.BackColor = System.Drawing.Color.Transparent;
            this.subLb1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.subLb1.Font = new System.Drawing.Font("Calibri", 24F, System.Drawing.FontStyle.Bold);
            this.subLb1.ForeColor = System.Drawing.Color.White;
            this.subLb1.Location = new System.Drawing.Point(13, 600);
            this.subLb1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.subLb1.Name = "subLb1";
            this.subLb1.Size = new System.Drawing.Size(178, 39);
            this.subLb1.TabIndex = 2;
            this.subLb1.Text = "Speicherort:";
            // 
            // progressBar
            // 
            this.progressBar.BackColor = System.Drawing.Color.White;
            this.progressBar.Location = new System.Drawing.Point(20, 93);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(1081, 23);
            this.progressBar.TabIndex = 5;
            // 
            // infoLb
            // 
            this.infoLb.AutoSize = true;
            this.infoLb.BackColor = System.Drawing.Color.Transparent;
            this.infoLb.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.infoLb.Font = new System.Drawing.Font("Calibri", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.infoLb.ForeColor = System.Drawing.Color.White;
            this.infoLb.Location = new System.Drawing.Point(12, 178);
            this.infoLb.Name = "infoLb";
            this.infoLb.Size = new System.Drawing.Size(79, 39);
            this.infoLb.TabIndex = 6;
            this.infoLb.Text = "Info:";
            // 
            // mp4SizeLb
            // 
            this.mp4SizeLb.AutoSize = true;
            this.mp4SizeLb.BackColor = System.Drawing.Color.Transparent;
            this.mp4SizeLb.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mp4SizeLb.ForeColor = System.Drawing.Color.White;
            this.mp4SizeLb.Location = new System.Drawing.Point(14, 347);
            this.mp4SizeLb.Name = "mp4SizeLb";
            this.mp4SizeLb.Size = new System.Drawing.Size(169, 26);
            this.mp4SizeLb.TabIndex = 7;
            this.mp4SizeLb.Text = ".mp4 Größe: 0 MB";
            // 
            // currentSizeLb
            // 
            this.currentSizeLb.AutoSize = true;
            this.currentSizeLb.BackColor = System.Drawing.Color.Transparent;
            this.currentSizeLb.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.currentSizeLb.ForeColor = System.Drawing.Color.White;
            this.currentSizeLb.Location = new System.Drawing.Point(12, 119);
            this.currentSizeLb.Name = "currentSizeLb";
            this.currentSizeLb.Size = new System.Drawing.Size(550, 26);
            this.currentSizeLb.TabIndex = 8;
            this.currentSizeLb.Text = "Herruntergeladen: 0 MB / Größe: 0 MB | Gesamtfortschritt: 0%";
            // 
            // titleLb
            // 
            this.titleLb.AutoSize = true;
            this.titleLb.BackColor = System.Drawing.Color.Transparent;
            this.titleLb.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.titleLb.Font = new System.Drawing.Font("Calibri", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLb.ForeColor = System.Drawing.Color.White;
            this.titleLb.Location = new System.Drawing.Point(11, 9);
            this.titleLb.Name = "titleLb";
            this.titleLb.Size = new System.Drawing.Size(192, 36);
            this.titleLb.TabIndex = 9;
            this.titleLb.Text = "Titel: Kein Link";
            // 
            // durationLb
            // 
            this.durationLb.AutoSize = true;
            this.durationLb.BackColor = System.Drawing.Color.Transparent;
            this.durationLb.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.durationLb.ForeColor = System.Drawing.Color.White;
            this.durationLb.Location = new System.Drawing.Point(14, 295);
            this.durationLb.Name = "durationLb";
            this.durationLb.Size = new System.Drawing.Size(152, 26);
            this.durationLb.TabIndex = 10;
            this.durationLb.Text = "Dauer: 00:00:00";
            // 
            // usbSticksPanel
            // 
            this.usbSticksPanel.BackColor = System.Drawing.Color.Transparent;
            this.usbSticksPanel.ForeColor = System.Drawing.Color.White;
            this.usbSticksPanel.Location = new System.Drawing.Point(25, 478);
            this.usbSticksPanel.Name = "usbSticksPanel";
            this.usbSticksPanel.Size = new System.Drawing.Size(235, 120);
            this.usbSticksPanel.TabIndex = 11;
            // 
            // subLb2
            // 
            this.subLb2.AutoSize = true;
            this.subLb2.BackColor = System.Drawing.Color.Transparent;
            this.subLb2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.subLb2.Font = new System.Drawing.Font("Calibri", 24F, System.Drawing.FontStyle.Bold);
            this.subLb2.ForeColor = System.Drawing.Color.White;
            this.subLb2.Location = new System.Drawing.Point(18, 439);
            this.subLb2.Name = "subLb2";
            this.subLb2.Size = new System.Drawing.Size(165, 39);
            this.subLb2.TabIndex = 12;
            this.subLb2.Text = "USB-Sticks:";
            // 
            // selectFolderBt
            // 
            this.selectFolderBt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.selectFolderBt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.selectFolderBt.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectFolderBt.ForeColor = System.Drawing.Color.White;
            this.selectFolderBt.Location = new System.Drawing.Point(12, 642);
            this.selectFolderBt.Name = "selectFolderBt";
            this.selectFolderBt.Size = new System.Drawing.Size(230, 33);
            this.selectFolderBt.TabIndex = 1;
            this.selectFolderBt.Text = "Speicherort ändern";
            this.selectFolderBt.UseVisualStyleBackColor = false;
            this.selectFolderBt.Click += new System.EventHandler(this.SelectFolderButton_Click);
            // 
            // downloadMp3Bt
            // 
            this.downloadMp3Bt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.downloadMp3Bt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.downloadMp3Bt.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.downloadMp3Bt.ForeColor = System.Drawing.Color.White;
            this.downloadMp3Bt.Location = new System.Drawing.Point(680, 642);
            this.downloadMp3Bt.Name = "downloadMp3Bt";
            this.downloadMp3Bt.Size = new System.Drawing.Size(205, 33);
            this.downloadMp3Bt.TabIndex = 2;
            this.downloadMp3Bt.Text = ".mp3 Herrunterladen";
            this.downloadMp3Bt.UseVisualStyleBackColor = false;
            this.downloadMp3Bt.Click += new System.EventHandler(this.DownloadMp3Bt_Click);
            // 
            // mp3SizeLb
            // 
            this.mp3SizeLb.AutoSize = true;
            this.mp3SizeLb.BackColor = System.Drawing.Color.Transparent;
            this.mp3SizeLb.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mp3SizeLb.ForeColor = System.Drawing.Color.White;
            this.mp3SizeLb.Location = new System.Drawing.Point(14, 321);
            this.mp3SizeLb.Name = "mp3SizeLb";
            this.mp3SizeLb.Size = new System.Drawing.Size(169, 26);
            this.mp3SizeLb.TabIndex = 15;
            this.mp3SizeLb.Text = ".mp3 Größe: 0 MB";
            // 
            // mp4QualityLb
            // 
            this.mp4QualityLb.AutoSize = true;
            this.mp4QualityLb.BackColor = System.Drawing.Color.Transparent;
            this.mp4QualityLb.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mp4QualityLb.ForeColor = System.Drawing.Color.White;
            this.mp4QualityLb.Location = new System.Drawing.Point(14, 373);
            this.mp4QualityLb.Name = "mp4QualityLb";
            this.mp4QualityLb.Size = new System.Drawing.Size(136, 26);
            this.mp4QualityLb.TabIndex = 16;
            this.mp4QualityLb.Text = ".mp4 Qualität:";
            // 
            // historyLb
            // 
            this.historyLb.AutoSize = true;
            this.historyLb.BackColor = System.Drawing.Color.Transparent;
            this.historyLb.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.historyLb.Font = new System.Drawing.Font("Calibri", 24F, System.Drawing.FontStyle.Bold);
            this.historyLb.ForeColor = System.Drawing.Color.White;
            this.historyLb.Location = new System.Drawing.Point(764, 178);
            this.historyLb.Name = "historyLb";
            this.historyLb.Size = new System.Drawing.Size(121, 39);
            this.historyLb.TabIndex = 20;
            this.historyLb.Text = "Verlauf:";
            // 
            // channelLb
            // 
            this.channelLb.AutoSize = true;
            this.channelLb.BackColor = System.Drawing.Color.Transparent;
            this.channelLb.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.channelLb.ForeColor = System.Drawing.Color.White;
            this.channelLb.Location = new System.Drawing.Point(14, 243);
            this.channelLb.Name = "channelLb";
            this.channelLb.Size = new System.Drawing.Size(147, 26);
            this.channelLb.TabIndex = 22;
            this.channelLb.Text = "Kanal: Kein Link";
            // 
            // idLb
            // 
            this.idLb.AutoSize = true;
            this.idLb.BackColor = System.Drawing.Color.Transparent;
            this.idLb.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.idLb.ForeColor = System.Drawing.Color.White;
            this.idLb.Location = new System.Drawing.Point(14, 217);
            this.idLb.Name = "idLb";
            this.idLb.Size = new System.Drawing.Size(118, 26);
            this.idLb.TabIndex = 23;
            this.idLb.Text = "ID: Kein Link";
            // 
            // uploadDateLb
            // 
            this.uploadDateLb.AutoSize = true;
            this.uploadDateLb.BackColor = System.Drawing.Color.Transparent;
            this.uploadDateLb.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.uploadDateLb.ForeColor = System.Drawing.Color.White;
            this.uploadDateLb.Location = new System.Drawing.Point(14, 269);
            this.uploadDateLb.Name = "uploadDateLb";
            this.uploadDateLb.Size = new System.Drawing.Size(211, 26);
            this.uploadDateLb.TabIndex = 24;
            this.uploadDateLb.Text = "Hochgeladen: Kein Link";
            // 
            // showInfoFormBt
            // 
            this.showInfoFormBt.BackColor = System.Drawing.Color.Transparent;
            this.showInfoFormBt.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.showInfoFormBt.FlatAppearance.BorderSize = 0;
            this.showInfoFormBt.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.showInfoFormBt.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.showInfoFormBt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.showInfoFormBt.Image = global::Youtube_Videos_Herrunterladen.Properties.Resources.Info2;
            this.showInfoFormBt.Location = new System.Drawing.Point(1068, 116);
            this.showInfoFormBt.Name = "showInfoFormBt";
            this.showInfoFormBt.Size = new System.Drawing.Size(34, 32);
            this.showInfoFormBt.TabIndex = 25;
            this.showInfoFormBt.UseVisualStyleBackColor = false;
            this.showInfoFormBt.Click += new System.EventHandler(this.showInfoFormBt_Click);
            // 
            // thumbnailPicBox
            // 
            this.thumbnailPicBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("thumbnailPicBox.BackgroundImage")));
            this.thumbnailPicBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.thumbnailPicBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.thumbnailPicBox.Location = new System.Drawing.Point(0, 0);
            this.thumbnailPicBox.Name = "thumbnailPicBox";
            this.thumbnailPicBox.Size = new System.Drawing.Size(1113, 683);
            this.thumbnailPicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.thumbnailPicBox.TabIndex = 18;
            this.thumbnailPicBox.TabStop = false;
            // 
            // mp4QualityComboBox
            // 
            this.mp4QualityComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mp4QualityComboBox.FormattingEnabled = true;
            this.mp4QualityComboBox.Location = new System.Drawing.Point(19, 402);
            this.mp4QualityComboBox.Name = "mp4QualityComboBox";
            this.mp4QualityComboBox.Size = new System.Drawing.Size(222, 34);
            this.mp4QualityComboBox.TabIndex = 27;
            // 
            // downloadSpeedLb
            // 
            this.downloadSpeedLb.AutoSize = true;
            this.downloadSpeedLb.BackColor = System.Drawing.Color.Transparent;
            this.downloadSpeedLb.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.downloadSpeedLb.ForeColor = System.Drawing.Color.White;
            this.downloadSpeedLb.Location = new System.Drawing.Point(12, 145);
            this.downloadSpeedLb.Name = "downloadSpeedLb";
            this.downloadSpeedLb.Size = new System.Drawing.Size(291, 26);
            this.downloadSpeedLb.TabIndex = 28;
            this.downloadSpeedLb.Text = "Geschwindigkeit: Kein Download";
            // 
            // shadowLeft
            // 
            this.shadowLeft.BackColor = System.Drawing.Color.Transparent;
            this.shadowLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.shadowLeft.Image = ((System.Drawing.Image)(resources.GetObject("shadowLeft.Image")));
            this.shadowLeft.Location = new System.Drawing.Point(0, 0);
            this.shadowLeft.Name = "shadowLeft";
            this.shadowLeft.Size = new System.Drawing.Size(1113, 683);
            this.shadowLeft.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.shadowLeft.TabIndex = 29;
            this.shadowLeft.TabStop = false;
            // 
            // historyPanel
            // 
            this.historyPanel.BackColor = System.Drawing.Color.Transparent;
            this.historyPanel.ForeColor = System.Drawing.Color.White;
            this.historyPanel.Location = new System.Drawing.Point(538, 220);
            this.historyPanel.Name = "historyPanel";
            this.historyPanel.Size = new System.Drawing.Size(564, 378);
            this.historyPanel.TabIndex = 30;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 26F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(1113, 683);
            this.Controls.Add(this.historyPanel);
            this.Controls.Add(this.historyLb);
            this.Controls.Add(this.infoLb);
            this.Controls.Add(this.downloadSpeedLb);
            this.Controls.Add(this.mp4QualityComboBox);
            this.Controls.Add(this.showInfoFormBt);
            this.Controls.Add(this.uploadDateLb);
            this.Controls.Add(this.idLb);
            this.Controls.Add(this.channelLb);
            this.Controls.Add(this.mp4QualityLb);
            this.Controls.Add(this.mp3SizeLb);
            this.Controls.Add(this.downloadMp3Bt);
            this.Controls.Add(this.selectFolderBt);
            this.Controls.Add(this.subLb2);
            this.Controls.Add(this.usbSticksPanel);
            this.Controls.Add(this.durationLb);
            this.Controls.Add(this.titleLb);
            this.Controls.Add(this.currentSizeLb);
            this.Controls.Add(this.mp4SizeLb);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.subLb1);
            this.Controls.Add(this.downloadMp4Bt);
            this.Controls.Add(this.linkBox);
            this.Controls.Add(this.shadowLeft);
            this.Controls.Add(this.thumbnailPicBox);
            this.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Main";
            this.Text = "Youtube Videos Herrunterladen";
            ((System.ComponentModel.ISupportInitialize)(this.thumbnailPicBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.shadowLeft)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox mp4QualityComboBox;
        private System.Windows.Forms.TextBox linkBox;
        private System.Windows.Forms.Button downloadMp4Bt;
        private System.Windows.Forms.Label subLb1;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label infoLb;
        private System.Windows.Forms.Label mp4SizeLb;
        private System.Windows.Forms.Label currentSizeLb;
        private System.Windows.Forms.Label titleLb;
        private System.Windows.Forms.Label durationLb;
        private System.Windows.Forms.Panel usbSticksPanel;
        private System.Windows.Forms.Label subLb2;
        private System.Windows.Forms.Button selectFolderBt;
        private System.Windows.Forms.Button downloadMp3Bt;
        private System.Windows.Forms.Label mp3SizeLb;
        private System.Windows.Forms.Label mp4QualityLb;
        private System.Windows.Forms.PictureBox thumbnailPicBox;
        private System.Windows.Forms.Label historyLb;
        private System.Windows.Forms.Label channelLb;
        private System.Windows.Forms.Label idLb;
        private System.Windows.Forms.Label uploadDateLb;
        private System.Windows.Forms.Button showInfoFormBt;
        private System.Windows.Forms.Label downloadSpeedLb;
        private System.Windows.Forms.PictureBox shadowLeft;
        private System.Windows.Forms.Panel historyPanel;
    }
}

