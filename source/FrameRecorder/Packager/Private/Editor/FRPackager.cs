using System;
using System.IO;
using UnityEngine;

namespace UnityEditor.Recorder
{
    static class FRPackager
    {
        const string k_PackageName = "Recorder";

        public static string GetFrameRecorderRootPath()
        {
            return Application.dataPath + "/Recorder/";
        }
        /*
        [MenuItem("Tools/Recorder/Generate Framework Package", false,100)]
        static void GeneratePackage()
        {
            var rootPath = FRPackagerPaths.GetFrameRecorderRootPath();
            UpdateVersion();

            string[] files = new string[]
            {
                Path.Combine(rootPath, "Framework.meta"),
                Path.Combine(rootPath, "Framework/Core"),
                Path.Combine(rootPath, "Framework/Inputs"),
                Path.Combine(rootPath, "Framework/Recorders"),
                Path.Combine(rootPath, "Framework/Packager/Editor"),
            };
            var destFile = k_PackageName + ".unitypackage";
            AssetDatabase.ExportPackage(files, destFile, ExportPackageOptions.Recurse);
            Debug.Log("Generated package: " + destFile);
        }
        */
        [MenuItem("Tools/Recorder/Generate Assetstore package", false, 100)]
        static void GenerateAssetStorePackage()
        {
            var rootPath = FRPackagerPaths.GetRecorderRootPath();
            var type = Type.GetType("UnityEditor.Recorder.MovieRecorderPackager");
            if (type != null)
            {
                var method = type.GetMethod("GeneratePackage");
                method.Invoke(null, null);
                AssetDatabase.Refresh();
            }
            UpdateVersion();

            var files = new []
            {
                Path.Combine(rootPath, "Recorder_install.pdf" ),
                Path.Combine(rootPath, "Framework.meta" ),
                Path.Combine(rootPath, "Framework/Core" ),
                Path.Combine(rootPath, "Framework/Inputs" ),
                Path.Combine(rootPath, "Framework/Recorders" ),
                Path.Combine(rootPath, "Framework/Packager/Editor" ),
                Path.Combine(rootPath, "Extensions/UTJ" ),
                Path.Combine(rootPath, "Extensions/FCIntegration" ),
                Path.Combine(rootPath, "Extensions/MovieRecorder/Packaging" ),
            };
            var destFile = k_PackageName + " " + RecorderVersion.Stage + " v"+ RecorderVersion.Version +  ".unitypackage";
            AssetDatabase.ExportPackage(files, destFile, ExportPackageOptions.Recurse);
            Debug.Log("Generated package: " + destFile);
        }

        static void UpdateVersion()
        {
            var path = FRPackagerPaths.GetRecorderVersionFilePath();
            var script = File.ReadAllText(path);

            var tag = "public const string Version = ";
            var startOffset = script.IndexOf(tag);
            var endOffset = script.IndexOf("\"", startOffset + tag.Length + 1);

            var pattern = script.Substring(startOffset, endOffset - startOffset);
            startOffset = pattern.LastIndexOf(".");
            var newValue = pattern.Substring(0, startOffset + 1) + DateTime.Now.ToString("yyMMdd-hh");
            script = script.Replace(pattern, newValue);
            File.WriteAllText(path, script);
        }

    }
}
