using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SillyGames.TreasureHunt
{
    public class HuntTemplateListEntry : MonoBehaviour
    {
        [SerializeField]
        private Text m_nameText = null;

        [SerializeField]
        private Text m_descriptionText = null;

        public void ReadFrom(HuntTemplate a_template)
        {
            m_nameText.text = a_template.Name;
            m_descriptionText.text = a_template.Description;
            HuntTemplate = a_template;
        }

        public HuntTemplate HuntTemplate
        {
            get; private set;
        }
        public void OnClickHost()
        {
            UIManager.GetUIScreenOfType<HostHuntUI>().OnClickHost(this);
        }
    }
}