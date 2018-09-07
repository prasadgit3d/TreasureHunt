using System;
using UnityEngine;

namespace SillyGames.SGBase.Localization
{
    [Serializable]
    public class AudioData : LocalizableData<AudioClip> { }
    public class AudioLocalization : LocalizationBase<AudioClip, AudioData> { }
}