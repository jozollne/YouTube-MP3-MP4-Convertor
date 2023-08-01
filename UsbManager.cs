using System;  // Import the System namespace
using System.Drawing;
using System.IO;  // Import the System.IO namespace for handling files and directories
using System.Linq;  // Import the System.Linq namespace for working with collections
using System.Windows.Forms;  // Import the System.Windows.Forms namespace for working with Windows Forms
using Forms = System.Windows.Forms;  // Alias the System.Windows.Forms namespace to 'Forms' for better readability

namespace Youtube_Videos_Herrunterladen
{
    internal class UsbManager
    {
        private readonly Main main;
        private readonly Panel usbSticksPanel;  // Field to store the Panel control used for displaying USB stick labels
        private readonly Label subLb1;  // Field to hold the Label control for displaying the selected folder path

        public UsbManager(Main main, Panel usbSticksPanel, Label subLb1)
        {
            this.usbSticksPanel = usbSticksPanel;  // Assign the passed-in Panel to the usbSticksPanel field
            this.subLb1 = subLb1;  // Assign the passed-in Label to the subLb1 field
            this.main = main;
        }

        public void UpdateUsbLabels(object sender, EventArgs e)
        {
            var drives = DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Removable && d.IsReady); // Get all removable and ready drives

            foreach (Control control in usbSticksPanel.Controls.OfType<Forms.Label>().ToList()) // Iterate over all existing labels
            {
                usbSticksPanel.Controls.Remove(control);  // Remove the control from the usbSticksPanel
                control.Dispose();  // Dispose the control to free any resources it's holding
            }

            foreach (var drive in drives) // Iterate over each removable drive
            {
                AddUsbLabel(drive.Name);  // Call the AddUsbLabel method to add a label for the current drive
            }
        }

        void AddUsbLabel(string driveName)
        {
            var driveLabel = new DriveInfo(driveName).VolumeLabel; // Get the volume label of the drive

            Forms.Label label = new Forms.Label
            {
                Text = $"{driveLabel} ({driveName})",  // Set the label text as "VolumeLabel (DriveName)"
                AutoSize = true,  // Enable auto-sizing of the label based on its content
                Margin = new Padding(3, 3, 3, 3),  // Set the margin around the label for better spacing
                Top = usbSticksPanel.Controls.OfType<Forms.Label>().Count() * 25  // Set the vertical position of the label based on the number of existing labels
            };

            label.Click += (s, e) => // Set up a click event handler
            {
                main.selectedFolderPath = driveName;  // Update the selectedFolderPath field with the current drive name
                subLb1.Text = "Speicherort: " + $"{driveLabel} ({driveName})";  // Update the subLb1 label to display the selected folder path
            };

            usbSticksPanel.Controls.Add(label);  // Add the label to the usbSticksPanel
        }
    }
}
