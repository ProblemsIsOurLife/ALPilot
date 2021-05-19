using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ALPilot
{
    public partial class meteors : Form
    {
        public meteors()
        {
            InitializeComponent();
        }

        private void Kernite_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; i++)
            Close();
        }

        private void Crokite_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; i++)
            Close();
        }
    }
}
