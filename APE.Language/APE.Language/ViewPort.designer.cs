﻿namespace APE
{
    partial class ViewPort
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
            this.breakButton = new System.Windows.Forms.Button();
            this.abortButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.aboutButton = new System.Windows.Forms.Button();
            this.formAnimationDisabledPictureBox = new System.Windows.Forms.PictureBox();
            this.formAnimationDisabledLabel = new System.Windows.Forms.Label();
            this.elevatedAdminPictureBox = new System.Windows.Forms.PictureBox();
            this.elevatedAdministratorLabel = new System.Windows.Forms.Label();
            this.rtbLogViewer = new System.Windows.Forms.RichTextBox();
            this.ctxtMenuViewPort = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sepToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.formAnimationDisabledPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.elevatedAdminPictureBox)).BeginInit();
            this.ctxtMenuViewPort.SuspendLayout();
            this.SuspendLayout();
            // 
            // breakButton
            // 
            this.breakButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.breakButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.breakButton.Location = new System.Drawing.Point(5, 46);
            this.breakButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.breakButton.Name = "breakButton";
            this.breakButton.Size = new System.Drawing.Size(204, 34);
            this.breakButton.TabIndex = 0;
            this.breakButton.Text = "Break";
            this.breakButton.UseVisualStyleBackColor = true;
            this.breakButton.Click += new System.EventHandler(this.btnBreak_Click);
            // 
            // abortButton
            // 
            this.abortButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.abortButton.Location = new System.Drawing.Point(5, 8);
            this.abortButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.abortButton.Name = "abortButton";
            this.abortButton.Size = new System.Drawing.Size(100, 34);
            this.abortButton.TabIndex = 1;
            this.abortButton.Text = "Abort";
            this.abortButton.UseVisualStyleBackColor = true;
            this.abortButton.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.aboutButton);
            this.panel1.Controls.Add(this.formAnimationDisabledPictureBox);
            this.panel1.Controls.Add(this.formAnimationDisabledLabel);
            this.panel1.Controls.Add(this.elevatedAdminPictureBox);
            this.panel1.Controls.Add(this.elevatedAdministratorLabel);
            this.panel1.Controls.Add(this.abortButton);
            this.panel1.Controls.Add(this.breakButton);
            this.panel1.Location = new System.Drawing.Point(241, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(214, 142);
            this.panel1.TabIndex = 6;
            // 
            // aboutButton
            // 
            this.aboutButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.aboutButton.Location = new System.Drawing.Point(108, 8);
            this.aboutButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.aboutButton.Name = "aboutButton";
            this.aboutButton.Size = new System.Drawing.Size(100, 34);
            this.aboutButton.TabIndex = 6;
            this.aboutButton.Text = "About";
            this.aboutButton.UseVisualStyleBackColor = true;
            this.aboutButton.Click += new System.EventHandler(this.AboutButton_Click);
            // 
            // formAnimationDisabledPictureBox
            // 
            this.formAnimationDisabledPictureBox.Location = new System.Drawing.Point(184, 108);
            this.formAnimationDisabledPictureBox.Name = "formAnimationDisabledPictureBox";
            this.formAnimationDisabledPictureBox.Size = new System.Drawing.Size(24, 24);
            this.formAnimationDisabledPictureBox.TabIndex = 5;
            this.formAnimationDisabledPictureBox.TabStop = false;
            // 
            // formAnimationDisabledLabel
            // 
            this.formAnimationDisabledLabel.AutoSize = true;
            this.formAnimationDisabledLabel.Location = new System.Drawing.Point(0, 112);
            this.formAnimationDisabledLabel.Name = "formAnimationDisabledLabel";
            this.formAnimationDisabledLabel.Size = new System.Drawing.Size(187, 20);
            this.formAnimationDisabledLabel.TabIndex = 4;
            this.formAnimationDisabledLabel.Text = "Form Animation Disabled";
            // 
            // elevatedAdminPictureBox
            // 
            this.elevatedAdminPictureBox.Location = new System.Drawing.Point(184, 82);
            this.elevatedAdminPictureBox.Name = "elevatedAdminPictureBox";
            this.elevatedAdminPictureBox.Size = new System.Drawing.Size(24, 24);
            this.elevatedAdminPictureBox.TabIndex = 3;
            this.elevatedAdminPictureBox.TabStop = false;
            // 
            // elevatedAdministratorLabel
            // 
            this.elevatedAdministratorLabel.AutoSize = true;
            this.elevatedAdministratorLabel.Location = new System.Drawing.Point(0, 86);
            this.elevatedAdministratorLabel.Name = "elevatedAdministratorLabel";
            this.elevatedAdministratorLabel.Size = new System.Drawing.Size(169, 20);
            this.elevatedAdministratorLabel.TabIndex = 2;
            this.elevatedAdministratorLabel.Text = "Elevated Administrator";
            // 
            // rtbLogViewer
            // 
            this.rtbLogViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbLogViewer.ContextMenuStrip = this.ctxtMenuViewPort;
            this.rtbLogViewer.Location = new System.Drawing.Point(8, 8);
            this.rtbLogViewer.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rtbLogViewer.Name = "rtbLogViewer";
            this.rtbLogViewer.ReadOnly = true;
            this.rtbLogViewer.Size = new System.Drawing.Size(234, 126);
            this.rtbLogViewer.TabIndex = 7;
            this.rtbLogViewer.Text = "";
            // 
            // ctxtMenuViewPort
            // 
            this.ctxtMenuViewPort.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.ctxtMenuViewPort.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.sepToolStripMenuItem,
            this.selectAllToolStripMenuItem,
            this.clearToolStripMenuItem});
            this.ctxtMenuViewPort.Name = "ctxtMenuViewPort";
            this.ctxtMenuViewPort.Size = new System.Drawing.Size(169, 130);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(168, 30);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(168, 30);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // sepToolStripMenuItem
            // 
            this.sepToolStripMenuItem.Name = "sepToolStripMenuItem";
            this.sepToolStripMenuItem.Size = new System.Drawing.Size(165, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(168, 30);
            this.selectAllToolStripMenuItem.Text = "Select All";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(168, 30);
            this.clearToolStripMenuItem.Text = "Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // ViewPort
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(455, 142);
            this.Controls.Add(this.rtbLogViewer);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ViewPort";
            this.Text = "APE ViewPort";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmViewPort_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.formAnimationDisabledPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.elevatedAdminPictureBox)).EndInit();
            this.ctxtMenuViewPort.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button breakButton;
        private System.Windows.Forms.Button abortButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RichTextBox rtbLogViewer;
        private System.Windows.Forms.ContextMenuStrip ctxtMenuViewPort;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator sepToolStripMenuItem;
        private System.Windows.Forms.PictureBox elevatedAdminPictureBox;
        private System.Windows.Forms.Label elevatedAdministratorLabel;
        private System.Windows.Forms.PictureBox formAnimationDisabledPictureBox;
        private System.Windows.Forms.Label formAnimationDisabledLabel;
        private System.Windows.Forms.Button aboutButton;
    }
}