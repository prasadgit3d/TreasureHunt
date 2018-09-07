using UnityEngine;
using System.Collections;
using SillyGames.SGBase.UI;

namespace SillyGames.SGBase.UI
{
    [RequireComponent(typeof(Renderer))]
    public class UIRendererLabel : UILabelBase
    {
        private Renderer m_Renderer;

        [SerializeField]
        private string colorPropertyName = "_Color";

        public Renderer UnityRenderer
        {
            get
            {
                if (m_Renderer == null)
                {
                    m_Renderer = GetComponent<Renderer>();
                }
                return m_Renderer;
            }
        }

        public override Color CurrentColor
        {
            get
            {
                if (Application.isPlaying)
                {
                    return UnityRenderer.material.GetColor(colorPropertyName);
                }
                else
                {
                    return UnityRenderer.sharedMaterial.GetColor(colorPropertyName);
                }
            }

            set
            {
                if (Application.isPlaying)
                {
                    UnityRenderer.material.SetColor(colorPropertyName, value);
                }
                else
                {
                    UnityRenderer.sharedMaterial.SetColor(colorPropertyName, value);
                }
            }
        }
    }
}
