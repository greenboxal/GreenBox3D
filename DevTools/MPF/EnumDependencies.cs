// EnumDependencies.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudio.Project
{
    [CLSCompliant(false)]
    public class EnumDependencies : IVsEnumDependencies
    {
        private readonly List<IVsDependency> dependencyList = new List<IVsDependency>();

        private uint nextIndex;

        public EnumDependencies(IList<IVsDependency> dependencyList)
        {
            if (dependencyList == null)
            {
                throw new ArgumentNullException("dependencyList");
            }

            foreach (IVsDependency dependency in dependencyList)
            {
                this.dependencyList.Add(dependency);
            }
        }

        public EnumDependencies(IList<IVsBuildDependency> dependencyList)
        {
            if (dependencyList == null)
            {
                throw new ArgumentNullException("dependencyList");
            }

            foreach (IVsBuildDependency dependency in dependencyList)
            {
                this.dependencyList.Add(dependency);
            }
        }

        public int Clone(out IVsEnumDependencies enumDependencies)
        {
            enumDependencies = new EnumDependencies(dependencyList);
            ErrorHandler.ThrowOnFailure(enumDependencies.Skip(nextIndex));
            return VSConstants.S_OK;
        }

        public int Next(uint elements, IVsDependency[] dependencies, out uint elementsFetched)
        {
            elementsFetched = 0;
            if (dependencies == null)
            {
                throw new ArgumentNullException("dependencies");
            }

            uint fetched = 0;
            int count = dependencyList.Count;

            while (nextIndex < count && elements > 0 && fetched < count)
            {
                dependencies[fetched] = dependencyList[(int)nextIndex];
                nextIndex++;
                fetched++;
                elements--;
            }

            elementsFetched = fetched;

            // Did we get 'em all?
            return (elements == 0 ? VSConstants.S_OK : VSConstants.S_FALSE);
        }

        public int Reset()
        {
            nextIndex = 0;
            return VSConstants.S_OK;
        }

        public int Skip(uint elements)
        {
            nextIndex += elements;
            uint count = (uint)dependencyList.Count;

            if (nextIndex > count)
            {
                nextIndex = count;
                return VSConstants.S_FALSE;
            }

            return VSConstants.S_OK;
        }
    }
}
