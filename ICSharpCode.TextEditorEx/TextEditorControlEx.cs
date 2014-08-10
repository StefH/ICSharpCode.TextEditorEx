using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Src.Actions;
using ICSharpCode.TextEditor.Src.Document.FoldingStrategy;
using ICSharpCode.TextEditor.Src.Document.HighlightingStrategy.SyntaxModes;
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

            // Add additional Syntax highlighting providers
            HighlightingManager.Manager.AddSyntaxModeFileProvider(new ResourceSyntaxModeProviderEx());

            TextChanged += TextChangedEventHandler;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            TextChanged -= TextChangedEventHandler;
        }

        private void TextChangedEventHandler(object sender, EventArgs e)
        {
            var editor = sender as TextEditorControlEx;
            if (editor != null)
            {
                bool vScrollBarIsNeeded = editor.Document.TotalNumberOfLines > ActiveTextAreaControl.TextArea.TextView.VisibleLineCount;
                if (ActiveTextAreaControl.VScrollBar.Visible && HideVScrollBarIfPossible && !vScrollBarIsNeeded)
                {
                    ActiveTextAreaControl.ShowScrollBars(Orientation.Vertical, false);
                }
            }
        }

        /// <value>
        /// The base font of the text area. No bold or italic fonts
        /// can be used because bold/italic is reserved for highlighting
        /// purposes.
        /// </value>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(typeof(Font), null)]
        [Description("The base font of the text area. No bold or italic fonts can be used because bold/italic is reserved for highlighting purposes.")]
        public override Font Font
        {
            get
            {
                return Document.TextEditorProperties.Font;
            }
            set
            {
                Document.TextEditorProperties.Font = value;
                OptionsChanged();
            }
        }

        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Hide the vertical ScrollBar if it's not needed. ")]
        public bool HideVScrollBarIfPossible { get; set; }

        private string _foldingStrategy;
        [Category("Appearance")]
        [Description("Set the Folding Strategy. Supported : XML and CSharp.")]
        public string FoldingStrategy
        {
            get
            {
                return _foldingStrategy;
            }
            set
            {
                SetFoldingStrategy(value);
                OptionsChanged();
            }
        }

        private string _syntaxHighlighting;
        [Category("Appearance")]
        [Description("Sets the Syntax Highlighting.")]
        public string SyntaxHighlighting
        {
            get
            {
                return _syntaxHighlighting;
            }
            set
            {
                _syntaxHighlighting = value;
                SetHighlighting(_syntaxHighlighting);
                OptionsChanged();
            }
        }

        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("If true document is readonly.")]
        [Browsable(true)]
        public new bool IsReadOnly
        {
            get
            {
                return Document.ReadOnly;
            }
            set
            {
                Document.ReadOnly = value;
                OptionsChanged();
            }
        }

        /// <summary>
        /// Sets the text and refreshes the control.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="updateFoldings">if set to <c>true</c> [update foldings].</param>
        public void SetTextAndRefresh(string text, bool updateFoldings = false)
        {
            ResetText();
            Text = text;

            if (updateFoldings && Document.TextEditorProperties.EnableFolding)
            {
                Document.FoldingManager.UpdateFoldings(null, null);
            }

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

            if (!Document.TextEditorProperties.EnableFolding)
            {
                return;
            }

            switch (foldingStrategy)
            {
                case "XML":
                    _foldingStrategy = foldingStrategy;
                    Document.FoldingManager.FoldingStrategy = new XmlFoldingStrategy();
                    break;

                case "CSharp":
                    _foldingStrategy = foldingStrategy;
                    Document.FoldingManager.FoldingStrategy = new CSharpFoldingStrategy();
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

    public static class TextAreaControlExtensions
    {
        /// <summary>
        /// Extension method to show a scrollbar.
        /// </summary>
        /// <param name="textAreaControl">The text area control.</param>
        /// <param name="orientation">The orientation.</param>
        /// <param name="isVisible">if set to <c>true</c> [is visible].</param>
        public static void ShowScrollBars(this TextAreaControl textAreaControl, Orientation orientation, bool isVisible)
        {
            if (orientation == Orientation.Vertical)
            {
                textAreaControl.VScrollBar.Visible = isVisible;
            }
            else
            {
                textAreaControl.HScrollBar.Visible = isVisible;
            }
        }
    }
}