using UnityEngine;
using System.Collections;
using SillyGames.SGBase.TaskSystem;

public class TasksTest : MonoBehaviour {

    public Task m_Ontask;
    public Task m_OffTask;

    void OnGUI()
    {
        if (GUI.Button(new Rect(50, 50, 100, 50), "On Task")) 
        {
            m_Ontask.Execute();    
        }

        if (GUI.Button(new Rect(200, 50, 100, 50), "Off Task"))
        {
            m_OffTask.Execute();
        }
    }
}
