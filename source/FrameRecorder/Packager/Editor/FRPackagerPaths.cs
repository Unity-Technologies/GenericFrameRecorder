using System.IO;
using UnityEngine;

namespace UnityEditor.FrameRecorder
{
    class FRPackagerPaths : ScriptableObject
    {
        public static string GetFrameRecorderRootPath()
        {
            var path = GetFrameRecorderPath();
            path = path.Substring(path.IndexOf("Assets"));
            return path;
        }

        public static string GetFrameRecorderVersionFilePath()
        {
            var dummy = ScriptableObject.CreateInstance<RecorderVersion>();
            var path = Application.dataPath + AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(dummy)).Substring("Assets".Length);
            return path;
        }

        public static string GetFrameRecorderPath()
        {
            var dummy = ScriptableObject.CreateInstance<FRPackagerPaths>();
            var path = Application.dataPath + AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(dummy)).Substring("Assets".Length);
            
            return path.Replace("/Framework/Packager/Editor/FRPackagerPaths.cs", "");
        }

    }
}