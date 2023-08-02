using System;
using System.IO;
using System.Management;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using Youtube_Videos_Herrunterladen;

public class UsbManager
{
    private ManagementEventWatcher insertWatcher;
    private ManagementEventWatcher removeWatcher;
    private List<Label> usbLabels = new List<Label>(); // List to store all USB labels
    private MainForm mainForm; // Form to operate on
    private Label subLb2; // Sub label to reference
    private PictureBox mainShadow;
    private Label locationLb;

    public UsbManager(MainForm mainForm, Label subLb2, PictureBox mainShadow, Label locationLb)
    {
        this.mainForm = mainForm;
        this.subLb2 = subLb2;
        this.mainShadow = mainShadow;
        this.locationLb = locationLb;

        // Check existing USB drives
        foreach (DriveInfo drive in DriveInfo.GetDrives())
        {
            if (drive.DriveType == DriveType.Removable)
            {
                CreateLabelForDrive(drive.Name.Trim('\\'));
            }
        }

        // USB Insertion event watcher
        insertWatcher = new ManagementEventWatcher();
        WqlEventQuery insertQuery = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2");
        insertWatcher.EventArrived += new EventArrivedEventHandler(USBInserted);
        insertWatcher.Query = insertQuery;
        insertWatcher.Start();

        // USB Removal event watcher
        removeWatcher = new ManagementEventWatcher();
        WqlEventQuery removeQuery = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 3");
        removeWatcher.EventArrived += new EventArrivedEventHandler(USBRemoved);
        removeWatcher.Query = removeQuery;
        removeWatcher.Start();
    }

    private void USBInserted(object sender, EventArrivedEventArgs e)
    {
        string driveName = e.NewEvent.Properties["DriveName"].Value.ToString();
        mainForm.Invoke(new Action(() => {
            CreateLabelForDrive(driveName);
        }));
    }

    private void USBRemoved(object sender, EventArrivedEventArgs e)
    {
        string driveName = e.NewEvent.Properties["DriveName"].Value.ToString();
        mainForm.Invoke(new Action(() => {
            // Remove the label
            for (int i = 0; i < usbLabels.Count; i++)
            {
                if (usbLabels[i].Text == $"{driveName}")
                {
                    mainShadow.Controls.Remove(usbLabels[i]);
                    usbLabels.RemoveAt(i);
                    break;
                }
            }

            // Reposition the remaining labels
            for (int i = 0; i < usbLabels.Count; i++)
            {
                usbLabels[i].Top = i > 0 ? usbLabels[i - 1].Top + 30 : subLb2.Top + subLb2.Height;
            }
        }));
    }

    private void CreateLabelForDrive(string driveName)
    {
        // Add a label
        Label usbLabel = new Label();
        usbLabel.ForeColor = Color.White;
        usbLabel.Text = $"{driveName}";
        usbLabel.Name = $"usbLabel{driveName}";
        usbLabel.Top = usbLabels.Count > 0 ? usbLabels[usbLabels.Count - 1].Top + 30 : subLb2.Top + subLb2.Height; // Position is below the last label or subLb1 if it's the first
        usbLabel.Left = subLb2.Left;
        usbLabel.AutoSize = true;  // Enable auto-sizing of the label based on its content
        usbLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

        usbLabel.Click += (s, e) => // Set up a click event handler
        {
            mainForm.selectedFolderPath = driveName + "\\";  // Update the selectedFolderPath field with the current drive name
            locationLb.Text = "Speicherort: " + $"({driveName})";  // Update the subLb1 label to display the selected folder path
        };

        mainShadow.Controls.Add(usbLabel);

        usbLabels.Add(usbLabel); // Add the label to the list
    }

    public void Stop()
    {
        if (insertWatcher != null)
        {
            insertWatcher.Stop();
            insertWatcher.Dispose();
            insertWatcher = null;
        }

        if (removeWatcher != null)
        {
            removeWatcher.Stop();
            removeWatcher.Dispose();
            removeWatcher = null;
        }
    }
}
