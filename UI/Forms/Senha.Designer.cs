
namespace Nottext_Data_Protector.Forms
{
    partial class Senha
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Senha));
            this.label1 = new System.Windows.Forms.Label();
            this.senhaTexto = new Guna.UI2.WinForms.Guna2TextBox();
            this.confirmar = new Guna.UI2.WinForms.Guna2Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(394, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Para continuar a operação, por favor, digite sua senha:";
            // 
            // senhaTexto
            // 
            this.senhaTexto.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.senhaTexto.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(221)))), ((int)(((byte)(226)))));
            this.senhaTexto.BorderThickness = 2;
            this.senhaTexto.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.senhaTexto.DefaultText = "";
            this.senhaTexto.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.senhaTexto.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.senhaTexto.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.senhaTexto.DisabledState.Parent = this.senhaTexto;
            this.senhaTexto.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.senhaTexto.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.senhaTexto.FocusedState.Parent = this.senhaTexto;
            this.senhaTexto.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.senhaTexto.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.senhaTexto.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.senhaTexto.HoverState.Parent = this.senhaTexto;
            this.senhaTexto.Location = new System.Drawing.Point(16, 72);
            this.senhaTexto.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.senhaTexto.Name = "senhaTexto";
            this.senhaTexto.PasswordChar = '*';
            this.senhaTexto.PlaceholderText = "";
            this.senhaTexto.SelectedText = "";
            this.senhaTexto.ShadowDecoration.Parent = this.senhaTexto;
            this.senhaTexto.Size = new System.Drawing.Size(450, 31);
            this.senhaTexto.TabIndex = 3;
            // 
            // confirmar
            // 
            this.confirmar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.confirmar.BackColor = System.Drawing.Color.Transparent;
            this.confirmar.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.confirmar.BorderRadius = 20;
            this.confirmar.BorderThickness = 1;
            this.confirmar.CheckedState.Parent = this.confirmar;
            this.confirmar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.confirmar.CustomBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.confirmar.CustomImages.Parent = this.confirmar;
            this.confirmar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.confirmar.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.confirmar.ForeColor = System.Drawing.Color.White;
            this.confirmar.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.confirmar.HoverState.Parent = this.confirmar;
            this.confirmar.Location = new System.Drawing.Point(16, 114);
            this.confirmar.Name = "confirmar";
            this.confirmar.ShadowDecoration.BorderRadius = 5;
            this.confirmar.ShadowDecoration.Color = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(234)))));
            this.confirmar.ShadowDecoration.Depth = 40;
            this.confirmar.ShadowDecoration.Parent = this.confirmar;
            this.confirmar.ShadowDecoration.Shadow = new System.Windows.Forms.Padding(7, 7, 7, 12);
            this.confirmar.Size = new System.Drawing.Size(195, 42);
            this.confirmar.TabIndex = 28;
            this.confirmar.Text = "OK";
            this.confirmar.Click += new System.EventHandler(this.confirmar_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(217, 125);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(126, 20);
            this.label2.TabIndex = 29;
            this.label2.Text = "Senha incorreta!";
            this.label2.Visible = false;
            // 
            // Senha
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(489, 165);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.confirmar);
            this.Controls.Add(this.senhaTexto);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(505, 204);
            this.MinimumSize = new System.Drawing.Size(505, 204);
            this.Name = "Senha";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Senha";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private Guna.UI2.WinForms.Guna2TextBox senhaTexto;
        private Guna.UI2.WinForms.Guna2Button confirmar;
        private System.Windows.Forms.Label label2;
    }
}