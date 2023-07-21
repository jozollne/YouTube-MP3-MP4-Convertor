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
        int i = 0;
        
        public InfoForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            i++;
            infoBox.AppendText(i + Environment.NewLine);
            infoBox.ScrollToCaret();
            infoBox.ScrollBars = ScrollBars.Vertical;  // Vertikale Scrollbar aktivieren
        }
    }
}
