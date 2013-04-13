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
    public class GetLastOutputs : Microsoft.Build.Utilities.Task
    {
        public string IntermediateDirectory { get; set; }

        [Output]
        public ITaskItem[] OutputContentFiles { get; set; }

        public override bool Execute()
        {
            // We use this isolation to permit developers to rebuild their pipeline libraries without getting locked by MSBuild AppDomain
            using (IsolationAppDomain isolation = new IsolationAppDomain())
            {
                string[] files;

                isolation.CreateProxy<IsolationProxy>().Execute(Log, IntermediateDirectory, out files);

                OutputContentFiles = Array.ConvertAll(files, new Converter<string, ITaskItem>((s) => new TaskItem(s)));
            }

            return true;
        }

        private class IsolationProxy : MarshalByRefObject
        {
            public bool Execute(TaskLoggingHelper log, string intermediateDirectory, out string[] files)
            {
                BuildCoordinatorSettings settings = new BuildCoordinatorSettings()
                {
                    IntermediateDirectory = intermediateDirectory,
                };

                BuildCoordinator build = new BuildCoordinator(settings, new MSBuildLoggerHelper(log));

                files = build.GetLastBuiltFiles();

                return true;
            }
        }
    }
}
