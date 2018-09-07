using UnityEngine;
using System.Collections;
using SillyGames.SGBase.UI;

namespace SillyGames.SGBase.UI
{
    [RequireComponent(typeof(ParticleSystem))]
    public class UIParticleLabel : UILabelBase
    {
        private ParticleSystem m_UnityParticle = null;

        public ParticleSystem UnityPatricle
        {
            get
            {
                if (m_UnityParticle == null)
                {
                    m_UnityParticle = GetComponent<ParticleSystem>();
                }
                return m_UnityParticle;
            }
        }

        public override Color CurrentColor
        {
            get
            {
                return UnityPatricle.main.startColor.color;
            }

            set
            {
                var main = UnityPatricle.main;
                main.startColor = value;
            }
        }
    }
}
