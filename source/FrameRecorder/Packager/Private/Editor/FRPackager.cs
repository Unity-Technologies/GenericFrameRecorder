using System.IO;
using UnityEngine;

namespace UnityEditor.FrameRecorder
{
    static class FRPackager
    {
        const string k_PackageName = "Recorder";
        
        public static string GetFrameRecorderRootPath()
        {
            return Application.dataPath + "/Recorder/";
        }

        [MenuItem("Assets/Recorder/Generate Framework Package")]
        static void GeneratePackage()
        {
            var rootPath = FRPackagerPaths.GetFrameRecorderRootPath();

            string[] files = new string[]
            {
                Path.Combine(rootPath, "Framework/Core" ),
                Path.Combine(rootPath, "Framework/Recorders" ),
                Path.Combine(rootPath, "Framework/Packager/Editor" ),
            };
            var destFile = k_PackageName + ".unitypackage";
            AssetDatabase.ExportPackage(files, destFile, ExportPackageOptions.Recurse);
            Debug.Log("Generated package: " + destFile);
        }

        [MenuItem("Assets/Recorder/Generate Package (full)")]
        static void GeneratePackageFull()
        {
            var rootPath = FRPackagerPaths.GetFrameRecorderRootPath();

            MovieRecorderPackager.GeneratePackage();
            AssetDatabase.Refresh();

            var files = new []
            {
                Path.Combine(rootPath, "Framework/Core" ),
                Path.Combine(rootPath, "Framework/Recorders" ),
                Path.Combine(rootPath, "Framework/Packager/Editor" ),
                Path.Combine(rootPath, "Extensions/UTJ" ),
                Path.Combine(rootPath, "Extensions/FrameCapturerRecorder" ),
                Path.Combine(rootPath, "Extensions/MovieRecorder/Packaging" ),
            };
            var destFile = k_PackageName + "(full).unitypackage";
            AssetDatabase.ExportPackage(files, destFile, ExportPackageOptions.Recurse);
            Debug.Log("Generated package: " + destFile);
        }
    }
}
