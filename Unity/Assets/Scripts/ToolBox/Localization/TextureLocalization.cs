using System;
using UnityEngine;

namespace SillyGames.SGBase.Localization
{
    [Serializable]
    public class TextureData : LocalizableData<Texture> { }
    public class TextureLocalization : LocalizationBase<Texture, TextureData> { }
}