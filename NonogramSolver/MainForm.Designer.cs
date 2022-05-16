namespace NonogramSolverGenerator
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.linkAbout = new System.Windows.Forms.LinkLabel();
            this.lblGenerate = new System.Windows.Forms.Label();
            this.lblSolve = new System.Windows.Forms.Label();
            this.pbGenerator = new System.Windows.Forms.PictureBox();
            this.pbSolving = new System.Windows.Forms.PictureBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbGenerator)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSolving)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.PowderBlue;
            this.panel1.Controls.Add(this.linkAbout);
            this.panel1.Controls.Add(this.lblGenerate);
            this.panel1.Controls.Add(this.lblSolve);
            this.panel1.Controls.Add(this.pbGenerator);
            this.panel1.Controls.Add(this.pbSolving);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(802, 451);
            this.panel1.TabIndex = 0;
            // 
            // linkAbout
            // 
            this.linkAbout.AutoSize = true;
            this.linkAbout.Font = new System.Drawing.Font("Book Antiqua", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkAbout.Location = new System.Drawing.Point(255, 393);
            this.linkAbout.Name = "linkAbout";
            this.linkAbout.Size = new System.Drawing.Size(243, 28);
            this.linkAbout.TabIndex = 4;
            this.linkAbout.TabStop = true;
            this.linkAbout.Text = "What is a Nonogram?";
            this.linkAbout.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkAbout_LinkClicked);
            // 
            // lblGenerate
            // 
            this.lblGenerate.AutoSize = true;
            this.lblGenerate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblGenerate.Font = new System.Drawing.Font("Book Antiqua", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGenerate.Location = new System.Drawing.Point(428, 301);
            this.lblGenerate.Name = "lblGenerate";
            this.lblGenerate.Size = new System.Drawing.Size(255, 28);
            this.lblGenerate.TabIndex = 3;
            this.lblGenerate.Text = "Generate a Nonogram!";
            this.lblGenerate.Click += new System.EventHandler(this.pbGenerator_Click);
            // 
            // lblSolve
            // 
            this.lblSolve.AutoSize = true;
            this.lblSolve.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblSolve.Font = new System.Drawing.Font("Book Antiqua", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSolve.Location = new System.Drawing.Point(96, 301);
            this.lblSolve.Name = "lblSolve";
            this.lblSolve.Size = new System.Drawing.Size(217, 28);
            this.lblSolve.TabIndex = 2;
            this.lblSolve.Text = "Solve a Nonogram!";
            this.lblSolve.Click += new System.EventHandler(this.pbSolving_Click);
            // 
            // pbGenerator
            // 
            this.pbGenerator.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbGenerator.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbGenerator.Image = global::NonogramSolverGenerator.Properties.Resources.ImgToPuzzle;
            this.pbGenerator.ImageLocation = "";
            this.pbGenerator.Location = new System.Drawing.Point(451, 86);
            this.pbGenerator.Name = "pbGenerator";
            this.pbGenerator.Size = new System.Drawing.Size(200, 200);
            this.pbGenerator.TabIndex = 1;
            this.pbGenerator.TabStop = false;
            this.pbGenerator.Click += new System.EventHandler(this.pbGenerator_Click);
            // 
            // pbSolving
            // 
            this.pbSolving.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbSolving.Image = global::NonogramSolverGenerator.Properties.Resources.Solving;
            this.pbSolving.Location = new System.Drawing.Point(101, 86);
            this.pbSolving.Name = "pbSolving";
            this.pbSolving.Size = new System.Drawing.Size(200, 200);
            this.pbSolving.TabIndex = 0;
            this.pbSolving.TabStop = false;
            this.pbSolving.Click += new System.EventHandler(this.pbSolving_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Nonogram Solver";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbGenerator)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSolving)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.PictureBox pbGenerator;
        private System.Windows.Forms.PictureBox pbSolving;
        private System.Windows.Forms.Label lblGenerate;
        private System.Windows.Forms.Label lblSolve;
        private System.Windows.Forms.LinkLabel linkAbout;
    }
}

