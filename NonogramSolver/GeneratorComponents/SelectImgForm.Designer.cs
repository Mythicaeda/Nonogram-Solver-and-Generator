namespace NonogramSolverGenerator
{
    partial class SelectImgForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectImgForm));
            this.openImgDialog = new System.Windows.Forms.OpenFileDialog();
            this.btnLoadImg = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.pbUploaded = new System.Windows.Forms.PictureBox();
            this.lblInstructions = new System.Windows.Forms.Label();
            this.bgwGenerator = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.pbUploaded)).BeginInit();
            this.SuspendLayout();
            // 
            // btnLoadImg
            // 
            this.btnLoadImg.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLoadImg.Location = new System.Drawing.Point(468, 157);
            this.btnLoadImg.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.btnLoadImg.Name = "btnLoadImg";
            this.btnLoadImg.Size = new System.Drawing.Size(275, 50);
            this.btnLoadImg.TabIndex = 1;
            this.btnLoadImg.Text = "Upload Image";
            this.btnLoadImg.UseVisualStyleBackColor = true;
            this.btnLoadImg.Click += new System.EventHandler(this.BtnLoadImg_Click);
            // 
            // btnBack
            // 
            this.btnBack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBack.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnBack.Location = new System.Drawing.Point(405, 300);
            this.btnBack.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(175, 50);
            this.btnBack.TabIndex = 2;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.BtnBack_Click);
            // 
            // btnGenerate
            // 
            this.btnGenerate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGenerate.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnGenerate.Location = new System.Drawing.Point(635, 300);
            this.btnGenerate.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(175, 50);
            this.btnGenerate.TabIndex = 3;
            this.btnGenerate.Text = "Proceed";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.BtnGenerate_Click);
            // 
            // pbUploaded
            // 
            this.pbUploaded.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.pbUploaded.BackColor = System.Drawing.Color.Transparent;
            this.pbUploaded.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbUploaded.Image = global::NonogramSolverGenerator.Properties.Resources.DefaultImg;
            this.pbUploaded.InitialImage = global::NonogramSolverGenerator.Properties.Resources.DefaultImg;
            this.pbUploaded.Location = new System.Drawing.Point(16, 16);
            this.pbUploaded.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.pbUploaded.Name = "pbUploaded";
            this.pbUploaded.Size = new System.Drawing.Size(334, 334);
            this.pbUploaded.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbUploaded.TabIndex = 0;
            this.pbUploaded.TabStop = false;
            this.pbUploaded.Click += new System.EventHandler(this.BtnLoadImg_Click);
            // 
            // lblInstructions
            // 
            this.lblInstructions.Location = new System.Drawing.Point(405, 37);
            this.lblInstructions.Name = "lblInstructions";
            this.lblInstructions.Size = new System.Drawing.Size(405, 90);
            this.lblInstructions.TabIndex = 4;
            this.lblInstructions.Text = "Upload a square grayscale image by either dragging it onto the form or by using t" +
    "he button below:";
            this.lblInstructions.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bgwGenerator
            // 
            this.bgwGenerator.WorkerReportsProgress = true;
            this.bgwGenerator.WorkerSupportsCancellation = true;
            this.bgwGenerator.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BgwGenerator_DoWork);
            this.bgwGenerator.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BgwGenerator_RunWorkerCompleted);
            // 
            // SelectImgForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.PowderBlue;
            this.CancelButton = this.btnBack;
            this.ClientSize = new System.Drawing.Size(827, 365);
            this.Controls.Add(this.lblInstructions);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnLoadImg);
            this.Controls.Add(this.pbUploaded);
            this.Font = new System.Drawing.Font("Book Antiqua", 18F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.Name = "SelectImgForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Upload Image";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SelectImgForm_FormClosed);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Uploaded_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Uploaded_DragEnter);
            ((System.ComponentModel.ISupportInitialize)(this.pbUploaded)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbUploaded;
        private System.Windows.Forms.OpenFileDialog openImgDialog;
        private System.Windows.Forms.Button btnLoadImg;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Label lblInstructions;
        private System.ComponentModel.BackgroundWorker bgwGenerator;
    }
}