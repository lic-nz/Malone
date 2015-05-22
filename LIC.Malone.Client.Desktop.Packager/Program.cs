using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NuGet;

namespace LIC.Malone.Client.Desktop.Packager
{
	class Program
	{
		static void Main(string[] args)
		{
			var packagerDirectory = Path.GetFullPath(Path.Combine(Assembly.GetExecutingAssembly().Location, "..", "..", ".."));
			var buildDirectory = Path.GetFullPath(Path.Combine(packagerDirectory, "build"));
			var clientDirectory = Path.GetFullPath(Path.Combine(packagerDirectory, "..", "LIC.Malone.Client.Desktop"));
			var clientBinDirectory = Path.GetFullPath(Path.Combine(clientDirectory, "bin", "Release"));

			Console.WriteLine(packagerDirectory);
			Console.WriteLine(buildDirectory);
			Console.WriteLine(clientDirectory);
			Console.WriteLine(clientBinDirectory);
			Console.WriteLine();

			var buildDirectoryInfo = Directory.CreateDirectory(buildDirectory);

			// Clean out build directory.
			buildDirectoryInfo.GetFiles("*.nupkg").ToList().ForEach(p => p.Delete());

			Directory.SetCurrentDirectory(buildDirectory);

			// Rely on standard nuget process to build the project and create a starting package to copy metadata from.
			// Make sure nuget.exe is in your PATH.

			var process = new Process
			{
				StartInfo =
				{
					FileName = "nuget.exe",
					Arguments = string.Format("pack {0}\\LIC.Malone.Client.Desktop.csproj -Build -Prop Configuration=Release", clientDirectory),
					UseShellExecute = false,
					RedirectStandardOutput = true
				}
			};

			try
			{
				process.Start();
			}
			catch (Exception innerException)
			{
				throw new Exception("Is nuget.exe in your PATH?", innerException);
			}
			
			var reader = process.StandardOutput;
			var output = reader.ReadToEnd();

			Console.WriteLine(output);

			process.WaitForExit();
			process.Close();

			var nupkg = buildDirectoryInfo.GetFiles("*.nupkg").Single();
			var package = new ZipPackage(nupkg.FullName);

			// Copy all of the metadata *EXCEPT* for dependencies. Kill those.
			var manifest = new ManifestMetadata
			{
				Id = package.Id,
				Version = package.Version.ToString(),
				Authors = string.Join(", ", package.Authors),
				Copyright = package.Copyright,
				DependencySets = null,
				Description = package.Description,
				Title = package.Title,
				IconUrl = package.IconUrl.ToString(),
				ProjectUrl = package.ProjectUrl.ToString(),
				LicenseUrl = package.LicenseUrl.ToString()
			};

			// TODO: Check if the SVG file is required.

			const string target = @"lib\net45";

			// Include dependencies in the package.
			var files = new List<ManifestFile>
			{
				new ManifestFile { Source = "*.dll", Target = target },
				new ManifestFile { Source = "Malone.exe", Target = target },
				new ManifestFile { Source = "Malone.exe.config", Target = target },
			};

			var builder = new PackageBuilder();
			builder.Populate(manifest);
			builder.PopulateFiles(clientBinDirectory, files);

			// TODO: Save with the right name once comparisons with manual method are complete.
			using (var stream = File.Open(Path.Combine(buildDirectory, "what.nupkg"), FileMode.OpenOrCreate))
			{
				builder.Save(stream);
			}

			// TODO: Releasify.

			Console.WriteLine("\n\nPress any key to exit.");
			Console.ReadLine();
		}
	}
}