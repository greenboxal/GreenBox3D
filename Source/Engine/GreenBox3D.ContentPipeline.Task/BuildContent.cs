// BuildContent.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

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
    public class BuildContent : Microsoft.Build.Utilities.Task
    {
        [Output]
        public ITaskItem[] OutputContentFiles { get; set; }

        [Required]
        public ITaskItem[] SourceAssets { get; set; }

        [Required]
        public ITaskItem[] PipelineAssemblies { get; set; }

        public ITaskItem[] PipelineAssemblyDependencies { get; set; }

        public string BuildConfiguration { get; set; }

        public string IntermediateDirectory { get; set; }

        public string OutputDirectory { get; set; }

        public string RootDirectory { get; set; }

        public override bool Execute()
        {
            // We use this isolation to permit developers to rebuild their pipeline libraries without getting locked by MSBuild AppDomain
            using (IsolationAppDomain isolation = new IsolationAppDomain())
            {
                string[] files;
                bool result = isolation.CreateProxy<IsolationProxy>()
                                .Execute(Log, SourceAssets, PipelineAssemblies, BuildConfiguration,
                                         IntermediateDirectory, OutputDirectory, RootDirectory, out files);

                if (!result)
                    return false;

                OutputContentFiles = new ITaskItem[files.Length];
                for (int i = 0; i < files.Length; i++)
                    OutputContentFiles[i] = new TaskItem(files[i]);

                return true;
            }
        }

        private class IsolationProxy : MarshalByRefObject
        {
            public bool Execute(TaskLoggingHelper log, ITaskItem[] sourceAssets, ITaskItem[] pipelineAssemblies,
                                string buildConfiguration, string intermediateDirectory, string outputDirectory,
                                string rootDirectory, out string[] files)
            {
                BuildCoordinatorSettings settings = new BuildCoordinatorSettings
                {
                    BuildConfiguration = buildConfiguration,
                    IntermediateDirectory = intermediateDirectory,
                    OutputDirectory = outputDirectory,
                    BasePath = rootDirectory
                };

                BuildCoordinator build = new BuildCoordinator(settings, new MSBuildLoggerHelper(log));

                if (pipelineAssemblies != null)
                    foreach (ITaskItem item in pipelineAssemblies)
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

                files = build.GetBuiltFiles();
                build.FinishBuild();

                return result;
            }
        }
    }
}
