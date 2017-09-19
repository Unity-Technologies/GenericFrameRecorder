using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace UnityEditor.Recorder
{
    class MovieRecorderPackager : ScriptableObject
    {
        public static void GeneratePackage()
        {
            var rootPath = FRPackagerPaths.GetRecorderRootPath();

            File.WriteAllText( Path.Combine(rootPath, "Extensions/MovieRecorder/Packaging/TS.txt" ), DateTime.Now.ToString() );

            var files = new []
            {
                Path.Combine(rootPath, "Extensions/MovieRecorder/Recorder" ),
                Path.Combine(rootPath, "Extensions/MovieRecorder/Audio" ),
                Path.Combine(rootPath, "Extensions/MovieRecorder/Recorder" ),
                Path.Combine(rootPath, "Extensions/MovieRecorder/Packaging/Editor" ),
                Path.Combine(rootPath, "Extensions/MovieRecorder/Packaging/TS.txt" ),
            };
            var destFile = Path.Combine(rootPath, "Extensions/MovieRecorder/Packaging/MovieRecorder.unitypackage");
            AssetDatabase.ExportPackage(files, destFile, ExportPackageOptions.Recurse);
            Debug.Log("Generated package: " + destFile);            
        }        
    }

    [InitializeOnLoad]
    class MovieRecorderPackagerInternal : ScriptableObject
    {
        const string k_PackageName = "MovieRecorder.unitypackage";

        static bool AutoExtractAllowed
        {
            get { return !AppDomain.CurrentDomain.GetAssemblies().Any(x => x.GetTypes().Any(y => y.Name == "FRPackager" && y.Namespace == "UnityEditor.Recorder")); }
        }

        static bool AudioRecordingAvailable
        {
            get
            {
                var className = "UnityEngine.AudioRenderer";
                var dllName = "UnityEngine";
                var audioRecorderType = Type.GetType(className + ", " + dllName);
                return audioRecorderType != null;
            }
        }

        static bool MovieRecordingAvailable
        {
            get { return AppDomain.CurrentDomain.GetAssemblies().Any(x => x.GetTypes().Any(y => y.Name == "MediaEncoder" && y.Namespace == "UnityEditor.Media")); }
        }

        static MovieRecorderPackagerInternal() // auto extracts
        {
            if(AutoExtractAllowed && AudioRecordingAvailable && MovieRecordingAvailable )
            {
                var pkgFile = Path.Combine(FRPackagerPaths.GetRecorderRootPath(), "Extensions/MovieRecorder/Packaging/" + k_PackageName);
                var tsFile = Path.Combine(FRPackagerPaths.GetRecorderRootPath(), "Extensions/MovieRecorder/Packaging/TS.txt");
                var recDir = Path.Combine(FRPackagerPaths.GetRecorderRootPath(), "Extensions/MovieRecorder/Recorder");
                if ( File.Exists(pkgFile) && 
                    (!Directory.Exists(recDir) || File.GetLastWriteTime(pkgFile) > File.GetLastWriteTime(tsFile).AddMinutes(5))) // extra 5min to compensate for package write duration
                {
                    Debug.Log("Importing MovieRecorder...");
                    AssetDatabase.ImportPackage(pkgFile, false);
                }
            }
        }

      
    }
}