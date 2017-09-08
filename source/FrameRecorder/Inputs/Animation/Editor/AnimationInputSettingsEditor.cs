using System;
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
                    
            }
            if (animImputSetting.gameObject != null)
            {
                var compos = animImputSetting.gameObject.GetComponents<UnityEngine.Component>()
                    .Where(x=>x!=null).Select(x => x.GetType())
                    .Distinct().ToList();
                var i = animImputSetting.bindingType == null ? 0 : Math.Max(compos.IndexOf(animImputSetting.bindingType), 0);
                i = EditorGUILayout.Popup(i, compos.Select(x=>x.Name).ToArray());
                animImputSetting.bindingTypeName = compos[i].AssemblyQualifiedName;
            }
               
            animImputSetting.enabled = EditorGUILayout.Toggle(animImputSetting.enabled);
            EditorGUILayout.EndHorizontal();
        }
    }
    

    
}