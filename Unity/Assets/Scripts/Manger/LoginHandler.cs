using SillyGames.TreasureHunt;
using System;
using System.Collections;
using UnityEngine;

public static class LoginHandler
{
    public static bool IsLoggedIn { get; private set; }

    public static void Login(string a_userName, string a_pwd)
    {
        LoginHandlerImplementer.Instance.StartCoroutine(LoginHandlerImplementer.WaitAndCall(1.0f, (bool a_bStatus)=> 
        {
            IsLoggedIn = a_bStatus;
            var player = new PlayerProfile();
            player.Name = "Prasad";
            player.ID = "1234";
            PlayerProfile.LocalPlayer = player;

            THGame.Instance.OnLogin(a_bStatus);
        }, true));        
    }

    
    /// <summary>
    /// a temporary implementation for login mechanism
    /// </summary>
    private class LoginHandlerImplementer : MonoBehaviour
    {
        private static LoginHandlerImplementer s_instnace = null;
        public static LoginHandlerImplementer Instance
        {
            get
            {
                if(s_instnace == null)
                {
                    var go = new GameObject("LoginHandlerImplementer", typeof(LoginHandlerImplementer));
                    s_instnace = go.GetComponent<LoginHandlerImplementer>();
                }
                return s_instnace;
            }
        }
        public static IEnumerator WaitAndCall(float a_fWaitDuration, Action<bool> a_callback, bool a_value)
        {
            yield return new WaitForSeconds(a_fWaitDuration);
            a_callback(a_value);
        }
    }
}
