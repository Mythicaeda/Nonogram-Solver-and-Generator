using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace NonogramSolverGenerator
{
    public partial class SelectImgForm : Form
    {
        private Form parent;
        private bool backButton = false;

        private int imgWidth, imgHeight;


        public SelectImgForm(Form parent)
        {
            this.parent = parent;

            InitializeComponent();

            pbUploaded.Image = Properties.Resources.DefaultImg;
            imgWidth = Properties.Resources.DefaultImg.Width;
            imgHeight = Properties.Resources.DefaultImg.Height;

            openImgDialog.Filter = "Image Files (*.PNG; *.JPEG; *.JPG; *.BMP) | *.PNG; *.JPEG; *.JPG; *.BMP";
            openImgDialog.Multiselect = false;
            //Filter string you provided is not valid. The filter string must contain a description of the filter, followed by the vertical bar (|) 
            //and the filter pattern. The strings for different filtering options must also be separated by the vertical bar. Example: "Text files (*.txt)|*.txt|All files (*.*)|*.*"'
        }

        private void BtnBack_Click(object sender, EventArgs e)
        {
            parent.Show();
            backButton = true;
            Close();
        }

        private void Uploaded_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)/*and img file, and valid img file at that */)
            {
                string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (filePaths.Length == 1 && Is_Img(filePaths[0]))
                {
                    e.Effect = DragDropEffects.Copy;
                    return;
                }
            }
            e.Effect = DragDropEffects.None;
        }

        private void Uploaded_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])(e.Data.GetData(DataFormats.FileDrop));

                using (var img = new Bitmap(((string[])e.Data.GetData(DataFormats.FileDrop))[0]))
                {
                    if (img.Height != img.Width)
                    {
                        MessageBox.Show("The image is not square.", "Image Upload Error", MessageBoxButtons.OK);
                        return;
                    }

                    /*var bmd = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, img.PixelFormat);

                    bool[] results = new bool[img.Height];

                    Parallel.For(0, img.Height, i => {
                        results[i] = ValidateRow(img, i);
                    });*/


                    if (img.PixelFormat == PixelFormat.Format1bppIndexed ||ValidateImg(img))
                    {
                        pbUploaded.ImageLocation = filePaths[0];
                        imgWidth = img.Width;
                        imgHeight = img.Height;

                        //btnGenerate.Enabled = true;
                    }
                    else { MessageBox.Show("The image is not in black and white.", "Image Upload Error", MessageBoxButtons.OK); }
                    //img.UnlockBits(bmd);
                }
            }
        }

        //supported file extensions: .PNG, .JPEG, .JPG, .BMP.
        private bool Is_Img(string imgPath)
        {
            if (imgPath == null||imgPath == string.Empty) return false;

            string ext = System.IO.Path.GetExtension(imgPath);

            if (ext.Equals(".png", StringComparison.OrdinalIgnoreCase) || ext.Equals(".jpeg", StringComparison.OrdinalIgnoreCase)
                || ext.Equals(".jpg", StringComparison.OrdinalIgnoreCase) || ext.Equals(".bmp", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        //parallel recursive scan that there is no 
        /*private bool ValidateRow(Bitmap image, int startRow)
        {
            foreach(var px in image.)
        }*/

        private bool ValidateImg(Bitmap img)
        {
            using (Bitmap converted = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format32bppArgb))
            {

                var bmpd = converted.LockBits(new Rectangle(0, 0, converted.Width, converted.Height), ImageLockMode.ReadOnly, converted.PixelFormat);

                // Get the address of the first line.
                IntPtr ptr = bmpd.Scan0;

                // Declare an array to hold the bytes of the bitmap.
                int bytes = Math.Abs(bmpd.Stride) * bmpd.Height;
                byte[] pixels = new byte[bytes];

                // Copy the Bytes
                System.Runtime.InteropServices.Marshal.Copy(ptr, pixels, 0, bytes);

                //Check that r = g = b (aka the pixel is a shade of gray)
                for(int pxCtr = 0; pxCtr <pixels.Length; pxCtr += 4)
                {
                    if (!(pixels[pxCtr] == pixels[pxCtr + 1] && pixels[pxCtr] == pixels[pxCtr + 2]))
                        return false;
                }

                
                //free memory
                converted.UnlockBits(bmpd);
                return true;
            }
        }

        private void SelectImgForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(!backButton)
                parent.Close();
        }

        private void BtnLoadImg_Click(object sender, EventArgs e)
        {
            if (openImgDialog.ShowDialog() == DialogResult.OK)
            {
                string imgPath = openImgDialog.FileName;
                if (!Is_Img(imgPath))
                {
                    MessageBox.Show("Select an image file (*.PNG; *.JPEG; *.JPG; *.BMP)");
                    openImgDialog.FileName = string.Empty;
                    return;
                }
                //we have an image, let's verify it
                using (var img = new Bitmap(imgPath))
                {
                    if (img.Height != img.Width)
                    {
                        MessageBox.Show("The image is not square.", "Image Upload Error", MessageBoxButtons.OK);
                        return;
                    }

                    if (img.PixelFormat == PixelFormat.Format1bppIndexed || ValidateImg(img))
                    {
                        pbUploaded.ImageLocation = imgPath;
                        imgWidth = img.Width;
                        imgHeight = img.Height;

                        //btnGenerate.Enabled = true;
                    }
                    else { MessageBox.Show("The image is not in grayscale.", "Image Upload Error", MessageBoxButtons.OK); }
                }
            }
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            //subscribe to SetDimension's Proceed click
            SetDimensionsForm dimenForm = new SetDimensionsForm(this, imgWidth, imgHeight);
            if(dimenForm.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine(dimenForm.RowCount + ", " + dimenForm.ColCount);
                Hide();
                ProgressBarForm pgf = new ProgressBarForm();// this);
                pgf.Show();
                bool[,] nonogram = Generate(pbUploaded.ImageLocation, dimenForm.RowCount, dimenForm.ColCount);               
                //this.Enabled = true;
                //Hide();
                new DisplayNonogramForm(this, nonogram).Show();
                pgf.Close();
            }
        }


        private void bgwGenerator_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            int[] args = e.Argument as int[];
            //args[0]; // dimenForm.RowCount
            //args[1]; //dimenForm.ColCount
        }

        //TODO: Speed this up. Currently, it takes ~30s to process a 3000x3000 image.
        //okay slowdown must be coming from GetPixel(), as when I halved its calls by moving its output to p, the time took went down to ~15s
        private bool[,] Generate(string imageLoc, int rowCount, int colCount)
        {
            bool[,] nonogram = new bool[colCount, rowCount];

            int entriesPerRow, entriesPerCol;
            double[,,] blackCounter = new double[colCount, rowCount,2];

            Bitmap tmp;

            if (imageLoc != null)
                tmp = new Bitmap(imageLoc);
            else
                tmp = (Bitmap)pbUploaded.Image.Clone();

            using (Bitmap bmp = tmp.Clone(new Rectangle(0, 0, tmp.Width, tmp.Height), PixelFormat.Format32bppPArgb))
            {
                tmp.Dispose();

                entriesPerRow = bmp.Height / rowCount;
                entriesPerCol = bmp.Width / colCount;

                for(int r = 0, entryR = 0; r < bmp.Height; r++)
                {
                    for(int c = 0, entryC = 0; c <bmp.Width; c++)
                    {
                        blackCounter[entryC, entryR, 1]++;
                        var p = bmp.GetPixel(r, c);
                        if(p.A > 128 && p.B < 128)
                        {
                            blackCounter[entryC, entryR,0]++;
                        }
                        if (entryC < colCount-1 && c >= (entryC + 1) * entriesPerCol)
                        {
                            entryC++;
                        }
                    }
                    if (entryR < rowCount-1 && r >= (entryR + 1) * entriesPerRow)
                    {
                        entryR++;
                    }
                }
            }

            //okay now convert to bool
            for (int r = 0; r < blackCounter.GetLength(0); r++)
            {
                for (int c = 0; c < blackCounter.GetLength(1); c++)
                {
                    nonogram[r, c] = (blackCounter[r, c, 0] / blackCounter[r, c, 1] > .5);
                }
            }
            return nonogram;
        }
    }
}
