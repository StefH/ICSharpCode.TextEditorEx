﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WinFormTestXmlEditor.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("WinFormTestXmlEditor.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to using System;
        ///using System.Windows.Forms;
        ///
        ///namespace WinFormTestXmlEditor
        ///{
        ///    static class Program
        ///    {
        ///        /// &lt;summary&gt;
        ///        /// The main entry point for the application.
        ///        /// &lt;/summary&gt;
        ///        [STAThread]
        ///        static void Main()
        ///        {
        ///#if !NET40
        ///            Application.SetHighDpiMode(HighDpiMode.SystemAware);
        ///#endif
        ///            Application.EnableVisualStyles();
        ///            Application.SetCompatibleTextRenderingDefault(false);
        ///            Application.Run(new Main [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ExampleCSharp {
            get {
                return ResourceManager.GetString("ExampleCSharp", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to open System
        ///
        ////// A sample function
        ///let hello (x:string) = &quot;Hello &quot; + x
        ///
        ///printfn &quot;%s&quot; (hello &quot;world&quot;)
        ///.
        /// </summary>
        internal static string ExampleFSharp {
            get {
                return ResourceManager.GetString("ExampleFSharp", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot; ?&gt;
        ///
        ///&lt;!-- comments --&gt;
        ///&lt;rootNode xmlns:namespace=&quot;http://www.w3.org/TR/html4/&quot;&gt;
        ///  &lt;number&gt;1234&lt;/number&gt;
        ///  &lt;number&gt;1234-aaaa&lt;/number&gt;
        ///  &lt;number&gt;1234-aaaa-5678&lt;/number&gt;
        ///	&lt;childNodes&gt;
        ///		&lt;childNode attribute1 = &quot;value&quot;
        ///               namespace:attribute2=&apos;value&apos;
        ///               attribute3=&apos;&apos;/&gt;
        ///		&lt;childNode /&gt;
        ///		&lt;childNode /&gt;
        ///		&lt;childNode /&gt;
        ///		&lt;childNode
        ///      attr1=&quot;value&quot;
        ///      attr2=&quot;10&quot;
        ///      attr3=&quot;hello&quot;
        ///          &gt;
        ///      value
        ///		&lt;/childNode&gt;
        ///
        ///		&lt;chi [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ExampleXML {
            get {
                return ResourceManager.GetString("ExampleXML", resourceCulture);
            }
        }
    }
}
