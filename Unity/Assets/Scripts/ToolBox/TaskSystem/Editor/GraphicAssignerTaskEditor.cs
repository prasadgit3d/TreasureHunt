using UnityEngine;
using System.Collections;
using SillyGames.SGBase.TaskSystem;
using UnityEditor;
using UnityEngine.UI;

namespace SillyGames.SGBase.TaskSystem
{
    [CustomEditor(typeof(GraphicAssignerTask), true)]
    public class GraphicAssignerTaskEditor : TaskBaseEditor
    {
        protected new GraphicAssignerTask TargetElement
        {
            get
            {
                return target as GraphicAssignerTask;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUI.changed = false;

            TargetElement.TargetImage = (Image)EditorGUILayout.ObjectField("Target Image", TargetElement.TargetImage, typeof(Image), true);
            TargetElement.TargetSprite = (Sprite)EditorGUILayout.ObjectField("Target Sprite", TargetElement.TargetSprite, typeof(Sprite), true);

            TargetElement.TargetRawImage = (RawImage)EditorGUILayout.ObjectField("Target RawImage", TargetElement.TargetRawImage, typeof(RawImage), true);
            TargetElement.TargetTexture = (Texture)EditorGUILayout.ObjectField("Target Texture", TargetElement.TargetTexture, typeof(Texture), true);
        }
    }
}
