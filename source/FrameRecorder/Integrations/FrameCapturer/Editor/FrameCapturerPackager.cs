using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.FrameRecorder;
using UnityEngine;

namespace UTJ.FrameCapturer.Recorders
{
    class FrameCapturerPackager : ScriptableObject {}

    [InitializeOnLoad]
    class FrameCapturerPackagerInternal : ScriptableObject
    {
        const string k_PackageName = "FrameCapturerRecorders";
        const string k_WitnessClass = "Recorder";
        const string k_WitnessNamespace = "UnityEngine.FrameRecorder";

        static string m_PkgFile;
        static string m_ScriptFile;

        public static string GeneratePackage()
        {
            string[] files = new string[]
            {
                Path.Combine(FRPackagerPaths.GetIntegrationPath(), "FrameCapturer/Recorders"),
            };
            var destFile = Path.Combine(FRPackagerPaths.GetIntegrationPackagePath(), k_PackageName + ".unitypackage");
            AssetDatabase.ExportPackage(files, destFile, ExportPackageOptions.Recurse);
            Debug.Log("Generated package: " + destFile);

            return destFile;
        }
        
        static FrameCapturerPackagerInternal() // auto extracts
        {
            var havePostProcessing = AppDomain.CurrentDomain.GetAssemblies()
                .Any(x => x.GetTypes().Any(y => y.Name == k_WitnessClass && y.Namespace == k_WitnessNamespace));

            if (havePostProcessing)
            {
                m_PkgFile = Path.Combine( FRPackagerPaths.GetIntegrationPackagePath(),  "../" + k_PackageName + ".unityPackage" );
                m_ScriptFile = Path.Combine(FRPackagerPaths.GetIntegrationPath(), "FrameCapturer/Recorders/BaseFCRecorderSettings.cs");
                if ( File.Exists(m_PkgFile) && 
                    (!File.Exists(m_ScriptFile) || File.GetLastWriteTime(m_PkgFile) > File.GetLastWriteTime(m_ScriptFile)))
                {
                    Debug.Log("PostProcessing asset detected - Importing FrameCapturer's Recorders");
                    AssetDatabase.importPackageCompleted += AssetDatabase_importPackageCompleted;
                    AssetDatabase.importPackageFailed += AssetDatabase_importPackageFailed;
                    AssetDatabase.importPackageCancelled += RemovePackageImportCallbacks;
                    AssetDatabase.ImportPackage(m_PkgFile, false);
                }
            }
        }
        
        static void AssetDatabase_importPackageCompleted(string packageName)
        {
            if (packageName == k_PackageName)
            {
                File.SetLastWriteTime(m_ScriptFile, File.GetLastWriteTime(m_PkgFile));
                RemovePackageImportCallbacks(k_PackageName);
                Debug.LogError("FrameRecorder enabled/updated integration package" + k_PackageName );
            }
        }

        static void AssetDatabase_importPackageFailed(string packageName, string errorMessage)
        {
            if (packageName == k_PackageName)
            {
                Debug.LogError("Failed to import " + k_PackageName + ": " + errorMessage);
                RemovePackageImportCallbacks(k_PackageName);
            }
        }

        static void RemovePackageImportCallbacks(string packageName)
        {
            AssetDatabase.importPackageCompleted -= AssetDatabase_importPackageCompleted;
            AssetDatabase.importPackageCancelled -= RemovePackageImportCallbacks;
            AssetDatabase.importPackageFailed -= AssetDatabase_importPackageFailed;
        }
      
    }

}