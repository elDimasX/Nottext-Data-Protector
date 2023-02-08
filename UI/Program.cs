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

            // Remover todos os componentes
            if (args.Contains("RemoveAllComponents"))
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

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
