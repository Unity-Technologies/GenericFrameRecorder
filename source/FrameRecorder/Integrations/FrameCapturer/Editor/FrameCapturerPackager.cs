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
        const string k_WitnessClass = "fcAPI";
        const string k_WitnessNamespace = "UTJ.FrameCapturer";

        static string m_PkgFile;
        static string m_ScriptFile;

        public static string GeneratePackage()
        {
            string[] files = new string[]
            {
                Path.Combine(FRPackagerPaths.GetIntegrationPath(), "FrameCapturer/Recorders"),
            };

            var destDir = FRPackagerPaths.GetIntegrationPackagePath();
            if (!Directory.Exists(destDir))
                Directory.CreateDirectory(destDir);

            var destFile = Path.Combine(destDir, k_PackageName + ".unitypackage");
            AssetDatabase.ExportPackage(files, destFile, ExportPackageOptions.Recurse);
            Debug.Log("Generated package: " + destFile);

            return destFile;
        }

        static bool AutoExtractAllowed
        {
            get { return !AppDomain.CurrentDomain.GetAssemblies().Any(x => x.GetTypes().Any(y => y.Name == "FRPackager" && y.Namespace == "UnityEditor.FrameRecorder")); }
        }

        static bool FrameCapturerPresent
        {
            get { return AppDomain.CurrentDomain.GetAssemblies().Any(x => x.GetTypes().Any(y => y.Name == k_WitnessClass && y.Namespace == k_WitnessNamespace)); }
        }
        
        static FrameCapturerPackagerInternal() // auto extracts
        {
            if(AutoExtractAllowed && FrameCapturerPresent )
            {
                m_PkgFile = Path.Combine( FRPackagerPaths.GetIntegrationPackagePath(),  k_PackageName + ".unityPackage" );
                m_ScriptFile = Path.Combine(FRPackagerPaths.GetIntegrationPath(), "FrameCapturer/Recorders/BaseFCRecorderSettings.cs");
                if ( File.Exists(m_PkgFile) && 
                    (!File.Exists(m_ScriptFile) || File.GetLastWriteTime(m_PkgFile) > File.GetLastWriteTime(m_ScriptFile)))
                {
                    Debug.Log("Importing FrameCapturer's Recorders: Processing...");
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
                Debug.Log("Importing FrameCapturer's Recorders: Done.");
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