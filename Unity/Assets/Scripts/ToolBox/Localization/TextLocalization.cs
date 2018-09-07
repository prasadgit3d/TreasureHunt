using System;
using UnityEngine;

namespace SillyGames.SGBase.Localization
{
    [Serializable]
    public class TextData : LocalizableData<string> { }
    public class TextLocalization : LocalizationBase<string, TextData> { }
}