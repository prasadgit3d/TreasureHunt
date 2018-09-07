using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SillyGames.SGBase.TaskSystem;

namespace SillyGames.SGBase.TaskSystem 
{
    public class GraphicAssignerTask : Task
    {

        [SerializeField]
        private Sprite m_targetSprite = null;

        [SerializeField]
        private Texture m_targetTexture = null;

        [SerializeField]
        private Image m_targetImage = null;

        [SerializeField]
        private RawImage m_targetRawImage = null;

        public Sprite TargetSprite
        {
            get
            {
                return m_targetSprite;
            }
            set
            {
                m_targetSprite = value;
            }
        }

        public Texture TargetTexture
        {
            get
            {
                return m_targetTexture;
            }
            set
            {
                m_targetTexture = value;
            }
        }

        public Image TargetImage
        {
            get
            {
                return m_targetImage;
            }
            set
            {
                m_targetImage = value;
            }
        }

        public RawImage TargetRawImage
        {
            get
            {
                return m_targetRawImage;
            }
            set
            {
                m_targetRawImage = value;
            }
        }

        protected override void OnStart()
        {
            if (TargetImage != null && TargetSprite != null)
            {
                TargetImage.sprite = TargetSprite;
            }

            if (TargetRawImage != null && TargetTexture != null)
            {
                TargetRawImage.texture = TargetTexture;
            }
            Stop();
        }
    }   
}
