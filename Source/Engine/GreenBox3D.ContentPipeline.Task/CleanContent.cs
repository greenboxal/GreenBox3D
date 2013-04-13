using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using GreenBox3D.ContentPipeline.CompilerServices;

namespace GreenBox3D.ContentPipeline.Task
{
    public class CleanContent : Microsoft.Build.Utilities.Task
    {
        public string IntermediateDirectory { get; set; }

        public string OutputDirectory { get; set; }

        public override bool Execute()
        {
            // We use this isolation to permit developers to rebuild their pipeline libraries without getting locked by MSBuild AppDomain
            using (IsolationAppDomain isolation = new IsolationAppDomain())
                return isolation.CreateProxy<IsolationProxy>().Execute(Log, IntermediateDirectory, OutputDirectory);
        }

        private class IsolationProxy : MarshalByRefObject
        {
            public bool Execute(TaskLoggingHelper log, string intermediateDirectory, string outputDirectory)
            {
                BuildCoordinatorSettings settings = new BuildCoordinatorSettings()
                {
                    IntermediateDirectory = intermediateDirectory,
                    OutputDirectory = outputDirectory,
                };

                BuildCoordinator build = new BuildCoordinator(settings, new MSBuildLoggerHelper(log));

                build.CleanAll();

                return true;
            }
        }
    }
}
