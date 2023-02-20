using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nottext_Data_Protector
{
    class SetAcess
    {
        // Pasta
        static string pasta = "C:\\SystemGuards";

        /// <summary>
        /// Inicia um processo
        /// </summary>
        /// 
        /// <param name="nome">Nome do processo</param>
        /// <param name="argumento">Argumento do processo</param>
        private static void IniciarProcesso(string nome, string argumento)
        {
            try
            {
                // Processo
                var process = new Process();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.FileName = nome;

                // Argumentos
                process.StartInfo.Arguments = argumento;

                // Inicie e espere
                process.Start();
                process.WaitForExit();
                process.Dispose();
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Instala os drivers
        /// </summary>
        private static void InstalarDriver()
        {
            IniciarProcesso("cmd.exe", "/c C:\\Windows\\System32\\InfDefaultInstall.exe " + '"' + "C:\\Windows\\Temp\\psd\\WlfS.inf" + '"');
        }

        /// <summary>
        /// Altera a permissão de uma pasta
        /// </summary>
        /// 
        /// <param name="pasta">Pasta para alterar</param>
        /// <param name="acesso">Acesso para adicionar ou remover</param>
        /// <param name="remover">Se é pra adicionar ou remover</param>
        private static void AlterarPermissao(string pasta, FileSystemAccessRule acesso)
        {
            try
            {
                // Acesso
                DirectorySecurity sec = Directory.GetAccessControl(pasta);

                sec.AddAccessRule(acesso);

                // Sete
                Directory.SetAccessControl(pasta, sec);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public static void InstalarTudo()
        {
            try
            {

                // Grupo "Todos"
                SecurityIdentifier everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

                // ACL, permissão completa para Todos
                FileSystemAccessRule novo = new FileSystemAccessRule(
                    everyone,
                    FileSystemRights.FullControl | FileSystemRights.Synchronize,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                    PropagationFlags.None,
                    AccessControlType.Allow
                );

                // Crie a pasta
                Directory.CreateDirectory(pasta);

                // Altera a permissão da pasta
                AlterarPermissao(pasta, novo);

                // Oculte a pasta
                DirectoryInfo inf = new DirectoryInfo(pasta);
                inf.Attributes |= FileAttributes.Hidden | FileAttributes.System;

                // Instale os drivers
                InstalarDriver();

                try
                {
                    // Inicie o driver
                    ServiceController sv = new ServiceController("WlfS");
                    sv.Start();
                }
                catch (Exception) { }

            }
            catch (Exception) { }
            Environment.Exit(0);
        }



    }
}
