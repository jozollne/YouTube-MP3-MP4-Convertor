using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Youtube_Videos_Herrunterladen
{
    public partial class HistoryForm : Form
    {
        private readonly MainForm mainForm;
        private readonly Utilityclass utilityClass;

        internal HistoryForm(MainForm mainForm, Utilityclass utilityClass)
        {
            InitializeComponent();
            
            this.mainForm = mainForm;  // Setzen Sie die Referenz auf die Hauptform
            this.utilityClass = utilityClass;

            this.Activated += (sender, e) => utilityClass.BringToFrontWithoutFocus(mainForm, this); // Abonniere das Activated Event
            
            this.BackColor = Color.FromArgb(0xDA, 0x0F, 0x0F);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;  // Cancel the form closing event
                Hide();  // Hide the form instead of closing it
            }
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MOVE = 0xF010;

            switch (m.Msg)
            {
                case WM_SYSCOMMAND:
                    int command = m.WParam.ToInt32() & 0xfff0;
                    if (command == SC_MOVE)
                        return;
                    break;
            }

            base.WndProc(ref m);
        }
    }
}
