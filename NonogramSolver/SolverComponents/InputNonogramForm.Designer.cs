namespace NonogramSolverGenerator
{
    partial class InputNonogramForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InputNonogramForm));
            this.btnUpload = new System.Windows.Forms.Button();
            this.tTips = new System.Windows.Forms.ToolTip(this.components);
            this.btnBack = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ulbCols = new NonogramSolverGenerator.SolverComponents.UserUpdatableListBox();
            this.ulbRows = new NonogramSolverGenerator.SolverComponents.UserUpdatableListBox();
            this.openNonDialog = new System.Windows.Forms.OpenFileDialog();
            this.bwSolver = new System.ComponentModel.BackgroundWorker();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnUpload
            // 
            this.btnUpload.Location = new System.Drawing.Point(44, 47);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(159, 38);
            this.btnUpload.TabIndex = 1;
            this.btnUpload.Text = "Upload";
            this.tTips.SetToolTip(this.btnUpload, "Upload a preexisting .NONOGRAM file");
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.BtnUpload_Click);
            // 
            // btnBack
            // 
            this.btnBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBack.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnBack.Location = new System.Drawing.Point(44, 394);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(159, 38);
            this.btnBack.TabIndex = 0;
            this.btnBack.Text = "Back";
            this.tTips.SetToolTip(this.btnBack, "Return to Main Page");
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.BtnBack_Click);
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNext.Location = new System.Drawing.Point(44, 321);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(159, 38);
            this.btnNext.TabIndex = 3;
            this.btnNext.Text = "Solve!";
            this.tTips.SetToolTip(this.btnNext, "Return to Main Page");
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.BtnNext_Click);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panel2.BackColor = System.Drawing.Color.PowderBlue;
            this.panel2.Controls.Add(this.btnNext);
            this.panel2.Controls.Add(this.btnUpload);
            this.panel2.Controls.Add(this.btnBack);
            this.panel2.Location = new System.Drawing.Point(1, 1);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(246, 478);
            this.panel2.TabIndex = 4;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(208)))), ((int)(((byte)(220)))));
            this.panel1.Controls.Add(this.ulbCols);
            this.panel1.Controls.Add(this.ulbRows);
            this.panel1.Location = new System.Drawing.Point(245, 1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(602, 478);
            this.panel1.TabIndex = 2;
            // 
            // ulbCols
            // 
            this.ulbCols.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ulbCols.BtnAddToolTip = "";
            this.ulbCols.BtnRemoveAllToolTip = "";
            this.ulbCols.BtnRemoveToolTip = "";
            this.ulbCols.Font = new System.Drawing.Font("Book Antiqua", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ulbCols.LabelToolTip = "";
            this.ulbCols.ListBoxToolTip = "";
            this.ulbCols.Location = new System.Drawing.Point(365, 24);
            this.ulbCols.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.ulbCols.Name = "ulbCols";
            this.ulbCols.Pattern = "^(\\b\\d+\\b\\s*)+$";
            this.ulbCols.Size = new System.Drawing.Size(183, 431);
            this.ulbCols.TabIndex = 9;
            this.ulbCols.Text = "Columns:";
            // 
            // ulbRows
            // 
            this.ulbRows.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ulbRows.BtnAddToolTip = "";
            this.ulbRows.BtnRemoveAllToolTip = "";
            this.ulbRows.BtnRemoveToolTip = "";
            this.ulbRows.Font = new System.Drawing.Font("Book Antiqua", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ulbRows.LabelToolTip = "";
            this.ulbRows.ListBoxToolTip = "";
            this.ulbRows.Location = new System.Drawing.Point(55, 24);
            this.ulbRows.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.ulbRows.Name = "ulbRows";
            this.ulbRows.Pattern = "^(\\b\\d+\\b\\s*)+$";
            this.ulbRows.Size = new System.Drawing.Size(183, 431);
            this.ulbRows.TabIndex = 8;
            this.ulbRows.Text = "Rows:";
            // 
            // bwSolver
            // 
            this.bwSolver.WorkerSupportsCancellation = true;
            this.bwSolver.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BwSolver_DoWork);
            this.bwSolver.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BwSolver_RunWorkerCompleted);
            // 
            // InputNonogramForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.PowderBlue;
            this.CancelButton = this.btnBack;
            this.ClientSize = new System.Drawing.Size(848, 480);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("Book Antiqua", 18F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.Name = "InputNonogramForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Input Nonogram";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.InputNonogramForm_FormClosed);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.INForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.INForm_DragEnter);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.ToolTip tTips;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Panel panel2;
        private SolverComponents.UserUpdatableListBox ulbRows;
        private SolverComponents.UserUpdatableListBox ulbCols;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.OpenFileDialog openNonDialog;
        private System.ComponentModel.BackgroundWorker bwSolver;
    }
}