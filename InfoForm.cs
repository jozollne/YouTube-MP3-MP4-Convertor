using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Youtube_Videos_Herrunterladen
{
    public partial class InfoForm : Form
    {
        public InfoForm()
        {
            InitializeComponent();
            infoBox.ScrollBars = ScrollBars.Vertical;
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

    }
}
