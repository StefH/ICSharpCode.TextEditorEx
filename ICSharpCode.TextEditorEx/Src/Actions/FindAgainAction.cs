namespace ICSharpCode.TextEditor.Src.Actions
{
    class FindAgainAction : FindAndReplaceFormAction
    {
        public FindAgainAction(FindAndReplaceForm findForm, TextEditorControlEx control)
            : base(findForm, control)
        {
        }

        public override void Execute(TextArea textArea)
        {
            FindForm.FindNext(true, false, string.Format("Search text «{0}» not found.", FindForm.LookFor));
        }
    }
}
