using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SillyGames.SGBase
{
	/// <summary>
	/// Various helpers which could not be specifically categorized.
	/// </summary>
	public static class Misc
	{
		// Salt for the vault methods - can't come up with anything more than security-through-obscurity for device side encryption at this point!
		private static string m_salt;

		// Use this for initialization
		static Misc()
		{
			// just use a hash of device UID
            string saltSeed = SystemInfo.deviceUniqueIdentifier;
			m_salt = saltSeed.GetHashMD5();
			return;
		}
		
        //// Update is called once per frame
        //void Update() 
        //{
        //    return;
        //}

		/// <summary>
		/// Stores a value after encrypting in persistent storage.
		/// </summary>
		/// <param name="aKey">The key to use.</param>
		/// <param name="aValue">The value to store.</param>
		public static void setToVault(string aKey, string aValue)
		{
			SillyGames.SGBase.Framework.SecurePlayerPrefs.SetString(aKey, aValue, m_salt);
			return;
		}
		
		/// <summary>
		/// Gets a value from the vault for the specified key.
		/// </summary>
		/// <returns>The value if the key was found, or null.</returns>
		/// <param name="aKey">A key.</param>
		public static string getFromVault(string aKey)
		{
			return( SillyGames.SGBase.Framework.SecurePlayerPrefs.GetString(aKey, m_salt) );
		}

        // Delegates for getWebContent()
        public delegate void CallbackURLGet(bool aSuccess, WWW aReqObject);
        // Holder class for required data
        private class URLRequest
        {
            public string m_reqURL = string.Empty;
            public CallbackURLGet m_callback = null;
        }
        //// Private coroutine for the actual download
        //// Has 302 (Redirect support)
        //private IEnumerator executeDownload(URLRequest aRequest)
        //{
        //    Debug.Log("Utility: getURL: Starting for " + aRequest.m_reqURL);
        //    WWW reqObj = new WWW(aRequest.m_reqURL);

        //    yield return reqObj;

        //    if (reqObj.error != null)
        //    {
        //        // ERROR!
        //        Debug.LogError("Utility: getURL: " + aRequest.m_reqURL + ": " + reqObj.error);
        //        aRequest.m_callback(false, reqObj);
        //    }
        //    else
        //    {
        //        // NOTE: Stupid WWW class doesnt even store the HTTP status code!!!!
        //        // check if there is a redirection by parsing for the "Location" header
        //        if (reqObj.responseHeaders.ContainsKey("LOCATION"))
        //        {
        //            // queue another fetch!
        //            string redirectLocation = reqObj.responseHeaders["LOCATION"];
        //            Debug.Log("Utility: getURL: Redirecting " + aRequest.m_reqURL + " to " + redirectLocation);
        //            Instance.getURL(redirectLocation, aRequest.m_callback);
        //        }
        //        else
        //        {
        //            // we are done!
        //            Debug.Log("Utility: getURL: Completed for " + aRequest.m_reqURL);
        //            aRequest.m_callback(true, reqObj);
        //        }
        //    }
        //}

        // Gets content from a web URL using Unity's WWW class
        //public void getURL(string aURL, CallbackURLGet aCallback)
        //{
        //    // create the object and set it's parameters
        //    URLRequest req = new URLRequest();
        //    req.m_reqURL = aURL;
        //    req.m_callback = aCallback;
        //    // start the coroutine
        //    StartCoroutine(executeDownload(req));
        //    return;
        //}

        public static void MailTo(string a_to = "", string a_subject = "", string a_body = "", string a_cc = "")
        {
            Application.OpenURL("mailto:" + FormatURL(a_to) + "?subject=" + FormatURL(a_subject) + "&body=" + FormatURL(a_body) + "&cc=" + FormatURL(a_cc));
        }
        public static string FormatURL(string a_strURL)
        {
            return System.Text.RegularExpressions.Regex.Replace(a_strURL, @"(\r\n|\n|\r)", "%0D%0A").Replace("\t", "%20%20%20%20").Replace(" ", "%20");
        }

        public static string ConvertToCurrencyString(decimal a_fValue)
        {
            string strValueToReturn = string.Empty;
            string strPrefix;
            if (a_fValue >= 1000000000000)
            {
                strValueToReturn = (a_fValue / 1000000000000).ToString(".00");
                strPrefix = "T";
            }
            else if (a_fValue >= 1000000000)
            {
                var value = (a_fValue / 1000000000);
                strValueToReturn = value.ToString(".00");
                strPrefix = "B";
            }
            else if (a_fValue >= 1000000)
            {
                strValueToReturn = (a_fValue / 1000000).ToString(".00");
                strPrefix = "M";
            }
            else if(a_fValue >= 1000)
            {
                strValueToReturn = (a_fValue / 1000).ToString(".00");
                strPrefix = "K";
            }
            else
            {
                strValueToReturn = a_fValue.ToString();
                strPrefix = string.Empty;
            }
            strValueToReturn = strValueToReturn.Replace(".00", string.Empty);
            return DigitGroup3(strValueToReturn) + strPrefix;
        }

        public static string ConvertToIndianCurrencyString(int a_fValue)
        {
            string strValueRoReturn = string.Empty;
            if (a_fValue >= 10000000.0f)
            {
                strValueRoReturn = (a_fValue / 10000000.0f).ToString(".00") + "Cr";
            }
            else if (a_fValue > 100000.0f)
            {
                strValueRoReturn = (a_fValue / 100000.0f).ToString(".00") + "L";
            }
            else if (a_fValue > 1000.0f)
            {
                strValueRoReturn = (a_fValue / 1000.0f).ToString(".00") + "K";
            }
            else
            {
                strValueRoReturn = a_fValue.ToString();
            }
            return DigitGroup3(strValueRoReturn);
        }

        public static string DigitGroup3(string a_strInput)
        {
            if (string.IsNullOrEmpty(a_strInput))
                return a_strInput;

            int iIndexOfDot = a_strInput.LastIndexOf('.');
            string strLeftOfDot = string.Empty;
            string strRightOfDot = string.Empty;
            //string str
            if (iIndexOfDot != -1)
            {
                strLeftOfDot = a_strInput.Substring(0, iIndexOfDot); ;
                strRightOfDot = a_strInput.Substring(iIndexOfDot);
            }
            else
            {
                strLeftOfDot = a_strInput;
            }

            for (int i = (strLeftOfDot.Length - 3); i > 0; i -= 3)
            {
                strLeftOfDot = strLeftOfDot.Insert(i, ",");
            }
            return strLeftOfDot + strRightOfDot;
        }

        public static void DumpRecursive(IList a_list, int a_iTabIndex = 0)
        {
            string strIndent = new string('\t', a_iTabIndex);
            Debug.Log(strIndent + "List-> Count: " + a_list.Count);
            a_iTabIndex++;
            foreach (var item in a_list)
            {
                Debug.Log(strIndent + "\t" + item);
                var list = item as IList;
                if (list != null)
                {
                    DumpRecursive(list, a_iTabIndex + 1);
                    continue;
                }

                var dictionary = item as IDictionary;
                if (dictionary != null)
                {
                    DumpRecursive(dictionary, a_iTabIndex + 1);
                    continue;
                }
            }
        }

        public static void DumpRecursive(IDictionary a_dictionary, int a_iTabIndex = 0)
        {
            string strIndent = new string('\t', a_iTabIndex);
            Debug.Log(strIndent + "Dictionary-> Count: " + a_dictionary.Count);
            a_iTabIndex++;
            foreach (DictionaryEntry entry in a_dictionary)
            {

                var item = entry.Value;
                string str = strIndent + '\t' + entry.Key.ToString() + ": " + item;
                Debug.Log(str);
                var list = item as IList;
                if (list != null)
                {
                    DumpRecursive(list, a_iTabIndex + 1);
                    continue;
                }
                var dictionary = item as IDictionary;
                if (dictionary != null)
                {
                    DumpRecursive(dictionary, a_iTabIndex + 1);
                    continue;
                }

            }
        }

        public static void DumpRecursive<K, V>(IDictionary<K, V> a_dictionary, int a_iTabIndex = 0)
        {
            string strIndent = new string('\t', a_iTabIndex);
            Debug.Log(strIndent + "Dictionary<" + typeof(K) + "," + typeof(V) + "> => Count: " + a_dictionary.Count);
            a_iTabIndex++;
            foreach (var entry in a_dictionary)
            {

                var item = entry.Value;
                string str = strIndent + '\t' + entry.Key.ToString() + ": " + item;
                Debug.Log(str);
                var list = item as IList;
                if (list != null)
                {
                    DumpRecursive(list, a_iTabIndex + 1);
                    continue;
                }
                var dictionary = item as IDictionary;
                if (dictionary != null)
                {
                    DumpRecursive(dictionary, a_iTabIndex + 1);
                    continue;
                }

            }
        }

        public static int NullCompare(System.Object o1, System.Object o2)
        {
            return (o1 != null).CompareTo(o2 != null);
        }
	}
	
}

