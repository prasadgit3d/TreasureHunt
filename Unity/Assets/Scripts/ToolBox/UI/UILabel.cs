#if UNITY_EDITOR
using UnityEditor;
#endif

using System;
using UnityEngine;
using SillyGames.SGBase.UI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SillyGames.SGBase.UI
{
    [RequireComponent(typeof(UnityEngine.UI.Graphic))]
    public class UILabel : UILabelBase
    {
        private UnityEngine.UI.Graphic m_unityImage = null;
        public UnityEngine.UI.Graphic UnityGraphic
        {
            get
            {
                if (m_unityImage == null)
                {
                    m_unityImage = GetComponent<UnityEngine.UI.Graphic>();
                }
                return m_unityImage;
            }
        }

        public override Color CurrentColor {
            get
            {
                return UnityGraphic.color;
            }
            set 
            {
                UnityGraphic.color = value;
            }
        }

        public Sprite Sprite
        {
            get
            {
                var image = UnityGraphic as Image;
                return image != null ? image.sprite : null;
            }
            set
            {
                var image = UnityGraphic as Image;
                if(image != null)
                {
                    image.sprite = value;
                }
                else
                {
                    Debug.LogWarning("Sprite could not be assigned, the Image component was not found on this object: " + this + " to assign sprite: " + value);
                }
            }
        }

        public Texture Texture
        {
            get
            {
                var rawImage = UnityGraphic as RawImage;
                return rawImage != null ? rawImage.texture : null;
            }
            set
            {
                var rawImage = UnityGraphic as RawImage;
                if (rawImage != null)
                {
                    rawImage.texture = value;
                }
                else
                {
                    Debug.LogWarning("Texture could not be assigned, the RawImage component was not found on this object: " + this + " to assign texture: " + value);
                }
            }
        }

        public string Text
        {
            get
            {
                var text = UnityGraphic as Text;
                if (text != null)
                {
                    return text.text;
                }
                else
                {
                    Debug.LogWarning("Text could not be assigned, the Text component was not found on this object: " + this);
                    return string.Empty;
                }
            }
            set
            {
                var text = UnityGraphic as Text;
                if(text != null)
                {
                    text.text = value;
                    return;
                }
                else
                {
                    Debug.LogWarning("Text could not be assigned, the Text component was not found on this object: " + this + " to assign Text: " + value);
                }
            }
        }

        public Text TextComponent
        {
            get
            {
                return UnityGraphic as Text;
            }
        }

        public Image ImageComponent
        {
            get
            {
                return UnityGraphic as Image;
            }
        }

        public RawImage RawImageComponent
        {
            get
            {
                return UnityGraphic as RawImage;
            }
        }
    }    
}
