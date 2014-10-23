// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Actions;
using System.Text.RegularExpressions;

namespace ICSharpCode.TextEditor
{
	/// <summary>
	/// This class is used for a basic text area control
	/// </summary>
	[ToolboxBitmap("ICSharpCode.TextEditor.Resources.TextEditorControl.bmp")]
	[ToolboxItem(true)]
	public class TextEditorControl : TextEditorControlBase
	{
		protected Panel textAreaPanel     = new Panel();
		TextAreaControl primaryTextArea;
		Splitter        textAreaSplitter  = null;
		TextAreaControl secondaryTextArea = null;
		
		PrintDocument   printDocument = null;
		
		[Browsable(false)]
		public PrintDocument PrintDocument {
			get {
				if (printDocument == null) {
					printDocument = new PrintDocument();
					printDocument.BeginPrint += new PrintEventHandler(this.BeginPrint);
					printDocument.PrintPage  += new PrintPageEventHandler(this.PrintPage);
				}
				return printDocument;
			}
		}
		
		TextAreaControl activeTextAreaControl;
		
		public override TextAreaControl ActiveTextAreaControl {
			get {
				return activeTextAreaControl;
			}
		}
		
		protected void SetActiveTextAreaControl(TextAreaControl value)
		{
			if (activeTextAreaControl != value) {
				activeTextAreaControl = value;
				
				if (ActiveTextAreaControlChanged != null) {
					ActiveTextAreaControlChanged(this, EventArgs.Empty);
				}
			}
		}
		
		public event EventHandler ActiveTextAreaControlChanged;
		
		public TextEditorControl()
		{
			SetStyle(ControlStyles.ContainerControl, true);
			
			textAreaPanel.Dock = DockStyle.Fill;
			
			Document = (new DocumentFactory()).CreateDocument();
			Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy();
			
			primaryTextArea  = new TextAreaControl(this);
			activeTextAreaControl = primaryTextArea;
			primaryTextArea.TextArea.GotFocus += delegate {
				SetActiveTextAreaControl(primaryTextArea);
			};
			primaryTextArea.Dock = DockStyle.Fill;
			textAreaPanel.Controls.Add(primaryTextArea);
			InitializeTextAreaControl(primaryTextArea);
			Controls.Add(textAreaPanel);
			ResizeRedraw = true;
			Document.UpdateCommited += new EventHandler(CommitUpdateRequested);
			OptionsChanged();
            base.Document.DocumentChanged += this.Document_DocumentChanged;
         

		}
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (EnableContextMenu)
            {
                this.CreateContextMenu();
            }
        }
		
		protected virtual void InitializeTextAreaControl(TextAreaControl newControl)
		{

		}
		
		public override void OptionsChanged()
		{
			primaryTextArea.OptionsChanged();
			if (secondaryTextArea != null) {
				secondaryTextArea.OptionsChanged();
			}
		}
		
		public void Split()
		{
			if (secondaryTextArea == null) {
				secondaryTextArea = new TextAreaControl(this);
				secondaryTextArea.Dock = DockStyle.Bottom;
				secondaryTextArea.Height = Height / 2;
				
				secondaryTextArea.TextArea.GotFocus += delegate {
					SetActiveTextAreaControl(secondaryTextArea);
				};
				
				textAreaSplitter =  new Splitter();
				textAreaSplitter.BorderStyle = BorderStyle.FixedSingle ;
				textAreaSplitter.Height = 8;
				textAreaSplitter.Dock = DockStyle.Bottom;
				textAreaPanel.Controls.Add(textAreaSplitter);
				textAreaPanel.Controls.Add(secondaryTextArea);
				InitializeTextAreaControl(secondaryTextArea);
				secondaryTextArea.OptionsChanged();
			} else {
				SetActiveTextAreaControl(primaryTextArea);
				
				textAreaPanel.Controls.Remove(secondaryTextArea);
				textAreaPanel.Controls.Remove(textAreaSplitter);
				
				secondaryTextArea.Dispose();
				textAreaSplitter.Dispose();
				secondaryTextArea = null;
				textAreaSplitter  = null;
			}
		}
		
		[Browsable(false)]
		public bool EnableUndo {
			get {
				return Document.UndoStack.CanUndo;
			}
		}
		
