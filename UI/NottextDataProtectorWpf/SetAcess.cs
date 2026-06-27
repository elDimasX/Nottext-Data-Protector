using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Nottext_Data_Protector
{
    class SetAcess
    {
        /// <summary>
        /// Inicia um processo
        /// </summary>
        /// 
        /// <param name="nome">Nome do processo</param>
        /// <param name="argumento">Argumento do processo</param>
        public static void IniciarProcesso(string nome, string argumento)
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
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                // Argumentos
                process.StartInfo.Arguments = argumento;

                // Inicie e espere
                process.Start();
                process.WaitForExit();
                process.Dispose();
            }
            catch (Exception) { }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct RTL_OSVERSIONINFOEX
        {
            public uint dwOSVersionInfoSize;
            public uint dwMajorVersion;
            public uint dwMinorVersion;
            public uint dwBuildNumber;
            public uint dwPlatformId;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szCSDVersion;

            public ushort wServicePackMajor;
            public ushort wServicePackMinor;
            public ushort wSuiteMask;
            public byte wProductType;
            public byte wReserved;
        }

        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int RtlGetVersion(ref RTL_OSVERSIONINFOEX versionInfo);

        private static bool IsWindows10OrGreater()
        {
            try
            {
                var os = new RTL_OSVERSIONINFOEX();
                os.dwOSVersionInfoSize = (uint)Marshal.SizeOf(typeof(RTL_OSVERSIONINFOEX));

                if (RtlGetVersion(ref os) != 0)
                    return Environment.OSVersion.Version.Major >= 10;

                return os.dwMajorVersion >= 10;
            }
            catch
            {
                return Environment.OSVersion.Version.Major >= 10;
            }
        }

        /// <summary>
        /// Instala os drivers
        /// </summary>
        private static void InstalarDriver()
        {
            string arquivoInf = "C:\\Windows\\Temp\\psd\\WlfS.inf";

            // Windows 10/11: pnputil evita a caixa padrão feia do InfDefaultInstall.
            // Windows antigo: mantém o fluxo original via InfDefaultInstall.
            if (IsWindows10OrGreater())
                IniciarProcesso(@"C:\Windows\System32\pnputil.exe", "/add-driver \"" + arquivoInf + "\" /install");
            else
                IniciarProcesso("cmd.exe", "/c C:\\Windows\\System32\\InfDefaultInstall.exe " + '"' + arquivoInf + '"');
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
            catch (Exception ex) { Nottext_Data_Protector.AppMessageBox.Show(ex.Message); }
        }

        /// <summary>
        /// Altera o registro para mudar o local e como iniciar (boot, system, auto...)
        /// </summary>
        private static void AlterarRegistro()
        {
            // Pegue permissão
            Kernel.RelerTudo();

            try
            {


                // Precisamos sabe se vamos abrir o registro em 32 ou 64 bits, porque se não
                // Não vamos encontrar o valor corretamente
                RegistryView abrirModo = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;

                // Chave
                RegistryKey elementos = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, abrirModo);

                // Abra
                RegistryKey WlfSSvc = elementos.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\WlfS", true);

                // Altrere
                WlfSSvc.SetValue("ImagePath", "system32\\Drivers\\WlfS.sys", RegistryValueKind.ExpandString);

                // Coloque pra iniciar no boot
                WlfSSvc.SetValue("Start", "0", RegistryValueKind.DWord);
            }
            catch (Exception)
            {
            }
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
                Directory.CreateDirectory(Global.pasta);

                // Altera a permissão da pasta
                AlterarPermissao(Global.pasta, novo);

                // Oculte a pasta
                DirectoryInfo inf = new DirectoryInfo(Global.pasta);
                inf.Attributes |= FileAttributes.Hidden | FileAttributes.System;

                // Instale os drivers
                InstalarDriver();

                try
                {
                    // Inicie o driver
                    ServiceController sv = new ServiceController("WlfS");
                    sv.Start();

                    // Altere o local para o arquivo indeletável
                    AlterarRegistro();

                    try
                    {
                        //File.Delete("C:\\Windows\\System32\\Drivers\\WlfS.sys");
                    } catch (Exception) { }
                }
                catch (Exception) { }

            }
            catch (Exception) { }
            Environment.Exit(0);
        }



    }
}
