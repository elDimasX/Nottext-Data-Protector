using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Nottext_Data_Protector.Services
{
    public static class PasswordService
    {
        private static readonly Encoding Encode = Encoding.GetEncoding("iso-8859-1");

        public static bool PasswordExists()
        {
            try
            {
                bool exists = false;
                WithDriverAccess(delegate { exists = File.Exists(Global.senhaArquivo); });
                return exists;
            }
            catch { return false; }
        }

        public static void SavePassword(string password)
        {
            WithDriverAccess(delegate
            {
                File.WriteAllText(Global.senhaArquivo, Hash(password), Encode);
            });
        }

        public static bool ValidatePassword(string password)
        {
            if (!PasswordExists())
                return false;

            string stored = string.Empty;
            WithDriverAccess(delegate
            {
                stored = File.ReadAllText(Global.senhaArquivo, Encode);
            });

            return stored == Hash(password);
        }

        public static void RemovePassword()
        {
            WithDriverAccess(delegate
            {
                File.Delete(Global.senhaArquivo);
            });
        }

        private static string Hash(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text ?? string.Empty);
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(bytes);
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hash)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        private static void WithDriverAccess(Action action)
        {
            try
            {
                Kernel.RelerTudo();
                action();
            }
            catch
            {
                Kernel.RelerTudo();
                Thread.Sleep(75);
                action();
            }
            finally
            {
                Kernel.RelerTudo();
            }
        }
    }
}
