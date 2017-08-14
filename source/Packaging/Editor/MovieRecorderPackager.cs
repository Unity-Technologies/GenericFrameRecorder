using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace UnityEditor.FrameRecorder
{

    class MovieRecorderPackager : ScriptableObject
    {
        public static void GeneratePackage()
        {
            var rootPath = FRPackagerPaths.GetFrameRecorderRootPath();

            var files = new []
            {
                Path.Combine(rootPath, "Extensions/MovieRecorder/Audio" ),
                Path.Combine(rootPath, "Extensions/MovieRecorder/Recorder" ),
                Path.Combine(rootPath, "Extensions/MovieRecorder/Packaging/Editor" ),
            };
            var destFile = Path.Combine(rootPath, "MovieRecorder.unitypackage");
            AssetDatabase.ExportPackage(files, destFile, ExportPackageOptions.Recurse);
            Debug.Log("Generated package: " + destFile);            
        }        
    }

    [InitializeOnLoad]
    class MovieRecorderPackagerInternal : ScriptableObject
    {
        const string k_WitnessClass = "fcAPI";
        const string k_WitnessNamespace = "UTJ.FrameCapturer";
        const string k_PackageName = "MovieRecorder.unitypackage";

        static string m_PkgFile;
        static string m_ScriptFile;

        static bool AutoExtractAllowed
        {
            get { return !AppDomain.CurrentDomain.GetAssemblies().Any(x => x.GetTypes().Any(y => y.Name == "FRPackager" && y.Namespace == "UnityEditor.FrameRecorder")); }
        }

        static bool AudioRecordingAvailable
        {
            get { return Type.GetType("UnityEditor.AudioRecorder") != null; }
        }

        static bool MovieRecordingAvailable
        {
            get { return Type.GetType("UnityEditor.Media.MediaEncoder") != null; }
        }

        static MovieRecorderPackagerInternal() // auto extracts
        {
            if(AutoExtractAllowed && AudioRecordingAvailable && MovieRecordingAvailable )
            {
                m_PkgFile = Path.Combine(FRPackagerPaths.GetFrameRecorderRootPath(), k_PackageName);
                m_ScriptFile = Path.Combine(FRPackagerPaths.GetFrameRecorderRootPath(), "Extensions/MovieRecorder/Recorder/Engine/MediaRecorder.cs");
                if ( File.Exists(m_PkgFile) && 
                    (!File.Exists(m_ScriptFile) || File.GetLastWriteTime(m_PkgFile) > File.GetLastWriteTime(m_ScriptFile)))
                {
                    Debug.Log("Importing MovieRecorder: Processing...");
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