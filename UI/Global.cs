using System;
using System.Runtime.InteropServices;

namespace Nottext_Data_Protector
{
    class Global
    {
        // Locais

        public static string pasta = "C:\\Windows\\System32\\sas-Spoiler-dism\\";

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
