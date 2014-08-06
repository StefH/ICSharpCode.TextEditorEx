using ICSharpCode.TextEditor.UserControls;

namespace ICSharpCode.TextEditor.Src.Actions
{
    class EditFindAction : FindAndReplaceFormAction
    {
        public EditFindAction(FindAndReplaceForm findForm, TextEditorControlEx control)
            : base(findForm, control)
        {
        }

        public override void Execute(TextArea textArea)
        {
            FindForm.ShowFor(Control, false);
        }
    }
}