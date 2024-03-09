using Nottext_Data_Protector.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nottext_Data_Protector
{
    public partial class CustomFileDialog : Form
    {
        // Local para retornar
        public string SelectedPath { get; private set; }

        NativeMethods.SHFILEINFO shfi = new NativeMethods.SHFILEINFO();
        IntPtr hSysImgList;

        /// <summary>
        /// Necessários para os icones
        /// </summary>
        internal static class NativeMethods
        {
            public const uint LVM_FIRST = 0x1000;
            public const uint LVM_GETIMAGELIST = (LVM_FIRST + 2);
            public const uint LVM_SETIMAGELIST = (LVM_FIRST + 3);

            public const uint LVSIL_NORMAL = 0;
            public const uint LVSIL_SMALL = 1;
            public const uint LVSIL_STATE = 2;
            public const uint LVSIL_GROUPHEADER = 3;

            [DllImport("user32")]
            public static extern IntPtr SendMessage(IntPtr hWnd,
                                                    uint msg,
                                                    uint wParam,
                                                    IntPtr lParam);

            [DllImport("comctl32")]
            public static extern bool ImageList_Destroy(IntPtr hImageList);

            public const uint SHGFI_DISPLAYNAME = 0x200;
            public const uint SHGFI_ICON = 0x100;
            public const uint SHGFI_LARGEICON = 0x0;
            public const uint SHGFI_SMALLICON = 0x1;
            public const uint SHGFI_SYSICONINDEX = 0x4000;

            [StructLayout(LayoutKind.Sequential)]
            public struct SHFILEINFO
            {
                public IntPtr hIcon;
                public int iIcon;
                public uint dwAttributes;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260 /* MAX_PATH */)]
                public string szDisplayName;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
                public string szTypeName;
            };

            [DllImport("shell32")]
            public static extern IntPtr SHGetFileInfo(string pszPath,
                                                      uint dwFileAttributes,
                                                      ref SHFILEINFO psfi,
                                                      uint cbSizeFileInfo,
                                                      uint uFlags);

            [DllImport("uxtheme", CharSet = CharSet.Unicode)]
            public static extern int SetWindowTheme(IntPtr hWnd,
                                                    string pszSubAppName,
                                                    string pszSubIdList);
        }

        /// <summary>
        /// Carrega as configurações para mostrar icones no listview
        /// Pegado em: https://stackoverflow.com/questions/37791149/c-sharp-show-file-and-folder-icons-in-listview
        /// </summary>
        private void CarregarConfiguracoesDeIcones()
        {
            // Obtain a handle to the system image list.
            hSysImgList = NativeMethods.SHGetFileInfo(
                "",
                0,
                ref shfi,
                (uint)Marshal.SizeOf(shfi),
                NativeMethods.SHGFI_SYSICONINDEX
                | NativeMethods.SHGFI_SMALLICON
            );

            Debug.Assert(hSysImgList != IntPtr.Zero);  // cross our fingers and hope to succeed!

            // Set the ListView control to use that image list.
            IntPtr hOldImgList = NativeMethods.SendMessage(
                listView.Handle,
                NativeMethods.LVM_SETIMAGELIST,
                NativeMethods.LVSIL_SMALL,
                hSysImgList
            );

            // If the ListView control already had an image list, delete the old one.
            if (hOldImgList != IntPtr.Zero)
            {
                NativeMethods.ImageList_Destroy(hOldImgList);
            }

            NativeMethods.SetWindowTheme(listView.Handle, "Explorer", null);
        }

        /// <summary>
        /// Quando carregar
        /// </summary>
        public CustomFileDialog()
        {
            InitializeComponent();
            CarregarConfiguracoesDeIcones();

            listView.View = View.Details;
            listView.Columns.Add("Nome", 380);
            listView.Columns.Add("Tipo", 80);

            CarregarListView();

            try
            {
                string ultimoLocal = File.ReadAllText(Global.ultimoLocalCustomFileDialog);

                // Se existir
                if (Directory.Exists(ultimoLocal))
                    CarregarPastaNoListView(ultimoLocal);
            }
            catch (Exception) { }
        }
        
        /// <summary>
        /// Carrega os drivers para a listview exibir algo quando carregar
        /// </summary>
        private void CarregarListView()
        {
            // Limpe
            localAtual.Text = "";
            listView.Items.Clear();

            // Adiciona as unidades de disco ao ListView
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                ListViewItem item = new ListViewItem(drive.Name, 0);
                item.SubItems.Add("Disco");
                item.Tag = drive.RootDirectory;

                // Carrega o icone
                IntPtr himl = NativeMethods.SHGetFileInfo(
                    drive.Name,
                    0,
                    ref shfi,
                    (uint)Marshal.SizeOf(shfi),
                    NativeMethods.SHGFI_DISPLAYNAME
                    | NativeMethods.SHGFI_SYSICONINDEX
                    | NativeMethods.SHGFI_SMALLICON
                );

                item.ImageIndex = shfi.iIcon;

                // Adicione
                listView.Items.Add(item);
            }
        }

        /// <summary>
        /// Carrega os arquivos e pastas no listview
        /// </summary>
        /// 
        /// <param name="local">Local para carregar</param>
        private void CarregarPastaNoListView(string local)
        {
            // Limpe
            listView.Items.Clear();

            // De voltar
            ListViewItem voltar = new ListViewItem("..", 0);
            voltar.SubItems.Add("Voltar");
            listView.Items.Add(voltar);

            try
            {
                // Obtenha as informações
                DirectoryInfo pasta = new DirectoryInfo(local);
                localAtual.Text = local;

                // Adiciona as pastas
                foreach (DirectoryInfo dir in pasta.GetDirectories())
                {
                    try
                    {
                        if (dir.FullName + "\\" != Global.pasta)
                        {
                            // Configura
                            ListViewItem item = new ListViewItem(dir.Name, 0);
                            item.SubItems.Add("Pasta");
                            item.Tag = dir;

                            // Carrega o icone
                            IntPtr himl = NativeMethods.SHGetFileInfo(
                                dir.FullName,
                                0,
                                ref shfi,
                                (uint)Marshal.SizeOf(shfi),
                                NativeMethods.SHGFI_DISPLAYNAME
                                | NativeMethods.SHGFI_SYSICONINDEX
                                | NativeMethods.SHGFI_SMALLICON
                            );

                            //Debug.Assert(himl == hSysImgList);

                            // Sete o icone
                            item.ImageIndex = shfi.iIcon;

                            // Adiciona
                            listView.Items.Add(item);
                        }
                    }
                    catch (Exception) { }
                }

                // Adiciona os arquivos
                foreach (FileInfo file in pasta.GetFiles())
                {
                    try
                    {
                        // Configura
                        ListViewItem item = new ListViewItem(file.Name, 0);
                        item.SubItems.Add("Arquivo");
                        item.Tag = file;

                        // Carrega o icone
                        IntPtr himl = NativeMethods.SHGetFileInfo(
                            file.FullName,
                            0,
                            ref shfi,
                            (uint)Marshal.SizeOf(shfi),
                            NativeMethods.SHGFI_DISPLAYNAME
                            | NativeMethods.SHGFI_SYSICONINDEX
                            | NativeMethods.SHGFI_SMALLICON
                        );

                        //Debug.Assert(himl == hSysImgList);

                        item.ImageIndex = shfi.iIcon;

                        // Adiciona
                        //listView.Items.Add(item, shfi.iIcon);
                        listView.Items.Add(item);
                    }
                    catch (Exception) { }
                }
            } catch (Exception) { }
        }

        /// <summary>
        /// Quando tiver um duplo click na listview
        /// </summary>
        /// 
        /// <param name="sender">s</param>
        /// <param name="e">e</param>
        private void listView_DoubleClick(object sender, EventArgs e)
        {
            // Se tiver selecionado
            if (listView.SelectedItems.Count > 0)
            {
                // Objeto selecionado
                object selecionado = listView.SelectedItems[0].Tag;

                // Se for para voltar
                if (listView.SelectedItems[0].Text == "..")
                {
                    // Estamos em C:\ ou algum driver inicial
                    if (localAtual.Text.EndsWith("\\"))
                    {
                        // Recarregue os drivers
                        CarregarListView();
                    }
                    else
                    {
                        // Obtém o diretório pai do diretório atual
                        CarregarPastaNoListView(Directory.GetParent(localAtual.Text).FullName);
                    }
                }
                else if (selecionado is DirectoryInfo)
                {
                    // Se o item selecionado for uma pasta, atualiza a exibição para mostrar o conteúdo dessa pasta
                    CarregarPastaNoListView(((DirectoryInfo)selecionado).FullName);
                    
                }
            }
        }

        /// <summary>
        /// Botão de sair
        /// </summary>
        /// 
        /// <param name="sender">s</param>
        /// <param name="e">e</param>
        private void cancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Botão de OK
        /// </summary>
        /// 
        /// <param name="sender">s</param>
        /// <param name="e">s</param>
        private void ok_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count == 0)
                return;

            string selecionado = listView.SelectedItems[0].Text;

            if (localAtual.Text != "")
            {
                SelectedPath = localAtual.Text + "\\" + selecionado;
            }
            else
            {
                SelectedPath = selecionado;
            }

            // Converta
            SelectedPath = SelectedPath.Replace("\\\\", "\\");

            try
            {
                File.WriteAllText(Global.ultimoLocalCustomFileDialog, localAtual.Text);
            } catch (Exception) { }

            DialogResult = DialogResult.OK;
            Close();
        }

    }
}
