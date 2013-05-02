// ContentTypeWriter.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.Content;
using GreenBox3D.ContentPipeline.CompilerServices;
using GreenBox3D.ContentPipeline.Writers;
using GreenBox3D.Utilities;

namespace GreenBox3D.ContentPipeline
{
    public abstract class ContentTypeWriter<TInput> : IContentTypeWriter
    {
        #region Explicit Interface Methods

        void IContentTypeWriter.Write(BuildCoordinator coordinator, Stream stream, object input)
        {
            ContentHeader header = GetHeader((TInput)input);
            BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, true);
            bool cenc = !header.Encoding.Equals(Encoding.UTF8);
            bool compress = ShouldCompressContent((TInput)input);
            byte flags = 0;

            if (cenc)
                flags |= 1;

            if (compress)
                flags |= 2;

            writer.Write(flags);
            writer.Write((byte)header.Magic.Length);
            writer.Write(Encoding.UTF8.GetBytes(header.Magic));
            writer.Write(header.Version.Major);
            writer.Write(header.Version.Minor);

            if (cenc)
                writer.Write(header.Encoding.CodePage);

            MemoryStream ms = new MemoryStream();
            Stream s = ms;

            if (compress)
                s = new DeflateStream(ms, CompressionMode.Compress, true);

            var cw = new ContentWriter(coordinator, s, header.Encoding);
            Write(cw, (TInput)input);
            cw.Close();

            if (s != ms)
                s.Close();

            ContentCrc32 crc32 = new ContentCrc32();
            ms.Position = 0;
            crc32.ComputeHash(ms);
            writer.Write(crc32.CrcValue);
            writer.Write(ms.Length);
            writer.Close();

            ms.Position = 0;
            ms.CopyTo(stream);
            ms.Close();
        }

        #endregion

        #region Methods

        protected abstract ContentHeader GetHeader(TInput input);
        protected abstract void Write(ContentWriter stream, TInput input);

        protected virtual bool ShouldCompressContent(TInput input)
        {
            return false;
        }

        #endregion
    }
}
