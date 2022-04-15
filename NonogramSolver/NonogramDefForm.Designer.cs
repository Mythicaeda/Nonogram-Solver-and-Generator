namespace NonogramSolverGenerator
{
    partial class NonogramDefForm
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
            this.rtbDef = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // rtbDef
            // 
            this.rtbDef.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbDef.BackColor = System.Drawing.Color.PowderBlue;
            this.rtbDef.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbDef.Font = new System.Drawing.Font("Book Antiqua", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbDef.Location = new System.Drawing.Point(12, 12);
            this.rtbDef.Name = "rtbDef";
            this.rtbDef.ReadOnly = true;
            this.rtbDef.Size = new System.Drawing.Size(460, 337);
            this.rtbDef.TabIndex = 0;
            this.rtbDef.TabStop = false;
            this.rtbDef.Text = "";
            this.rtbDef.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.RtbDef_LinkClicked);
            this.rtbDef.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RtbDef_KeyPress);
            this.rtbDef.KeyUp += new System.Windows.Forms.KeyEventHandler(this.RtbDef_KeyUp);
            this.rtbDef.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RtbDef_MouseDown);
            // 
            // NonogramDefForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.PowderBlue;
            this.ClientSize = new System.Drawing.Size(484, 361);
            this.Controls.Add(this.rtbDef);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NonogramDefForm";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "What is a Nonogram?";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbDef;
    }
}