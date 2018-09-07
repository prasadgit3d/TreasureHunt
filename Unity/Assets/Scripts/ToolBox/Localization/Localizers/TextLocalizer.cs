using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace SillyGames.SGBase.Localization
{
    public class TextLocalizer : TextLocalization.LocalizerBehaviour<Text>
    {
        public override void Set(string a_data)
        {
            if(Target != null)
            {
                Target.text = a_data;
            }
        }
    }
}