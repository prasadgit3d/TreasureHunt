using System;
using UnityEngine;

namespace SillyGames.SGBase.Localization
{
    [Serializable]
    public class SpriteData : LocalizableData<Sprite> { }
    public class SpriteLocalization : LocalizationBase<Sprite, SpriteData> { }
}