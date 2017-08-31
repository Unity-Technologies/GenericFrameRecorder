using System.IO;
using UnityEngine;

namespace UnityEditor.FrameRecorder
{
    class FRPackagerPaths : ScriptableObject
    {
        public static string GetFrameRecorderRootPath()
        {
            var dummy = ScriptableObject.CreateInstance<FRPackagerPaths>();
            string path = Application.dataPath + AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(dummy)).Substring("Assets".Length);
            
            path = path.Substring(path.IndexOf("Assets"));
            path = path.Replace("/Framework/Packager/Editor/FRPackagerPaths.cs", "");
            return path;
        }
    }
}