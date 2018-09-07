using SillyGames.SGBase.UI;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace SillyGames.SGBase.UI
{
    [CustomEditor(typeof(UIButton), true)]
    class UIButtonEditor : ButtonEditor
    {
        protected UIButton TargetElement
        {
            get
            {
                return target as UIButton;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            OnButtonInspectorGUI();
        }

        protected virtual void OnButtonInspectorGUI()
        {
            DrawDefaultInspector();
        }
    }
}
