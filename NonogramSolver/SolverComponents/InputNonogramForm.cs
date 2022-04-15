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
    public partial class InputNonogramForm : Form
    {
        private Form parent;
        private bool backButton = false;

        public InputNonogramForm(Form parent)
        {
            this.parent = parent;
            InitializeComponent();
            MinimumSize = Size;
            openNonDialog.Filter = "Nonogram Files (*.NONOGRAM) | *.NONOGRAM";
            openNonDialog.Multiselect = false;
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            parent.Show();
            backButton = true;
            Close();
        }

        private void InputNonogramForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!backButton)
            {
                parent.Close();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (ValidatePuzzle())
            {
                //DO THE SOLVING HERE. Well not literally, bump it into a background worker and pop up a progressbar
                //ALSO VALIDATE FIRST
                //temp code
                new DisplayNonogramForm(this, new bool[1, 1]).Show();

            }        
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            if (openNonDialog.ShowDialog() == DialogResult.OK)
            {
                string path = openNonDialog.FileName;
                if (!Is_Nonogram(path))
                {
                    MessageBox.Show("Select a puzzle file (*.NONOGRAM)");
                    openNonDialog.FileName = string.Empty;
                    return;
                }
                //read and parse the file
                int rowCount = 0;
                int colCount = 0;
                int i = 0;
                //var lines = System.IO.File.ReadLines(openNonDialog.FileName, Encoding.UTF8);
                foreach (string line in System.IO.File.ReadLines(openNonDialog.FileName))
                {
                    if (i == 0)
                    {
                        bool rowSuccessful = Int32.TryParse(line, out rowCount);
                        if (!rowSuccessful)
                        {
                            MessageBox.Show("File " + openNonDialog.FileName + " is improperly formatted.");
                            openNonDialog.FileName = string.Empty;
                            return;
                        }
                    }
                    else if (i == 1)
                    {
                        bool colSuccessful = Int32.TryParse(line, out colCount);
                        if (!colSuccessful)
                        {
                            MessageBox.Show("File " + openNonDialog.FileName + " is improperly formatted.");
                            openNonDialog.FileName = string.Empty;
                            return;
                        }
                    }
                    else if (i <= rowCount + 1)
                    {
                        ulbRows.ListBox.Items.Add(line);
                    }
                    else if (i <= colCount + rowCount + 1)
                    {
                        ulbCols.ListBox.Items.Add(line);
                    }
                    else
                    {
                        MessageBox.Show("File " + openNonDialog.FileName + " is improperly formatted.");
                        openNonDialog.FileName = string.Empty;
                        return;
                    }

                    i++;
                }

                if (ulbRows.ListBox.Items.Count > 0)
                {
                    ulbRows.RemoveAllButtom.Enabled = true;
                }

                if (ulbCols.ListBox.Items.Count > 0)
                {
                    ulbCols.RemoveAllButtom.Enabled = true;
                }
            }
        }

        private void INForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (filePaths.Length == 1 && Is_Nonogram(filePaths[0]))
                {
                    e.Effect = DragDropEffects.Copy;
                    return;
                }
            }
            e.Effect = DragDropEffects.None;
        }

        /// <summary>
        /// Check if the file is a .NONOGRAM file.
        /// </summary>
        /// <param name="filePath">The path to the file in question</param>
        /// <returns>If the file has an extension of .NONOGRAM</returns>
        private bool Is_Nonogram(string filePath)
        {
            if (filePath == null || filePath == string.Empty) return false;

            string ext = System.IO.Path.GetExtension(filePath);

            if (ext.Equals(".nonogram", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        private void INForm_DragDrop(object sender, DragEventArgs e)
        {
            /*if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])(e.Data.GetData(DataFormats.FileDrop));
                //read and parse file
            }*/
        }

        /// <summary>
        /// Validate the puzzle currently defined by the Rows and Cols. 
        /// If the puzzle is invalid, it displays a Message Box explaining why it's invalid and where, if possible (ie, a row contains clues that sum to above the puzzle's dimensions), then returns false.
        /// Puzzle dimensions are exctracted from the Count of ulbRow and ulbCol, as each row/col must have one clue, even if that's 0.
        /// </summary>
        /// <returns>If the puzzle defined by ulbRow and ulbCol is valid</returns>
        private bool ValidatePuzzle()
        {
            int rowLength = ulbRows.ListBox.Items.Count, colLength = ulbCols.ListBox.Items.Count;

            //validate that there is at least one entry in each task
            if (rowLength == 0)
            {
                MessageBox.Show("Rows needs at least one entry.", "Invalid Puzzle", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (colLength == 0)
            {
                MessageBox.Show("Columns needs at least one entry.", "Invalid Puzzle", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }


            //is there a way to reformat this to work in parallel?


            //external counter bc ListBox.Items is an enumerator, which means I can't fetch by index unless I do something odd with the .Selected property
            int rowCounter = 1;
            int totalRowSum = 0, totalColSum = 0;
            foreach(string s in ulbRows.ListBox.Items)
            {
                //get the sum of the string
                int sum = s.Split(' ').Select(n => int.Parse(n)).Sum();
                totalRowSum += sum;

                //add in the gap of at least one
                sum += s.Split(' ').Length - 1;
                //check if we've violated the col length
                if(sum > colLength)
                {
                    MessageBox.Show("Sum of Row "+rowCounter + " (" + sum + ") exceeds maximum length of row (" + colLength+").", "Invalid Puzzle", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ulbRows.ListBox.SelectedIndex = rowCounter - 1;
                    return false;
                }
                rowCounter++;
                
            }

            int colCount = 1;
            foreach (string s in ulbCols.ListBox.Items)
            {
                //get the sum of the string
                int sum = s.Split(' ').Select(n => int.Parse(n)).Sum();
                totalColSum += sum;

                //add in the gap of at least one
                sum += s.Split(' ').Length - 1;
                //check if we've violated the col length
                if (sum > rowLength)
                {
                    MessageBox.Show("Sum of Column " + colCount + " (" + sum + ") exceeds maximum length of column (" + rowLength + ").", "Invalid Puzzle", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ulbCols.ListBox.SelectedIndex = colCount - 1;
                    return false;
                }
                colCount++;
                
            }

            //validate that the defined number of squares by each dimension is equal
            if(totalColSum != totalRowSum)
            {
                MessageBox.Show("Total sum of Row Clues does not equal total sum of Column Clues" +
                    ".", "Invalid Puzzle", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }


            return true;
        }


        //validate that the solution found isnt inconsistent with the clues
        //private bool ValidateSolution()
    }
}
