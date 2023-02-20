using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
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
        /// Ativa o modo de teste do Windows
        /// </summary>
        private void AtivarModoTeste()
        {
            // Se for OK
            if (MessageBox.Show("O modo de teste não foi ativado\r\nPara continuar, clique em OK e ativaremos o modo teste", "info!", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                try
                {
                    // Processo de bcdedit
                    Process pp = new Process();
                    pp.StartInfo.FileName = "C:\\Windows\\System32\\bcdedit.exe";
                    pp.StartInfo.Arguments = "-set testsigning on";
                    pp.StartInfo.UseShellExecute = false;
                    pp.StartInfo.Verb = "runas";
                    pp.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    pp.StartInfo.CreateNoWindow = true;
                    pp.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "erro!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }

                MessageBox.Show("Sucesso! Reinicie o computador para executar o software", "info!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Environment.Exit(0);
                return;
            }
            else
            {


                MessageBox.Show("Não é possível continuar o programa sem o driver", "error!", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
               
                try
                {
                    File.Delete("C:\\Windows\\System32\\Drivers\\WlfS.sys");
                } catch (Exception) { }
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Inicia o FORM
        /// </summary>
        public Form1()
        {
            
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

            VerificarTodosOsComponentes();
        }

        /// <summary>
        /// Corrige todos os erros
        /// </summary>
        private void InstalarTodosOsComponentes()
        {
            string pasta = "C:\\Windows\\Temp\\psd\\";

            try
            {
                Directory.CreateDirectory(pasta);

                // 32 bits
                byte[] driver = null;

                // 64 bits
                if (Environment.Is64BitProcess)
                    driver = Properties.Resources.WlfS_x64;
                else
                    driver = Properties.Resources.WlfS2;

                // Cat
                File.WriteAllBytes(pasta + "WlfS.cat", Properties.Resources.wlfs);

                // Sys
                File.WriteAllBytes(pasta + "WlfS.sys", driver);

                // Inf
                File.WriteAllText(pasta + "WlfS.inf", Properties.Resources.WlfS1);

                // Inicie
                Process processo = new Process();
                processo.StartInfo.FileName = Application.ExecutablePath;
                processo.StartInfo.Arguments = "InstallAllComponents";
                processo.StartInfo.Verb = "runas";

                // Espere
                processo.Start();
                processo.WaitForExit();

                // Envie o IRP para o kernel para ele fazer o backup do processo
                Kernel.RelerTudo();

                try
                {
                    // Verifique se está rodando
                    ServiceController driverSv = new ServiceController("WlfS");

                    // Se não estiver rodando
                    if (driverSv.Status != ServiceControllerStatus.Running)
                    {
                        AtivarModoTeste();
                    }

                    Kernel.RelerTudo();

                } catch (Exception) {

                    MessageBox.Show("Não foi possível instalar os componentes necessários", "error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    Directory.Delete(pasta, true);
                } catch (Exception) { }

                MessageBox.Show(ex.Message, "error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }

            Directory.Delete(pasta, true);
        }

        /// <summary>
        /// Verifica se tudo está ok
        /// </summary>
        private void VerificarTodosOsComponentes()
        {
            // Serviço
            try
            {
                // Serviço
                ServiceController sv = new ServiceController("WlfS");

                // Se o serviço 
                if (sv.Status == ServiceControllerStatus.Stopped)
                    sv.Start();

                // Envie o IRP para o kernel para ele fazer o backup do processo
                Kernel.RelerTudo();

            } catch (Exception) {

                // Resolva os problemas
                InstalarTodosOsComponentes();
                return;
            }

            // Pasta
            try
            {
                // Tente obter as informações da pasta
                DirectoryInfo df = new DirectoryInfo(Global.pasta);
            }
            catch (Exception)
            {
                // Resolva os problemas
                InstalarTodosOsComponentes();
                return;
            }
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
