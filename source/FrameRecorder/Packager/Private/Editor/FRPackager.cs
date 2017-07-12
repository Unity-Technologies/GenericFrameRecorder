using System.IO;
using UnityEngine;
using UTJ.FrameCapturer.Recorders;

namespace UnityEditor.FrameRecorder
{
    static class FRPackager
    {
        const string k_PackageName = "GenericFrameRecorder";

        [MenuItem("Assets/FrameRecorder - Generate Package")]
        static void GeneratePackage()
        {
            var rootPath = FRPackagerPaths.GetFrameRecorderRootPath();
            FrameCapturerPackagerInternal.GeneratePackage();

            string[] files = new string[]
            {
                Path.Combine(rootPath, "Core" ),
                Path.Combine(rootPath, "Inputs" ),
                Path.Combine(rootPath, "Recorders" ),
                Path.Combine(rootPath, "Packager/Editor" ),
                Path.Combine(rootPath, "Integrations/FrameCapturer/Editor" ), FRPackagerPaths.GetIntegrationPackagePath(),
            };
            var destFile = k_PackageName + ".unitypackage";
            AssetDatabase.ExportPackage(files, destFile, ExportPackageOptions.Recurse);
            Debug.Log("Generated package: " + destFile);
        }
    }
}
