
namespace Nottext_Data_Protector.Forms
{
    partial class AdicionarObjetos
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdicionarObjetos));
            this.lista = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listaIcones = new System.Windows.Forms.ImageList(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.salvar = new Guna.UI2.WinForms.Guna2Button();
            this.cancelar = new Guna.UI2.WinForms.Guna2Button();
            this.removerArquivo = new Guna.UI2.WinForms.Guna2Button();
            this.label2 = new System.Windows.Forms.Label();
            this.adicionarMais = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.adicionarMais)).BeginInit();
            this.SuspendLayout();
            // 
            // lista
            // 
            this.lista.Alignment = System.Windows.Forms.ListViewAlignment.Default;
            this.lista.AllowColumnReorder = true;
            this.lista.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lista.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lista.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lista.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.lista.FullRowSelect = true;
            this.lista.GridLines = true;
            this.lista.HideSelection = false;
            this.lista.LargeImageList = this.listaIcones;
            this.lista.Location = new System.Drawing.Point(12, 5);
            this.lista.Name = "lista";
            this.lista.Size = new System.Drawing.Size(760, 238);
            this.lista.SmallImageList = this.listaIcones;
            this.lista.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lista.TabIndex = 12;
            this.lista.UseCompatibleStateImageBehavior = false;
            this.lista.View = System.Windows.Forms.View.Details;
            this.lista.SelectedIndexChanged += new System.EventHandler(this.lista_SelectedIndexChanged);
            this.lista.DoubleClick += new System.EventHandler(this.lista_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Arquivo/Pasta";
            this.columnHeader1.Width = 578;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Status";
            this.columnHeader2.Width = 164;
            // 
            // listaIcones
            // 
            this.listaIcones.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.listaIcones.ImageSize = new System.Drawing.Size(24, 24);
            this.listaIcones.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(246, 119);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(311, 20);
            this.label1.TabIndex = 13;
            this.label1.Text = "Arraste a pasta ou arquivo para adiciona-lo";
            // 
            // salvar
            // 
            this.salvar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.salvar.BackColor = System.Drawing.Color.Transparent;
            this.salvar.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.salvar.BorderRadius = 20;
            this.salvar.BorderThickness = 1;
            this.salvar.CheckedState.Parent = this.salvar;
            this.salvar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.salvar.CustomBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.salvar.CustomImages.Parent = this.salvar;
            this.salvar.Enabled = false;
            this.salvar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.salvar.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.salvar.ForeColor = System.Drawing.Color.White;
            this.salvar.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.salvar.HoverState.Parent = this.salvar;
            this.salvar.Location = new System.Drawing.Point(12, 269);
            this.salvar.Name = "salvar";
            this.salvar.ShadowDecoration.BorderRadius = 5;
            this.salvar.ShadowDecoration.Color = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(234)))));
            this.salvar.ShadowDecoration.Depth = 40;
            this.salvar.ShadowDecoration.Parent = this.salvar;
            this.salvar.ShadowDecoration.Shadow = new System.Windows.Forms.Padding(7, 7, 7, 12);
            this.salvar.Size = new System.Drawing.Size(195, 42);
            this.salvar.TabIndex = 28;
            this.salvar.Text = "SALVAR";
            this.salvar.EnabledChanged += new System.EventHandler(this.salvar_EnabledChanged);
            this.salvar.Click += new System.EventHandler(this.salvar_Click);
            // 
            // cancelar
            // 
            this.cancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cancelar.BackColor = System.Drawing.Color.Transparent;
            this.cancelar.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.cancelar.BorderRadius = 20;
            this.cancelar.BorderThickness = 1;
            this.cancelar.CheckedState.Parent = this.cancelar;
            this.cancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cancelar.CustomBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.cancelar.CustomImages.Parent = this.cancelar;
            this.cancelar.Enabled = false;
            this.cancelar.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.cancelar.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.cancelar.ForeColor = System.Drawing.Color.White;
            this.cancelar.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.cancelar.HoverState.Parent = this.cancelar;
            this.cancelar.Location = new System.Drawing.Point(427, 269);
            this.cancelar.Name = "cancelar";
            this.cancelar.ShadowDecoration.BorderRadius = 5;
            this.cancelar.ShadowDecoration.Color = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(234)))));
            this.cancelar.ShadowDecoration.Depth = 40;
            this.cancelar.ShadowDecoration.Parent = this.cancelar;
            this.cancelar.ShadowDecoration.Shadow = new System.Windows.Forms.Padding(7, 7, 7, 12);
            this.cancelar.Size = new System.Drawing.Size(195, 42);
            this.cancelar.TabIndex = 29;
            this.cancelar.Text = "CANCELAR";
            this.cancelar.Click += new System.EventHandler(this.cancelar_Click);
            // 
            // removerArquivo
            // 
            this.removerArquivo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.removerArquivo.BackColor = System.Drawing.Color.Transparent;
            this.removerArquivo.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.removerArquivo.BorderRadius = 20;
            this.removerArquivo.BorderThickness = 1;
            this.removerArquivo.CheckedState.Parent = this.removerArquivo;
            this.removerArquivo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.removerArquivo.CustomBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.removerArquivo.CustomImages.Parent = this.removerArquivo;
            this.removerArquivo.Enabled = false;
            this.removerArquivo.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.removerArquivo.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.removerArquivo.ForeColor = System.Drawing.Color.White;
            this.removerArquivo.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(255)))));
            this.removerArquivo.HoverState.Parent = this.removerArquivo;
            this.removerArquivo.Location = new System.Drawing.Point(219, 269);
            this.removerArquivo.Name = "removerArquivo";
            this.removerArquivo.ShadowDecoration.BorderRadius = 5;
            this.removerArquivo.ShadowDecoration.Color = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(234)))));
            this.removerArquivo.ShadowDecoration.Depth = 40;
            this.removerArquivo.ShadowDecoration.Parent = this.removerArquivo;
            this.removerArquivo.ShadowDecoration.Shadow = new System.Windows.Forms.Padding(7, 7, 7, 12);
            this.removerArquivo.Size = new System.Drawing.Size(195, 42);
            this.removerArquivo.TabIndex = 30;
            this.removerArquivo.Text = "REMOVER ARQUIVO";
            this.removerArquivo.Click += new System.EventHandler(this.removerArquivo_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 246);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(495, 18);
            this.label2.TabIndex = 31;
            this.label2.Text = "Proteja algum arquivo ou pasta com o tipo de proteção que você escolher";
            // 
            // adicionarMais
            // 
            this.adicionarMais.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.adicionarMais.Cursor = System.Windows.Forms.Cursors.Hand;
            this.adicionarMais.Image = global::Nottext_Data_Protector.Properties.Resources.mais;
            this.adicionarMais.Location = new System.Drawing.Point(638, 269);
            this.adicionarMais.Name = "adicionarMais";
            this.adicionarMais.Size = new System.Drawing.Size(46, 42);
            this.adicionarMais.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.adicionarMais.TabIndex = 32;
            this.adicionarMais.TabStop = false;
            this.adicionarMais.Click += new System.EventHandler(this.adicionarMais_Click);
            // 
            // AdicionarObjetos
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 339);
            this.Controls.Add(this.adicionarMais);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.removerArquivo);
            this.Controls.Add(this.cancelar);
            this.Controls.Add(this.salvar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lista);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "AdicionarObjetos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Adicionar Objetos";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.AdicionarObjetos_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.AdicionarObjetos_DragEnter);
            ((System.ComponentModel.ISupportInitialize)(this.adicionarMais)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListView lista;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Label label1;
        private Guna.UI2.WinForms.Guna2Button salvar;
        private Guna.UI2.WinForms.Guna2Button cancelar;
        private Guna.UI2.WinForms.Guna2Button removerArquivo;
        public System.Windows.Forms.ImageList listaIcones;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox adicionarMais;
    }
}