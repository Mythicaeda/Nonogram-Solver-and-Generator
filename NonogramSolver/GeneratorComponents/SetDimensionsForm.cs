using System;
using System.Windows.Forms;

namespace NonogramSolverGenerator
{
    public partial class SetDimensionsForm : Form
    {
        Form parent;

        internal int RowCount { get; private set; }
        internal int ColCount { get; private set; }


        public SetDimensionsForm(Form parent, int imgWidth, int imgHeight)
        {
            this.parent = parent;
            InitializeComponent();

            //these limits are placed based on requirements, so we cant generate a nonogram that is larger than what our program can solve
            nudRows.Maximum = imgHeight < 1000 ? imgHeight : 1000;
            nudCols.Maximum = imgWidth < 1000 ? imgWidth : 1000;

            RowCount = (int)nudRows.Value;
            ColCount = (int)nudCols.Value;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            //dont have to validate these anymore bc im using a numericupdownbox

            //if their valid, add the hook here and pass the rows and cols along with the hook
            //then, display the progress bar while the generate function runs
            //at the end of that, close this form
            //Close(); passed this down to its parent, as we've

            RowCount = (int)nudRows.Value;
            ColCount = (int)nudCols.Value;

            //new ProgressBarForm().ShowDialog();
        }

        private void SetDimensionsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            parent.Show();
        }
    }
}
