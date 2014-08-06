using System.Drawing;
using System.Windows.Forms;

namespace WinFormTestXmlEditor
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            textEditorControl1.SetHighlighting("XML");
            textEditorControl1.Font = new Font("Courier New", 8.25f, FontStyle.Regular);
        }
    }
}