// ContentProcessorContext.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.ContentPipeline.CompilerServices;

namespace GreenBox3D.ContentPipeline
{
    public sealed class ContentProcessorContext
    {
        private readonly BuildCoordinator _coordinator;
        private readonly BuildCacheEntry _entry;

        internal ContentProcessorContext(BuildCoordinator coordinator, BuildCacheEntry entry, BuildParameters parameters,
                                         string filename, string inputFilename)
        {
            _entry = entry;
            _coordinator = coordinator;

            BuildConfiguration = coordinator.Settings.BuildConfiguration;
            IntermediateDirectory = coordinator.Settings.IntermediateDirectory;
            InputFilename = inputFilename;
            OutputDirectory = coordinator.Settings.OutputDirectory;
            OutputFilename = filename;
            Parameters = parameters;
            Logger = coordinator.Logger;
        }

        public string BuildConfiguration { get; private set; }
        public string IntermediateDirectory { get; private set; }
        public string InputFilename { get; private set; }
        public string OutputDirectory { get; private set; }
        public string OutputFilename { get; private set; }
        public BuildParameters Parameters { get; private set; }
        public ILoggerHelper Logger { get; private set; }

        public void AddDependency(string filename)
        {
            _coordinator.AddDependency(_entry, filename);
        }

        public string ResolveRelativePath(string path)
        {
            if (!Path.IsPathRooted(path))
                path = Path.Combine(Path.GetDirectoryName(InputFilename), path);

            return _coordinator.ResolveRelativePath(Path.GetFullPath(path));
        }

        public T BuildAndLoadAsset<T>(string path, string processor = null, BuildParameters parameters = null,
                                      string importer = null)
        {
            return (T)_coordinator.BuildAndLoadAsset(path, processor, parameters ?? new BuildParameters(), importer);
        }
    }
}
