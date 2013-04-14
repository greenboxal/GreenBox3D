// ShaderImporter.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.ContentPipeline.CompilerServices;
using GreenBox3D.ContentPipeline.Graphics.Shading;
using Ast = GreenBox3D.ContentPipeline.Graphics.Shading.Ast;

namespace GreenBox3D.ContentPipeline.Importers
{
    [ContentImporter(".shader", DisplayName = "Shader Importer", DefaultProcessor = "ShaderProcessor")]
    public class ShaderImporter : ContentImporter<Ast.CompilationUnit>
    {
        protected override Ast.CompilationUnit Import(string filename, ContentImporterContext context)
        {
            StreamReader reader = new StreamReader(filename);
            Scanner scanner = new Scanner(filename, reader);
            Parser parser = new Parser(scanner, context.Logger);

            return parser.Parse();
        }
    }
}
