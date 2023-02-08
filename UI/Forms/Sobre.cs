using System.Diagnostics;
using System.Windows.Forms;

namespace Nottext_Data_Protector.Forms
{
    public partial class Sobre : Form
    {
        public Sobre()
        {
            InitializeComponent();
            AlterarMouse.AlterarCursor(this);
        }

        private void label3_Click(object sender, System.EventArgs e)
        {

            Process.Start("https://github.com/elDimasX/Nottext-Data-Protector");
        }
    }
}
