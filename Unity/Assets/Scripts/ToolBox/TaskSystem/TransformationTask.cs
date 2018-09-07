using UnityEngine;
using System.Collections;

namespace SillyGames.SGBase.TaskSystem
{
    public class TransformationTask : LerpTask
    {
        [SerializeField]
        private Transform startTransform = null;

        public Transform StartTransform
        {
            get
            {
                return startTransform;
            }

            set
            {
                startTransform = value;
            }
        }
        
        [SerializeField]
        private Transform endTransform = null;

        public Transform EndTransform
        {
            get
            {
                return endTransform;
            }

            set
            {
                endTransform = value;
            }
        }

        [SerializeField]
        private Transform target = null;

        public Transform TargetTransform
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
        private bool enableTranlation = true;

        public bool IsTranslationEnabled
        {
            get 
            { 
                return enableTranlation; 
            }
            set 
            { 
                enableTranlation = value; 
            }
        }

        [SerializeField]
        private bool enableScaling = false;

        public bool IsScalingEnabled
        {
            get 
            { 
                return enableScaling; 
            }
            set 
            { 
                enableScaling = value; 
            }
        }

        [SerializeField]
        private bool enableRotation = false;

        public bool IsRotationEnabled
        {
            get 
            { 
                return enableRotation; 
            }
            set 
            { 
                enableRotation = value; 
            }
        }

        [SerializeField]
        private bool useDynamicTransforms = false;

        public bool IsUsingDynamicTransforms
        {
            get
            {
                return useDynamicTransforms;
            }

            set
            {
                useDynamicTransforms = value;
            }
        }

        [SerializeField]
        private Vector3 startPosition = Vector3.zero;
        
        [SerializeField]
        private Vector3 endPosition = Vector3.zero;
        
        [SerializeField]
        private Vector3 startScale = Vector3.one;
        
        [SerializeField]
        private Vector3 endScale = Vector3.one;
        
        [SerializeField]
        private Quaternion startRotation = Quaternion.identity;
        
        [SerializeField]
        private Quaternion endRotation = Quaternion.identity;

        public Vector3 StartPosition
        {
            get
            {
                return startPosition;
            }

            set
            {
                startPosition = value;
            }
        }

        public Vector3 EndPosition
        {
            get
            {
                return endPosition;
            }

            set
            {
                endPosition = value;
            }
        }

        public Vector3 StartScale
        {
            get
            {
                return startScale;
            }

            set
            {
                startScale = value;
            }
        }

        public Vector3 EndScale
        {
            get
            {
                return endScale;
            }

            set
            {
                endScale = value;
            }
        }

        public Quaternion StartRotation
        {
            get
            {
                return startRotation;
            }

            set
            {
                startRotation = value;
            }
        }

        public Quaternion EndRotation
        {
            get
            {
                return endRotation;
            }

            set
            {
                endRotation = value;
            }
        }

        protected override void OnStart()
        {
            if (StartTransform == null || EndTransform == null)
            {
                IsUsingDynamicTransforms = false;
            }

            if (StartTransform != null)
            {
                StartPosition = StartTransform.position;
                StartScale = StartTransform.localScale;
                StartRotation = StartTransform.rotation;
            }

            if (EndTransform != null)
            {
                EndPosition = EndTransform.position;
                EndScale = EndTransform.localScale;
                EndRotation = EndTransform.rotation;
            }
            base.OnStart();
        }
        
        protected override void OnUpdate(float a_fFraction)
        {
        
            if (IsUsingDynamicTransforms)
            {
                StartPosition = StartTransform.position;
                StartScale = StartTransform.localScale;
                StartRotation = StartTransform.rotation;

                EndPosition = EndTransform.position;
                EndScale = EndTransform.localScale;
                EndRotation = EndTransform.rotation;
            }

            if (IsTranslationEnabled)
            {
                TargetTransform.position = Vector3.LerpUnclamped(StartPosition, EndPosition, a_fFraction);
            }

            if (IsScalingEnabled)
            {
                TargetTransform.localScale = Vector3.LerpUnclamped(StartScale, EndScale, a_fFraction);
            }

            if (IsRotationEnabled)
            {
                TargetTransform.rotation = Quaternion.LerpUnclamped(StartRotation, EndRotation, a_fFraction);
            }
        }

        protected override void OnEnd()
        {
            if (IsUsingDynamicTransforms)
            {
                StartPosition = StartTransform.position;
                StartScale = StartTransform.localScale;
                StartRotation = StartTransform.rotation;

                EndPosition = EndTransform.position;
                EndScale = EndTransform.localScale;
                EndRotation = EndTransform.rotation;
            }

            if (SnapOnStop)
            {
                if (IsUsingAnimationCurve)
                {
                    EndPosition = Vector3.LerpUnclamped(StartPosition, EndPosition, Curve.Evaluate(1.0f));
                    EndScale = Vector3.LerpUnclamped(StartScale, EndScale, Curve.Evaluate(1.0f));
                    EndRotation = Quaternion.LerpUnclamped(StartRotation, EndRotation, Curve.Evaluate(1.0f));
                }

                if (IsTranslationEnabled)
                {
                    TargetTransform.position = EndPosition;
                }

                if (IsScalingEnabled)
                {
                    TargetTransform.localScale = EndScale;
                }

                if (IsRotationEnabled)
                {
                    TargetTransform.rotation = EndRotation;
                }
            }
            base.OnEnd();
        }
    }
}
