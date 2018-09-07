using System;
using UnityEngine;

namespace SillyGames.SGBase.Localization
{
    public static class LocalizationManager
    {
        private static GameObject s_localizationDataObject = null;
        //public static void LoadLanguage(string a_strLanguage)
        //{
        //    var strLanguagePath = GameProfileHandler.CurrentGameProfile.BaseURL + "/languages/" + a_strLanguage;
        //    AssetBundleManager.DownloadAssetBundle(strLanguagePath,LoadFromAssetBundle);
        //}

        //private static void OnLanguageAssetBundleDownloaded(string a_strID, AssetBundle a_assetBundle)
        //{
        //    if(a_assetBundle != null)
        //    {
        //        var data = a_assetBundle.LoadAllAssets();

        //        if (data != null)
        //        {
        //            //distroying old localization object
        //            if (s_localizationDataObject != null)
        //            {
        //                GameObject.Destroy(s_localizationDataObject);
        //            }
        //            s_localizationDataObject = GameObject.Instantiate(data[0]) as GameObject;
        //        }

        //        a_assetBundle.Unload(true);

        //    }
        //}

        public static void LoadLanguageFromAssetBundle(AssetBundle a_assetBundle)
        {
            if (a_assetBundle != null)
            {
                var data = a_assetBundle.LoadAllAssets();

                if (data != null)
                {
                    //destroying old localization object
                    if (s_localizationDataObject != null)
                    {
                        GameObject.Destroy(s_localizationDataObject);
                    }
                    s_localizationDataObject = GameObject.Instantiate(data[0]) as GameObject;
                }

                a_assetBundle.Unload(true);

            }
        }
    }
}