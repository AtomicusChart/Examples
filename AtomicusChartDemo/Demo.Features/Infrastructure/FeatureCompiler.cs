using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;

namespace AtomicusChart.Demo.Features.Infrastructure
{
	class FeatureCompiler
	{
		private readonly CSharpCodeProvider provider;
		private readonly CompilerParameters parameters;

		public FeatureCompiler()
		{
			provider = new CSharpCodeProvider();
			parameters = GetCompilerParameters();
		}

		private CompilerParameters GetCompilerParameters()
		{
			string ver = $"{Environment.Version.Major}.{Environment.Version.MajorRevision}.{Environment.Version.Build}";

			string path = Environment.SystemDirectory;
			string exWpfDir = $@"{path.Substring(0, path.LastIndexOf('\\'))}\Microsoft.NET\Framework\v{ver}\WPF";
			var compilerParameters = new CompilerParameters
			{
				GenerateExecutable = false,
				GenerateInMemory = true,
				CompilerOptions = string.Concat(" /unsafe", $" /lib:{exWpfDir}"),
			};
			compilerParameters.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().ManifestModule.Name);
			foreach (AssemblyName assemblyName in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
			{
				compilerParameters.ReferencedAssemblies.Add(assemblyName.Name + ".dll");
			}
			return compilerParameters;
		}

		public CompilerErrorCollection TryCompile(string code, out Type featureDemoType)
		{
			CompilerResults results = provider.CompileAssemblyFromSource(parameters, code);

			if (results.Errors.HasErrors)
			{
				featureDemoType = null;
				return results.Errors;
			}
			else
			{
				Type featureType = results.CompiledAssembly.GetTypes().FirstOrDefault(type => type.IsSubclassOf(typeof(FeatureDemo)));
				if (featureType == null)
				{
					featureDemoType = null;
					return GetCustomCompilerError(
						"Demo viewer error.",
						$"Compiled result doesn't contain type derived from {nameof(FeatureDemo)}.");
				}
				featureDemoType = featureType;
				return results.Errors;
			}
		}

		public static CompilerErrorCollection GetCustomCompilerError(string errorNumber, string errorText)
			=> new CompilerErrorCollection(new[] { new CompilerError("---", -1, -1, errorNumber, errorText) });
	}
}
