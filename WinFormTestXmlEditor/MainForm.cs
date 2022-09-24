using System.Windows.Forms;
using WinFormTestXmlEditor.Properties;

namespace WinFormTestXmlEditor
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            UpdateText("XML");

            //textEditorControl1.SetHighlighting("XML");
            //textEditorControl1.SetFoldingStrategy("XML");
            //textEditorControl1.Font = new Font("Courier New", 8.25f, FontStyle.Regular);

            //UpdateAndCheckFoldings();
        }

        private void textEditorControl1_TextChanged(object sender, System.EventArgs e)
        {
            UpdateAndCheckFoldings();
        }

        private void UpdateAndCheckFoldings()
        {
            textEditorControl1.Document.FoldingManager.UpdateFoldings(null, null);
            textBox1.Text = string.Join("\r\n", textEditorControl1.GetFoldingErrors());
        }

        private void cmbHighlight_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            var cmb = (ComboBox)sender;

            var selectedItem = cmb.SelectedItem.ToString()!;

            UpdateText(selectedItem);
        }

        private void UpdateText(string selectedItem)
        {
            textEditorControl1.Text = selectedItem switch
            {
                "XML" => Resources.ExampleXML,
                "C#" => Resources.ExampleCSharp,
                _ => string.Empty
            };

            textEditorControl1.SetHighlighting(selectedItem);
            textEditorControl1.SetFoldingStrategy(selectedItem);

            UpdateAndCheckFoldings();
        }
    }
}