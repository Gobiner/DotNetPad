﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using Microsoft.FSharp.Compiler.CodeDom;

namespace Gobiner.CSharpPad.Compilers
{
	class FSharpCompiler : MarshalByRefObject, ICompiler
	{
		public string Code { get; set; }
		public CompilerError[] Errors { get; private set; }
		public string FileName { get; private set; }
		public bool ProducedExecutable { get; private set; }
		public IILFormatter ILFormatter { get; set; }
		public string[] FormattedILDisassembly { get; set; }

		private IDictionary<Type, TypeMethodInfo> ILLookup { get; set; }
		private string[] GacAssembliesToCompileAgainst = { "System.dll", "System.Core.dll", "System.Data.dll", "System.Data.DataSetExtensions.dll", 
															 "FSharp.Core.dll", "System.Xml.dll", "System.Xml.Linq.dll", "System.Data.Entity.dll", 
															 "System.Windows.Forms.dll", "System.Numerics.dll" };
		public FSharpCompiler()
		{
			ILFormatter = new DefaultILFormatter();
            FormattedILDisassembly = new string[] { };
		}

		public void Compile(string filename)
		{
			var provider = new FSharpCodeProvider();
			var compileParams = new CompilerParameters(GacAssembliesToCompileAgainst);
			compileParams.GenerateExecutable = true;
			compileParams.GenerateInMemory = false;
			compileParams.OutputAssembly = filename;

			CompilerResults r = provider.CompileAssemblyFromSource(compileParams, new string[] { this.Code });
			Errors = r.Errors.Cast<CompilerError>().ToArray();
			if (Errors.Length > 0)
			{
				ProducedExecutable = false;
			}
			if (ILFormatter != null && ProducedExecutable)
			{
				ILLookup = new ILDisassembler().GetDisassembly(r.CompiledAssembly);
				FormattedILDisassembly = ILFormatter.Format(ILLookup);
			}
		}
	}
}
