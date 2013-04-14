// ProjectOptions.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System.CodeDom.Compiler;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

namespace Microsoft.VisualStudio.Project
{
    public class ProjectOptions : CompilerParameters
    {
        public ProjectOptions()
        {
            EmitManifest = true;
            ModuleKind = ModuleKindFlags.ConsoleApplication;
        }

        public string Config { get; set; }

        public ModuleKindFlags ModuleKind { get; set; }

        public bool EmitManifest { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public StringCollection DefinedPreprocessorSymbols { get; set; }

        public string XmlDocFileName { get; set; }

        public string RecursiveWildcard { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public StringCollection ReferencedModules { get; set; }

        public string Win32Icon { get; set; }

        public bool PdbOnly { get; set; }

        public bool Optimize { get; set; }

        public bool IncrementalCompile { get; set; }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public int[] SuppressedWarnings { get; set; }

        public bool CheckedArithmetic { get; set; }

        public bool AllowUnsafeCode { get; set; }

        public bool DisplayCommandLineHelp { get; set; }

        public bool SuppressLogo { get; set; }

        public long BaseAddress { get; set; }

        public string BugReportFileName { get; set; }

        /// <devdoc>must be an int if not null</devdoc>
        public object CodePage { get; set; }

        public bool EncodeOutputInUtf8 { get; set; }

        public bool FullyQualifyPaths { get; set; }

        public int FileAlignment { get; set; }

        public bool NoStandardLibrary { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public StringCollection AdditionalSearchPaths { get; set; }

        public bool HeuristicReferenceResolution { get; set; }

        public string RootNamespace { get; set; }

        public bool CompileAndExecute { get; set; }

        /// <devdoc>must be an int if not null.</devdoc>
        public object UserLocaleId { get; set; }

        public FrameworkName TargetFrameworkMoniker { get; set; }

        public virtual string GetOptionHelp()
        {
            return null;
        }
    }
}
