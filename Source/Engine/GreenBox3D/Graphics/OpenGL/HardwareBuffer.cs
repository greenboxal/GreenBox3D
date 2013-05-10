// HardwareBuffer.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions
#if DESKTOP

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace GreenBox3D.Graphics
{
    public abstract class HardwareBuffer : GraphicsResource
    {
        private bool _isLocked;

        internal int BufferID;
        internal BufferTarget BufferTarget;
        internal BufferUsageHint BufferUsageHint;
        
        public BufferUsage BufferUsage { get; private set; }
        public int ElementCount { get; private set; }

        internal protected HardwareBuffer(BufferTarget bufferTarget, BufferUsage usage)
        {
            BufferID = GL.GenBuffer();
            BufferUsage = usage;
            BufferTarget = bufferTarget;

            switch (BufferUsage)
            {
                case BufferUsage.DynamicCopy:
                    BufferUsageHint = BufferUsageHint.DynamicCopy;
                    break;
                case BufferUsage.DynamicDraw:
                    BufferUsageHint = BufferUsageHint.DynamicDraw;
                    break;
                case BufferUsage.DynamicRead:
                    BufferUsageHint = BufferUsageHint.DynamicRead;
                    break;
                case BufferUsage.StaticCopy:
                    BufferUsageHint = BufferUsageHint.StaticCopy;
                    break;
                case BufferUsage.StaticDraw:
                    BufferUsageHint = BufferUsageHint.StaticDraw;
                    break;
                case BufferUsage.StaticRead:
                    BufferUsageHint = BufferUsageHint.StaticRead;
                    break;
                case BufferUsage.StreamCopy:
                    BufferUsageHint = BufferUsageHint.StreamCopy;
                    break;
                case BufferUsage.StreamDraw:
                    BufferUsageHint = BufferUsageHint.StreamDraw;
                    break;
                case BufferUsage.StreamRead:
                    BufferUsageHint = BufferUsageHint.StreamRead;
                    break;
            }
        }

        public void SetData<T>(T[] data) where T : struct
        {
            SetData(data, 0, data.Length);
        }

        public void SetData<T>(T[] data, int offset, int count) where T : struct
        {
            if (data == null)
            {
                Bind();
                GL.BufferData(BufferTarget, (IntPtr)(count * Marshal.SizeOf(typeof(T))), IntPtr.Zero, BufferUsageHint);
                return;
            }

            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            IntPtr address = Marshal.UnsafeAddrOfPinnedArrayElement(data, offset);
            
            Bind();
            GL.BufferData(BufferTarget, (IntPtr)(count * Marshal.SizeOf(typeof(T))), address, BufferUsageHint);

            handle.Free();
            ElementCount = count;
        }

        public void SetData<T>(int offsetInBytes, T[] data, int offset, int count) where T : struct
        {
            if (data == null)
                throw new ArgumentNullException("data");

            GCHandle handle = GCHandle.Alloc(data);
            IntPtr address = Marshal.UnsafeAddrOfPinnedArrayElement(data, offset);

            Bind();
            GL.BufferSubData(BufferTarget, (IntPtr)offsetInBytes, (IntPtr)(count * Marshal.SizeOf(typeof(T))), address);

            handle.Free();
        }

        public IntPtr LockBits(BufferAccess access)
        {
            if (_isLocked)
                throw new InvalidOperationException("This buffer is already locked elsewhere");

            _isLocked = true;

            Bind();
            IntPtr address = GL.MapBuffer(BufferTarget, GLUtils.GetBufferAccess(access));
            Unbind();

            return address;
        }

        public void UnlockBits()
        {
            if (!_isLocked)
                return;

            Bind();
            GL.UnmapBuffer(BufferTarget);
            Unbind();

            _isLocked = false;
        }

        public virtual void Bind()
        {
            GL.BindBuffer(BufferTarget, BufferID);
        }

        public virtual void Unbind()
        {
            GL.BindBuffer(BufferTarget, 0);
        }

        protected override void Dispose(bool disposing)
        {
            if (BufferID != 0)
            {
                GL.DeleteBuffer(BufferID);
                BufferID = 0;
            }

            base.Dispose(disposing);
        }
    }
}

#endif
