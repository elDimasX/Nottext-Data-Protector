using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nottext_Data_Protector
{
    class Global
    {
        // Locais

        public static string pasta = "C:\\SystemGuards\\";

        public static string arquivoSomenteLeitura = pasta + "pp.cat";
        public static string arquivoBloquear = pasta + "pp.sys";
        public static string arquivoProcessos = pasta + "pp.inf";

        public static string protecaoHabilitada = pasta + "pp.end";

        public static string terminarProcessos = pasta + "pp.term";
        public static string messageBox = pasta + "pp.msg";

    }
}
