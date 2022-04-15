using System;
using System.Windows.Forms;

namespace NonogramSolverGenerator
{
    public partial class MainForm : Form
    {
        private NonogramDefForm infoForm;
        public MainForm()
        {
            InitializeComponent();
        }

        private void pbGenerator_Click(object sender, EventArgs e)
        {
            Hide();
            new SelectImgForm(this).Show();
        }

        private void pbSolving_Click(object sender, EventArgs e)
        {
            Hide();
            new InputNonogramForm(this).Show();
        }

        private void linkAbout_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (infoForm == null)
            {
                infoForm = new NonogramDefForm();
                infoForm.FormClosed += delegate { infoForm = null; };
            }
            infoForm.Show();
        }
    }
}
