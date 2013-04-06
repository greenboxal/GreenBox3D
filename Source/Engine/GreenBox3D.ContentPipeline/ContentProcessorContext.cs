using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GreenBox3D.ContentPipeline.CompilerServices;

namespace GreenBox3D.ContentPipeline
{
    public sealed class ContentProcessorContext
    {
        private BuildCoordinator _coordinator;
        private BuildCacheEntry _entry;

        public string BuildConfiguration { get; private set; }
        public string IntermediateDirectory { get; private set; }
        public string OutputDirectory { get; private set; }
        public string OutputFilename { get; private set; }
        public BuildParameters Parameters { get; private set; }
        public ILoggerHelper Logger { get; private set; }

        internal ContentProcessorContext(BuildCoordinator coordinator, BuildCacheEntry entry, BuildParameters parameters, string filename)
        {
            _entry = entry;
            _coordinator = coordinator;

            BuildConfiguration = coordinator.Settings.BuildConfiguration;
            IntermediateDirectory = coordinator.Settings.IntermediateDirectory;
            OutputDirectory = coordinator.Settings.OutputDirectory;
            OutputFilename = filename;
            Parameters = parameters;
            Logger = coordinator.Logger;
        }

        public void AddDependency(string filename)
        {
            _coordinator.AddDependency(_entry, filename);
        }
    }
}
