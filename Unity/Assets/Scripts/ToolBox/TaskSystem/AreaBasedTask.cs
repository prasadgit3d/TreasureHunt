using UnityEngine;
using System.Collections;
using SillyGames.SGBase.TaskSystem;

namespace SillyGames.SGBase.TaskSystem
{
    public class AreaBasedTask : Task
    {
        [SerializeField]
        private Transform m_target = null;

        public Transform TargetTransform
        {
            get
            {
                return m_target;
            }
            set
            {
                m_target = value;
            }
        }

        [SerializeField]
        private Transform m_hitPoint;

        public Transform HitPoint
        {
            get { return m_hitPoint; }
            set { m_hitPoint = value; }
        }

        [SerializeField]
        private float m_hitPointRadius = 0.0f;

        public float HitPointRadius
        {
            get
            {
                return m_hitPointRadius;
            }
            set
            {
                m_hitPointRadius = value;
            }
        }

        protected override void OnStart()
        {
            if (TargetTransform == null || HitPoint == null)
            {
                Debug.LogWarning("Either Target or Hit Point is null, stopping task.");
                Stop();
            }
        }
        protected override void OnUpdate()
        {
            if (Mathf.Abs(Vector3.Distance(TargetTransform.position, HitPoint.position)) <= HitPointRadius)
            {
                Stop();
            }
        }

        protected override void OnEnd()
        {
            Debug.Log("Area Based Task Ended");
        }
    }
}
