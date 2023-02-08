
namespace Nottext_Data_Protector.Forms
{
    partial class Configuracoes
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Configuracoes));
            this.label1 = new System.Windows.Forms.Label();
            this.protecaoGlobal = new Guna.UI2.WinForms.Guna2ToggleSwitch();
            this.label2 = new System.Windows.Forms.Label();
            this.removerTudo = new Guna.UI2.WinForms.Guna2Button();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(123, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Proteção global:";
            // 
            // protecaoGlobal
            // 
            this.protecaoGlobal.Checked = true;
            this.protecaoGlobal.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.protecaoGlobal.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.protecaoGlobal.CheckedState.InnerBorderColor = System.Drawing.Color.White;
            this.protecaoGlobal.CheckedState.InnerColor = System.Drawing.Color.White;
            this.protecaoGlobal.CheckedState.Parent = this.protecaoGlobal;
            this.protecaoGlobal.Cursor = System.Windows.Forms.Cursors.Hand;
            this.protecaoGlobal.Location = new System.Drawing.Point(141, 24);
            this.protecaoGlobal.Name = "protecaoGlobal";
            this.protecaoGlobal.ShadowDecoration.Parent = this.protecaoGlobal;
            this.protecaoGlobal.Size = new System.Drawing.Size(50, 20);
            this.protecaoGlobal.TabIndex = 2;
            this.protecaoGlobal.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(218)))), ((int)(((byte)(223)))));
            this.protecaoGlobal.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(218)))), ((int)(((byte)(223)))));
            this.protecaoGlobal.UncheckedState.InnerBorderColor = System.Drawing.Color.White;
            this.protecaoGlobal.UncheckedState.InnerColor = System.Drawing.Color.White;
            this.protecaoGlobal.UncheckedState.Parent = this.protecaoGlobal;
            this.protecaoGlobal.Click += new System.EventHandler(this.protecaoGlobal_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(13, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(290, 18);
            this.label2.TabIndex = 38;
            this.label2.Text = "Configura a proteção de todos os arquivos";
            // 
            // removerTudo
            // 
            this.removerTudo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.removerTudo.BackColor = System.Drawing.Color.Transparent;
            this.removerTudo.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.removerTudo.BorderRadius = 20;
            this.removerTudo.BorderThickness = 1;
            this.removerTudo.CheckedState.Parent = this.removerTudo;
            this.removerTudo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.removerTudo.CustomBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.removerTudo.CustomImages.Parent = this.removerTudo;
            this.removerTudo.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.removerTudo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.removerTudo.ForeColor = System.Drawing.Color.White;
            this.removerTudo.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.removerTudo.HoverState.Parent = this.removerTudo;
            this.removerTudo.Location = new System.Drawing.Point(16, 119);
            this.removerTudo.Name = "removerTudo";
            this.removerTudo.ShadowDecoration.BorderRadius = 5;
            this.removerTudo.ShadowDecoration.Color = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(234)))));
            this.removerTudo.ShadowDecoration.Depth = 40;
            this.removerTudo.ShadowDecoration.Parent = this.removerTudo;
            this.removerTudo.ShadowDecoration.Shadow = new System.Windows.Forms.Padding(7, 7, 7, 12);
            this.removerTudo.Size = new System.Drawing.Size(195, 42);
            this.removerTudo.TabIndex = 39;
            this.removerTudo.Text = "REMOVER TODOS COMPONENTES";
            this.removerTudo.Click += new System.EventHandler(this.removerTudo_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(13, 164);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(500, 18);
            this.label3.TabIndex = 40;
            this.label3.Text = "Desabilita a proteção e remove tudo relacionado ao Nottext Data Protector";
            // 
            // Configuracoes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 339);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.removerTudo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.protecaoGlobal);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Configuracoes";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Configurações";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        public Guna.UI2.WinForms.Guna2ToggleSwitch protecaoGlobal;
        private System.Windows.Forms.Label label2;
        private Guna.UI2.WinForms.Guna2Button removerTudo;
        private System.Windows.Forms.Label label3;
    }
}