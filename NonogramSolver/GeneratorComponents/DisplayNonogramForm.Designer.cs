namespace NonogramSolverGenerator
{
    partial class DisplayNonogramForm
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
            this.components = new System.ComponentModel.Container();
            this.sfd = new System.Windows.Forms.SaveFileDialog();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tips = new System.Windows.Forms.ToolTip(this.components);
            this.btnBack = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.puzzlePanel = new System.Windows.Forms.Panel();
            this.pnlCorner = new System.Windows.Forms.Panel();
            this.vScroller = new System.Windows.Forms.VScrollBar();
            this.hScroller = new System.Windows.Forms.HScrollBar();
            this.btnPanel = new System.Windows.Forms.Panel();
            this.puzzlePanel.SuspendLayout();
            this.btnPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // sfd
            // 
            this.sfd.DefaultExt = "NONOGRAM";
            this.sfd.Filter = "Nonogram Files | *.NONOGRAM";
            this.sfd.Title = "Save Puzzle";
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // btnBack
            // 
            this.btnBack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBack.Location = new System.Drawing.Point(41, 25);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(147, 50);
            this.btnBack.TabIndex = 0;
            this.btnBack.Text = "Back";
            this.tips.SetToolTip(this.btnBack, "Return to Upload Image");
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.BtnBack_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.Location = new System.Drawing.Point(613, 25);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(147, 50);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save";
            this.tips.SetToolTip(this.btnSave, "Save your Nonogram");
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // puzzlePanel
            // 
            this.puzzlePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.puzzlePanel.AutoScroll = true;
            this.puzzlePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(208)))), ((int)(((byte)(220)))));
            this.puzzlePanel.Controls.Add(this.pnlCorner);
            this.puzzlePanel.Controls.Add(this.vScroller);
            this.puzzlePanel.Controls.Add(this.hScroller);
            this.puzzlePanel.Location = new System.Drawing.Point(0, 99);
            this.puzzlePanel.Name = "puzzlePanel";
            this.puzzlePanel.Size = new System.Drawing.Size(800, 791);
            this.puzzlePanel.TabIndex = 23;
            this.puzzlePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.puzzlePanel_Paint);
            this.puzzlePanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.puzzlePanel_MouseDown);
            this.puzzlePanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.puzzlePanel_MouseMove);
            this.puzzlePanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.puzzlePanel_MouseUp);
            // 
            // pnlCorner
            // 
            this.pnlCorner.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlCorner.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.pnlCorner.Location = new System.Drawing.Point(770, 761);
            this.pnlCorner.Name = "pnlCorner";
            this.pnlCorner.Size = new System.Drawing.Size(30, 30);
            this.pnlCorner.TabIndex = 4;
            this.pnlCorner.Visible = false;
            // 
            // vScroller
            // 
            this.vScroller.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.vScroller.Location = new System.Drawing.Point(770, 0);
            this.vScroller.Name = "vScroller";
            this.vScroller.Size = new System.Drawing.Size(30, 761);
            this.vScroller.TabIndex = 24;
            this.vScroller.Visible = false;
            // 
            // hScroller
            // 
            this.hScroller.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hScroller.Location = new System.Drawing.Point(0, 761);
            this.hScroller.Name = "hScroller";
            this.hScroller.Size = new System.Drawing.Size(772, 30);
            this.hScroller.TabIndex = 23;
            this.hScroller.Visible = false;
            // 
            // btnPanel
            // 
            this.btnPanel.BackColor = System.Drawing.Color.PowderBlue;
            this.btnPanel.Controls.Add(this.btnSave);
            this.btnPanel.Controls.Add(this.btnBack);
            this.btnPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnPanel.Location = new System.Drawing.Point(0, 0);
            this.btnPanel.Name = "btnPanel";
            this.btnPanel.Size = new System.Drawing.Size(800, 100);
            this.btnPanel.TabIndex = 24;
            // 
            // DisplayNonogramForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.PowderBlue;
            this.ClientSize = new System.Drawing.Size(800, 890);
            this.Controls.Add(this.btnPanel);
            this.Controls.Add(this.puzzlePanel);
            this.Font = new System.Drawing.Font("Book Antiqua", 18F);
            this.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.Name = "DisplayNonogramForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Display Nonogram";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DisplayNonogramForm_FormClosed);
            this.puzzlePanel.ResumeLayout(false);
            this.btnPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.SaveFileDialog sfd;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolTip tips;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Panel puzzlePanel;
        private System.Windows.Forms.Panel btnPanel;
        private System.Windows.Forms.Panel pnlCorner;
        private System.Windows.Forms.VScrollBar vScroller;
        private System.Windows.Forms.HScrollBar hScroller;
    }
}