		[Browsable(false)]
		public bool EnableRedo {
			get {
				return Document.UndoStack.CanRedo;
			}
		}

		public void Undo()
		{
			if (Document.ReadOnly) {
				return;
			}
			if (Document.UndoStack.CanUndo) {
				BeginUpdate();
				Document.UndoStack.Undo();
				
				Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
				this.primaryTextArea.TextArea.UpdateMatchingBracket();
				if (secondaryTextArea != null) {
					this.secondaryTextArea.TextArea.UpdateMatchingBracket();
				}
				EndUpdate();
			}
		}
		
		public void Redo()
		{
			if (Document.ReadOnly) {
				return;
			}
			if (Document.UndoStack.CanRedo) {
				BeginUpdate();
				Document.UndoStack.Redo();
				
				Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
				this.primaryTextArea.TextArea.UpdateMatchingBracket();
				if (secondaryTextArea != null) {
					this.secondaryTextArea.TextArea.UpdateMatchingBracket();
				}
				EndUpdate();
			}
		}
		
		public virtual void SetHighlighting(string name)
		{
			Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy(name);
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (printDocument != null) {
					printDocument.BeginPrint -= new PrintEventHandler(this.BeginPrint);
					printDocument.PrintPage  -= new PrintPageEventHandler(this.PrintPage);
					printDocument = null;
				}
				Document.UndoStack.ClearAll();
				Document.UpdateCommited -= new EventHandler(CommitUpdateRequested);
				if (textAreaPanel != null) {
					if (secondaryTextArea != null) {
						secondaryTextArea.Dispose();
						textAreaSplitter.Dispose();
						secondaryTextArea = null;
						textAreaSplitter  = null;
					}
					if (primaryTextArea != null) {
						primaryTextArea.Dispose();
					}
					textAreaPanel.Dispose();
					textAreaPanel = null;
				}
			}
			base.Dispose(disposing);
		}
		
		#region Update Methods
		public override void EndUpdate()
		{
			base.EndUpdate();
			Document.CommitUpdate();
			if (!IsInUpdate) {
				ActiveTextAreaControl.Caret.OnEndUpdate();
			}
		}
		
		void CommitUpdateRequested(object sender, EventArgs e)
		{
			if (IsInUpdate) {
				return;
			}
			foreach (TextAreaUpdate update in Document.UpdateQueue) {
				switch (update.TextAreaUpdateType) {
					case TextAreaUpdateType.PositionToEnd:
						this.primaryTextArea.TextArea.UpdateToEnd(update.Position.Y);
						if (this.secondaryTextArea != null) {
							this.secondaryTextArea.TextArea.UpdateToEnd(update.Position.Y);
						}
						break;
					case TextAreaUpdateType.PositionToLineEnd:
					case TextAreaUpdateType.SingleLine:
						this.primaryTextArea.TextArea.UpdateLine(update.Position.Y);
						if (this.secondaryTextArea != null) {
							this.secondaryTextArea.TextArea.UpdateLine(update.Position.Y);
						}
						break;
					case TextAreaUpdateType.SinglePosition:
						this.primaryTextArea.TextArea.UpdateLine(update.Position.Y, update.Position.X, update.Position.X);
						if (this.secondaryTextArea != null) {
							this.secondaryTextArea.TextArea.UpdateLine(update.Position.Y, update.Position.X, update.Position.X);
						}
						break;
					case TextAreaUpdateType.LinesBetween:
						this.primaryTextArea.TextArea.UpdateLines(update.Position.X, update.Position.Y);
						if (this.secondaryTextArea != null) {
							this.secondaryTextArea.TextArea.UpdateLines(update.Position.X, update.Position.Y);
						}
						break;
					case TextAreaUpdateType.WholeTextArea:
						this.primaryTextArea.TextArea.Invalidate();
						if (this.secondaryTextArea != null) {
							this.secondaryTextArea.TextArea.Invalidate();
						}
						break;
				}
			}
			Document.UpdateQueue.Clear();
//			this.primaryTextArea.TextArea.Update();
//			if (this.secondaryTextArea != null) {
//				this.secondaryTextArea.TextArea.Update();
//			}
		}
		#endregion
		
		#region Printing routines
		int          curLineNr = 0;
		float        curTabIndent = 0;
		StringFormat printingStringFormat;
		
		void BeginPrint(object sender, PrintEventArgs ev)
		{
			curLineNr = 0;
			printingStringFormat = (StringFormat)System.Drawing.StringFormat.GenericTypographic.Clone();
			
			// 100 should be enough for everyone ...err ?
			float[] tabStops = new float[100];
			for (int i = 0; i < tabStops.Length; ++i) {
				tabStops[i] = TabIndent * primaryTextArea.TextArea.TextView.WideSpaceWidth;
			}
			
			printingStringFormat.SetTabStops(0, tabStops);
		}
		
		void Advance(ref float x, ref float y, float maxWidth, float size, float fontHeight)
		{
			if (x + size < maxWidth) {
				x += size;
			} else {
				x  = curTabIndent;
				y += fontHeight;
			}
		}
		
		// btw. I hate source code duplication ... but this time I don't care !!!!
		float MeasurePrintingHeight(Graphics g, LineSegment line, float maxWidth)
		{
			float xPos = 0;
			float yPos = 0;
			float fontHeight = Font.GetHeight(g);
//			bool  gotNonWhitespace = false;
			curTabIndent = 0;
			FontContainer fontContainer = TextEditorProperties.FontContainer;
			foreach (TextWord word in line.Words) {
				switch (word.Type) {
					case TextWordType.Space:
						Advance(ref xPos, ref yPos, maxWidth, primaryTextArea.TextArea.TextView.SpaceWidth, fontHeight);
//						if (!gotNonWhitespace) {
//							curTabIndent = xPos;
//						}
						break;
					case TextWordType.Tab:
						Advance(ref xPos, ref yPos, maxWidth, TabIndent * primaryTextArea.TextArea.TextView.WideSpaceWidth, fontHeight);
//						if (!gotNonWhitespace) {
//							curTabIndent = xPos;
//						}
						break;
					case TextWordType.Word:
//						if (!gotNonWhitespace) {
//							gotNonWhitespace = true;
//							curTabIndent    += TabIndent * primaryTextArea.TextArea.TextView.GetWidth(' ');
//						}
						SizeF drawingSize = g.MeasureString(word.Word, word.GetFont(fontContainer), new SizeF(maxWidth, fontHeight * 100), printingStringFormat);
						Advance(ref xPos, ref yPos, maxWidth, drawingSize.Width, fontHeight);
						break;
				}
			}
			return yPos + fontHeight;
		}
		
		void DrawLine(Graphics g, LineSegment line, float yPos, RectangleF margin)
		{
			float xPos = 0;
			float fontHeight = Font.GetHeight(g);
//			bool  gotNonWhitespace = false;
			curTabIndent = 0 ;
			
			FontContainer fontContainer = TextEditorProperties.FontContainer;
			foreach (TextWord word in line.Words) {
				switch (word.Type) {
					case TextWordType.Space:
						Advance(ref xPos, ref yPos, margin.Width, primaryTextArea.TextArea.TextView.SpaceWidth, fontHeight);
//						if (!gotNonWhitespace) {
//							curTabIndent = xPos;
//						}
						break;
					case TextWordType.Tab:
						Advance(ref xPos, ref yPos, margin.Width, TabIndent * primaryTextArea.TextArea.TextView.WideSpaceWidth, fontHeight);
//						if (!gotNonWhitespace) {
//							curTabIndent = xPos;
//						}
						break;
					case TextWordType.Word:
//						if (!gotNonWhitespace) {
//							gotNonWhitespace = true;
//							curTabIndent    += TabIndent * primaryTextArea.TextArea.TextView.GetWidth(' ');
//						}
						g.DrawString(word.Word, word.GetFont(fontContainer), BrushRegistry.GetBrush(word.Color), xPos + margin.X, yPos);
						SizeF drawingSize = g.MeasureString(word.Word, word.GetFont(fontContainer), new SizeF(margin.Width, fontHeight * 100), printingStringFormat);
						Advance(ref xPos, ref yPos, margin.Width, drawingSize.Width, fontHeight);
						break;
				}
			}
		}
		
		void PrintPage(object sender, PrintPageEventArgs ev)
		{
			Graphics g = ev.Graphics;
			float yPos = ev.MarginBounds.Top;
			
			while (curLineNr < Document.TotalNumberOfLines) {
				LineSegment curLine  = Document.GetLineSegment(curLineNr);
				if (curLine.Words != null) {
					float drawingHeight = MeasurePrintingHeight(g, curLine, ev.MarginBounds.Width);
					if (drawingHeight + yPos > ev.MarginBounds.Bottom) {
						break;
					}
					
					DrawLine(g, curLine, yPos, ev.MarginBounds);
					yPos += drawingHeight;
				}
				++curLineNr;
			}
			
			// If more lines exist, print another page.
			ev.HasMorePages = curLineNr < Document.TotalNumberOfLines;
		}
		#endregion
	


        

        #region Extended properties

        public string SelectedText
        {
            get
            {
                return ActiveTextAreaControl.SelectionManager.SelectedText;
            }
        }

        public string[] Lines
        {
            get
            {
                return base.Text.Split(new[] { "\r\n" }, StringSplitOptions.None);
            }
        }

        #endregion

        private int previousSearchLine = -1;
        private int previousSearchWord;

      
         

        #region Commands implementations

        private bool CanUndo()
        {
            return   base.Document.UndoStack.CanUndo;
        }

        private bool CanRedo()
        {
            return   base.Document.UndoStack.CanRedo;
        }

        private bool CanCopy()
        {
            return   ActiveTextAreaControl.SelectionManager.HasSomethingSelected;
        }

        private bool CanCut()
        {
            return  ActiveTextAreaControl.SelectionManager.HasSomethingSelected;
        }

        private bool CanPaste()
        {
            return   ActiveTextAreaControl.TextArea.ClipboardHandler.EnablePaste;
        }

        private bool CanSelectAll()
        {
          
            if (base.Document.TextContent == null) return false;
            return !base.Document.TextContent.Trim().Equals(String.Empty);
        }




        private void DoCut()
        {
            new Cut().Execute(ActiveTextAreaControl.TextArea);
            ActiveTextAreaControl.Focus();
        }

        private void DoCopy()
        {
            new Copy().Execute(ActiveTextAreaControl.TextArea);
            ActiveTextAreaControl.Focus();
        }

        private void DoPaste()
        {
            new Paste().Execute(ActiveTextAreaControl.TextArea);
            ActiveTextAreaControl.Focus();
        }

        private void DoSelectAll()
        {
            new SelectWholeDocument().Execute(ActiveTextAreaControl.TextArea);
            ActiveTextAreaControl.Focus();
        }

        public void DoToggleFoldings()
        {
            new ToggleAllFoldings().Execute(ActiveTextAreaControl.TextArea);
        }

        #endregion

        [DefaultValue(false)]
        public bool  EnableContextMenu { get; set; }
        # region Initialization

        private void CreateContextMenu()
        {
            //contextmenu
            var mnu = new ContextMenuStrip();
            var mnuUndo = new ToolStripMenuItem(Resources.Language.Undo );
            mnuUndo.ShortcutKeys = Keys.Control | Keys.Z;
            mnuUndo.Click += (object sender, EventArgs e) => { Undo(); };
            var mnuRedo = new ToolStripMenuItem(Resources.Language.Redo);
           // mnuRedo.ShortcutKeys = Keys.Shift | Keys.Alt | Keys.Back;
            mnuRedo.Click += (object sender, EventArgs e) => { Redo(); };
            var mnuCut = new ToolStripMenuItem(Resources.Language.Cut);
            mnuCut.ShortcutKeys = Keys.Control | Keys.X;
            var mnutemp1 = new  ToolStripSeparator();
            mnuCut.Click += (object sender, EventArgs e) => { DoCut(); };
            var mnuCopy = new ToolStripMenuItem(Resources.Language.Copy);
            mnuCopy.ShortcutKeys = Keys.Control | Keys.C;
            mnuCopy.Click += (object sender, EventArgs e) => { DoCopy (); };
            var mnuPaste = new ToolStripMenuItem(Resources.Language.Paste);
            mnuPaste.ShortcutKeys = Keys.Control | Keys.V;
            var mnutemp2 = new ToolStripSeparator();
            mnuPaste.Click += (object sender, EventArgs e) => { DoPaste(); };
            var mnuSelectAll = new ToolStripMenuItem(Resources.Language.Select_All);
            mnuSelectAll.ShortcutKeys = Keys.Control | Keys.A;
            mnuSelectAll.Click += (object sender, EventArgs e) => { DoSelectAll(); };
            //Add to main context menu
            mnu.Items.AddRange(new ToolStripItem[] { mnuUndo, mnuRedo, mnutemp1,mnuCut, mnuCopy, mnuPaste,mnutemp2, mnuSelectAll });
              mnu.Opening += (object sender, CancelEventArgs e) => {
                  mnuUndo.Enabled = CanUndo();
                  mnuCopy.Enabled = CanCopy();
                  mnuCut.Enabled = CanCut();
                  mnuPaste.Enabled = CanPaste();
                  mnuRedo.Enabled = CanRedo();
                  mnuSelectAll.Enabled  = CanSelectAll();
            };
            //Assign to datagridview
            ActiveTextAreaControl.ContextMenuStrip = mnu;
        }
 

        #endregion

        public void SelectText(int start, int length)
        {
            var textLength = Document.TextLength;
            if (textLength < (start + length))
            {
                length = (textLength - 1) - start;
            }
            ActiveTextAreaControl.Caret.Position = Document.OffsetToPosition(start + length);
            ActiveTextAreaControl.SelectionManager.ClearSelection();
            ActiveTextAreaControl.SelectionManager.SetSelection(new DefaultSelection(Document, Document.OffsetToPosition(start), Document.OffsetToPosition(start + length)));
            Refresh();
        }

        public void Find(string search)
        {
            this.Find(search, false);
        }

        public void Find(string search, bool caseSensitive)
        {
            var found = false;

            var i = 0;
            var lines = this.Lines;
            foreach (var line in lines)
            {
                if (i > previousSearchLine)
                {
                    int start;
                    if (previousSearchWord > line.Length)
                    {
                        start = caseSensitive ?
                            line.IndexOf(search) :
                            line.ToLower().IndexOf(search.ToLower());

                        previousSearchWord = 0;
                    }
                    else
                    {
                        start = caseSensitive ?
                            line.IndexOf(search, previousSearchWord) :
                            line.ToLower().IndexOf(search.ToLower(), previousSearchWord);
                    }
                    var end = start + search.Length;
                    if (start != -1)
                    {
                        var p1 = new Point(start, i);
                        var p2 = new Point(end, i);

                        //TODO base.ActiveTextAreaControl.SelectionManager.SetSelection(p1, p2);
                        ActiveTextAreaControl.ScrollTo(i);
                        base.Refresh();

                        previousSearchWord = end;
                        previousSearchLine = i - 1;
                        found = true;
                        break;
                    }

                    previousSearchWord = 0;
                }

                i += 1;
                if (i >= lines.Length - 1)
                {
                    previousSearchLine = -1;
                }
            }

            if (!found)
            {
                MessageBox.Show("The following specified text was not found: " + Environment.NewLine + Environment.NewLine + search);
            }
        }

        public void Replace(string search, string replace, bool caseSensitive)
        {
            if (ActiveTextAreaControl.SelectionManager.HasSomethingSelected &&  ActiveTextAreaControl.SelectionManager.SelectedText == search)
            {
                var text = ActiveTextAreaControl.SelectionManager.SelectedText;
                ActiveTextAreaControl.Caret.Position =ActiveTextAreaControl.SelectionManager.SelectionCollection[0].StartPosition;
                ActiveTextAreaControl.SelectionManager.ClearSelection();
                ActiveTextAreaControl.Document.Replace(ActiveTextAreaControl.Caret.Offset, text.Length, replace);
            }
            this.Find(search, caseSensitive);
        }

        public void ReplaceAll(string search, string replace)
        {
            this.ReplaceAll(search, replace, false);
        }

        public void ReplaceAll(string search, string replace, bool caseSensitive)
        {
            base.Text = Regex.Replace(base.Text, search, replace, caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase);
            base.Refresh();
        }

        public void ResetLastFound()
        {
            previousSearchLine = -1;
            previousSearchWord = 0;
        }

        private void Document_DocumentChanged(object sender, DocumentEventArgs e)
        {
            //base.Document.FoldingManager.UpdateFoldings(string.Empty, null);
            bool isVisible = (base.Document.TotalNumberOfLines > this.ActiveTextAreaControl.TextArea.TextView.VisibleLineCount);
          ActiveTextAreaControl.VScrollBar.Visible = isVisible;               
        }

    }
}
