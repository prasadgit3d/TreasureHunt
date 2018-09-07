using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace SillyGames.SGBase.Localization
{
    public class AudioSourceLocalizer : AudioLocalization.LocalizerBehaviour<AudioSource>
    {
        public override void Set(AudioClip a_data)
        {
            if (Target != null)
            {
                Target.clip = a_data;
            }
        }
    }
}