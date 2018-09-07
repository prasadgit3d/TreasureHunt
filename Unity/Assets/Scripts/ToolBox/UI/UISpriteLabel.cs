using UnityEngine;
using System.Collections;
using SillyGames.SGBase.UI;

namespace SillyGames.SGBase.UI
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class UISpriteLabel : UILabelBase
    {
        private SpriteRenderer m_spriteRenderer;

        public SpriteRenderer UnitySpriteRenderer
        {
            get
            {
                if (m_spriteRenderer == null)
                {
                    m_spriteRenderer = GetComponent<SpriteRenderer>();
                }
                return m_spriteRenderer;
            }
        }

        public override Color CurrentColor
        {
            get
            {
                return UnitySpriteRenderer.color;
            }
            set
            {
                UnitySpriteRenderer.color = value;
            }
        }
    }

}