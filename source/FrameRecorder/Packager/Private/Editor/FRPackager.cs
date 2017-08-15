using System;
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
                Path.Combine(rootPath, "Framework.meta"),
                Path.Combine(rootPath, "Framework/FrameRecorder.meta"),
                Path.Combine(rootPath, "Framework/FrameRecorder/Core"),
                Path.Combine(rootPath, "Framework/FrameRecorder/Inputs"),
                Path.Combine(rootPath, "Framework/FrameRecorder/Recorders"),
                Path.Combine(rootPath, "Framework/FrameRecorder/Packager/Editor"),
            };
            var destFile = k_PackageName + ".unitypackage";
            AssetDatabase.ExportPackage(files, destFile, ExportPackageOptions.Recurse);
            Debug.Log("Generated package: " + destFile);
        }

        [MenuItem("Assets/Recorder/Generate Package (full)")]
        static void GeneratePackageFull()
        {
            var rootPath = FRPackagerPaths.GetFrameRecorderRootPath();
            var type = System.Type.GetType("UnityEditor.FrameRecorder.MovieRecorderPackager");
            var method = type.GetMethod("GeneratePackage");
            method.Invoke(null, null);
            AssetDatabase.Refresh();

            var files = new []
            {
                Path.Combine(rootPath, "Framework.meta" ),
                Path.Combine(rootPath, "Framework/FrameRecorder.meta" ),
                Path.Combine(rootPath, "Framework/FrameRecorder/Core" ),
                Path.Combine(rootPath, "Framework/FrameRecorder/Inputs" ),
                Path.Combine(rootPath, "Framework/FrameRecorder/Recorders" ),
                Path.Combine(rootPath, "Framework/FrameRecorder/Packager/Editor" ),
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
