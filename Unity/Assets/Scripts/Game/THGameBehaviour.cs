using SillyGames.SGBase;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SillyGames.TreasureHunt
{
    public class THGameBehaviour : GameBehaviour<THGame>
    {
        //private void Awake()
        //{
        //    Debug.Log("Awake");
        //}
        private void Start()
        {
            MessageBox.Init(m_messageBoxPrefab,1);
        }

        [SerializeField]
        private MessageBoxUI m_messageBoxPrefab = null;


        void OnEnable()
        {
            DontDestroyOnLoad(this);
            SceneManager.sceneLoaded += THGame.Instance.OnSceneLoaded;            
        }

    }

    [Serializable]
    public class THGame : Game
    {
        public static new THGame Instance
        {
            get
            {
                return (THGame)Game.Instance;
            }
        }

        //public void Init()
        //{
        //    Debug.Log("Inited");
        //    //SceneManager.sceneLoaded += THGame.Instance.OnSceneLoaded;
        //}

       

        internal void OnSceneLoaded(Scene a_scene, LoadSceneMode a_loadSceneMode)
        {
            Debug.Log("Scene loaded1: " + a_scene.name + ", index: " + a_scene.buildIndex);
            if(a_scene.buildIndex == 0)
            {
                SceneManager.LoadScene(1);
            }
            else
            {
                if(!LoginHandler.IsLoggedIn)
                {
                    UIManager.Show<LoginUI>();
                }
                else
                {
                    Debug.Log("Time for a main menu");
                }
            }
        }

        internal void OnLogin(bool a_status)
        {
            if (a_status)
            {
                UIManager.Show<MainMenuUI>();
            }
            else
            {
                Debug.Log("Login failed!!");
            }
        }

        public static HuntData HuntDataToEdit { get; private set; }

        internal void OnHuntStarted(HuntData a_huntDataToEdit)
        {
            MessageBox.ShowOKCancel("Hunt Started", "Wallah, hunt is started!!", "Ok", "Cancel", (a_response) => 
            {
                Debug.Log("MB response was: " + a_response);
            });
        }

        internal void EditHunt(HuntData a_huntData)
        {
            HuntDataToEdit = a_huntData;
            UIManager.Show<EditHuntUI>();
        }

        internal void RunTheHunt()
        {
            var huntData = HuntDataToEdit;
            HuntDataToEdit = null;
            HuntHandler.RunTheHunt(huntData);
        }


    }

}