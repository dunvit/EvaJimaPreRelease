namespace EveJimaCore.Logic.MapInformation
{
    partial class InformationMapSettingsView
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtServerAddress = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.cmdUpdateMapSettings = new EveJimaCore.WhlControls.ejButton();
            this.txtOwner = new System.Windows.Forms.TextBox();
            this.txtKey = new System.Windows.Forms.TextBox();
            this.lnlSystemText = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmdReload = new EveJimaCore.WhlControls.ejButton();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.cmdReload);
            this.groupBox2.Controls.Add(this.txtServerAddress);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.checkBox2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.checkBox1);
            this.groupBox2.Controls.Add(this.cmdUpdateMapSettings);
            this.groupBox2.Controls.Add(this.txtOwner);
            this.groupBox2.Controls.Add(this.txtKey);
            this.groupBox2.Controls.Add(this.lnlSystemText);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold);
            this.groupBox2.ForeColor = System.Drawing.Color.FloralWhite;
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(310, 345);
            this.groupBox2.TabIndex = 146;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = " Map Settings ";
            // 
            // txtServerAddress
            // 
            this.txtServerAddress.BackColor = System.Drawing.Color.Gray;
            this.txtServerAddress.Location = new System.Drawing.Point(169, 143);
            this.txtServerAddress.Name = "txtServerAddress";
            this.txtServerAddress.ReadOnly = true;
            this.txtServerAddress.Size = new System.Drawing.Size(125, 20);
            this.txtServerAddress.TabIndex = 151;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(12, 143);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(151, 23);
            this.label2.TabIndex = 150;
            this.label2.Text = "Server address";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Enabled = false;
            this.checkBox2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBox2.Location = new System.Drawing.Point(168, 106);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(12, 11);
            this.checkBox2.TabIndex = 149;
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(1, 103);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(162, 40);
            this.label1.TabIndex = 148;
            this.label1.Text = "Member can delete systems";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Enabled = false;
            this.checkBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBox1.Location = new System.Drawing.Point(168, 83);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(12, 11);
            this.checkBox1.TabIndex = 147;
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // cmdUpdateMapSettings
            // 
            this.cmdUpdateMapSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdUpdateMapSettings.AutoSize = true;
            this.cmdUpdateMapSettings.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmdUpdateMapSettings.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(14)))), ((int)(((byte)(14)))));
            this.cmdUpdateMapSettings.FlatAppearance.BorderSize = 0;
            this.cmdUpdateMapSettings.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(14)))), ((int)(((byte)(14)))));
            this.cmdUpdateMapSettings.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(14)))), ((int)(((byte)(14)))));
            this.cmdUpdateMapSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdUpdateMapSettings.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdUpdateMapSettings.ForeColor = System.Drawing.Color.OliveDrab;
            this.cmdUpdateMapSettings.Location = new System.Drawing.Point(211, 304);
            this.cmdUpdateMapSettings.Name = "cmdUpdateMapSettings";
            this.cmdUpdateMapSettings.Size = new System.Drawing.Size(82, 23);
            this.cmdUpdateMapSettings.TabIndex = 146;
            this.cmdUpdateMapSettings.Tag = "MapSignatures";
            this.cmdUpdateMapSettings.Text = "Update All";
            this.cmdUpdateMapSettings.UseVisualStyleBackColor = true;
            this.cmdUpdateMapSettings.Click += new System.EventHandler(this.cmdUpdateMapSettings_Click);
            // 
            // txtOwner
            // 
            this.txtOwner.BackColor = System.Drawing.Color.Gray;
            this.txtOwner.Location = new System.Drawing.Point(168, 27);
            this.txtOwner.Name = "txtOwner";
            this.txtOwner.ReadOnly = true;
            this.txtOwner.Size = new System.Drawing.Size(125, 20);
            this.txtOwner.TabIndex = 132;
            // 
            // txtKey
            // 
            this.txtKey.BackColor = System.Drawing.Color.Silver;
            this.txtKey.Location = new System.Drawing.Point(168, 53);
            this.txtKey.Name = "txtKey";
            this.txtKey.Size = new System.Drawing.Size(125, 20);
            this.txtKey.TabIndex = 131;
            // 
            // lnlSystemText
            // 
            this.lnlSystemText.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lnlSystemText.ForeColor = System.Drawing.Color.White;
            this.lnlSystemText.Location = new System.Drawing.Point(11, 27);
            this.lnlSystemText.Name = "lnlSystemText";
            this.lnlSystemText.Size = new System.Drawing.Size(151, 23);
            this.lnlSystemText.TabIndex = 125;
            this.lnlSystemText.Text = "Map Owner";
            this.lnlSystemText.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(1, 80);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(162, 23);
            this.label5.TabIndex = 129;
            this.label5.Text = "Save LowSec systems";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(11, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(151, 23);
            this.label4.TabIndex = 126;
            this.label4.Text = "Map Key";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // cmdReload
            // 
            this.cmdReload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdReload.AutoSize = true;
            this.cmdReload.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmdReload.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(14)))), ((int)(((byte)(14)))));
            this.cmdReload.FlatAppearance.BorderSize = 0;
            this.cmdReload.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(14)))), ((int)(((byte)(14)))));
            this.cmdReload.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(14)))), ((int)(((byte)(14)))));
            this.cmdReload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdReload.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdReload.ForeColor = System.Drawing.Color.OliveDrab;
            this.cmdReload.Location = new System.Drawing.Point(114, 304);
            this.cmdReload.Name = "cmdReload";
            this.cmdReload.Size = new System.Drawing.Size(83, 23);
            this.cmdReload.TabIndex = 152;
            this.cmdReload.Tag = "MapSignatures";
            this.cmdReload.Text = "Reload Map";
            this.cmdReload.UseVisualStyleBackColor = true;
            this.cmdReload.Click += new System.EventHandler(this.cmdReload_Click);
            // 
            // InformationMapSettingsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(14)))), ((int)(((byte)(14)))));
            this.Controls.Add(this.groupBox2);
            this.Name = "InformationMapSettingsView";
            this.Size = new System.Drawing.Size(349, 351);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtServerAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox1;
        private WhlControls.ejButton cmdUpdateMapSettings;
        private System.Windows.Forms.TextBox txtOwner;
        private System.Windows.Forms.TextBox txtKey;
        private System.Windows.Forms.Label lnlSystemText;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private WhlControls.ejButton cmdReload;
    }
}
