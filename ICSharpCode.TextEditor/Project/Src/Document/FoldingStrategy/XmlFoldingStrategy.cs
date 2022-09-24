﻿// Copied from http://codingeditor.googlecode.com/svn/trunk/libs/ICSharpCode.TextEditor/Project/Src/Document/FoldingStrategy/
#region Header

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1971 $</version>
// </file>

#endregion Header

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.TextEditor.Src.Document.FoldingStrategy
{
    /// <summary>
    /// Holds information about the start of a fold in an xml string.
    /// </summary>
    class XmlFoldStart
    {
        #region Fields

        readonly int _col;
        string _foldText = string.Empty;
        readonly int _line;
        readonly string _name = string.Empty;
        readonly string _prefix = string.Empty;

        #endregion Fields

        #region Constructors

        public XmlFoldStart(string prefix, string name, int line, int col)
        {
            _line = line;
            _col = col;
            _prefix = prefix;
            _name = name;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// The column where the fold should start.  Columns start from 0.
        /// </summary>
        public int Column
        {
            get
            {
                return _col;
            }
        }

        /// <summary>
        /// The text to be displayed when the item is folded.
        /// </summary>
        public string FoldText
        {
            get
            {
                return _foldText;
            }

            set
            {
                _foldText = value;
            }
        }

        /// <summary>
        /// The line where the fold should start.  Lines start from 0.
        /// </summary>
        public int Line
        {
            get
            {
                return _line;
            }
        }

        /// <summary>
        /// The name of the xml item with its prefix if it has one.
        /// </summary>
        public string Name
        {
            get
            {
                return _prefix.Length > 0 ? string.Concat(_prefix, ":", _name) : _name;
            }
        }

        #endregion Properties
    }

    /// <summary>
    /// Determines folds for an xml string in the editor.
    /// </summary>
    public class XmlFoldingStrategy : IFoldingStrategyEx
    {
        #region Fields

        /// <summary>
        /// Flag indicating whether attributes should be displayed on folded elements.
        /// </summary>
        public bool ShowAttributesWhenFolded = false;

        private List<string> _foldingErrors = new List<string>();

        #endregion Fields

        #region Methods

        public List<string> GetFoldingErrors()
        {
            return _foldingErrors;
        }

        /// <summary>
        /// Adds folds to the text editor around each start-end element pair.
        /// </summary>
        /// <remarks>
        /// <para>If the xml is not well formed then no folds are created.</para> 
        /// <para>Note that the xml text reader lines and positions start 
        /// from 1 and the SharpDevelop text editor line information starts from 0.</para>
        /// </remarks>
        public List<FoldMarker> GenerateFoldMarkers(IDocument document, string fileName, object parseInformation)
        {
            _foldingErrors = new List<string>();
            //showAttributesWhenFolded = XmlEditorAddInOptions.ShowAttributesWhenFolded;

            var foldMarkers = new List<FoldMarker>();
            var stack = new Stack();

            try
            {
                string xml = document.TextContent;
                var reader = new XmlTextReader(new StringReader(xml));
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (!reader.IsEmptyElement)
                            {
                                XmlFoldStart newFoldStart = CreateElementFoldStart(reader);
                                stack.Push(newFoldStart);
                            }
                            break;

                        case XmlNodeType.EndElement:
                            var foldStart = (XmlFoldStart)stack.Pop();
                            CreateElementFold(document, foldMarkers, reader, foldStart);
                            break;

                        case XmlNodeType.Comment:
                            CreateCommentFold(document, foldMarkers, reader);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _foldingErrors.Add(ex.Message);

                // If the xml is not well formed keep the foldings that already exist in the document.
                return new List<FoldMarker>(document.FoldingManager.FoldMarker);
            }

            return foldMarkers;
        }

        /// <summary>
        /// Xml encode the attribute string since the string returned from
        /// the XmlTextReader is the plain unencoded string and .NET
        /// does not provide us with an xml encode method.
        /// </summary>
        static string XmlEncodeAttributeValue(string attributeValue, char quoteChar)
        {
            var encodedValue = new StringBuilder(attributeValue);

            encodedValue.Replace("&", "&amp;");
            encodedValue.Replace("<", "&lt;");
            encodedValue.Replace(">", "&gt;");

            if (quoteChar == '"')
            {
                encodedValue.Replace("\"", "&quot;");
            }
            else
            {
                encodedValue.Replace("'", "&apos;");
            }

            return encodedValue.ToString();
        }

        /// <summary>
        /// Creates a comment fold if the comment spans more than one line.
        /// </summary>
        /// <remarks>The text displayed when the comment is folded is the first 
        /// line of the comment.</remarks>
        void CreateCommentFold(IDocument document, List<FoldMarker> foldMarkers, XmlTextReader reader)
        {
            if (reader.Value != null)
            {
                string comment = reader.Value.Replace("\r\n", "\n");
                string[] lines = comment.Split('\n');
                if (lines.Length > 1)
                {

                    // Take off 5 chars to get the actual comment start (takes
                    // into account the <!-- chars.
                    int startCol = reader.LinePosition - 5;
                    int startLine = reader.LineNumber - 1;

                    // Add 3 to the end col value to take into account the '-->'
                    int endCol = lines[lines.Length - 1].Length + startCol + 3;
                    int endLine = startLine + lines.Length - 1;
                    string foldText = string.Concat("<!--", lines[0], "-->");
                    var foldMarker = new FoldMarker(document, startLine, startCol, endLine, endCol, FoldType.TypeBody, foldText);
                    foldMarkers.Add(foldMarker);
                }
            }
        }

        /// <summary>
        /// Create an element fold if the start and end tag are on 
        /// different lines.
        /// </summary>
        void CreateElementFold(IDocument document, List<FoldMarker> foldMarkers, XmlTextReader reader, XmlFoldStart foldStart)
        {
            int endLine = reader.LineNumber - 1;
            if (endLine > foldStart.Line)
            {
                int endCol = reader.LinePosition + foldStart.Name.Length;
                var foldMarker = new FoldMarker(document, foldStart.Line, foldStart.Column, endLine, endCol, FoldType.TypeBody, foldStart.FoldText);
                foldMarkers.Add(foldMarker);
            }
        }

        /// <summary>
        /// Creates an XmlFoldStart for the start tag of an element.
        /// </summary>
        XmlFoldStart CreateElementFoldStart(XmlTextReader reader)
        {
            // Take off 2 from the line position returned
            // from the xml since it points to the start
            // of the element name and not the beginning
            // tag.
            var newFoldStart = new XmlFoldStart(reader.Prefix, reader.LocalName, reader.LineNumber - 1, reader.LinePosition - 2);

            if (ShowAttributesWhenFolded && reader.HasAttributes)
            {
                newFoldStart.FoldText = string.Concat("<", newFoldStart.Name, " ", GetAttributeFoldText(reader), ">");
            }
            else
            {
                newFoldStart.FoldText = string.Concat("<", newFoldStart.Name, ">");
            }

            return newFoldStart;
        }

        /// <summary>
        /// Gets the element's attributes as a string on one line that will
        /// be displayed when the element is folded.
        /// </summary>
        /// <remarks>
        /// Currently this puts all attributes from an element on the same
        /// line of the start tag.  It does not cater for elements where attributes
        /// are not on the same line as the start tag.
        /// </remarks>
        string GetAttributeFoldText(XmlTextReader reader)
        {
            var text = new StringBuilder();

            for (int i = 0; i < reader.AttributeCount; ++i)
            {
                reader.MoveToAttribute(i);

                text.Append(reader.Name);
                text.Append("=");
                text.Append(reader.QuoteChar.ToString(CultureInfo.InvariantCulture));
                text.Append(XmlEncodeAttributeValue(reader.Value, reader.QuoteChar));
                text.Append(reader.QuoteChar.ToString(CultureInfo.InvariantCulture));

                // Append a space if this is not the
                // last attribute.
                if (i < reader.AttributeCount - 1)
                {
                    text.Append(" ");
                }
            }

            return text.ToString();
        }

        #endregion Methods
    }
}