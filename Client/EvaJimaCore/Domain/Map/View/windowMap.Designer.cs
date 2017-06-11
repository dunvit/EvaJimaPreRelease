namespace EveJimaCore.Domain.Map.View
{
    partial class WindowMap
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
            this.lblUpdateTime = new System.Windows.Forms.Label();
            this.lblMouseMoveCoordinates = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblUpdateTime
            // 
            this.lblUpdateTime.ForeColor = System.Drawing.Color.Gray;
            this.lblUpdateTime.Location = new System.Drawing.Point(12, 9);
            this.lblUpdateTime.Name = "lblUpdateTime";
            this.lblUpdateTime.Size = new System.Drawing.Size(141, 18);
            this.lblUpdateTime.TabIndex = 0;
            this.lblUpdateTime.Text = "label1";
            // 
            // lblMouseMoveCoordinates
            // 
            this.lblMouseMoveCoordinates.ForeColor = System.Drawing.Color.Gray;
            this.lblMouseMoveCoordinates.Location = new System.Drawing.Point(12, 27);
            this.lblMouseMoveCoordinates.Name = "lblMouseMoveCoordinates";
            this.lblMouseMoveCoordinates.Size = new System.Drawing.Size(175, 40);
            this.lblMouseMoveCoordinates.TabIndex = 1;
            this.lblMouseMoveCoordinates.Text = "label1";
            // 
            // WindowMap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(14)))), ((int)(((byte)(14)))));
            this.ClientSize = new System.Drawing.Size(720, 555);
            this.Controls.Add(this.lblMouseMoveCoordinates);
            this.Controls.Add(this.lblUpdateTime);
            this.DoubleBuffered = true;
            this.Name = "WindowMap";
            this.Text = "windowMap";
            this.Load += new System.EventHandler(this.WindowMap_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Event_OnPaint);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Event_MouseMove);
            this.Resize += new System.EventHandler(this.Event_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblUpdateTime;
        private System.Windows.Forms.Label lblMouseMoveCoordinates;
    }
}