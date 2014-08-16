using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.TextEditor.Src.Document.HighlightingStrategy.SyntaxModes
{
    public class ResourceSyntaxModeProviderEx : ISyntaxModeFileProvider
    {
        private const string ResourcesDir = "ICSharpCode.TextEditor.Resources.";

        readonly List<SyntaxMode> _syntaxModes;

        public ICollection<SyntaxMode> SyntaxModes
        {
            get
            {
                return _syntaxModes;
            }
        }

        public ResourceSyntaxModeProviderEx()
        {
            var syntaxModeStream = GetSyntaxModeStream("SyntaxModesEx.xml");

            _syntaxModes = syntaxModeStream != null ? SyntaxMode.GetSyntaxModes(syntaxModeStream) : new List<SyntaxMode>();
        }

        public XmlTextReader GetSyntaxModeFile(SyntaxMode syntaxMode)
        {
            var stream = GetSyntaxModeStream(syntaxMode.FileName);

            return stream != null ? new XmlTextReader(stream) : null;
        }

        public void UpdateSyntaxModeList()
        {
            // resources don't change during runtime
        }

        private Stream GetSyntaxModeStream(string filename)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream(string.Format("{0}{1}", ResourcesDir, filename));
        }
    }
}
