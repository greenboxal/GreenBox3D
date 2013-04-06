using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D
{
    public abstract class ConfigurationProvider
    {
        public abstract string Get(string key);
        public abstract string Set(string key);
        public abstract string Remove(string key);
    }
}
