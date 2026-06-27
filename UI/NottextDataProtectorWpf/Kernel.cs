using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Nottext_Data_Protector
{
    class Kernel
    {

        /// <summary>
        /// Importação da DLL KERNEL32.DLL CreateFile
        /// </summary>
        /// <returns></returns>
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int CreateFile(
            String lpFileName, // Nome da porta
            int dwDesiredAccess, // Acesso
            int dwShareMode, // Compartilhamento
            IntPtr lpSecurityAttributes, // Security
            int dwCreationDisposition, // Disposition
            int dwFlagsAndAttributes, // Atributos
            int hTemplateFile // Arquivo
        );

        /// <summary>
        /// Fechar o dispositivo
        /// </summary>
        [DllImport("kernel32", SetLastError = true)]
        static extern bool CloseHandle(
            IntPtr handle // O que fechar
        );

        /// <summary>
        /// DeviceIoControl, necessário para receber e enviar mensagens
        /// </summary>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int DeviceIoControl(
            IntPtr hDevice, // Dispositivo
            uint dwIoControlCode, // Control
            StringBuilder lpInBuffer, // Buffer
            int nInBufferSize, // BufferSize
            StringBuilder lpOutBuffer, // Outbuffer
            Int32 nOutBufferSize, // OutbufferSize
            ref Int32 lpBytesReturned, // Retorno
            IntPtr lpOverlapped //
        );

        // Definições, necessárias
        internal const uint FILE_DEVICE_UNKNOWN = 0x00000022;
        internal const uint FILE_ANY_ACCESS = 0;
        internal const uint METHOD_BUFFERED = 0;
        private const int GENERIC_WRITE = 0x40000000;
        private const int GENERIC_READ = unchecked((int)0x80000000);
        private const int FILE_SHARE_READ = 1;
        private const int FILE_SHARE_WRITE = 2;
        private const int OPEN_EXISTING = 3;
        private const int IOCTL_DISK_GET_DRIVE_LAYOUT_EX = unchecked((int)0x00070050);
        private const int ERROR_INSUFFICIENT_BUFFER = 122;

        /// <summary>
        /// CTL_CODE, necessário para o driver saber se queremos continuar operações
        /// </summary>
        public static uint CTL_CODE(uint DeviceType, uint Function, uint Method, uint Access)
        {
            // Retorne o valor
            return ((DeviceType << 16) | (Access << 14) | (Function << 2) | Method);
        }

        /// <summary>
        /// Envia um IRP para o kernel para reler tudo
        /// </summary>
        public static void RelerTudo()
        {
            // Crie um arquivo, necessário para outras
            // Operações depois
            IntPtr device = (IntPtr)CreateFile(
                "\\\\.\\WlfS", // Nome do dispositivo
                GENERIC_READ | GENERIC_WRITE, // Escrita
                FILE_SHARE_READ | FILE_SHARE_WRITE, // Escrita
                IntPtr.Zero,
                OPEN_EXISTING, // Abra o que já existe
                0,
                0
            );

            // Feche o dispositivo
            CloseHandle(
                device
            );
        }


    }
}
