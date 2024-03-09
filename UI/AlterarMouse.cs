using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Nottext_Data_Protector
{
    class AlterarMouse
    {

        /// <summary>
        /// Importação da DLL para alterar o cursor
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

        // Novo cursor
        private static readonly Cursor CursorMao = new Cursor(LoadCursor(IntPtr.Zero, 32649));

        /// <summary>
        /// Configurar o cursor
        /// </summary>
        public static void AlterarCursor(Control controle)
        {
            try
            {
                // Procure todos os controles na FORM
                foreach (Control controleC in controle.Controls)
                {
                    try
                    {
                        // Int
                        int i;

                        // Se for um 
                        if (controleC.Cursor == Cursors.Hand)
                        {
                            // Altere o cursor
                            controleC.Cursor = CursorMao;
                        }

                        // Procure outros paineis na FORM
                        for (i = 0; i < 2; i++)
                        {
                            // Altere de novo
                            AlterarCursor(controleC);
                        }
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception) { }
        }


    }
}
