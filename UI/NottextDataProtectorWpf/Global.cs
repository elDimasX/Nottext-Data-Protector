using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Nottext_Data_Protector
{
    class Global
    {
        // Locais
        //
        // Importante para AnyCPU:
        // quando um processo 32-bit roda em Windows 64-bit, "C:\Windows\System32"
        // é redirecionado automaticamente para "C:\Windows\SysWOW64".
        // As configurações reais do app/driver ficam no System32 nativo; por isso,
        // em processo 32-bit sobre OS 64-bit usamos o alias "Sysnative".
        public static readonly string diretorioSistemaNativo = GetNativeSystemDirectory();

        public static string pasta = Path.Combine(diretorioSistemaNativo, "sas-Spoiler-dism") + "\\";

        public static string arquivoSomenteLeitura = pasta + "pp.cat";
        public static string arquivoBloquear = pasta + "pp.sys";
        public static string arquivoOcultar = pasta + "pp.hnd";
        public static string arquivoNaoExecutar = pasta + "pp.enp";
        public static string arquivoProtegerProcesso = pasta + "pp.prc";
        public static string arquivoProcessos = pasta + "pp.inf";

        public static string protecaoHabilitada = pasta + "pp.end";

        public static string terminarProcessos = pasta + "pp.term";
        public static string messageBox = pasta + "pp.msg";

        public static string senhaArquivo = pasta + "pp.psh";


        public static string substituirExistenteOpcao = pasta + "pp.koo";
        public static string ultimoLocalCustomFileDialog = pasta + "pp.zpp";
        public static string ultimaOpcaoProtecao = pasta + "pp.fpp";
        public static string temaArquivo = pasta + "pp.theme";

        private static string GetNativeSystemDirectory()
        {
            string windows = Environment.GetFolderPath(Environment.SpecialFolder.Windows);

            if (Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess)
                return Path.Combine(windows, "Sysnative");

            return Path.Combine(windows, "System32");
        }


        /// <summary>
        /// Necessário para atualizar o explorer
        /// </summary>
        [DllImport("shell32.dll")]
        private static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

        /// <summary>
        /// Atualize o explorer, necessário para arquivos ocultos ou bloquear
        /// </summary>
        public static void AtualizarExplorer()
        {
            // Atualize o explorer
            //SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
        }
    }
}
