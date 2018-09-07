using SillyGames.TreasureHunt;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HuntHandler
{
    //private static HuntData[] s_hostedHunts = null;
    //private static HuntTemplate[] s_huntTemplates = null;

    public static void RetrieveHostedHunts(Action<HuntData[]> a_hostedHuntsCallback)
    {
        var huntData = new HuntData();
        huntData.DisplayName = "my Hunt1";
        huntData.ID = "001_001";
        huntData.TemplateID = "001";

        var huntDataArray = new HuntData[] { huntData };
        ///just a temp provision to call is async
        Utilities.WaitAndCall<HuntData[]>(1.5f,a_hostedHuntsCallback, huntDataArray);
    }

    public static void RetrieveHuntTemplates(Action<HuntTemplate[]> a_huntTemplatesCallback)
    {
        var hunTemplate1 = new HuntTemplate();
        hunTemplate1.ID = "001";
        hunTemplate1.Name = "template1";
        hunTemplate1.Description = "the first one";

        var hunTemplate2 = new HuntTemplate();
        hunTemplate2.ID = "002";
        hunTemplate2.Name = "template2";
        hunTemplate2.Description = "the second one";

        var templates = new HuntTemplate[] { hunTemplate1, hunTemplate2 };
        ///just a temp provision to call is async
        Utilities.WaitAndCall<HuntTemplate[]>(1.5f, a_huntTemplatesCallback, templates);
    }

    internal static void HostHunt(HuntTemplate a_huntTemplate)
    {
        var huntData = new HuntData();
        huntData.TemplateID = a_huntTemplate.ID;
        EditHunt(huntData);
    }
    
    internal static void EditHunt(HuntData a_huntData)
    {
        THGame.Instance.EditHunt(a_huntData);
    }

    internal static void RunTheHunt(HuntData a_huntDataToEdit)
    {
        THGame.Instance.OnHuntStarted(a_huntDataToEdit);
    }
}
