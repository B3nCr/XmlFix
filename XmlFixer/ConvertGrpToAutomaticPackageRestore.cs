using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XmlFixer
{
    /// <summary>
    /// The old way to do package restore:
    /// In the old way, you right click on your solution in VS and choose Enable package restore. 
    /// This causes VS to modify your csproj files, and create .nuget folder containing nuget.exe and some other files.
    /// 
    /// The Right way to do package restore:
    /// What the NuGet team is now recommending is both a lot cleaner and a lot simpler. 
    /// In short, you don't do anything special, and it just happens! 
    /// This is because NuGet now always restores packages before building in VS. 
    /// http://blog.davidebbo.com/2014/01/the-right-way-to-restore-nuget-packages.html
    /// 
    /// http://docs.nuget.org/docs/workflows/migrating-to-automatic-package-restore
    /// </summary>
    [TestClass]
    public class ConvertGrpToAutomaticPackageRestore
    {
        [TestMethod]
        public void DeleteNugetFiles()
        {
            const string rootFolderName = @"C:\Dev\Repos\Rutland\";

            var dir = new DirectoryInfo(rootFolderName);
            var nugetExes = dir.EnumerateFiles("NuGet.exe", SearchOption.AllDirectories);
            var nugetTargets = dir.EnumerateFiles("NuGet.targets", SearchOption.AllDirectories);

            foreach (var fileInfo in Enumerable.Union(nugetExes, nugetTargets))
            {
                fileInfo.Delete();
            }
            Assert.AreEqual(0, nugetExes.Count());
            Assert.AreEqual(0, nugetTargets.Count());
        }


        [TestMethod]
        public void FixAllProjectFiles()
        {
            const string rootFolderName = @"C:\Dev\Repos\Rutland\";
            var dir = new DirectoryInfo(rootFolderName);

            var files = dir.EnumerateFiles("*.csproj", SearchOption.AllDirectories).ToList();
            //Assert.AreEqual(51, files.Count());
            
            foreach (var fileInfo in files)
            {
                FixProjectFile(fileInfo.FullName);
            }
        }

        [TestMethod] [Ignore] // it worked, so not needed now
        public void FixOneProjectFiles()
        {
            const string fileName = @"D:\GRP\GRP2App\GRP2App.BL\GRP2App.BL.csproj";

            FixProjectFile(fileName);
        }

        private static void FixProjectFile(string fileName)
        {
            var doc = new XmlDocument();
            doc.Load(fileName);

            var namespaceManager = new XmlNamespaceManager(doc.NameTable);
            namespaceManager.AddNamespace("ns", "http://schemas.microsoft.com/developer/msbuild/2003");

            RemoveNugetImport(ref doc, namespaceManager);
            RemoveNugetTarget(ref doc, namespaceManager);

            doc.Save(fileName);
            // doc.Save(fileName + ".fixed");
        }

        /// <summary>
        /// Remove this element:
        /// <Import Project="$(SolutionDir)\.nuget\NuGet.targets" Condition="Exists('$(SolutionDir)\.nuget\NuGet.targets')" />
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="namespaceManager"></param>
        private static void RemoveNugetTarget(ref XmlDocument doc, XmlNamespaceManager namespaceManager)
        {
            var node = doc.SelectSingleNode(@"//ns:Import[@Project='$(SolutionDir)\.nuget\NuGet.targets']", namespaceManager);
            if (node != null && node.ParentNode != null)
                node.ParentNode.RemoveChild(node);
        }

        /// <summary>
        /// Remove this element
        /// <Target Name="EnsureNuGetPackageBuildImports" BeforeTargets="PrepareForBuild"></Target>
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="namespaceManager"></param>
        private static void RemoveNugetImport(ref XmlDocument doc, XmlNamespaceManager namespaceManager)
        {
            var node = doc.SelectSingleNode(@"//ns:Target[@Name='EnsureNuGetPackageBuildImports']", namespaceManager);
            if (node != null && node.ParentNode != null)
                node.ParentNode.RemoveChild(node);
        }
    }
}
