using ICSharpCode.TextEditor.UserControls;

namespace ICSharpCode.TextEditor.Src.Actions
{
    class FindAgainReverseAction : FindAndReplaceFormAction
    {
        public FindAgainReverseAction(FindAndReplaceForm findForm, TextEditorControlEx control)
            : base(findForm, control)
        {
        }

        public override void Execute(TextArea textArea)
        {
            FindForm.FindNext(true, true, string.Format("Search text «{0}» not found.", FindForm.LookFor));
        }
    }
}