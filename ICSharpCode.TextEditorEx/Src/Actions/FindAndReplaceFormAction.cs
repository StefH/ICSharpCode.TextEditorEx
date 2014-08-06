using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor.UserControls;

namespace ICSharpCode.TextEditor.Src.Actions
{
    abstract class FindAndReplaceFormAction : AbstractEditAction
    {
        protected readonly TextEditorControlEx Control;
        protected readonly FindAndReplaceForm FindForm;

        protected FindAndReplaceFormAction(FindAndReplaceForm findForm, TextEditorControlEx control)
        {
            FindForm = findForm;
            Control = control;
        }
    }
}