using UnityEngine;
using System.Collections;
using SillyGames.SGBase.TaskSystem;

namespace SillyGames.SGBase.TaskSystem
{
    public class TriggerBasedTask : Task
    {
        [SerializeField]
        private Collider m_targetCollier;

        public Collider TargetCollider
        {
            get { return m_targetCollier; }
            set { m_targetCollier = value; }
        }

        private Collider m_hitCollider;

        public Collider HitCollider
        {
            get { return m_hitCollider; }
            set { m_hitCollider = value; }
        }
        
        protected override void OnStart()
        {
            if (TargetCollider == null)
            {
                Debug.LogWarning("Target Collider is null, stopping task.");
                Stop();
            }

            if (HitCollider == null)
            {
                HitCollider = GetComponent<Collider>();
            }
        }

        void OnTriggerEnter(Collider collider)
        {
            if (TargetCollider == collider)
            {
                Debug.Log("Trigger");
                Stop();
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.GetComponent<Collider>() == TargetCollider)
            {
                Debug.Log("Collider");
                Stop();
            }
        }
        protected override void OnEnd()
        {
            Debug.Log("Trigger Based Task Ended");
        }
    }
}
