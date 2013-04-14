// ContentImporterContext.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using GreenBox3D.ContentPipeline.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.ContentPipeline
{
    public sealed class ContentImporterContext
    {
        private readonly BuildCoordinator _coordinator;
        private readonly BuildCacheEntry _entry;

        internal ContentImporterContext(BuildCoordinator coordinator, BuildCacheEntry entry)
        {
            _entry = entry;
            _coordinator = coordinator;

            Logger = coordinator.Logger;
            IntermediateDirectory = coordinator.Settings.IntermediateDirectory;
            OutputDirectory = coordinator.Settings.OutputDirectory;
        }

        public string IntermediateDirectory { get; private set; }
        public string OutputDirectory { get; private set; }
        public ILoggerHelper Logger { get; private set; }

        public void AddDependency(string filename)
        {
            _coordinator.AddDependency(_entry, filename);
        }
    }
}
