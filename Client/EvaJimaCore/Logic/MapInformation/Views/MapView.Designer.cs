namespace EveJimaCore.Logic.MapInformation
{
    partial class MapView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblUpdateTime = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblUpdateTime
            // 
            this.lblUpdateTime.ForeColor = System.Drawing.Color.Gray;
            this.lblUpdateTime.Location = new System.Drawing.Point(3, 0);
            this.lblUpdateTime.Name = "lblUpdateTime";
            this.lblUpdateTime.Size = new System.Drawing.Size(141, 18);
            this.lblUpdateTime.TabIndex = 1;
            this.lblUpdateTime.Text = "label1";
            // 
            // MapView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(14)))), ((int)(((byte)(14)))));
            this.Controls.Add(this.lblUpdateTime);
            this.DoubleBuffered = true;
            this.Name = "MapView";
            this.Size = new System.Drawing.Size(523, 411);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Event_Paint);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Event_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblUpdateTime;
    }
}
