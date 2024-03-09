
namespace Nottext_Data_Protector
{
    partial class Form1
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.corpo = new System.Windows.Forms.Panel();
            this.cabecalhoLinha = new System.Windows.Forms.Panel();
            this.inicio = new Guna.UI2.WinForms.Guna2Button();
            this.configuracoes = new Guna.UI2.WinForms.Guna2Button();
            this.sobre = new Guna.UI2.WinForms.Guna2Button();
            this.label1 = new System.Windows.Forms.Label();
            this.processos = new Guna.UI2.WinForms.Guna2Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.corpo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // corpo
            // 
            this.corpo.AllowDrop = true;
            this.corpo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.corpo.AutoScroll = true;
            this.corpo.AutoSize = true;
            this.corpo.BackColor = System.Drawing.SystemColors.Control;
            this.corpo.Controls.Add(this.cabecalhoLinha);
            this.corpo.Location = new System.Drawing.Point(0, 87);
            this.corpo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.corpo.Name = "corpo";
            this.corpo.Size = new System.Drawing.Size(781, 338);
            this.corpo.TabIndex = 0;
            // 
            // cabecalhoLinha
            // 
            this.cabecalhoLinha.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cabecalhoLinha.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(223)))), ((int)(((byte)(223)))));
            this.cabecalhoLinha.Location = new System.Drawing.Point(1, 0);
            this.cabecalhoLinha.Name = "cabecalhoLinha";
            this.cabecalhoLinha.Size = new System.Drawing.Size(781, 2);
            this.cabecalhoLinha.TabIndex = 34;
            // 
            // inicio
            // 
            this.inicio.BackColor = System.Drawing.Color.Transparent;
            this.inicio.BorderColor = System.Drawing.Color.Transparent;
            this.inicio.BorderThickness = 1;
            this.inicio.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.inicio.Checked = true;
            this.inicio.CheckedState.BorderColor = System.Drawing.Color.Transparent;
            this.inicio.CheckedState.CustomBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.inicio.CheckedState.FillColor = System.Drawing.Color.Transparent;
            this.inicio.CheckedState.Parent = this.inicio;
            this.inicio.Cursor = System.Windows.Forms.Cursors.Hand;
            this.inicio.CustomBorderThickness = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.inicio.CustomImages.Parent = this.inicio;
            this.inicio.FillColor = System.Drawing.Color.Transparent;
            this.inicio.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.inicio.ForeColor = System.Drawing.Color.Black;
            this.inicio.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.inicio.HoverState.Parent = this.inicio;
            this.inicio.Location = new System.Drawing.Point(0, 45);
            this.inicio.Name = "inicio";
            this.inicio.ShadowDecoration.BorderRadius = 5;
            this.inicio.ShadowDecoration.Color = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(234)))));
            this.inicio.ShadowDecoration.Depth = 40;
            this.inicio.ShadowDecoration.Parent = this.inicio;
            this.inicio.ShadowDecoration.Shadow = new System.Windows.Forms.Padding(7, 7, 7, 12);
            this.inicio.Size = new System.Drawing.Size(195, 42);
            this.inicio.TabIndex = 29;
            this.inicio.Text = "ARQUIVOS / PASTAS";
            this.inicio.Click += new System.EventHandler(this.inicio_Click);
            // 
            // configuracoes
            // 
            this.configuracoes.BackColor = System.Drawing.Color.Transparent;
            this.configuracoes.BorderColor = System.Drawing.Color.Transparent;
            this.configuracoes.BorderThickness = 1;
            this.configuracoes.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.configuracoes.CheckedState.BorderColor = System.Drawing.Color.Transparent;
            this.configuracoes.CheckedState.CustomBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.configuracoes.CheckedState.FillColor = System.Drawing.Color.Transparent;
            this.configuracoes.CheckedState.Parent = this.configuracoes;
            this.configuracoes.Cursor = System.Windows.Forms.Cursors.Hand;
            this.configuracoes.CustomBorderThickness = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.configuracoes.CustomImages.Parent = this.configuracoes;
            this.configuracoes.FillColor = System.Drawing.Color.Transparent;
            this.configuracoes.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.configuracoes.ForeColor = System.Drawing.Color.Black;
            this.configuracoes.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.configuracoes.HoverState.Parent = this.configuracoes;
            this.configuracoes.Location = new System.Drawing.Point(390, 45);
            this.configuracoes.Name = "configuracoes";
            this.configuracoes.ShadowDecoration.BorderRadius = 5;
            this.configuracoes.ShadowDecoration.Color = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(234)))));
            this.configuracoes.ShadowDecoration.Depth = 40;
            this.configuracoes.ShadowDecoration.Parent = this.configuracoes;
            this.configuracoes.ShadowDecoration.Shadow = new System.Windows.Forms.Padding(7, 7, 7, 12);
            this.configuracoes.Size = new System.Drawing.Size(195, 42);
            this.configuracoes.TabIndex = 30;
            this.configuracoes.Text = "CONFIGURAÇÕES";
            this.configuracoes.Click += new System.EventHandler(this.configuracoes_Click);
            // 
            // sobre
            // 
            this.sobre.BackColor = System.Drawing.Color.Transparent;
            this.sobre.BorderColor = System.Drawing.Color.Transparent;
            this.sobre.BorderThickness = 1;
            this.sobre.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.sobre.CheckedState.BorderColor = System.Drawing.Color.Transparent;
            this.sobre.CheckedState.CustomBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.sobre.CheckedState.FillColor = System.Drawing.Color.Transparent;
            this.sobre.CheckedState.Parent = this.sobre;
            this.sobre.Cursor = System.Windows.Forms.Cursors.Hand;
            this.sobre.CustomBorderThickness = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.sobre.CustomImages.Parent = this.sobre;
            this.sobre.FillColor = System.Drawing.Color.Transparent;
            this.sobre.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.sobre.ForeColor = System.Drawing.Color.Black;
            this.sobre.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.sobre.HoverState.Parent = this.sobre;
            this.sobre.Location = new System.Drawing.Point(585, 45);
            this.sobre.Name = "sobre";
            this.sobre.ShadowDecoration.BorderRadius = 5;
            this.sobre.ShadowDecoration.Color = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(234)))));
            this.sobre.ShadowDecoration.Depth = 40;
            this.sobre.ShadowDecoration.Parent = this.sobre;
            this.sobre.ShadowDecoration.Shadow = new System.Windows.Forms.Padding(7, 7, 7, 12);
            this.sobre.Size = new System.Drawing.Size(195, 42);
            this.sobre.TabIndex = 31;
            this.sobre.Text = "SOBRE";
            this.sobre.Click += new System.EventHandler(this.sobre_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(68, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(177, 20);
            this.label1.TabIndex = 32;
            this.label1.Text = "Nottext Data Protector";
            // 
            // processos
            // 
            this.processos.BackColor = System.Drawing.Color.Transparent;
            this.processos.BorderColor = System.Drawing.Color.Transparent;
            this.processos.BorderThickness = 1;
            this.processos.ButtonMode = Guna.UI2.WinForms.Enums.ButtonMode.RadioButton;
            this.processos.CheckedState.BorderColor = System.Drawing.Color.Transparent;
            this.processos.CheckedState.CustomBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.processos.CheckedState.FillColor = System.Drawing.Color.Transparent;
            this.processos.CheckedState.Parent = this.processos;
            this.processos.Cursor = System.Windows.Forms.Cursors.Hand;
            this.processos.CustomBorderThickness = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.processos.CustomImages.Parent = this.processos;
            this.processos.FillColor = System.Drawing.Color.Transparent;
            this.processos.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.processos.ForeColor = System.Drawing.Color.Black;
            this.processos.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.processos.HoverState.Parent = this.processos;
            this.processos.Location = new System.Drawing.Point(195, 45);
            this.processos.Name = "processos";
            this.processos.ShadowDecoration.BorderRadius = 5;
            this.processos.ShadowDecoration.Color = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(234)))));
            this.processos.ShadowDecoration.Depth = 40;
            this.processos.ShadowDecoration.Parent = this.processos;
            this.processos.ShadowDecoration.Shadow = new System.Windows.Forms.Padding(7, 7, 7, 12);
            this.processos.Size = new System.Drawing.Size(195, 42);
            this.processos.TabIndex = 34;
            this.processos.Text = "PROCESSOS";
            this.processos.Click += new System.EventHandler(this.processos_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Nottext_Data_Protector.Properties.Resources.Nottext_Data_Protector;
            this.pictureBox1.Location = new System.Drawing.Point(12, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(50, 39);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 33;
            this.pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(780, 425);
            this.Controls.Add(this.processos);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.sobre);
            this.Controls.Add(this.configuracoes);
            this.Controls.Add(this.inicio);
            this.Controls.Add(this.corpo);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MinimumSize = new System.Drawing.Size(796, 464);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Nottext - Data Protector";
            this.corpo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel corpo;
        private Guna.UI2.WinForms.Guna2Button inicio;
        private Guna.UI2.WinForms.Guna2Button configuracoes;
        private Guna.UI2.WinForms.Guna2Button sobre;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel cabecalhoLinha;
        private Guna.UI2.WinForms.Guna2Button processos;
    }
}

