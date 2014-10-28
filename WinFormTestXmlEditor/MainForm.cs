using System.Windows.Forms;

namespace WinFormTestXmlEditor
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

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
            textEditorControl1.SetHighlighting(cmb.SelectedItem.ToString());
        }
    }
}