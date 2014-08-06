using ICSharpCode.TextEditor.UserControls;

namespace ICSharpCode.TextEditor.Src.Actions
{
    class EditReplaceAction : FindAndReplaceFormAction
    {
        public EditReplaceAction(FindAndReplaceForm findForm, TextEditorControlEx control)
            : base(findForm, control)
        {
        }

        public override void Execute(TextArea textArea)
        {
            FindForm.ShowFor(Control, true);
        }
    }
}