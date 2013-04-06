using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.ContentPipeline.CompilerServices
{
    public class BuildCache : IEnumerable<BuildCacheEntry>
    {
        private Dictionary<string, BuildCacheEntry> _entries;

        public BuildCache()
        {
            _entries = new Dictionary<string, BuildCacheEntry>(StringComparer.InvariantCultureIgnoreCase);
        }

        public void Add(BuildCacheEntry entry)
        {
            _entries[entry.Filename] = entry;
        }

        public BuildCacheEntry Query(string filename)
        {
            BuildCacheEntry entry;
            _entries.TryGetValue(filename, out entry);
            return entry;
        }

        public bool IsCached(string filename)
        {
            return _entries.ContainsKey(filename);
        }

        public void Remove(string filename)
        {
            _entries.Remove(filename);
        }

        public void Clear()
        {
            _entries.Clear();
        }

        public void ClearLastBuiltFlags()
        {
            foreach (BuildCacheEntry entry in _entries.Values)
                entry.LastBuilt = false;
        }

        public bool LoadFrom(string file, bool onlyLastBuilt = false)
        {
            if (!File.Exists(file))
                return false;

            using (FileStream fs = new FileStream(file, FileMode.Open))
            {
                try
                {
                    BinaryReader br = new BinaryReader(fs, Encoding.UTF8, true);

                    if (br.ReadString() != "GB3DC")
                        return false;

                    int count = br.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        BuildCacheEntry entry = new BuildCacheEntry(br.ReadString());
                        
                        entry.LastBuilt = br.ReadBoolean();
                        entry.Timestamp = DateTime.FromBinary(br.ReadInt64());

                        int depCount = br.ReadInt32();
                        int outputCount = br.ReadInt32();

                        for (int n = 0; n < depCount; n++)
                            entry.Dependencies.Add(br.ReadString());

                        for (int n = 0; n < outputCount; n++)
                            entry.OutputFiles.Add(br.ReadString());

                        Add(entry);
                    }
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        public void Save(string file)
        {
            using (FileStream fs = new FileStream(file, FileMode.Create))
            {
                BinaryWriter bw = new BinaryWriter(fs, Encoding.UTF8, true);

                bw.Write("GB3DC");
                bw.Write(_entries.Count);

                foreach (KeyValuePair<string, BuildCacheEntry> entry in _entries)
                {
                    bw.Write(entry.Key);
                    bw.Write(entry.Value.LastBuilt);
                    bw.Write(entry.Value.Timestamp.ToBinary());
                    bw.Write(entry.Value.Dependencies.Count);
                    bw.Write(entry.Value.OutputFiles.Count);

                    foreach (string dep in entry.Value.Dependencies)
                        bw.Write(dep);

                    foreach (string outfile in entry.Value.OutputFiles)
                        bw.Write(outfile);
                }
            }
        }

        public IEnumerator<BuildCacheEntry> GetEnumerator()
        {
            foreach (BuildCacheEntry entry in _entries.Values)
                yield return entry;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
