#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace UnityEngine.FrameRecorder.Input
{

    public class GameViewSize
    {
        static object m_InitialSizeObj;

        public static EditorWindow GetMainGameView()
        {
            System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            System.Reflection.MethodInfo GetMainGameView = T.GetMethod("GetMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            System.Object Res = GetMainGameView.Invoke(null, null);
            return (EditorWindow)Res;
        }


        public static void GetGameRenderSize(out int width, out int height)
        {
            var gameView = GetMainGameView();
            var prop = gameView.GetType().GetProperty("targetSize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var size = (Vector2)prop.GetValue(gameView, new object[0] { });
            width = (int)size.x;
            height = (int)size.y;
        }

        static object Group()
        {
            var T = System.Type.GetType("UnityEditor.GameViewSizes,UnityEditor");
            var sizes = T.BaseType.GetProperty("instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            var instance = sizes.GetValue(null, new object[0] { });

            var currentGroup = instance.GetType().GetProperty("currentGroup", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var group = currentGroup.GetValue(instance, new object[0] { });

            return group;
        }

        static int TotalCount()
        {
            var group = Group();
            var totalCount = group.GetType().GetMethod("GetTotalCount", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            return (int) totalCount.Invoke(group, null) ;
        }


        static object GetGameViewSize(object group, int index)
        {
            var obj = group.GetType().GetMethod("GetGameViewSize", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            return obj.Invoke(group, new object[] {index}) ;            
        }

        static void SizeOf(object gameViewSize, out int width, out int height)
        {
            width = (int)gameViewSize.GetType().GetProperty("width", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).GetValue(gameViewSize, new object[0] { });
            height = (int)gameViewSize.GetType().GetProperty("height", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).GetValue(gameViewSize, new object[0] { });
        }

        public static object SetCustomSize(int width, int height)
        {
            // Find recorder size object
            var sizeObj = FindRecorderSizeObj();
            if (sizeObj != null)
            {
                sizeObj.GetType().GetField("m_Width",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(sizeObj,width);
                sizeObj.GetType().GetField("m_Height", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(sizeObj,height);
            }
            else
            {
                sizeObj = AddSize(width, height);
            }

            return sizeObj;
        }


        private static object FindRecorderSizeObj()
        {
            var group = Group();

            var customs = group.GetType().GetField("m_Custom", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(group);

            var itr = (System.Collections.IEnumerator)customs.GetType().GetMethod("GetEnumerator").Invoke(customs, new object[] {});
            while (itr.MoveNext())
            {
                var txt = (string)itr.Current.GetType().GetField("m_BaseText", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(itr.Current);
                if (txt == "(Recording resolution)")
                    return itr.Current;
            }

            return null;
        }


        public static int IndexOf(object sizeObj)
        {
            var group = Group();
            var method = group.GetType().GetMethod("IndexOf", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            int index = (int)method.Invoke(group, new object[] {sizeObj}) ;

            var builtinList = group.GetType().GetField("m_Builtin", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(group);

            method = builtinList.GetType().GetMethod("Contains");
            if ((bool)method.Invoke(builtinList, new object[] { sizeObj }))
                return index;

            method = group.GetType().GetMethod("GetBuiltinCount");
            index += (int)method.Invoke(group, new object[] { });

            return index;
        }

        static object NewSizeObj(int width, int height)
        {
            var T = System.Type.GetType("UnityEditor.GameViewSize,UnityEditor");
            var sizeObj =Activator.CreateInstance(T, new object[] {1,width, height,  string.Format("FrameRecorder", width, height) });

            T.GetProperty("sizeType", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).SetValue(sizeObj,  1, new object[0] { });
            T.GetProperty("width", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).SetValue(sizeObj,  width, new object[0] { });
            T.GetProperty("height", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).SetValue(sizeObj,  height, new object[0] { });
            T.GetProperty("baseText", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).SetValue(sizeObj,  "(Recording resolution)", new object[0] { });

            return sizeObj;
        }

        public static object AddSize(int width, int height)
        {
            var sizeObj = NewSizeObj(width, height);

            var group = Group();
            var obj = group.GetType().GetMethod("AddCustomSize", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            obj.Invoke(group, new object[] {sizeObj}) ;

            return sizeObj;
        }

        public static void SelectSize(object size)
        {
            var index = IndexOf(size);

            var gameView = GetMainGameView();
            var obj = gameView.GetType().GetMethod("SizeSelectionCallback", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            obj.Invoke(gameView, new object[] { index, size });
        }

        public static object StoredInitialSize
        {
            get
            {
                return m_InitialSizeObj; 
            }
        }

        public static object currentSize
        {
            get
            {
                var gv = GetMainGameView();
                var prop = gv.GetType().GetProperty("currentGameViewSize", BindingFlags.NonPublic | BindingFlags.Instance);
                return prop.GetValue(gv, new object[0] { });
            }
        }

        public static void BackupCurrentSize()
        {
            m_InitialSizeObj = currentSize;
        }

        public static void RestoreSize()
        {
            SelectSize(m_InitialSizeObj);
            m_InitialSizeObj = null;
        }
    }
}

#endif