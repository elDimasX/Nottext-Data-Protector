using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Nottext_Data_Protector.Forms
{
    public partial class Configuracoes : Form
    {
        static string pasta = "C:\\Windows\\System32\\DriverStore\\FileRepository\\dcfi.inf_amd64_d0xi01qksz0";

        /// <summary>
        /// Quando carregar
        /// </summary>
        public Configuracoes()
        {
            InitializeComponent();

            AlterarMouse.AlterarCursor(this);

            // Verifique
            if (File.Exists(Global.protecaoHabilitada))
            {
                protecaoGlobal.Checked = false;
            }
        }

        /// <summary>
        /// Quando a proteção for alterada
        /// </summary>
        /// 
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void protecaoGlobal_Click(object sender, EventArgs e)
        {
            try
            {
                if (protecaoGlobal.Checked == false)
                {
                    File.WriteAllText(Global.protecaoHabilitada, "");
                }
                else
                {
                    File.Delete(Global.protecaoHabilitada);
                }

                // Releia tudo
                Kernel.RelerTudo();
            } catch (Exception)
            {
                protecaoGlobal.Checked = !protecaoGlobal.Checked;
                MessageBox.Show("Ocorreu um erro ao verificar os dados do usuário", "error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Botão de remover tudo
        /// </summary>
        /// 
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void removerTudo_Click(object sender, EventArgs e)
        {
            // Se cancelar
            if (MessageBox.Show("Tem certeza? toda a proteção será desabilitada", "info!", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                return; // Pare

            try
            {
                removerTudo.Enabled = false;

                // PP
                ProcessStartInfo info = default(ProcessStartInfo);

                info = new ProcessStartInfo(Application.ExecutablePath)
                {
                    UseShellExecute = true,
                    Verb = "runas",
                    WindowStyle = ProcessWindowStyle.Normal,
                    FileName = Application.ExecutablePath,
                    Arguments = "RemoveAllComponents",
                    CreateNoWindow = false
                };

                // Inicie e espere
                Process.Start(info);

                MessageBox.Show("Sucesso! Reinicie o computador para completar a remoção", "info!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Environment.Exit(0);
            } catch (Exception ex) {

                MessageBox.Show(ex.Message, "error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            removerTudo.Enabled = true;

        }
    }
}
