using dnlib.DotNet;
using NETRDump;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Linq;
using Tool.Interface;
using Tool.Logging;

namespace TestTool;

sealed class Tool : ITool<ToolOptions> {
	public string Title => null;

    public void Execute(ToolOptions options) {
		Logger.Level = LogLevel.Verbose1;

		Assembly moduleRef = null;
		try
		{
			moduleRef = Assembly.UnsafeLoadFrom(options.AssemblyFilePath);
			RuntimeHelpers.RunModuleConstructor(moduleRef.ManifestModule.ModuleHandle);
		}
		catch (Exception ex)
		{
            Logger.Error(ex.Message);
        }

		var module = ModuleDefMD.Load(moduleRef.ManifestModule);
		var outputPath = moduleRef.GetOutputPath();

		if (!Directory.Exists(outputPath))
			Directory.CreateDirectory(outputPath);

		var names = moduleRef.GetManifestResourceNames();
		for (int i = 0; i < names.Length; i++)
		{
			var name = names[i];
            var resource = moduleRef.GetManifestResourceStream(name);
            var resourceData = new byte[resource.Length];
            resource.Read(resourceData, 0, resourceData.Length);
            // print all resource name and max 200 bytes of data
            Logger.Info("----------------------------------------");
            Logger.Info($"Resource: {name}");

			if (name.IndexOf("metadata") >= 0)
			{
				// print metadata
				var metadata = System.Text.Encoding.UTF8.GetString(resourceData);
				Logger.Info(metadata);
			}
			else if (name.EndsWith(".compressed"))
			{
				resourceData = resourceData.Decompress();
                Logger.Info($"Decompress Data: {System.Text.Encoding.UTF8.GetString(resourceData, 0, Math.Min(100, resourceData.Length))}");
            }
			else
			{
				Logger.Info($"Data: {System.Text.Encoding.UTF8.GetString(resourceData, 0, Math.Min(100, resourceData.Length))}");
			}
			string outputFileName = name.Replace(".compressed", null);
			if (outputFileName.StartsWith("costura.") && outputFileName.Split('.').Length > 1)
				outputFileName = outputFileName.Substring("costura.".Length);

			string fullPath = Path.Combine(outputPath, outputFileName);
			File.WriteAllBytes(fullPath, resourceData);
			Logger.Info($"Decompressed costura file: {name}");
		}


        Logger.Flush();
	}
}
