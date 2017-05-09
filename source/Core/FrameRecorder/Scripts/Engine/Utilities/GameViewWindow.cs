using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityEngine.Recorder.FrameRecorder.Utilities
{
    public class GameViewWindow
    {

 
 public static class LayoutUtility {
 
     private static MethodInfo _miLoadWindowLayout;
     private static MethodInfo _miSaveWindowLayout;
     private static MethodInfo _miReloadWindowLayoutMenu;
 
     private static bool _available;
     private static string _layoutsPath;
 
     static LayoutUtility() {
         Type tyWindowLayout = Type.GetType("UnityEditor.WindowLayout,UnityEditor");
         Type tyEditorUtility = Type.GetType("UnityEditor.EditorUtility,UnityEditor");
         Type tyInternalEditorUtility = Type.GetType("UnityEditorInternal.InternalEditorUtility,UnityEditor");
 
         if (tyWindowLayout != null && tyEditorUtility != null && tyInternalEditorUtility != null) {
             MethodInfo miGetLayoutsPath = tyWindowLayout.GetMethod("GetLayoutsPath", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
             _miLoadWindowLayout = tyWindowLayout.GetMethod("LoadWindowLayout", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null);
             _miSaveWindowLayout = tyWindowLayout.GetMethod("SaveWindowLayout", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null);
             _miReloadWindowLayoutMenu = tyInternalEditorUtility.GetMethod("ReloadWindowLayoutMenu", BindingFlags.Public | BindingFlags.Static);
 
             if (miGetLayoutsPath == null || _miLoadWindowLayout == null || _miSaveWindowLayout == null || _miReloadWindowLayoutMenu == null)
                 return;
 
             _layoutsPath = (string)miGetLayoutsPath.Invoke(null, null);
             if (string.IsNullOrEmpty(_layoutsPath))
                 return;
 
             _available = true;
         }
     }
 
     // Gets a value indicating whether all required Unity API
     // functionality is available for usage.
     public static bool IsAvailable {
         get { return _available; }
     }
 
     // Gets absolute path of layouts directory.
     // Returns `null` when not available.
     public static string LayoutsPath {
         get { return _layoutsPath; }
     }
 
     // Save current window layout to asset file.
     // `assetPath` must be relative to project directory.
     public static void SaveLayoutToAsset(string assetPath) {
         SaveLayout(Path.Combine(Directory.GetCurrentDirectory(), assetPath));
     }
 
     // Load window layout from asset file.
     // `assetPath` must be relative to project directory.
     public static void LoadLayoutFromAsset(string assetPath) {
         if (_miLoadWindowLayout != null) {
             string path = Path.Combine(Directory.GetCurrentDirectory(), assetPath);
             _miLoadWindowLayout.Invoke(null, new object[] { path });
         }
     }
 
     // Save current window layout to file.
     // `path` must be absolute.
     public static void SaveLayout(string path) {
         if (_miSaveWindowLayout != null)
             _miSaveWindowLayout.Invoke(null, new object[] { path });
     }
 
 }



        Rect m_OrgPosition;
        Vector2 m_OrgSize;
        Vector2 m_OrgMaxSize;
        EImageSizeMode m_SizeMode = EImageSizeMode.Dynamic;
        public Vector2 size { get; private set; }

        //The size of the toolbar above the game view, excluding the OS border.
        private static int tabHeight = 22;

        static EditorWindow GetMainGameView()
        {
            EditorApplication.ExecuteMenuItem("Window/Game");

            System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            System.Reflection.MethodInfo GetMainGameView = T.GetMethod("GetMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            System.Object Res = GetMainGameView.Invoke(null, null);
            return (EditorWindow)Res;
        }

        public void FullScreenGameWindow( EImageSizeMode sizeMode, int width, int height )
        {
            m_SizeMode = sizeMode;
            var gameView = GetMainGameView();

            m_OrgPosition = gameView.position;
            m_OrgSize = gameView.minSize;
            m_OrgMaxSize = gameView.maxSize;
            switch (sizeMode)
            {
                case EImageSizeMode.Dynamic:
                {
                    size = new Vector2(gameView.position.width, gameView.position.height);
                    return;
                }
                case EImageSizeMode.FullScreen:
                {
                    width = Screen.currentResolution.width;
                    height = Screen.currentResolution.height;
                    break;
                }
                case EImageSizeMode.Width:
                {
                    height = (int)((height / (double)Screen.currentResolution.width) * width);
                    break;
                }
                case EImageSizeMode.Custom:
                    break;
            }

            var newPos = new Rect(0, 0 - tabHeight, Screen.currentResolution.width, Screen.currentResolution.height + tabHeight);
            gameView.position = newPos;
            size = new Vector2(width, height);
            gameView.minSize = size = new Vector2(width, height + tabHeight);;
            gameView.maxSize = gameView.minSize;
            gameView.position = newPos;
        }

        public void RestoreGameWindow()
        {
            if (m_SizeMode != EImageSizeMode.Dynamic)
            {
                var gameView = GetMainGameView();
                gameView.Close();
                gameView = GetMainGameView();
                gameView.minSize = m_OrgSize;
                gameView.maxSize = m_OrgMaxSize;
                gameView.position = m_OrgPosition;
            }
        }

    }
}