using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Nottext_Data_Protector.Forms;

namespace Nottext_Data_Protector
{
    public partial class Form1 : Form
    {
        Configuracoes cf = new Configuracoes();
        AdicionarObjetos addOb = new AdicionarObjetos();
        AdicionarProcessos addPc = new AdicionarProcessos();
        Sobre sb = new Sobre();

        /// <summary>
        /// Inicia o FORM
        /// </summary>
        public Form1()
        {
            // Carregar a DLL como uma matriz de bytes
            byte[] assemblyBytes = Properties.Resources.Guna_UI2;

            // Carregar a DLL a partir da matriz de bytes
            Assembly assembly = Assembly.Load(assemblyBytes);

            InitializeComponent();
            AlterarMouse.AlterarCursor(this);

            // Configure
            addOb.TopLevel = false;
            addOb.Dock = DockStyle.Fill;

            sb.TopLevel = false;
            sb.Dock = DockStyle.Fill;

            cf.TopLevel = false;
            cf.Dock = DockStyle.Fill;

            addPc.TopLevel = false;
            addPc.Dock = DockStyle.Fill;

            // Coloque
            corpo.Controls.Add(addOb);
            corpo.Controls.Add(sb);
            corpo.Controls.Add(cf);
            corpo.Controls.Add(addPc);

            addOb.Show();
        }

        /// <summary>
        /// Botão de sobre
        /// </summary>
        /// 
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void sobre_Click(object sender, EventArgs e)
        {
            addOb.Hide();
            cf.Hide();
            addPc.Hide();
            sb.Show();
        }

        /// <summary>
        /// Botão de início
        /// </summary>
        /// 
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void inicio_Click(object sender, EventArgs e)
        {
            sb.Hide();
            cf.Hide();
            addPc.Hide();
            addOb.Show();
        }

        /// <summary>
        /// Botão de configuração
        /// </summary>
        /// 
        /// <param name="sender">sende</param>
        /// <param name="e">e</param>
        private void configuracoes_Click(object sender, EventArgs e)
        {
            sb.Hide();
            addOb.Hide();
            addPc.Hide();
            cf.Show();
        }

        /// <summary>
        /// Botão de processos
        /// </summary>
        /// 
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void processos_Click(object sender, EventArgs e)
        {
            sb.Hide();
            addOb.Hide();
            cf.Hide();
            addPc.Show();
        }

    }
}
