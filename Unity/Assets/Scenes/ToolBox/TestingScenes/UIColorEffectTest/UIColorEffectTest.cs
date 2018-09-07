using UnityEngine;
using System.Collections;
using SillyGames.SGBase.UI;

public class UIColorEffectTest : MonoBehaviour {

    public UILabel Label;
    public Color Color;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Label.SetColorAndPropagate(Color);
	}
}
