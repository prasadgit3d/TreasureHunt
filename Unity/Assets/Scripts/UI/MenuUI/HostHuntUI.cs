using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SillyGames.TreasureHunt
{
    public class HostHuntUI : UIScreen
    {
        [SerializeField]
        private HuntTemplateListEntry m_entryPrefab = null;

        [SerializeField]
        private Transform m_entryParentContainer = null;

        protected internal override void ShowInternal()
        {
            base.ShowInternal();
            HuntHandler.RetrieveHuntTemplates(OnHuntTemplatesRetrieved);
        }

        private void OnHuntTemplatesRetrieved(HuntTemplate[] a_huntTemplates)
        {
            PopulateHuntTemplates(a_huntTemplates);
        }

        private void PopulateHuntTemplates(HuntTemplate[] a_huntTemplates)
        {
            for (int i = 0; i < a_huntTemplates.Length; i++)
            {
                var entry = GameObject.Instantiate<HuntTemplateListEntry>(m_entryPrefab);
                entry.transform.SetParent(m_entryParentContainer, false);
                entry.ReadFrom(a_huntTemplates[i]);
            }
        }

        internal void OnClickHost(HuntTemplateListEntry a_huntTemplateListEntry)
        {
            HuntHandler.HostHunt(a_huntTemplateListEntry.HuntTemplate);
        }
    }
}