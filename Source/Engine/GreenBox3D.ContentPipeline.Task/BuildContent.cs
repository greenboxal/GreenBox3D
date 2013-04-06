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
    [LoadInSeparateAppDomain]
    public class BuildContent : AppDomainIsolatedTask
    {
        private ITaskItem[] _outputFiles;

        [Required]
        public ITaskItem[] SourceAssets { get; set; }

        [Required]
        public ITaskItem[] PipelineAssemblies { get; set; }

        public ITaskItem[] PipelineAssemblyDependencies { get; set; }

        public string BuildConfiguration { get; set; }

        public string IntermediateDirectory { get; set; }

        public string OutputDirectory { get; set; }

        public bool RebuildAll { get; set; }

        [Output]
        public ITaskItem[] OutputContentFiles { get { return _outputFiles; } }

        public string RootDirectory { get; set; }

        public override bool Execute()
        {
            // We use this isolation to permit developers to rebuild their pipeline libraries without getting locked by MSBuild AppDomain
            //using (IsolationAppDomain isolation = new IsolationAppDomain())
            //{
            IsolationProxy proxy = new IsolationProxy();// isolation.CreateProxy<IsolationProxy>();

                BuildCoordinatorSettings settings = new BuildCoordinatorSettings()
                {
                    BuildConfiguration = BuildConfiguration,
                    IntermediateDirectory = IntermediateDirectory,
                    OutputDirectory = OutputDirectory,
                    BasePath = RootDirectory,
                    RebuildAll = RebuildAll
                };

                return proxy.Execute(Log, settings, SourceAssets, PipelineAssemblies, out _outputFiles);
            //}
        }

        private class IsolationProxy : MarshalByRefObject
        {
            public bool Execute(TaskLoggingHelper log, BuildCoordinatorSettings settings, ITaskItem[] sourceAssets, ITaskItem[] references, out ITaskItem[] outputFiles)
            {
                BuildCoordinator build = new BuildCoordinator(settings, new MSBuildLoggerHelper(log));

                if (references != null)
                    foreach (ITaskItem item in references)
                        build.LoadReference(Assembly.Load(AssemblyName.GetAssemblyName(item.ItemSpec)));

                build.StartBuild();

                bool result = true;
                foreach (ITaskItem item in sourceAssets)
                {
                    BuildParameters parameters = new BuildParameters();

                    foreach (string name in item.MetadataNames)
                    {
                        if (name == "ProcessorParameters")
                        {
                            string xml = item.GetMetadata(name);
                            BuildParameters args = new BuildParameters();
                            XmlDocument doc = new XmlDocument();

                            try
                            {
                                doc.LoadXml(xml);
                            }
                            catch (XmlException ex)
                            {
                                log.LogWarningFromException(ex);
                                continue;
                            }

                            foreach (XmlNode node in doc)
                                args.Add(node.Name, node.InnerXml);

                            parameters.Add(name, args);
                        }
                        else
                        {
                            parameters.Add(name, item.GetMetadata(name));
                        }
                    }

                    if (!build.RequestBuild(item.ItemSpec, parameters))
                        result = false;
                }

                outputFiles = Array.ConvertAll<string, ITaskItem>(build.GetOutputFiles(), (s) => new TaskItem(s));

                build.FinishBuild();
                return result;
            }
        }
    }
}
