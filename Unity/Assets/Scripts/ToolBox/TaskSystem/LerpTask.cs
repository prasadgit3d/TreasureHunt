using UnityEngine;
using System.Collections;

namespace SillyGames.SGBase.TaskSystem
{
    public class LerpTask : Task
    {
        [SerializeField]
        private bool m_useTimeToUpdate = true;

        public bool UseTimeToUpdate
        {
            get { return m_useTimeToUpdate; }
            set { m_useTimeToUpdate = value; }
        }
        
        [SerializeField]
        private float m_TimeToComplete = 1.0f;

        public float TimeToComplete
        {
            get
            {
                return m_TimeToComplete;
            }

            set
            {
                m_TimeToComplete = value;
            }
        }

        

        public enum WrapMode
        {
            Loop = 0,
            PingPong = 1
        };

        [SerializeField]
        private WrapMode m_wrapMode;

        public WrapMode Mode
        {
            get
            {
                return m_wrapMode;
            }

            set
            {
                m_wrapMode = value;
            }
        }

        private float m_currentValue = 0.0f;

        private float time = 0.0f;

        public float CurrentValue
        {
            get
            {
                return m_currentValue;
            }

            private set
            {
                m_currentValue = value;
            }
        }

        private float CurrentValueInternal
        {
            get
            {
                return m_currentValue;
            }
            set
            {
                CurrentValue = value;
                OnUpdate(CurrentValue);
            }
        }

        [SerializeField]
        private bool useAnimationCurve = false;

        [SerializeField]
        private AnimationCurveBehavior m_anim = null;

        public AnimationCurve Curve
        {
            get
            {
                if (m_anim != null)
                {
                    return m_anim.curve;
                }
                return null;
            }

            set
            {
                if (value == null)
                {
                    if (m_anim != null)
                    {
                        if (Application.isPlaying)
                        {
                            Destroy(m_anim);
                        }
                        else
                        {
                            DestroyImmediate(m_anim);
                        }
                    }
                }
                else
                {
                    if (m_anim == null)
                    {
                        m_anim = gameObject.AddComponent<AnimationCurveBehavior>();
                    }
                    m_anim.curve = value;
                }
            }
        }

        public bool IsUsingAnimationCurve
        {
            get
            {
                return useAnimationCurve;
            }
            set
            {
                if (useAnimationCurve != value)
                {
                    if (value)
                    {
                        if (Curve == null)
                        {
                            m_anim = GetComponent<AnimationCurveBehavior>();
                            if (m_anim == null)
                            {
                                Curve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
                            }
                        }
                    }
                    useAnimationCurve = value;
                }
            }
        }

        [SerializeField]
        private bool executeReverse = false;

        public bool IsExecutingReverse
        {
            get
            {
                return executeReverse;
            }

            set
            {
                executeReverse = value;
            }
        }

        public void ExecuteTask(WrapMode a_eWrapMode, int a_fTimeToComplete)
        {
            Mode = a_eWrapMode;
            m_TimeToComplete = a_fTimeToComplete;
            Execute();
        }

        public void Execute(bool a_bReversed)
        {
            IsExecutingReverse = a_bReversed;
            base.Execute();
        }

        protected override void OnStart()
        {
            if (UseTimeToUpdate)
            {
                time = 0.0f;
                UpdateUsingTime();
            }
            if (IsUsingAnimationCurve)
            {
                if (Curve == null)
                {
                    m_anim = GetComponent<AnimationCurveBehavior>();
                    if (m_anim == null)
                    {
                        IsUsingAnimationCurve = false;
                    }
                }
            }
            //CurrentValueInternal = IsUsingAnimationCurve ? Curve.Evaluate(initialValue) : initialValue;
        }

        protected override void OnEnd()
        {
            base.OnEnd();
        }
        
        protected override sealed void OnUpdate()
        {
            if (!IsPlaying)
            {
                return;
            }
            if (UseTimeToUpdate)
            {
                if (time >= 1.0f)
                {
                    Stop();
                }
                else
                {
                    time += Time.deltaTime / m_TimeToComplete;

                    if (time > 1.0f)
                    {
                        time = 1.0f;
                    }

                    UpdateUsingTime();
                }
            }
        }

        private void UpdateUsingTime()
        {
            time = m_TimeToComplete == 0.0f ? 1.0f : time;
            //float value = Mathf.LerpUnclamped(initialValue, targetValue, time);
            float value = time;

            if (Mode == WrapMode.PingPong)
            {
                value = value <= 0.5f ? Mathf.Lerp(0.0f, 1.0f, value / 0.5f) : Mathf.Lerp(1.0f, 0.0f, (value - 0.5f) / 0.5f);
            }

            CurrentValueInternal = IsUsingAnimationCurve ? Curve.Evaluate(value) : value;
            if (m_TimeToComplete == 0.0f)
            {
                Stop();
            }
        }

        protected virtual void OnUpdate(float a_fFraction) {}

        public void UpdateUsingFraction(float a_fFraction)
        {
            CurrentValueInternal = IsUsingAnimationCurve ? Curve.Evaluate(a_fFraction) : a_fFraction;
        }
        public void OnInspectorSliderUpdateInternal(float a_fFraction)
        {
            OnUpdate(IsExecutingReverse ? 1.0f - a_fFraction : a_fFraction);
        }
    }
}
