using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinHuntDataEntry : MonoBehaviour
{
    [SerializeField]
    private HuntData m_huntData;

    public HuntData HuntData
    {
        get { return m_huntData; }
        set { m_huntData = value; }
    }

    public void UpdateUIWithData()
    {

    }


}
