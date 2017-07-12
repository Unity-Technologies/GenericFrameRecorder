using System.IO;
using UnityEngine;


namespace UnityEditor.FrameRecorder
{
    static class FRPackager
    {
        const string k_PackageName = "GenericFrameFrameRecorder";

        [MenuItem("Assets/FrameRecorder - Generate Package")]
        static void GeneratePackage()
        {
            var rootPath = FRPackagerPaths.GetFrameRecorderRootPath();
            var fcPackagePath = UTJ.FrameCapturer.Recorders.FrameCapturerPackagerInternal.GeneratePackage();


            string[] files = new string[]
            {
                Path.Combine(rootPath, "Core" ),
                Path.Combine(rootPath, "Inputs" ),
                Path.Combine(rootPath, "Recorders" ),
                Path.Combine(rootPath, "Integrations/FrameCapturer/Editor" ), FRPackagerPaths.GetIntegrationPackagePath(),
            };
            var destFile = k_PackageName + ".unitypackage";
            AssetDatabase.ExportPackage(files, destFile, ExportPackageOptions.Recurse);
            Debug.Log("Generated package: " + destFile);
        }
    }

    class FRPackagerPaths : ScriptableObject
    {
        public static string GetFrameRecorderRootPath()
        {
            ScriptableObject dummy = ScriptableObject.CreateInstance<FRPackagerPaths>();
            string path = Application.dataPath + AssetDatabase.GetAssetPath(
                MonoScript.FromScriptableObject(dummy)).Substring("Assets".Length);

            path = path.Substring(path.IndexOf("Assets"));
            path = path.Substring(0, path.LastIndexOf('/'));
            path = path.Substring(0, path.LastIndexOf('/'));
            path = path.Substring(0, path.LastIndexOf('/'));
            return path;
        }

        public static string GetIntegrationPath()
        {
            return Path.Combine(GetFrameRecorderRootPath(), "Integrations");
        }

        public static string GetIntegrationPackagePath()
        {
            return Path.Combine(GetIntegrationPath(), "SelfExtractPackages");
        }
    }
}
