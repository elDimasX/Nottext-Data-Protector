using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Nottext_Data_Protector.Forms
{
    public partial class Protecao : Form
    {
        // Para salvar os valores que forem passados em Protecao
        ListView lista = new ListView();
        ImageList listaIcones = new ImageList();

        // Para saber qual é a form que tá alterando o listview, e assim, mostrar os icones corretamente
        static bool objetos = true;

        /// <summary>
        /// Quando carregar o form
        /// </summary>
        /// 
        /// <param name="listaAntiga">Lista da form AdicionarObjetos</param>
        /// <param name="imagemLista">ImagemIcon do AdicionarObjetos</param>
        /// <param name="arquivo">Arquivo para adicionar/alterar</param>
        public Protecao(ListView listaAntiga, ImageList imagemLista, string arquivo, bool objetoForm)
        {
            InitializeComponent();
            AlterarMouse.AlterarCursor(this);

            // Pegue os valores
            lista = listaAntiga;
            localArquivo.Text = arquivo;
            listaIcones = imagemLista;

            objetos = objetoForm;

            try
            {
                // Carregue a opção, para não precisar ficar clicando toda vez
                substituirExistente.Checked = !File.Exists(Global.substituirExistenteOpcao);
            } catch (Exception) { }

            try
            {
                string ultimaOpcao = File.ReadAllText(Global.ultimaOpcaoProtecao);

                // Carregue a opção de proteção, para não precisar ficar trocando toda vez
                opcoes.Text = ultimaOpcao;
            }   catch (Exception) { }

            // Verifique
            if (objetoForm == false)
            {
                opcoes.Visible = false;
                label2.Visible = false;
                label3.Visible = false;
                substituirExistente.Visible = false;
                sobreAsProtecoes.Visible = false;

                Text = "Adicionar programas para a lista de execução";

                
            }
        }

        /// <summary>
        /// Funções e tudo mais para o ícones
        /// </summary>
        public class Icones
        {
            /// <summary>
            /// SHFILEINFO
            /// </summary>
            public struct SHFILEINFO
            {
                // Icone
                public IntPtr hIcon;

                // Icone
                public IntPtr iIcon;

                // Atributos
                public uint dwAttributes;

                // Nome
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
                public string szDisplayName;

                // Type
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
                public string szTypeName;
            };

            /// <summary>
            /// Win32
            /// </summary>
            public class Win32
            {
                public const uint SHGFI_ICON = 0x100;       // Icon
                public const uint SHGFI_LARGEICON = 0x0;    // Large icon
                public const uint SHGFI_SMALLICON = 0x1;    // Small icon

                /// <summary>
                /// Importação da DLL 32
                /// </summary>
                [DllImport("shell32.dll")]
                public static extern IntPtr SHGetFileInfo(
                    string pszPath, // Local
                    uint dwFileAttributes, // Atributos 
                    ref SHFILEINFO psfi, // Informações
                    uint cbSizeFileInfo, // Tamanho
                    uint uFlags // Flags
               );
            }

            // Novo SHFILEINFO
            public static SHFILEINFO iconInfo = new SHFILEINFO();

            //the handle to the system image list
            public static IntPtr hImgLarge;
       
            public static int iconesObjetos = -1, iconesProcessos = -1;
        }

        /// <summary>
        /// Adiciona um arquivo á listBox
        /// </summary>
        /// 
        /// <param name="arquivo">Arquivo para adicionar</param>
        /// <param name="opcao">Opção para adicionar ao proteger</param>
        public void AdicionarArquivo(string arquivo, string opcao)
        {
            try
            {
                // Verifique se esse já existe na lista
                try
                {
                    bool repetidoEncontrado = false;

                    // Se for para substituir
                    if (substituirExistente.Checked == true)
                    {
                        // Procure todos os items na listView
                        for (int i = 0; i < lista.Items.Count; i++)
                        {
                            try
                            {
                                // Pegue os arquivos
                                string items = lista.Items[i].SubItems[0].Text;
                                string opcaoAnterior = lista.Items[i].SubItems[1].Text;

                                // Se o arquivo já estiver na listBox
                                if (arquivo == items)
                                {
                                    // Se a opção for diferente, significa que queremos mudar
                                    if (opcaoAnterior != opcao || opcaoAnterior == opcoes.Text)
                                    {
                                        // Avise, para removermos todos os duplicados e
                                        // Colocarmos o original depois
                                        repetidoEncontrado = true;

                                        // Remova para não bugar a listview
                                        lista.Items.Remove(lista.Items[i]);
                                        //lista.Items[i].SubItems[1].Text = opcoes.Text; // Mude

                                        DialogResult = DialogResult.OK;
                                    }

                                    // Continue para o próximo
                                    continue;
                                }
                            }
                            catch (Exception) { }
                        }
                    }

                    if (repetidoEncontrado == true)
                    {
                        AdicionarArquivo(arquivo, opcao);
                        return;
                    }
                }
                catch (Exception) { }


                try
                {
                    // Obtenha o icone grande
                    Icones.hImgLarge = Icones.Win32.SHGetFileInfo(
                        arquivo, // Arquivo
                        0,
                        ref Icones.iconInfo, // iconInfo
                        (uint)Marshal.SizeOf(Icones.iconInfo), // iconInfo
                        Icones.Win32.SHGFI_ICON | Icones.Win32.SHGFI_LARGEICON // Icone
                    );

                    // O ícone é retornado no membro hIcon do shinfo
                    // estrutura
                    Icon icon = Icon.FromHandle(Icones.iconInfo.hIcon);

                    // Adiciona a lista de imagens
                    listaIcones.Images.Add(icon);

                    if (objetos == true)
                    {
                        // Aumente o valor para detectar o icone na lista
                        Icones.iconesObjetos++;
                    } else
                    {
                        Icones.iconesProcessos++;
                    }
                }
                catch (Exception) { }

                ListViewItem item = null;

                if (objetos == true)
                {
                    // Novo item, para configurar o listBox
                    item = new ListViewItem(arquivo, Icones.iconesObjetos);
                }
                else
                {
                    item = new ListViewItem(arquivo, Icones.iconesProcessos);
                }

                // Adicione
                item.SubItems.Add(opcao);

                // Adicione a listBox
                lista.Items.Add(item);
                DialogResult = DialogResult.OK;
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Botão de confirmar
        /// </summary>
        /// 
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void confirmar_Click(object sender, EventArgs e)
        {
            try
            {
                // Escreva para carregar depois
                File.WriteAllText(Global.ultimaOpcaoProtecao, opcoes.Text);
            } catch (Exception) { }

            AdicionarArquivo(localArquivo.Text, opcoes.Text);
            Close();
        }

        private void sobreAsProtecoes_Click(object sender, EventArgs e)
        {
            SobreProtecoes sobre = new SobreProtecoes();
            sobre.ShowDialog();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void substituirExistente_Click(object sender, EventArgs e)
        {
            try
            {
                if (substituirExistente.Checked == true)
                {
                    File.Delete(Global.substituirExistenteOpcao);
                }
                else
                {
                    File.Create(Global.substituirExistenteOpcao).Close();
                }
            } catch (Exception) { }
        }
    }
}
