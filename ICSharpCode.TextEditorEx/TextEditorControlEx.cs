using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.TextEditor.Src.Actions;
using ICSharpCode.TextEditor.UserControls;

namespace ICSharpCode.TextEditor
{
    [ToolboxBitmap("ICSharpCode.TextEditor.Resources.TextEditorControl.bmp")]
    [ToolboxItem(true)]
    public class TextEditorControlEx : TextEditorControl
    {
        public TextEditorControlEx()
        {
            var findForm = new FindAndReplaceForm();

            editactions[Keys.Control | Keys.F] = new EditFindAction(findForm, this);
            editactions[Keys.Control | Keys.H] = new EditReplaceAction(findForm, this);
            editactions[Keys.F3] = new FindAgainAction(findForm, this);
            editactions[Keys.F3 | Keys.Shift] = new FindAgainReverseAction(findForm, this);

            editactions[Keys.Control | Keys.G] = new GoToLineNumberAction();
        }
    }
}
