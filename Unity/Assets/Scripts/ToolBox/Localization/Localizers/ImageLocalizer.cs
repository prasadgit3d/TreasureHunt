using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace SillyGames.SGBase.Localization
{
    public class ImageLocalizer : SpriteLocalization.LocalizerBehaviour<Image>
    {
        public override void Set(Sprite a_data)
        {
            if (Target != null)
            {
                Target.overrideSprite = a_data;
            }
        }

    }
}