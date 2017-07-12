using System.IO;
using UnityEngine;

namespace UnityEditor.FrameRecorder
{
    class FRPackagerPaths : ScriptableObject
    {
        public static string GetFrameRecorderRootPath()
        {
            ScriptableObject dummy = ScriptableObject.CreateInstance<FRPackagerPaths>();
            string path = Application.dataPath + AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(dummy)).Substring("Assets".Length);

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