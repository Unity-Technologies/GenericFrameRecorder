using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.FrameRecorder.Input;

namespace UnityEditor.FrameRecorder.Input
{
    [CustomEditor(typeof(AnimationInputSettings))]
    public class AnimationInputSettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var animImputSetting = target as AnimationInputSettings;;
            EditorGUILayout.BeginHorizontal();
                   
            EditorGUI.BeginChangeCheck();
            animImputSetting.gameObject = EditorGUILayout.ObjectField(animImputSetting.gameObject, typeof(GameObject), true) as GameObject;
            if (EditorGUI.EndChangeCheck())
            {
                animImputSetting.enabled = animImputSetting.gameObject != null;

                if (animImputSetting.gameObject != null)
                {
                    animImputSetting.bindingTypeName.Add(animImputSetting.gameObject.GetComponent<UnityEngine.Component>().GetType().AssemblyQualifiedName);
                }
            }

            if (animImputSetting.gameObject != null)
            {
                var compos = animImputSetting.gameObject.GetComponents<UnityEngine.Component>()
                    .Where(x => x != null).Select(x => x.GetType());
                if (animImputSetting.recursive)
                {
                    compos = compos.Union(animImputSetting.gameObject.GetComponentsInChildren<UnityEngine.Component>()
                        .Where(x => x != null).Select(x => x.GetType()));
                }
                
                compos = compos.Distinct().ToList();
                var compoNames = compos.Select(x => x.AssemblyQualifiedName).ToList();

                int flags = 0;
                foreach (var t in animImputSetting.bindingTypeName)
                {
                    var found = compoNames.IndexOf(t);
                    if (found != -1)
                        flags |= 1 << found;
                }
                EditorGUI.BeginChangeCheck();
                flags = EditorGUILayout.MaskField("", flags, compos.Select(x => x.Name).ToArray(),
                    GUILayout.MaxWidth(100));
                if (EditorGUI.EndChangeCheck())
                {
                    animImputSetting.bindingTypeName = new List<string>();
                    for (int i=0;i<compoNames.Count;++i)                               
                    {
                        if ((flags & (1 << i )) == 1 << i )
                        {
                            animImputSetting.bindingTypeName.Add(compoNames[i]);
                        }
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Recursive",GUILayout.MaxWidth(70));
            animImputSetting.recursive = EditorGUILayout.Toggle(animImputSetting.recursive,GUILayout.MaxWidth(30), GUILayout.MinWidth(30));   
            
            EditorGUILayout.LabelField("On",GUILayout.MaxWidth(35));
            animImputSetting.enabled = EditorGUILayout.Toggle(animImputSetting.enabled,GUILayout.MaxWidth(30), GUILayout.MinWidth(30));
            EditorGUILayout.EndHorizontal();
        }
    }
    

    
}