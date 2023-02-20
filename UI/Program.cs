using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nottext_Data_Protector
{
    static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Obtena os argumentos
            string[] args = Environment.GetCommandLineArgs();

            // Instalar todos os componenetes
            if (args.Contains("InstallAllComponents"))
            {
                // Instale tudo
                SetAcess.InstalarTudo();
            }
            // Remover todos os componentes
            else if (args.Contains("RemoveAllComponents"))
            {
                // Tire tudo
                File.Delete("C:\\Windows\\System32\\Drivers\\WlfS.sys");
                Directory.Delete(Global.pasta, true);

                Environment.Exit(0);
            }

            try
            {
                // Descompacte o guna
                File.WriteAllBytes(
                    Application.StartupPath + "\\Guna.UI2.dll",
                    Properties.Resources.Guna_UI2
                );
            }
            catch (Exception) { }

            // Envie o IRP para o kernel para ele fazer o backup do processo
            Kernel.RelerTudo();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
