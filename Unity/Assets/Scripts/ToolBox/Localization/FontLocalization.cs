using System;
using UnityEngine;

namespace SillyGames.SGBase.Localization
{
    [Serializable]
    public class FontData : LocalizableData<Font> { }
    public class FontLocalization : LocalizationBase<Font, FontData>{}
}