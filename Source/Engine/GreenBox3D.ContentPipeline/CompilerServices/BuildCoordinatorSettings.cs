using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.ContentPipeline.CompilerServices
{
    [Serializable]
    public class BuildCoordinatorSettings
    {
        public string BuildConfiguration { get; set; }
        public string BasePath { get; set; }
        public string OutputDirectory { get; set; }
        public string IntermediateDirectory { get; set; }
        public bool RebuildAll { get; set; }
    }
}
