using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace UnityEditor.FrameRecorder
{
    public class FRPackager : ScriptableObject
    {
        const string k_PackageName = "GenericFrameFrameRecorder";

        [MenuItem("Assets/FrameRecorder - Generate Package")]
        static void GeneratePackage()
        {
            var rootPath = GetFrameRecorderPath();
            var fcPackagePath = UTJ.FrameCapturer.Recorders.FrameCapturerPackagerInternal.GeneratePackage(rootPath);


            string[] files = new string[]
            {
                Path.Combine(rootPath, "Core" ),
                Path.Combine(rootPath, "Inputs" ),
                Path.Combine(rootPath, "Recorders" ),
                Path.Combine(rootPath, "Integrations/FrameCapturer/Editor" ),
                fcPackagePath,
            };
            var destFile = k_PackageName + ".unitypackage";
            AssetDatabase.ExportPackage(files, destFile, ExportPackageOptions.Recurse);
            Debug.Log("Generated package: " + destFile);
        }

        static string GetFrameRecorderPath()
        {
            ScriptableObject dummy = ScriptableObject.CreateInstance<FRPackager>();
            string path = Application.dataPath + AssetDatabase.GetAssetPath(
                MonoScript.FromScriptableObject(dummy)).Substring("Assets".Length);

            path = path.Substring(path.IndexOf("Assets"));
            path = path.Substring(0, path.LastIndexOf('/'));
            path = path.Substring(0, path.LastIndexOf('/'));
            path = path.Substring(0, path.LastIndexOf('/'));
            return path;
        }
    }
}
