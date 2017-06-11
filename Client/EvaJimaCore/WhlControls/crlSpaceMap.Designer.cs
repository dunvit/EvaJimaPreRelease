namespace EveJimaCore.WhlControls
{
    partial class crlSpaceMap
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
            this.mapContainer = new System.Windows.Forms.Panel();
            this.toolbarContainer = new System.Windows.Forms.Panel();
            this.informationContainer = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // mapContainer
            // 
            this.mapContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mapContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(34)))), ((int)(((byte)(34)))));
            this.mapContainer.Location = new System.Drawing.Point(333, 2);
            this.mapContainer.Name = "mapContainer";
            this.mapContainer.Size = new System.Drawing.Size(700, 511);
            this.mapContainer.TabIndex = 0;
            // 
            // toolbarContainer
            // 
            this.toolbarContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(34)))), ((int)(((byte)(34)))));
            this.toolbarContainer.Location = new System.Drawing.Point(3, 2);
            this.toolbarContainer.Name = "toolbarContainer";
            this.toolbarContainer.Size = new System.Drawing.Size(326, 22);
            this.toolbarContainer.TabIndex = 1;
            // 
            // informationContainer
            // 
            this.informationContainer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.informationContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(14)))), ((int)(((byte)(14)))));
            this.informationContainer.Location = new System.Drawing.Point(3, 30);
            this.informationContainer.Name = "informationContainer";
            this.informationContainer.Size = new System.Drawing.Size(326, 485);
            this.informationContainer.TabIndex = 2;
            // 
            // crlSpaceMap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(14)))), ((int)(((byte)(14)))));
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.informationContainer);
            this.Controls.Add(this.toolbarContainer);
            this.Controls.Add(this.mapContainer);
            this.Name = "crlSpaceMap";
            this.Size = new System.Drawing.Size(1037, 518);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel mapContainer;
        private System.Windows.Forms.Panel toolbarContainer;
        private System.Windows.Forms.Panel informationContainer;
    }
}
