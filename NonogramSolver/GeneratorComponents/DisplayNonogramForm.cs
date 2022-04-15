using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NonogramSolverGenerator
{
    public partial class DisplayNonogramForm : Form
    {
        private Form parent;
        private bool backButton = false;
        private bool[,] nonogram;
        private string[] rowClues;
        private string[] colClues;

        private int scale = 50;

        private Font backup = new Font("Book Antigua", 14);

        private List<Label> rows, cols;
        private Point mouseStartingPt;


        public DisplayNonogramForm(Form parent, bool[,] nonogram)
        {
            InitializeComponent();

            this.MinimumSize = new Size(400,400);
            this.parent = parent;
            this.nonogram = nonogram;

            CreateClues();
            CreateLabels();

            puzzlePanel.AutoScrollMargin = new Size(100, 100);
        }

        private void CreateClues()
        {
            rowClues = new string[nonogram.GetLength(0)];
            colClues = new string[nonogram.GetLength(1)];

            //Hold row steady, loop over the columns, then increment rows and repeat

            Parallel.Invoke(
             () =>
             Parallel.For(0, rowClues.Length, index =>
             {
                 string s = "";
                 int ctr = 0;
                 for (int c = 0; c < nonogram.GetLength(1); c++)
                 {
                     if (nonogram[index, c])
                         ctr++;
                     else
                     {
                         if (ctr > 0)
                         {
                             s += ctr + " ";
                             ctr = 0;
                         }
                     }
                 }
                 if (s == "" || ctr != 0)
                 {
                     s += ctr;
                 }
                 rowClues[index] = s.Trim();
             }),
            () => Parallel.For(0, colClues.Length, index =>
            {
                string s = "";
                int ctr = 0;
                for (int r = 0; r < nonogram.GetLength(0); r++)
                {
                    if (nonogram[r, index]) ctr++;
                    else
                    {
                        if (ctr > 0)
                        {
                            s += ctr + " ";
                            ctr = 0;
                        }
                    }
                }
                if (s == "" || ctr != 0)
                {
                    s += ctr;
                }
                colClues[index] = s.Trim();
            }));
        }

        //creating the labels in batches was causing issues, so I'm creating them all at once for now
        private void CreateLabels()
        {
           // if (colClues.Length < (int)Math.Ceiling((double)Width/30.0))
                cols = new List<Label>(colClues.Length);
            //else
            //    cols = new List<Label>((int)Math.Ceiling((Width) / 30.0));

            //Similarly, 140 is also the corner offset and 30 is the label height
           // if (rowClues.Length < (int)Math.Ceiling((Height) / 30.0))
                rows = new List<Label>(rowClues.Length);
            //else
            //    rows = new List<Label>((int)Math.Ceiling((Height) / 30.0));

            for(int i = 0; i < rows.Capacity; i++)
            {
                rows.Add(new Label());
                puzzlePanel.Controls.Add(rows[i]);
                if (rowClues[i].Length < 10)
                    rows[i].Text = rowClues[i];
                else
                {
                    rows[i].Text = rowClues[i].Substring(0, 6) + "...";
                }
                tips.SetToolTip(rows[i], rowClues[i]);
                rows[i].Size = TextRenderer.MeasureText(rows[i].Text, rows[i].Font);
                rows[i].Height = 30;
                rows[i].Location = new Point(140 - rows[i].Width, 140 + scale * i+15);
                rows[i].TextAlign = ContentAlignment.MiddleRight;
                rows[i].SendToBack();                
            }

            for (int i = 0; i < cols.Capacity; i++)
            {
                cols.Add(new Label());
                puzzlePanel.Controls.Add(cols[i]);
                if (colClues[i].Length < 10)
                {
                    cols[i].Text = colClues[i].Replace(" ", "\n");
                }
                else
                {
                    cols[i].Text = colClues[i].Substring(0, 6).Replace(" ", "\n") + "\n...";
                }
                tips.SetToolTip(cols[i], colClues[i]);
                cols[i].Size = TextRenderer.MeasureText(cols[i].Text, cols[i].Font);
                if(cols[i].Width >= scale)
                {
                    cols[i].Font = backup; 
                }
                cols[i].Width = scale;
                cols[i].Location = new Point(140 + scale*i, 135 - cols[i].Height);               
                cols[i].TextAlign = ContentAlignment.BottomCenter;
                cols[i].SendToBack();
            }
        }

        private void DisplayNonogramForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!backButton)
            {
                parent.Close();
            }
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            parent.Show();
            backButton = true;
            Close();
        }

        private void puzzlePanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.TranslateTransform(puzzlePanel.AutoScrollPosition.X, puzzlePanel.AutoScrollPosition.Y);
            Graphics gfx = e.Graphics;
            Rectangle puzzleZone = new Rectangle(140, 140, 0, 0);
            Rectangle square = new Rectangle(140, 140, scale, scale);


            for (int c = 0; c < cols.Count; c++, square.X += scale, square.Y = 140)
            {
                for (int r = 0; r < rows.Count; r++, square.Y += scale)
                {
                    if (nonogram[r, c])
                    {
                        gfx.FillRectangle(Brushes.Black, square);
                        gfx.DrawRectangle(Pens.White, square);
                    }
                    else
                    {
                        gfx.FillRectangle(Brushes.White, square);
                        gfx.DrawRectangle(Pens.Black, square);
                    }
                }
            }

        }


        /*private void UpdateLabels()
        {
            int incCols = 0;
            int incRows = 0;

            //chech that we dont have all the clues rendered and that we can fit more clues
            if(cols.Count < colClues.Length && cols.Count < Math.Ceiling(Width / 30.0))
            {
                incCols = (int)Math.Ceiling(Width / 30.0) - cols.Count; //how many more cols do we need to add;
            }
            if (rows.Count < rowClues.Length && rows.Count < Math.Ceiling(Height / 30.0))
            {
                incRows = (int)Math.Ceiling(Height / 30.0) - rows.Count; //how many more rows do we need to add;
            }     

            for (int i = 0, ri = rows.Count; i < incRows && ri < rowClues.Length; i++, ri++)
            {
                rows.Add(new Label());
                puzzlePanel.Controls.Add(rows[ri]);
                if (rowClues[ri].Length < 10)
                    rows[ri].Text = rowClues[ri];
                else
                {
                    rows[ri].Text = rowClues[ri].Substring(0, 6) + "...";
                }
                tips.SetToolTip(rows[ri], rowClues[ri]);
                rows[ri].Size = TextRenderer.MeasureText(rows[ri].Text, rows[ri].Font);
                rows[ri].Height = 30;
                rows[ri].Location = new Point(140 - rows[ri].Width, 140 + 50 * ri + 15 -puzzlePanel.AutoScrollPosition.Y);
                rows[ri].TextAlign = ContentAlignment.MiddleRight;
                rows[ri].SendToBack();
            }

            for (int i = 0, ci = cols.Count; i < incCols && ci < colClues.Length; i++, ci++)
            {
                cols.Add(new Label());
                puzzlePanel.Controls.Add(cols[ci]);
                if (colClues[ci].Length < 10)
                {
                    cols[ci].Text = colClues[ci].Replace(" ", "\n");
                }
                else
                {
                    cols[ci].Text = colClues[ci].Substring(0, 6).Replace(" ", "\n") + "\n...";
                }
                tips.SetToolTip(cols[ci], colClues[ci]);
                cols[ci].Size = TextRenderer.MeasureText(cols[ci].Text, cols[ci].Font);
                cols[ci].Width = 30;
                cols[ci].Location = new Point(140 + 50 * ci + 15 - puzzlePanel.AutoScrollPosition.X, 135 - cols[ci].Height);
                cols[ci].TextAlign = ContentAlignment.BottomCenter;
                cols[ci].SendToBack();
            }
        }*/

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (sfd.FileName != "")
                {
                    //TODO if I have time: verify if this is an efficient way to write the file instead of making a mega string
                    using (System.IO.FileStream stream = (System.IO.FileStream)sfd.OpenFile())
                    {
                        //create string to save
                        string fileContents = rowClues.Length + "\n" + colClues.Length + "\n";
                        UTF8Encoding encoder = new UTF8Encoding(true);
                        //int oldOffset = 0;
                        //int newOffset = encoder.GetByteCount(fileContents);

                        stream.Write(encoder.GetBytes(fileContents), 0, encoder.GetBytes(fileContents).Length);//oldOffset, newOffset);

                        //fileContents = "";
                        foreach (string s in rowClues)
                        {
                            var bytes = encoder.GetBytes(s);
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Write(encoder.GetBytes("\n"), 0, encoder.GetBytes("\n").Length);
                            //oldOffset = newOffset;
                            //newOffset += encoder.GetByteCount(s);
                            //stream.Write(encoder.GetBytes(s), oldOffset, newOffset);
                        }

                        foreach (string s in colClues)
                        {
                            var bytes = encoder.GetBytes(s);
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Write(encoder.GetBytes("\n"), 0, encoder.GetBytes("\n").Length);
                            //   oldOffset = newOffset;
                            // newOffset += encoder.GetByteCount(s);
                            //stream.Write(encoder.GetBytes(s), oldOffset, newOffset);
                        }

                    }
                }
            }
        }

        private void puzzlePanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                Cursor = Cursors.NoMove2D;
                mouseStartingPt = e.Location;
            }
        }

        private void puzzlePanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                bool up = false, down = false, left = false, right = false;
                if (e.Location.Y < mouseStartingPt.Y - 20)
                {
                    up = true;
                }
                else if (e.Location.Y > 20 + mouseStartingPt.Y)
                {
                    down = true;
                }
                if (e.Location.X < mouseStartingPt.X - 20)
                {
                    left = true;
                }
                else if (e.Location.X > 20 + mouseStartingPt.X)
                {
                    right = true;
                }

                //Cursors
                if (up && right) { Cursor = Cursors.PanNE; }
                else if (down && right) { Cursor = Cursors.PanSE; }
                else if (up && left) { Cursor = Cursors.PanNW; }
                else if (down && left) { Cursor = Cursors.PanSW; }
                else if (up) { Cursor = Cursors.PanNorth; }
                else if (down) { Cursor = Cursors.PanSouth; }
                else if (right) { Cursor = Cursors.PanEast; }
                else if (left) { Cursor = Cursors.PanWest; }
                else { Cursor = Cursors.NoMove2D; }


                //Scroll
                if(Cursor != Cursors.NoMove2D)
                {
                    //Get a vector of the difference btwn the points
                    Point ratio = new Point(e.Location.X - mouseStartingPt.X, e.Location.Y - mouseStartingPt.Y);
                    ratio.X /= 50;
                    ratio.Y /= 50;
                    //then scroll
                    puzzlePanel.AutoScrollPosition = new Point(puzzlePanel.HorizontalScroll.Value + puzzlePanel.HorizontalScroll.SmallChange * ratio.X, 
                        puzzlePanel.VerticalScroll.Value + puzzlePanel.VerticalScroll.SmallChange * ratio.Y);
                }
            }
        }

        private void puzzlePanel_MouseUp(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Default;
        }
    }
}
