using UnityEngine;
using System.Collections.Generic;
using SillyGames.SGBase;

public class AudioClipSegmentObject : MonoBehaviour 
{
    [SerializeField]
    private AudioSource m_audio = null;

    [SerializeField]
    private string m_audioKey = string.Empty;

    [SerializeField]
    private bool m_loop = false;

    private float m_fTime = 0.0f;
	// Use this for initialization
	public void Start_() 
    {
        if(s_prefabInstance == null && this.m_loop == false)
        {
            s_prefabInstance = this;
        }
        m_fTime = SoundManager.Instance.GetAudioSegmentLength(m_audioKey);
        SoundManager.Instance.Play(m_audioKey,m_audio);
	}


	
	// Update is called once per frame
	void Update () 
    {
        m_fTime -= Time.deltaTime;
        if(m_fTime <= 0.0f)
        {
            if (!m_loop)
            {
                s_lstCache.Add(this);
                enabled = false;
                s_lstCache.Add(this);
            }
            else
            {
                Start_();
            }
        }
	}

    

    private static List<AudioClipSegmentObject> s_lstCache = new List<AudioClipSegmentObject>();
    private static AudioClipSegmentObject s_prefabInstance = null;

    public static void StartNew()
    {
        if(s_lstCache.Count > 0)
        {
            var obj = s_lstCache[s_lstCache.Count -1];
            s_lstCache.RemoveAt(s_lstCache.Count - 1);
            Activate(obj);
        }
        else
        {
            var obj = GameObject.Instantiate(s_prefabInstance);
            Activate(obj);
        }
    }

    static void Activate(AudioClipSegmentObject a_obj)
    {
        a_obj.enabled = true;
        a_obj.Start_();
    }
}
