// Tracing.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Diagnostics;
using System.Reflection;

namespace Microsoft.VisualStudio.Project
{
    internal class CCITracing
    {
        private CCITracing()
        {
        }

        [Conditional("Enable_CCIDiagnostics")]
        private static void InternalTraceCall(int levels)
        {
            StackFrame stack;
            stack = new StackFrame(levels);
            MethodBase method = stack.GetMethod();
            if (method != null)
            {
                string name = method.Name + " \tin class " + method.DeclaringType.Name;
                System.Diagnostics.Trace.WriteLine("Call Trace: \t" + name);
            }
        }

        [Conditional("CCI_TRACING")]
        public static void TraceCall()
        {
            // skip this one as well
            InternalTraceCall(2);
        }

        [Conditional("CCI_TRACING")]
        public static void TraceCall(string strParameters)
        {
            InternalTraceCall(2);
            System.Diagnostics.Trace.WriteLine("\tParameters: \t" + strParameters);
        }

        [Conditional("CCI_TRACING")]
        public static void Trace(Exception e)
        {
            InternalTraceCall(2);
            System.Diagnostics.Trace.WriteLine("ExceptionInfo: \t" + e);
        }

        [Conditional("CCI_TRACING")]
        public static void Trace(string strOutput)
        {
            System.Diagnostics.Trace.WriteLine(strOutput);
        }

        [Conditional("CCI_TRACING")]
        public static void TraceData(string strOutput)
        {
            System.Diagnostics.Trace.WriteLine("Data Trace: \t" + strOutput);
        }

        [Conditional("Enable_CCIFileOutput")]
        [Conditional("CCI_TRACING")]
        public static void AddTraceLog(string strFileName)
        {
            TextWriterTraceListener tw = new TextWriterTraceListener("c:\\mytrace.log");
            System.Diagnostics.Trace.Listeners.Add(tw);
        }
    }
}
