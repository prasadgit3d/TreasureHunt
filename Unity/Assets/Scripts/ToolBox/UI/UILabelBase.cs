using UnityEngine;
using System.Collections;
using SillyGames.SGBase.UI;
using System;
using UnityEngine.UI;

namespace SillyGames.SGBase.UI
{
    public abstract class UILabelBase : UIElement, IUILabel
    {
        public abstract Color CurrentColor
        {
            get;
            set;
        }

        public float ColorMultiplierR
        {
            get
            {
                return m_ColorMultiplier.x;
            }

            set
            {
                m_ColorMultiplier.x = value;
            }
        }

        public float ColorMultiplierG
        {
            get
            {
                return m_ColorMultiplier.y;
            }

            set
            {
                m_ColorMultiplier.y = value;
            }
        }

        public float ColorMultiplierB
        {
            get
            {
                return m_ColorMultiplier.z;
            }

            set
            {
                m_ColorMultiplier.z = value;
            }
        }

        public float ColorMultiplierA
        {
            get
            {
                return m_ColorMultiplier.w;
            }

            set
            {
                m_ColorMultiplier.w = value;
            }
        }

        [SerializeField]
        private Vector4 m_ColorMultiplier = Vector4.one * -1;

        public Vector4 ColorMultiplier
        {
            get
            {
                return m_ColorMultiplier;
            }

            set
            {
                m_ColorMultiplier = value;
            }
        }

        void Reset()
        {
            SetColorMultiplier(3, CurrentColor.a);
        }

        public void SetColorMultiplier(int a_index, float a_value)
        {
            m_ColorMultiplier[a_index] = a_value;
        }

        public void SetColorAndPropagate(Color a_color)
        {
            MultiplyAndUpdateColor( a_color );
        }

        private void MultiplyAndUpdateColor(Color a_color)
        {
            Color currentColor = CurrentColor;
            for (int i = 0; i < 4; i++)
            {
                if (ColorMultiplier[i] >= 0.0f)
                {
                    currentColor[i] = a_color[i] * ColorMultiplier[i];
                }
            }
            CurrentColor = currentColor;
            IUILabel iUILabel;
            foreach (var item in Children)
            {
                iUILabel = ((IUILabel)item);
                if (iUILabel != null)
                {
                    iUILabel.MultiplyAndUpdateColor(a_color);
                }
            }
        }

        void IUILabel.MultiplyAndUpdateColor(Color a_color)
        {
            MultiplyAndUpdateColor(a_color);
        }
    }
}
