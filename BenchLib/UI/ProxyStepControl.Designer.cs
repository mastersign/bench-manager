namespace Mastersign.Bench.UI
{
    partial class ProxyStepControl
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.Label lblHttpProxy;
            System.Windows.Forms.Label lblHttpsProxy;
            System.Windows.Forms.Label lblExample;
            this.txtHttpProxy = new System.Windows.Forms.TextBox();
            this.txtHttpsProxy = new System.Windows.Forms.TextBox();
            lblHttpProxy = new System.Windows.Forms.Label();
            lblHttpsProxy = new System.Windows.Forms.Label();
            lblExample = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblHttpProxy
            // 
            lblHttpProxy.AutoSize = true;
            lblHttpProxy.Location = new System.Drawing.Point(15, 31);
            lblHttpProxy.Name = "lblHttpProxy";
            lblHttpProxy.Size = new System.Drawing.Size(36, 13);
            lblHttpProxy.TabIndex = 0;
            lblHttpProxy.Text = "&HTTP";
            // 
            // lblHttpsProxy
            // 
            lblHttpsProxy.AutoSize = true;
            lblHttpsProxy.Location = new System.Drawing.Point(15, 57);
            lblHttpsProxy.Name = "lblHttpsProxy";
            lblHttpsProxy.Size = new System.Drawing.Size(43, 13);
            lblHttpsProxy.TabIndex = 1;
            lblHttpsProxy.Text = "HTTP&S";
            // 
            // txtHttpProxy
            // 
            this.txtHttpProxy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHttpProxy.Location = new System.Drawing.Point(64, 28);
            this.txtHttpProxy.Name = "txtHttpProxy";
            this.txtHttpProxy.Size = new System.Drawing.Size(385, 20);
            this.txtHttpProxy.TabIndex = 2;
            // 
            // txtHttpsProxy
            // 
            this.txtHttpsProxy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHttpsProxy.Location = new System.Drawing.Point(64, 54);
            this.txtHttpsProxy.Name = "txtHttpsProxy";
            this.txtHttpsProxy.Size = new System.Drawing.Size(385, 20);
            this.txtHttpsProxy.TabIndex = 3;
            // 
            // lblExample
            // 
            lblExample.AutoSize = true;
            lblExample.ForeColor = System.Drawing.SystemColors.GrayText;
            lblExample.Location = new System.Drawing.Point(61, 12);
            lblExample.Name = "lblExample";
            lblExample.Size = new System.Drawing.Size(139, 13);
            lblExample.TabIndex = 4;
            lblExample.Text = "e.g.: http://10.0.20.1:3128/";
            // 
            // ProxyStepControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(lblExample);
            this.Controls.Add(this.txtHttpsProxy);
            this.Controls.Add(this.txtHttpProxy);
            this.Controls.Add(lblHttpsProxy);
            this.Controls.Add(lblHttpProxy);
            this.Name = "ProxyStepControl";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtHttpProxy;
        private System.Windows.Forms.TextBox txtHttpsProxy;
    }
}
