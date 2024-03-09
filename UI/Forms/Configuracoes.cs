using Microsoft.Win32;
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
            protecaoGlobal.Checked = !File.Exists(Global.protecaoHabilitada);

            terminarProcessos.Checked = File.Exists(Global.terminarProcessos);
            messageBox.Checked = File.Exists(Global.messageBox);

            senha.Checked = File.Exists(Global.senhaArquivo);

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

                // Atualize
                Global.AtualizarExplorer();
            } catch (Exception)
            {
                protecaoGlobal.Checked = !protecaoGlobal.Checked;
                MessageBox.Show("Ocorreu um erro ao verificar os dados do usuário", "error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Executa o proprio Nottext com argumentos para realizar operações
        /// Que exigem acesso de administrador
        /// </summary>
        /// 
        /// <param name="argumentos">Argumentos</param>
        private void ExecutarArgumentosComoAdmin(string argumentos)
        {

            // PP
            ProcessStartInfo info = new ProcessStartInfo(Application.ExecutablePath)
            {
                UseShellExecute = true,
                Verb = "runas",
                WindowStyle = ProcessWindowStyle.Normal,
                FileName = Application.ExecutablePath,
                Arguments = argumentos,
                CreateNoWindow = false
            };

            // Inicie e espere
            Process.Start(info).WaitForExit();
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

            removerTudo.Enabled = false;

            try
            {

                // Execute
                ExecutarArgumentosComoAdmin("RemoveAllComponents");

                // Obtenha o acesso novamente do kernel
                Kernel.RelerTudo();

                // Abra
                RegistryKey key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\WlfS", false);

                // Se não tiver apagado
                if (key != null)
                {
                    // Mensagem
                    MessageBox.Show("Não foi possível remover os arquivos do Nottext Data Protector", "error!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    removerTudo.Enabled = true;
                    return;
                }

                // Mensagem
                MessageBox.Show("Sucesso! Reinicie o computador para completar a remoção", "info!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Saia
                Environment.Exit(0);
            } catch (Exception ex) {

                MessageBox.Show(ex.Message, "error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            removerTudo.Enabled = true;

        }

        /// <summary>
        /// Checkbox de terminar processo
        /// </summary>
        /// 
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void terminarProcessos_Click(object sender, EventArgs e)
        {
            try
            {
                if (terminarProcessos.Checked == true)
                {
                    File.WriteAllText(Global.terminarProcessos, "");
                }
                else
                {
                    File.Delete(Global.terminarProcessos);
                }

                // Releia tudo
                Kernel.RelerTudo();
            }
            catch (Exception)
            {
                terminarProcessos.Checked = !terminarProcessos.Checked;
                MessageBox.Show("Ocorreu um erro ao verificar os dados do usuário", "error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Checkbox de messageBox
        /// </summary>
        /// 
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void messageBox_Click(object sender, EventArgs e)
        {
            try
            {
                if (messageBox.Checked == true)
                {
                    File.WriteAllText(Global.messageBox, "");
                }
                else
                {
                    File.Delete(Global.messageBox);
                }

                // Releia tudo
                Kernel.RelerTudo();
            }
            catch (Exception)
            {
                messageBox.Checked = !messageBox.Checked;
                MessageBox.Show("Ocorreu um erro ao verificar os dados do usuário", "error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Senha
        /// </summary>
        /// 
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void senha_Click(object sender, EventArgs e)
        {
            // Se estiver ativando...
            if (senha.Checked == true)
            {
                // Se criar a senha
                Senha senhaFrm = new Senha();
                if (senhaFrm.ShowDialog() == DialogResult.OK)
                {
                    senha.Checked = true;
                } else
                {
                    senha.Checked = false;
                }
            } else
            {
                try
                {
                    // Delete a senha
                    File.Delete(Global.senhaArquivo);
                    senha.Checked = false;
                } catch (Exception)
                {
                    MessageBox.Show("Error, não foi possível desativar a senha por um erro inesperado.", "error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Coloque em foco, se não, o scroll terá que ser clicado manualmente
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">e</param>
        private void Configuracoes_MouseEnter(object sender, EventArgs e)
        {
            Focus();
        }

        /// <summary>
        /// Tire o foco, para não detectar o scroll sem estar em foco
        /// </summary>
        /// 
        /// <param name="sender">s</param>
        /// <param name="e">e</param>
        private void Configuracoes_MouseLeave(object sender, EventArgs e)
        {
            // Tire o foco
            this.ActiveControl = null;
        }
    }
}
