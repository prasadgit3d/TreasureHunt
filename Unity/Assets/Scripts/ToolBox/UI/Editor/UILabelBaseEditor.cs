using SillyGames.SGBase.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace SillyGames.SGBase.UI
{
    [CustomEditor(typeof(UILabelBase), true)]
    public class UILabelBaseEditor : UIElementEditor
    {
        protected new UILabelBase TargetElement
        {
            get
            {
                return target as UILabelBase;
            }
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Color color = EditorGUILayout.ColorField("Color:", TargetElement.CurrentColor);
            if (color != TargetElement.CurrentColor)
            {
                TargetElement.CurrentColor = color;
            }


            string[] channelText = new string[]
            {
                "R Channel",
                "G Channel",
                "B Channel",
                "A Channel"
            };

            string channelToolTip = "Multiplying factor for color channel. Value between 0 & 1 is darker than source color. Value >1 is brigther than source color.";

            GUILayout.BeginHorizontal();
            bool enableChannelEditing = TargetElement.ColorMultiplier[0] >= 0.0f || TargetElement.ColorMultiplier[1] >= 0.0f || TargetElement.ColorMultiplier[2] >= 0.0f || TargetElement.ColorMultiplier[3] >= 0.0f;
            bool enableAll = GUILayout.Toggle(enableChannelEditing, enableChannelEditing ? "Color Multiplication Enabled" : "Enable Color Multiplication");
            if (enableAll != enableChannelEditing)
            {
                if (enableAll)
                {
                    for (int i = 0; i < channelText.Length; i++)
                    {
                        TargetElement.SetColorMultiplier(i, 1.0f);
                    }
                }
                else
                {
                    for (int i = 0; i < channelText.Length; i++)
                    {
                        TargetElement.SetColorMultiplier(i, -1.0f);
                    }
                }
            }
            GUILayout.EndHorizontal();
            if (enableAll)
            {
                for (int i = 0; i < channelText.Length; i++)
                {
                    GUILayout.BeginHorizontal();
                    var channelEnabled = TargetElement.ColorMultiplier[i] >= 0.0f;
                    var strStart = channelEnabled ? " Enabled" : "Enable ";
                    var contentString = channelEnabled ? channelText[i] + strStart : strStart + channelText[i];
                    bool value = GUILayout.Toggle(channelEnabled, new GUIContent(contentString, channelToolTip));
                    if (value != channelEnabled)
                    {
                        if (value)
                        {
                            TargetElement.SetColorMultiplier(i, 1.0f);
                        }
                        else
                        {
                            TargetElement.SetColorMultiplier(i, -1.0f);
                        }
                    }
                    if (value)
                    {

                        float channelValue = EditorGUILayout.FloatField(String.Empty, TargetElement.ColorMultiplier[i]);
                        channelValue = Mathf.Clamp(channelValue, 0.0f, float.MaxValue);
                        TargetElement.SetColorMultiplier(i, channelValue);

                    }
                    GUILayout.EndHorizontal();
                }
            }
        }
    }
}
