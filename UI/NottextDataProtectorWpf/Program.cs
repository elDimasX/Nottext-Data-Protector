using System;
using System.Windows;

namespace Nottext_Data_Protector
{
    internal static class Program
    {
        /// <summary>
        /// Entrada real do aplicativo.
        ///
        /// Importante: o driver WlfS precisa receber o IRP antes do WPF criar/carregar
        /// o App, janelas, páginas ou qualquer fluxo que leia/escreva arquivos protegidos.
        /// No WinForms isso acontecia logo no Program.Main(), antes do restante do app.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            try
            {
                // Tem que ser a primeira operação relevante do processo.
                Kernel.RelerTudo();
            }
            catch
            {
                // Mantém o comportamento permissivo do WinForms: se ainda não houver driver,
                // o fluxo de inicialização vai tentar instalar/verificar os componentes depois.
            }

            var app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }
}
