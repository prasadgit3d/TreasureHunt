using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using SillyGames.SGBase.Localization;
using System.Reflection;

namespace SillyGames.SGBase.Localization
{
    /// <summary>
    /// Generic localization editor.
    /// Generic Editor for all LocalizationObject instances
    /// </summary>
    [CustomEditor(typeof(LocalizationObject), true)]
    public class GenericLocalizationEditor : Editor
    {
        private System.Type T;
        private System.Type V;
        private LocalizationObject mFocusedLocalizationObject;
        private IGenericLocalizationEditor mEditorInstance;

        private void SpawnAndSetEditorInstance()
        {
            LocalizationObject locObjectToConsider = (target as LocalizationObject);
            if (locObjectToConsider == null)
            {
                return;
            }
            if (mFocusedLocalizationObject != locObjectToConsider && mEditorInstance != null)
            {
                //release the old object
                mEditorInstance.ReleaseEditor();
            }

            mFocusedLocalizationObject = locObjectToConsider;
            LocalizationObject baseObject = mFocusedLocalizationObject;
            System.Type type = typeof(GenericLocalizationEditorRenderer<,>);
            System.Type[] typeArgs = { T, V };
            System.Type makeme = type.MakeGenericType(typeArgs);
            mEditorInstance = (IGenericLocalizationEditor)System.Activator.CreateInstance(makeme);
            mEditorInstance.InitializeEditor(baseObject, this);
        }

        private void OnEnable()
        {
            LocalizationObject baseObject = (target as LocalizationObject);
            if (baseObject != null)
            {
                T = baseObject.KeyType();
                V = baseObject.DataType();
                SpawnAndSetEditorInstance();
                Repaint();
                return;
            }
        }

        public override void OnInspectorGUI()
        {
            if (mEditorInstance != null)
            {
                mEditorInstance.OnInspectorGUI();
            }
            else
            {
                base.OnInspectorGUI();
            }
        }
    }
}