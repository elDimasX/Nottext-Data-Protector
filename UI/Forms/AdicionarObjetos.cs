using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Nottext_Data_Protector.Forms
{
    public partial class AdicionarObjetos : Form
    {

        // Faça um backup, para quando querer restaurar
        ImageList backup = new ImageList();

        // Tipo de enconder, necessário para ler catacterias especiais
        Encoding encode = Encoding.GetEncoding("iso-8859-1");

        /// <summary>
        /// Carrega todos os arquivos salvos
        /// </summary>
        private void CarregarArquivosSalvos()
        {
            // Limpe os itens
            lista.Items.Clear();
            label1.Visible = true;

            // Restaure o backup
            listaIcones = backup;

            // Desabilite os botões
            cancelar.Enabled = false;
            salvar.Enabled = false;
            removerArquivo.Enabled = false;

            // Instância
            Protecao pp = new Protecao(lista, listaIcones, "", true);

            // Backup
            bool backupCheck = pp.substituirExistente.Checked;
            pp.substituirExistente.Checked = false;

            try
            {
                // Procure e adiciona o somente leitura
                foreach (string arquivo in File.ReadAllLines(Global.arquivoSomenteLeitura, encode))
                {
                    if (arquivo != "")
                    {
                        pp.AdicionarArquivo(arquivo, "Somente Leitura");
                        label1.Visible = false;
                    }
                }

                // Procure e adiciona o somente leitura
                foreach (string arquivo in File.ReadAllLines(Global.arquivoBloquear, encode))
                {
                    if (arquivo != "")
                    {
                        pp.AdicionarArquivo(arquivo, "Bloquear");
                        label1.Visible = false;
                    }
                }

                // Procure e adiciona o somente leitura
                foreach (string arquivo in File.ReadAllLines(Global.arquivoOcultar, encode))
                {
                    if (arquivo != "")
                    {
                        pp.AdicionarArquivo(arquivo, "Ocultar");
                        label1.Visible = false;
                    }
                }

                // Procure e adiciona o somente leitura
                foreach (string arquivo in File.ReadAllLines(Global.arquivoNaoExecutar, encode))
                {
                    if (arquivo != "")
                    {
                        pp.AdicionarArquivo(arquivo, "Não Executar");
                        label1.Visible = false;
                    }
                }

                // Procure e adiciona o somente leitura
                foreach (string arquivo in File.ReadAllLines(Global.arquivoProtegerProcesso, encode))
                {
                    if (arquivo != "")
                    {
                        pp.AdicionarArquivo(arquivo, "Proteger Processo");
                        label1.Visible = false;
                    }
                }

            }
            catch (Exception) { }

            // Volte
            pp.substituirExistente.Checked = backupCheck;
        }

        /// <summary>
        /// Quando o FORM carregar
        /// </summary>
        public AdicionarObjetos()
        {
            InitializeComponent();

            backup = listaIcones;
            AlterarMouse.AlterarCursor(this);

            CarregarArquivosSalvos();
        }

        /// <summary>
        /// Adiciona um arquivo ou edita um
        /// </summary>
        /// 
        /// <param name="arquivo">Arquivo para adicionar ou alterar</param>
        private void AdicionarOuEditArquivo(string arquivo)
        {
            // Form
            Protecao pp = new Protecao(lista, listaIcones, arquivo, true);

            // Verifique se houve modificação em algum componente
            if (pp.ShowDialog() == DialogResult.OK)
                salvar.Enabled = true;

            // Se adicionou algo
            if (lista.Items.Count > 0)
                label1.Visible = false;
            
        }

        /// <summary>
        /// Quando é arrastado para dentro
        /// </summary>
        /// 
        /// <param name="sender">Sender</param>
        /// <param name="e">Args</param>
        private void AdicionarObjetos_DragDrop(object sender, DragEventArgs e)
        {
            // Pega os arquivos arrastados
            string[] arquivos = (string[])e.Data.GetData(DataFormats.FileDrop);

            // Procura todos os arquivos
            foreach (string arquivo in arquivos)
            {
                // Adicione um arquivo
                AdicionarOuEditArquivo(arquivo);
            }
        }

        /// <summary>
        /// Quando um item estiver entrando na tela do form
        /// </summary>
        /// 
        /// <param name="sender">Sender</param>
        /// <param name="e">Args</param>
        private void AdicionarObjetos_DragEnter(object sender, DragEventArgs e)
        {
            // Quando um arquivo está sendo arrastado
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Mostre o efeito de sucesso
                e.Effect = DragDropEffects.Copy;
            }
        }
        
        /// <summary>
        /// Se clicar 2 vezes em um item
        /// </summary>
        /// 
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void lista_DoubleClick(object sender, EventArgs e)
        {
            // Procure todos os itens selecionados
            foreach (ListViewItem i in lista.SelectedItems)
            {
                // Abra a opção de editar
                AdicionarOuEditArquivo(i.Text);
            }
        }

        /// <summary>
        /// Quando algum item é selecionado ou não
        /// </summary>
        /// 
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void lista_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lista.SelectedItems.Count == 0)
                removerArquivo.Enabled = false;
            else
                removerArquivo.Enabled = true;
        }

        /// <summary>
        /// Botão de salvar
        /// </summary>
        /// 
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void salvar_Click(object sender, EventArgs e)
        {
            try
            {
                File.WriteAllText(Global.arquivoSomenteLeitura, "");
                File.WriteAllText(Global.arquivoBloquear, "");
                File.WriteAllText(Global.arquivoOcultar, "");
                File.WriteAllText(Global.arquivoNaoExecutar, "");
                File.WriteAllText(Global.arquivoProtegerProcesso, "");

                // Procure os itens
                foreach (ListViewItem item in lista.Items)
                {
                    // Arquivo e opção de bloqueio
                    string arquivo = item.SubItems[0].Text;
                    string opcao = item.SubItems[1].Text;

                    // Se for somente leitura
                    if (opcao == "Somente Leitura")
                    {
                        File.AppendAllText(Global.arquivoSomenteLeitura, arquivo + "\r\n", encode);
                    }

                    // Se for bloquear
                    if (opcao == "Bloquear")
                    {
                        File.AppendAllText(Global.arquivoBloquear, arquivo + "\r\n", encode);
                    }

                    // Se for ocultar
                    if (opcao == "Ocultar")
                    {
                        File.AppendAllText(Global.arquivoOcultar, arquivo + "\r\n", encode);
                    }

                    // Se for não executar
                    if (opcao == "Não Executar")
                    {
                        File.AppendAllText(Global.arquivoNaoExecutar, arquivo + "\r\n", encode);
                    }

                    // Se for proteger processo
                    if (opcao == "Proteger Processo")
                    {
                        File.AppendAllText(Global.arquivoProtegerProcesso, arquivo + "\r\n", encode);
                    }
                }

                // Releia
                Kernel.RelerTudo();

                // Atualize
                Global.AtualizarExplorer();

                salvar.Enabled = false;
            } catch (Exception)
            {
                MessageBox.Show("Ocorreu um erro ao salvar os dados do usuário", "error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Botão de cancelar
        /// </summary>
        /// 
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void cancelar_Click(object sender, EventArgs e)
        {
            CarregarArquivosSalvos();
        }

        /// <summary>
        /// Quando alterar o Enabled do botão de salvar
        /// </summary>
        /// 
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void salvar_EnabledChanged(object sender, EventArgs e)
        {
            cancelar.Enabled = salvar.Enabled;
        }

        /// <summary>
        /// Botão de remover
        /// </summary>
        /// 
        /// <param name="sender">sender</param>
        /// <param name="e">e</param>
        private void removerArquivo_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lista.SelectedItems)
            {
                lista.Items.Remove(item);
                salvar.Enabled = true;
            }

            //lista.Select();
            //lista.Focus();

            if (lista.Items.Count == 0)
            {
                label1.Visible = true;
            }
        }

        /// <summary>
        /// Botão para adicionar o local personalizado
        /// </summary>
        /// 
        /// <param name="sender">s</param>
        /// <param name="e">e</param>
        private void adicionarMais_Click(object sender, EventArgs e)
        {
            CustomFileDialog dd = new CustomFileDialog();
            if (dd.ShowDialog() == DialogResult.OK)
            {
                AdicionarOuEditArquivo(dd.SelectedPath);
            }
        }
    }
}
