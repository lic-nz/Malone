using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyCompany("Livestock Improvement")]
[assembly: AssemblyCopyright("Copyright © Livestock Improvement 2015")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Make it easy to distinguish Debug and Release (i.e. Retail) builds through the file properties window.
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Retail")]
#endif

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components. If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// http://stackoverflow.com/questions/64602/what-are-differences-between-assemblyversion-assemblyfileversion-and-assemblyin
// http://martinwilley.com/blog/2014/07/23/NetVersioning.aspx
// http://blog.ploeh.dk/2013/12/10/semantic-versioning-with-continuous-deployment/
// http://docs.nuget.org/create/nuspec-reference#Replacement_Tokens

[assembly: AssemblyVersion("0.0.2")]
[assembly: AssemblyFileVersion("0.0.2-alpha")]
[assembly: AssemblyInformationalVersion("0.0.2-alpha")]