using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace SillyGames.TreasureHunt
{

    public class MainMenuUI : UIScreen
    {
        [SerializeField]
        private Button m_createHuntBtn;
        [SerializeField]
        private Button m_joinHuntBtn;
        [SerializeField]
        private Button m_HostHuntBtn;
        [SerializeField]
        private Button m_settingBtn;
        [SerializeField]
        private Button m_quitBtn;
        // Use this for initialization
        void Start()
        {
            m_createHuntBtn.onClick.AddListener(OnClickCreateBtn);
            m_joinHuntBtn.onClick.AddListener(OnClickJoinHuntBtn);
            m_HostHuntBtn.onClick.AddListener(OnClickHostHuntBtn);
            m_settingBtn.onClick.AddListener(OnClickSettingBtn);
            m_quitBtn.onClick.AddListener(OnClickQuitBtn);
        }


        private void OnClickQuitBtn()
        {

        }

        private void OnClickSettingBtn()
        {

        }

        private void OnClickJoinHuntBtn()
        {
            //FiniteStateMachine.Instance.ChangeState(FSMState.EState.GameLobby);
        }

        private void OnClickHostHuntBtn()
        {
            UIManager.Show<HostHuntUI>();
        }

        private void OnClickCreateBtn()
        {
            Debug.Log("______________sending custom request 'game'");
            //NetworkManager.Instance.SendCustomRequestToCreateHunt();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}