// BuildCoordinator.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.ContentPipeline.CompilerServices
{
    // FIXME: Deleted files will stay stuck in the file until a Rebuild is performed
    public class BuildCoordinator
    {
        private readonly BuildCache _cache;

        private readonly Dictionary<string, string> _extension2importer;
        private readonly Dictionary<string, ImporterDescriptor> _importers;
        private readonly ILoggerHelper _logger;
        private readonly BuildCache _newCache;
        private readonly Dictionary<string, ProcessorDescriptor> _processors;

        private readonly BuildCoordinatorSettings _settings;
        private readonly Dictionary<string, WriterDescriptor> _writers;

        public BuildCoordinator(BuildCoordinatorSettings settings, ILoggerHelper logger)
        {
            _cache = new BuildCache();
            _newCache = new BuildCache();
            _settings = settings;
            _logger = logger;

            _extension2importer = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            _importers = new Dictionary<string, ImporterDescriptor>(StringComparer.InvariantCultureIgnoreCase);
            _processors = new Dictionary<string, ProcessorDescriptor>(StringComparer.InvariantCultureIgnoreCase);
            _writers = new Dictionary<string, WriterDescriptor>(StringComparer.InvariantCultureIgnoreCase);
        }

        public BuildCoordinatorSettings Settings
        {
            get { return _settings; }
        }

        public ILoggerHelper Logger
        {
            get { return _logger; }
        }

        public void LoadReference(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (Attribute.IsDefined(type, typeof(ContentImporterAttribute)))
                {
                    ContentImporterAttribute attribute = type.GetCustomAttribute<ContentImporterAttribute>();

                    _importers.Add(type.Name, new ImporterDescriptor
                    {
                        Type = type,
                        DefaultProcessor = attribute.DefaultProcessor
                    });

                    foreach (string ext in attribute.Extensions)
                    {
                        if (!_extension2importer.ContainsKey(ext))
                            _extension2importer[ext] = type.Name;
                    }
                }
                else if (Attribute.IsDefined(type, typeof(ContentProcessorAttribute)))
                {
                    ContentProcessorAttribute attribute = type.GetCustomAttribute<ContentProcessorAttribute>();

                    _processors.Add(type.Name, new ProcessorDescriptor
                    {
                        Type = type,
                        Writer = attribute.Writer
                    });
                }
                else if (Attribute.IsDefined(type, typeof(ContentTypeWriterAttribute)))
                {
                    ContentTypeWriterAttribute attribute = type.GetCustomAttribute<ContentTypeWriterAttribute>();

                    _writers.Add(type.Name, new WriterDescriptor
                    {
                        Type = type,
                        Extension = attribute.Extension
                    });
                }
            }
        }

        public void StartBuild()
        {
            if (!_settings.RebuildAll)
                _cache.LoadFrom(Path.Combine(_settings.IntermediateDirectory, "PipelineBuildCache.cache"));

            _cache.ClearLastBuiltFlags();
            _newCache.Clear();
        }

        public void FinishBuild()
        {
            _newCache.Save(Path.Combine(_settings.IntermediateDirectory, "PipelineBuildCache.cache"));
        }

        public string[] GetOutputFiles()
        {
            List<string> files = new List<string>();

            foreach (BuildCacheEntry entry in _newCache)
                files.AddRange(entry.OutputFiles);

            return files.ToArray();
        }

        public bool RequestBuild(string partialName, BuildParameters parameters)
        {
            if (CanBuild(partialName))
            {
                BuildCacheEntry entry = new BuildCacheEntry(partialName);
                string fullPath = Path.Combine(_settings.BasePath, partialName);
                string extension = Path.GetExtension(partialName);
                string importerName = parameters.GetValue<string>("Importer");
                string processorName = parameters.GetValue<string>("Processor");

                if (importerName == null)
                    _extension2importer.TryGetValue(extension, out importerName);

                if (importerName == null)
                {
                    _logger.Log(MessageLevel.Error, "GB0001", fullPath, 0, 0, 0, 0,
                                "No default Content Importer defined for {0} files.", extension);
                    return false;
                }

                ImporterDescriptor importer = GetImporter(importerName);

                if (importer == null)
                {
                    _logger.Log(MessageLevel.Error, "GB0002", fullPath, 0, 0, 0, 0, "No such Content Importer: {0}",
                                importerName);
                    return false;
                }

                if (processorName == null)
                    processorName = importer.DefaultProcessor;

                if (processorName == null)
                {
                    _logger.Log(MessageLevel.Error, "GB0003", fullPath, 0, 0, 0, 0,
                                "No default processor defined for Content Importer {0}.", importer);
                    return false;
                }

                ProcessorDescriptor processor = GetProcessor(processorName);

                if (processor == null)
                {
                    _logger.Log(MessageLevel.Error, "GB0004", fullPath, 0, 0, 0, 0, "No such Content Processor: {0}",
                                processorName);
                    return false;
                }

                WriterDescriptor writer = GetWriter(processor.Writer);

                if (processor == null)
                {
                    _logger.Log(MessageLevel.Error, "GB0005", fullPath, 0, 0, 0, 0, "No such Content Type Writer: {0}",
                                processorName);
                    return false;
                }

                string outputFilename =
                    Path.Combine(_settings.OutputDirectory, Path.GetDirectoryName(partialName),
                                 Path.GetFileNameWithoutExtension(partialName)) + writer.Extension;
                object temporary = importer.CachedInstance.Import(fullPath, new ContentImporterContext(this, entry));

                if (temporary == null)
                    return false;

                temporary = processor.CachedInstance.Process(temporary,
                                                             new ContentProcessorContext(this, entry,
                                                                                         parameters
                                                                                             .GetValue<BuildParameters>(
                                                                                                 "ProcessorParameters") ??
                                                                                         new BuildParameters(),
                                                                                         outputFilename));

                if (temporary == null)
                    return false;

                FileStream stream;

                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(outputFilename));
                    stream = new FileStream(outputFilename, FileMode.Create);
                }
                catch (IOException ex)
                {
                    _logger.Log(MessageLevel.Error, "GB0006", fullPath, 0, 0, 0, 0, "Failed to open output file: {0}",
                                ex.Message);
                    return false;
                }

                writer.CachedInstance.Write(stream, temporary);
                stream.Close();

                entry.LastBuilt = true;
                entry.Timestamp = File.GetLastWriteTimeUtc(fullPath);
                entry.OutputFiles.Add(outputFilename);

                _newCache.Add(entry);
            }
            else
            {
                _newCache.Add(_cache.Query(partialName));
            }

            return true;
        }

        public void AddDependency(BuildCacheEntry entry, string filename)
        {
            if (Path.IsPathRooted(filename))
                filename = new Uri(_settings.BasePath).MakeRelativeUri(new Uri(filename)).ToString();

            string fullPath = Path.Combine(_settings.BasePath, filename);

            if (entry.Filename == filename)
                return;

            if (!File.Exists(fullPath))
                throw new InvalidOperationException("Dependency file doesn't exist");

            entry.Dependencies.Add(filename);

            if (!_newCache.IsCached(filename))
            {
                _newCache.Add(new BuildCacheEntry(filename)
                {
                    Timestamp = File.GetLastWriteTimeUtc(fullPath)
                });
            }
        }

        private ImporterDescriptor GetImporter(string name)
        {
            ImporterDescriptor descriptor;

            if (!_importers.TryGetValue(name, out descriptor))
                return null;

            if (descriptor.CachedInstance == null)
                descriptor.CachedInstance = (IContentImporter)Activator.CreateInstance(descriptor.Type);

            return descriptor;
        }

        private ProcessorDescriptor GetProcessor(string name)
        {
            ProcessorDescriptor descriptor;

            if (!_processors.TryGetValue(name, out descriptor))
                return null;

            if (descriptor.CachedInstance == null)
                descriptor.CachedInstance = (IContentProcessor)Activator.CreateInstance(descriptor.Type);

            return descriptor;
        }

        private WriterDescriptor GetWriter(string name)
        {
            WriterDescriptor descriptor;

            if (!_writers.TryGetValue(name, out descriptor))
                return null;

            if (descriptor.CachedInstance == null)
                descriptor.CachedInstance = (IContentTypeWriter)Activator.CreateInstance(descriptor.Type);

            return descriptor;
        }

        private bool CanBuild(string partialName)
        {
            BuildCacheEntry entry = _cache.Query(partialName);
            string fullPath = Path.Combine(_settings.BasePath, partialName);

            if (entry == null)
                return true;

            // Invalidade this cache entry if the base file doesn't exist
            if (!File.Exists(fullPath))
                return true;

            if (entry.Timestamp != File.GetLastWriteTimeUtc(fullPath))
                return true;

            foreach (string file in entry.OutputFiles)
                if (!File.Exists(Path.Combine(_settings.OutputDirectory, file)))
                    return true;

            foreach (string file in entry.Dependencies)
                if (CanBuild(file))
                    return true;

            return false;
        }

        public void CleanAll()
        {
            if (_cache != null ||
                _cache.LoadFrom(Path.Combine(_settings.IntermediateDirectory, "PipelineBuildCache.cache")))
            {
                foreach (BuildCacheEntry entry in _cache)
                {
                    foreach (string file in entry.OutputFiles)
                    {
                        string path = Path.Combine(_settings.OutputDirectory, file);

                        if (File.Exists(path))
                            File.Delete(path);
                    }
                }

                File.Delete(Path.Combine(_settings.IntermediateDirectory, "PipelineBuildCache.cache"));
            }
        }

        public string[] GetLastBuiltFiles()
        {
            List<string> files = new List<string>();

            if (_cache != null ||
                _cache.LoadFrom(Path.Combine(_settings.IntermediateDirectory, "PipelineBuildCache.cache")))
                foreach (BuildCacheEntry entry in _cache)
                    if (entry.LastBuilt)
                        foreach (string file in entry.OutputFiles)
                            files.Add(file);

            return files.ToArray();
        }

        private class ImporterDescriptor
        {
            public IContentImporter CachedInstance;
            public string DefaultProcessor;
            public Type Type;
        }

        private class ProcessorDescriptor
        {
            public IContentProcessor CachedInstance;
            public Type Type;
            public string Writer;
        }

        private class WriterDescriptor
        {
            public IContentTypeWriter CachedInstance;
            public string Extension;
            public Type Type;
        }
    }
}
