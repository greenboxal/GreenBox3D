// BitmapFontImporter.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GreenBox3D.ContentPipeline.Importers
{
    [ContentImporter(".bmfont", DisplayName = "Bitmap Font Importer", DefaultProcessor = "BitmapFontProcessor")]
    public class BitmapFontImporter : ContentImporter<XmlDocument>
    {
        protected override XmlDocument Import(string filename, ContentImporterContext context)
        {
            XmlDocument doc = new XmlDocument();

            doc.Load(filename);

            return doc;
        }
    }
}
