using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace SillyGames.SGBase.Localization
{
    public class TextFontLocalizer : FontLocalization.LocalizerBehaviour<Text>
    {
        public override void Set(Font a_data)
        {
            if (Target != null)
            {
                Target.font = a_data;
            }
        }
    }
}