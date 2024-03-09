
namespace Nottext_Data_Protector.Forms
{
    partial class Protecao
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Protecao));
            this.localArquivo = new Guna.UI2.WinForms.Guna2TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.opcoes = new Guna.UI2.WinForms.Guna2ComboBox();
            this.confirmar = new Guna.UI2.WinForms.Guna2Button();
            this.sobreAsProtecoes = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.substituirExistente = new Guna.UI2.WinForms.Guna2ToggleSwitch();
            ((System.ComponentModel.ISupportInitialize)(this.sobreAsProtecoes)).BeginInit();
            this.SuspendLayout();
            // 
            // localArquivo
            // 
            this.localArquivo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.localArquivo.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(221)))), ((int)(((byte)(226)))));
            this.localArquivo.BorderThickness = 2;
            this.localArquivo.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.localArquivo.DefaultText = "";
            this.localArquivo.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.localArquivo.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.localArquivo.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.localArquivo.DisabledState.Parent = this.localArquivo;
            this.localArquivo.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.localArquivo.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.localArquivo.FocusedState.Parent = this.localArquivo;
            this.localArquivo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.localArquivo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.localArquivo.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.localArquivo.HoverState.Parent = this.localArquivo;
            this.localArquivo.Location = new System.Drawing.Point(87, 17);
            this.localArquivo.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.localArquivo.Name = "localArquivo";
            this.localArquivo.PasswordChar = '\0';
            this.localArquivo.PlaceholderText = "";
            this.localArquivo.SelectedText = "";
            this.localArquivo.ShadowDecoration.Parent = this.localArquivo;
            this.localArquivo.Size = new System.Drawing.Size(450, 31);
            this.localArquivo.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 20);
            this.label1.TabIndex = 1;
            this.label1.Text = "Objeto:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Status:";
            // 
            // opcoes
            // 
            this.opcoes.BackColor = System.Drawing.Color.Transparent;
            this.opcoes.BorderThickness = 2;
            this.opcoes.Cursor = System.Windows.Forms.Cursors.Hand;
            this.opcoes.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.opcoes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.opcoes.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.opcoes.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.opcoes.FocusedState.Parent = this.opcoes;
            this.opcoes.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.opcoes.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.opcoes.FormattingEnabled = true;
            this.opcoes.HoverState.Parent = this.opcoes;
            this.opcoes.IntegralHeight = false;
            this.opcoes.ItemHeight = 30;
            this.opcoes.Items.AddRange(new object[] {
            "Somente Leitura",
            "Bloquear",
            "Ocultar",
            "Não Executar",
            "Proteger Processo"});
            this.opcoes.ItemsAppearance.Parent = this.opcoes;
            this.opcoes.Location = new System.Drawing.Point(87, 69);
            this.opcoes.Name = "opcoes";
            this.opcoes.ShadowDecoration.Parent = this.opcoes;
            this.opcoes.Size = new System.Drawing.Size(234, 36);
            this.opcoes.StartIndex = 0;
            this.opcoes.TabIndex = 26;
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
            this.confirmar.Location = new System.Drawing.Point(16, 134);
            this.confirmar.Name = "confirmar";
            this.confirmar.ShadowDecoration.BorderRadius = 5;
            this.confirmar.ShadowDecoration.Color = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(234)))));
            this.confirmar.ShadowDecoration.Depth = 40;
            this.confirmar.ShadowDecoration.Parent = this.confirmar;
            this.confirmar.ShadowDecoration.Shadow = new System.Windows.Forms.Padding(7, 7, 7, 12);
            this.confirmar.Size = new System.Drawing.Size(195, 42);
            this.confirmar.TabIndex = 27;
            this.confirmar.Text = "ADICIONAR ARQUIVO";
            this.confirmar.Click += new System.EventHandler(this.confirmar_Click);
            // 
            // sobreAsProtecoes
            // 
            this.sobreAsProtecoes.Cursor = System.Windows.Forms.Cursors.Hand;
            this.sobreAsProtecoes.Image = global::Nottext_Data_Protector.Properties.Resources.question_64x64;
            this.sobreAsProtecoes.Location = new System.Drawing.Point(328, 69);
            this.sobreAsProtecoes.Name = "sobreAsProtecoes";
            this.sobreAsProtecoes.Size = new System.Drawing.Size(43, 36);
            this.sobreAsProtecoes.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.sobreAsProtecoes.TabIndex = 28;
            this.sobreAsProtecoes.TabStop = false;
            this.sobreAsProtecoes.Click += new System.EventHandler(this.sobreAsProtecoes_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(236, 143);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(148, 20);
            this.label3.TabIndex = 29;
            this.label3.Text = "Substituir existente:";
            // 
            // substituirExistente
            // 
            this.substituirExistente.Checked = true;
            this.substituirExistente.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.substituirExistente.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.substituirExistente.CheckedState.InnerBorderColor = System.Drawing.Color.White;
            this.substituirExistente.CheckedState.InnerColor = System.Drawing.Color.White;
            this.substituirExistente.CheckedState.Parent = this.substituirExistente;
            this.substituirExistente.Cursor = System.Windows.Forms.Cursors.Hand;
            this.substituirExistente.Location = new System.Drawing.Point(390, 143);
            this.substituirExistente.Name = "substituirExistente";
            this.substituirExistente.ShadowDecoration.Parent = this.substituirExistente;
            this.substituirExistente.Size = new System.Drawing.Size(50, 20);
            this.substituirExistente.TabIndex = 30;
            this.substituirExistente.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(218)))), ((int)(((byte)(223)))));
            this.substituirExistente.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(218)))), ((int)(((byte)(223)))));
            this.substituirExistente.UncheckedState.InnerBorderColor = System.Drawing.Color.White;
            this.substituirExistente.UncheckedState.InnerColor = System.Drawing.Color.White;
            this.substituirExistente.UncheckedState.Parent = this.substituirExistente;
            this.substituirExistente.Click += new System.EventHandler(this.substituirExistente_Click);
            // 
            // Protecao
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 188);
            this.Controls.Add(this.substituirExistente);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.sobreAsProtecoes);
            this.Controls.Add(this.confirmar);
            this.Controls.Add(this.opcoes);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.localArquivo);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(566, 227);
            this.MinimumSize = new System.Drawing.Size(566, 227);
            this.Name = "Protecao";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Adicione ou altere a proteção";
            ((System.ComponentModel.ISupportInitialize)(this.sobreAsProtecoes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Guna.UI2.WinForms.Guna2TextBox localArquivo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private Guna.UI2.WinForms.Guna2ComboBox opcoes;
        private Guna.UI2.WinForms.Guna2Button confirmar;
        private System.Windows.Forms.PictureBox sobreAsProtecoes;
        private System.Windows.Forms.Label label3;
        public Guna.UI2.WinForms.Guna2ToggleSwitch substituirExistente;
    }
}