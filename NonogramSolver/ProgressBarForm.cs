using System.ComponentModel;
using System.Windows.Forms;

namespace NonogramSolverGenerator
{
    public partial class ProgressBarForm : Form
    {
        private BackgroundWorker worker;
        public bool WorkerFinished { get; set; }

        public ProgressBarForm(BackgroundWorker bgw, string title)
        {
            InitializeComponent();
            progressBar1.Style = ProgressBarStyle.Marquee;
            worker = bgw;
            WorkerFinished = false;
            Text = title;
        }

        private void ProgressBarForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!WorkerFinished)
            {
                worker.CancelAsync();
            }
        }

        //public Progress
    }
}
