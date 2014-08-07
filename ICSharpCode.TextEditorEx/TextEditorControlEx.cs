using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.TextEditor.Src.Actions;
using ICSharpCode.TextEditor.Src.Document.FoldingStrategy;
using ICSharpCode.TextEditor.UserControls;

namespace ICSharpCode.TextEditor
{
    [ToolboxBitmap("ICSharpCode.TextEditor.Resources.TextEditorControl.bmp")]
    [ToolboxItem(true)]
    public class TextEditorControlEx : TextEditorControl
    {
        private string _foldingStrategy;

        public TextEditorControlEx()
        {
            var findForm = new FindAndReplaceForm();

            editactions[Keys.Control | Keys.F] = new EditFindAction(findForm, this);
            editactions[Keys.Control | Keys.H] = new EditReplaceAction(findForm, this);
            editactions[Keys.F3] = new FindAgainAction(findForm, this);
            editactions[Keys.F3 | Keys.Shift] = new FindAgainReverseAction(findForm, this);

            editactions[Keys.Control | Keys.G] = new GoToLineNumberAction();
        }

        /// <summary>
        /// Sets the text and refreshes the control.
        /// </summary>
        /// <param name="text">The text.</param>
        public void SetTextAndRefresh(string text)
        {
            ResetText();
            Text = text;
            Refresh();
        }

        /// <summary>
        /// Sets the folding strategy. Currently only XML is supported.
        /// </summary>
        /// <param name="foldingStrategy">The foldingStrategy.</param>
        public void SetFoldingStrategy(string foldingStrategy)
        {
            if (foldingStrategy == null)
            {
                throw new ArgumentNullException("foldingStrategy");
            }

            switch (foldingStrategy.ToUpper())
            {
                case "XML":
                    _foldingStrategy = "XML";
                    Document.FoldingManager.FoldingStrategy = new XmlFoldingStrategy();
                    break;

                default:
                    Document.FoldingManager.FoldingStrategy = null;
                    _foldingStrategy = null;
                    break;
            }

            Document.FoldingManager.UpdateFoldings(null, null);
        }

        /// <summary>
        /// Gets the folding errors. Currently only XML is supported.
        /// </summary>
        /// <returns>List of errors, else empty list</returns>
        public List<string> GetFoldingErrors()
        {
            if (_foldingStrategy == "XML")
            {
                var foldingStrategy = Document.FoldingManager.FoldingStrategy as IFoldingStrategyEx;
                if (foldingStrategy != null)
                {
                    return foldingStrategy.GetFoldingErrors();
                }
            }

            return new List<string>();
        }
    }
}
