using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class SillyUIRegistration : MonoBehaviour {

    // Use this for initialization
    [SerializeField]
    private Button m_submitBtn;


	void Start ()
    {
        m_submitBtn.onClick.AddListener(OnClickSubmitBtn);
	}

    private void OnClickSubmitBtn()
    {
        //FiniteStateMachine.Instance.ChangeState(FSMState.EState.MainMenu);
    }

    // Update is called once per frame
    void Update () {
	
	}
}
