using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NuGet;

namespace LIC.Malone.Client.Desktop.Packager
{
	public class Releasifyer
	{
		public string PackagerDirectory { get; private set; }
		public string BuildDirectory { get; private set; }
		public string ReleasesDirectory { get; private set; }
		public string ClientDirectory { get; private set; }
		public string ClientBinDirectory { get; private set; }
		public string Squirrel { get; private set; }
		public string MaloneIco { get; private set; }

		public Releasifyer()
		{
			// Assume we are running in project bin folder.
			PackagerDirectory = Path.GetFullPath(Path.Combine(Assembly.GetExecutingAssembly().Location, "..", "..", ".."));

			BuildDirectory = Path.GetFullPath(Path.Combine(PackagerDirectory, "build"));
			ReleasesDirectory = Path.GetFullPath(Path.Combine(BuildDirectory, "Releases"));
			ClientDirectory = Path.GetFullPath(Path.Combine(PackagerDirectory, "..", "LIC.Malone.Client.Desktop"));
			ClientBinDirectory = Path.GetFullPath(Path.Combine(ClientDirectory, "bin", "Release"));
			MaloneIco = Path.GetFullPath(Path.Combine(ClientDirectory, "Malone.ico"));

			var nugetPackagesDirectory = Path.GetFullPath(Path.Combine(ClientDirectory, "..", "packages"));
			var squirrelPackageDirectory = Directory.GetDirectories(nugetPackagesDirectory, "squirrel.windows.*").Single();

			Squirrel = Path.GetFullPath(Path.Combine(squirrelPackageDirectory, "tools", "Squirrel.exe"));
		}

		public void Releasify()
		{
			var nugget = CreateNugget();
			Releasify(nugget);
		}

		private static void StartProcess(string fileName, string args)
		{
			var process = new Process
			{
				StartInfo =
				{
					FileName = fileName,
					Arguments = args,
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
				throw new Exception(string.Format("Is {0} in your PATH?", fileName), innerException);
			}

			var reader = process.StandardOutput;
			var output = reader.ReadToEnd();

			Console.WriteLine(output);

			process.WaitForExit();
			process.Close();
		}

		private void Clean(DirectoryInfo buildDirectoryInfo)
		{
			if (buildDirectoryInfo.FullName.StartsWith(@"C:\Windows", StringComparison.OrdinalIgnoreCase) || buildDirectoryInfo.FullName.StartsWith(@"C:\Program Files", StringComparison.OrdinalIgnoreCase))
				throw new UnauthorizedAccessException("Danger!");

			buildDirectoryInfo
				.GetFiles("Malone*.nupkg")
				.ToList()
				.ForEach(p => p.Delete());

			var releasesDirectoryInfos = buildDirectoryInfo.GetDirectories("Releases");

			if (!releasesDirectoryInfos.Any())
				return;

			var releasesDirectoryInfo = releasesDirectoryInfos.Single();

			var files = new List<FileInfo>();

			files.AddRange(releasesDirectoryInfo.GetFiles("Malone*.nupkg"));
			files.AddRange(releasesDirectoryInfo.GetFiles("RELEASES"));
			files.AddRange(releasesDirectoryInfo.GetFiles("Setup-Malone*.exe"));

			files.ForEach(f => f.Delete());

			releasesDirectoryInfo.Delete();
		}

		private string CreateNugget()
		{
			var csproj = Path.GetFullPath(Path.Combine(ClientDirectory, "LIC.Malone.Client.Desktop.csproj"));
			var bin = Path.GetFullPath(Path.Combine(ClientDirectory, "bin", "Release"));
			var buildDirectoryInfo = Directory.CreateDirectory(BuildDirectory);

			Directory.SetCurrentDirectory(buildDirectoryInfo.FullName);
			
			Clean(buildDirectoryInfo);

			// Rely on standard nuget process to build the project and create a starting package to copy metadata from.
			StartProcess("nuget.exe", string.Format("pack {0} -Build -Prop Configuration=Release", csproj));

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
			builder.PopulateFiles(bin, files);

			var nugget = Path.Combine(buildDirectoryInfo.FullName, nupkg.Name);

			using (var stream = File.Open(nugget, FileMode.OpenOrCreate))
			{
				builder.Save(stream);
			}

			return nugget;
		}

		private void Releasify(string nugget)
		{
			StartProcess(Squirrel, string.Format("--releasify={0} --setupIcon={1} --no-msi", nugget, MaloneIco));

			var version = Path.GetFileNameWithoutExtension(nugget);
			version = version.Replace("Malone.", string.Empty);
			var versionedSetup = Path.Combine(ReleasesDirectory, string.Format("Setup-Malone-{0}.exe", version));
			var setup = new FileInfo(Path.Combine(ReleasesDirectory, "Setup.exe"));

			if (File.Exists(versionedSetup))
				File.Delete(versionedSetup);

			setup.MoveTo(versionedSetup);
		}
	}
}