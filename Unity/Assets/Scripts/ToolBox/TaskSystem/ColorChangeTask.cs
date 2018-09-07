using UnityEngine;
using System.Collections;
using SillyGames.SGBase.UI;

namespace SillyGames.SGBase.TaskSystem
{
    public class ColorChangeTask : LerpTask
    {
        [SerializeField]
        private Color startColor = Color.white;

        public Color StartColor
        {
            get
            {
                return startColor;
            }

            set
            {
                startColor = value;
            }
        }

        [SerializeField]
        private Color endColor = Color.white;

        public Color EndColor
        {
            get
            {
                return endColor;
            }

            set
            {
                endColor = value;
            }
        }

        [SerializeField]
        private UILabelBase target = null;

        public UILabelBase TargetTransform
        {
            get
            {
                return target;
            }

            set
            {
                target = value;
            }
        }

        [SerializeField]
        private bool snapOnStop = false;

        public bool SnapOnStop
        {
            get
            {
                return snapOnStop;
            }

            set
            {
                snapOnStop = value;
            }
        }

        [SerializeField]
        private bool setStartColorFromTarget = false;

        public bool SetStartColorFromTarget
        {
            get
            {
                return setStartColorFromTarget;
            }
            set
            {
                setStartColorFromTarget = value;
            }
        }

        protected override void OnStart()
        {
            if (SetStartColorFromTarget)
            {
                if (TargetTransform != null)
                {
                    StartColor = TargetTransform.CurrentColor;
                }
            }

            base.OnStart();
           
        }

        protected override void OnUpdate(float a_fFraction)
        {
            if (Application.isPlaying)
            {
                TargetTransform.CurrentColor = Color.Lerp(StartColor, EndColor, a_fFraction);
            }
            else
            {
                TargetTransform.CurrentColor = Color.Lerp(StartColor, EndColor, a_fFraction);
            }
        }

        protected override void OnEnd()
        {
            base.OnEnd();
            if (SnapOnStop)
            {
                TargetTransform.CurrentColor = EndColor;
            }
        }
    }
}
