using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SillyGames.SGBase.Localization;
using System.IO;

namespace SillyGames.SGBase.Localization
{
    public class GenericLocalizationShipper<T, V> where V : LocalizableData<T>
    {

        [System.Serializable]
        private struct DataShippingFormat
        {
            public List<V> mData;
        }

        public bool mOverwrite;
        private string _path;
        public string mPath
        {
            get
            {
                return _path;
            }
            set
            {
                string exportPath = value;
                if (value != null)
                {
                    exportPath = exportPath.Replace("\\", "/");
                    exportPath = exportPath.Replace("//", "/");
                }
                _path = exportPath;
            }
        }

        public bool mIsPathValid
        {
            get
            {
                string path = mPath;
                if (path != null && path.Length > 0)
                {
                    if (path.Contains(".json") && path.Split('/').Length > 1)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public void ExportData(LocalizationBase<T, V> toExport)
        {
            List<V> rawData = toExport.m_rawData;
            DataShippingFormat format = new DataShippingFormat();
            //format.mKeyType = toExport.KeyType();
            //format.mValueType = toExport.DataType();
            format.mData = rawData;
            string json = JsonUtility.ToJson(format);
            Debug.Log(json);
            if (!mIsPathValid)
            {
                Debug.Log("File path specified is not valid. Please check path");
                return;
            }
            if (File.Exists(mPath) && !mOverwrite)
            {
                Debug.Log("File already exists, please check the overwrite toggle to overwrite");
                return;
            }

            File.WriteAllText(mPath, json);

        }

        public void ImportData(LocalizationBase<T, V> toImportInto)
        {
            if (mIsPathValid)
            {
                string json = File.ReadAllText(_path);
                DataShippingFormat format = JsonUtility.FromJson<DataShippingFormat>(json);
                if (toImportInto != null)
                {
                    List<V> newData = new List<V>();
                    List<V> existingData = new List<V>();
                    if (toImportInto.m_rawData != null && toImportInto.m_rawData.Count > 0)
                    {
                        for (int i = 0; i < format.mData.Count; ++i)
                        {
                            bool found = false;
                            for (int j = 0; j < toImportInto.m_rawData.Count; ++j)
                            {
                                if (format.mData[i].key == toImportInto.m_rawData[j].key && format.mData[i].group == toImportInto.m_rawData[j].group)
                                {
                                    existingData.Add(format.mData[i]);
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                            {
                                newData.Add(format.mData[i]);
                            }
                        }

                    }
                    else
                    {
                        newData = format.mData;
                    }

                    //add all new keys
                    if (newData != null && newData.Count > 0)
                    {
                        if (toImportInto.m_rawData == null)
                        {
                            toImportInto.m_rawData = new List<V>();
                        }
                        toImportInto.m_rawData.AddRange(newData);
                    }

                    if (existingData != null && existingData.Count > 0)
                    {
                        //modify all existing keys
                        for (int i = 0; i < existingData.Count; ++i)
                        {
                            //same key, same group, different data
                            V entry = toImportInto.m_rawData.Find((x) => x.key.Equals(existingData[i].key) && x.group.Equals(existingData[i].group));
                            if (entry != null)
                            {
                                Debug.LogWarning("Modifying entry in group \"" + existingData[i].group + "\" with key \"" + existingData[i].key + "\" from \"" + entry.data + "\" to \"" + existingData[i].data + "\"");
                                entry.data = existingData[i].data;
                            }
                        }
                    }

                }
            }
        }
    }
}
