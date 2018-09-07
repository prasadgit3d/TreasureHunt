using UnityEngine;
using System.Collections;


public class AudioSegmentShooter : MonoBehaviour 
{

    [SerializeField]
    private float m_fTimeDelay = 1.0f;
    
    [SerializeField]
    private float m_fCurrentTime = 0.0f;

    void Update()
    {
        m_fCurrentTime += Time.deltaTime;
        if(m_fCurrentTime >= m_fTimeDelay)
        {
            m_fCurrentTime = m_fCurrentTime - m_fTimeDelay;
            AudioClipSegmentObject.StartNew();
        }
    }
    void OnGUI()
    {
        GUILayout.Label("Playing..");    
    }
}
