using System;
using System.Windows.Forms;

namespace NonogramSolverGenerator
{
    public partial class NonogramDefForm : Form
    {
        public NonogramDefForm()
        {
            InitializeComponent();

            rtbDef.Text =
                "\tNonograms, also known as Hanjie, Griddles or Picross (short for ‘Picture Crossword’), are a form of logic puzzles in which a picture is "
                +"formed by filling in squares of a grid based on clues at the side of each column and row. " 
                + "\n\tYou can learn more about Nonograms here: https://en.wikipedia.org/wiki/Nonogram";
        }

        private void RtbDef_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int HideCaret(IntPtr hwnd);

        private void RtbDef_MouseDown(object sender, MouseEventArgs e)
        {
            HideCaret(rtbDef.Handle);
        }

        private void RtbDef_KeyUp(object sender, KeyEventArgs e)
        {
            HideCaret(rtbDef.Handle);
        }

        private void RtbDef_KeyPress(object sender, KeyPressEventArgs e)
        {
            HideCaret(rtbDef.Handle);
        }
    }
}
