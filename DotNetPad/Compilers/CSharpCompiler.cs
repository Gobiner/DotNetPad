﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;

namespace Gobiner.CSharpPad.Compilers
{
    public class CSharpCompiler : MarshalByRefObject, ICompiler
    {
        public string Code { get; set; }
        public CompilerError[] Errors { get; private set; }
        public string FileName { get; private set; }
        public bool ProducedExecutable { get; private set; }

        private IDictionary<string, string> providerOptions = new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } };
        private string[] GacAssembliesToCompileAgainst = 
		{ "System.dll", "System.Core.dll", "System.Data.dll",
		"System.Data.DataSetExtensions.dll", "Microsoft.CSharp.dll",
		"System.Xml.dll", "System.Xml.Linq.dll", "System.Data.Entity.dll",
		"System.Windows.Forms.dll", "System.Numerics.dll" };

        public CSharpCompiler()
        {
            Errors = new CompilerError[] { };
        }

        public void Compile(string filename)
        {
            string mainClass = FindMainClass();
            if (mainClass == null && Errors.Length == 0)
            {
                Errors = new CompilerError[] { new CompilerError(filename, 0, 0, "", "Could not find a static void Main(string[]) method") };
                ProducedExecutable = false;
                return;
            }

            var provider = new CSharpCodeProvider(providerOptions);
            var compileParams = new CompilerParameters(GacAssembliesToCompileAgainst);
            compileParams.MainClass = mainClass;
            compileParams.GenerateExecutable = true;
            compileParams.GenerateInMemory = false;
            compileParams.OutputAssembly = filename;

            CompilerResults r = provider.CompileAssemblyFromSource(compileParams, new string[] { this.Code });
            Errors = r.Errors.Cast<CompilerError>().ToArray();
            ProducedExecutable = Errors.Length == 0;
        }

        private string FindMainClass()
        {
            var provider = new CSharpCodeProvider(providerOptions);
            var compileParams = new CompilerParameters(GacAssembliesToCompileAgainst);
            compileParams.GenerateExecutable = false;
            compileParams.GenerateInMemory = true;

            CompilerResults r = provider.CompileAssemblyFromSource(compileParams, new string[] { this.Code });

            Errors = r.Errors.Cast<CompilerError>().ToArray();

            if (Errors.Count() > 0)
                return null;

            var mainMethods = r.CompiledAssembly.GetTypes()
                              .SelectMany(x => x.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                              .Where(x => x.Name == "Main")
                              .Where(x => x.GetParameters().Length == 0 || (x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType == typeof(string[])));

            if (!mainMethods.Any()) return null;

            return mainMethods.First().DeclaringType.FullName;
        }
    }
}
