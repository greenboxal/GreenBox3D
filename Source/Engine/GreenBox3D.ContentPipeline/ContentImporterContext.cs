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
        private BuildCoordinator _coordinator;
        private BuildCacheEntry _entry;

        public string IntermediateDirectory { get; private set; }
        public string OutputDirectory { get; private set; }
        public ILoggerHelper Logger { get; private set; }

        internal ContentImporterContext(BuildCoordinator coordinator, BuildCacheEntry entry)
        {
            _entry = entry;
            _coordinator = coordinator;

            Logger = coordinator.Logger;
            IntermediateDirectory = coordinator.Settings.IntermediateDirectory;
            OutputDirectory = coordinator.Settings.OutputDirectory;
        }

        public void AddDependency(string filename)
        {
            _coordinator.AddDependency(_entry, filename);
        }
    }
}
