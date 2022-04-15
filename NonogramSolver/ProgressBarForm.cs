using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NonogramSolverGenerator
{
    public partial class ProgressBarForm : Form
    {
        //private Form parent; 
        public ProgressBarForm()//Form parent)
        {
            InitializeComponent();
            progressBar1.Style = ProgressBarStyle.Marquee;
            //this.parent = parent;
        }

        private void ProgressBarForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //parent.Show();
        }

        //public Progress
    }
}
