using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SillyGames.SGBase.TaskSystem
{
    [ExecuteInEditMode]
    public class LerpTaskPreviewer : MonoBehaviour
    {
        [SerializeField]
        private List<LerpTask> m_TaskList = null;

        [SerializeField]
        private bool previewEnabled = false;

        [Range(0,1)]
        [SerializeField]
        private float m_Value = 0.0f;

        void Update()
        {
            if (previewEnabled)
            {
                foreach (var task in m_TaskList)
                {
                    task.OnInspectorSliderUpdateInternal(m_Value);
                }
            }
        }
    }
}
