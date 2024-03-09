using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Nottext_Data_Protector.Forms
{
    public partial class Senha : Form
    {
        // Tipo de enconder, necessário para ler catacterias especiais
        Encoding encode = Encoding.GetEncoding("iso-8859-1");

        /// <summary>
        /// Quando carregar
        /// </summary>
        public Senha()
        {
            InitializeComponent();
            AlterarMouse.AlterarCursor(this);

            // Se for para definir senha
            if (!File.Exists(Global.senhaArquivo))
            {
                label1.Text = "Para continuar, por favor, primeiro, defina uma senha";
            }
        }

        /// <summary>
        /// Criptografa um texto com HASH, o que torna impossível de descriptografar
        /// </summary>
        /// 
        /// <param name="texto">Texto</param>
        /// 
        /// <returns>Retorna a string criptografada</returns>
        private string CriptografarIrreversivel(string texto)
        {
            // Converter a string para um array de bytes
            byte[] bytesTexto = Encoding.UTF8.GetBytes(texto);

            using (SHA256 sha256 = SHA256.Create())
            {
                // Calcular o hash dos bytes do texto
                byte[] hash = sha256.ComputeHash(bytesTexto);

                // Converter o hash em uma string hexadecimal
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("x2"));
                }

                // Retornar o hash como uma string
                return sb.ToString();
            }
        }

        int tentativas = 4;

        /// <summary>
        /// Quando clicar em confirmar
        /// </summary>
        /// 
        /// <param name="sender">Sender</param>
        /// <param name="e">e</param>
        private void confirmar_Click(object sender, EventArgs e)
        {
            try
            {
                // Se não tiver senha
                if (senhaTexto.Text.Length == 0)
                {
                    label2.Visible = true;
                    label2.Text = "Por favor, digite uma senha válida.";
                    return;
                }

                // Senha criptografada
                string senhaTexoCrip = CriptografarIrreversivel(senhaTexto.Text);

                // Se a senha não estiver definido e o textBox tiver a senha
                if (!File.Exists(Global.senhaArquivo))
                {
                    // Escreva e saia
                    File.WriteAllText(Global.senhaArquivo, senhaTexoCrip, encode);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    // Se a senha estiver correta
                    if (File.ReadAllText(Global.senhaArquivo) == senhaTexoCrip)
                    {
                        DialogResult = DialogResult.OK;
                        Close();
                    } else
                    {
                        tentativas--;

                        // Se acabar as tentativas
                        if (tentativas <= 0)
                            Environment.Exit(0);

                        label2.Visible = true;
                        label2.Text = "Senha incorreta! Mais " + tentativas.ToString() + " tentativas.";
                    }
                }
            } catch (Exception)
            {
                MessageBox.Show("Ocorreu um erro ao gravar os dados do usuário", "error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
