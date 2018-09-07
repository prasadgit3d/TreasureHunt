using UnityEngine;
using SillyGames.SGBase.UI;

public class FadingEffectExecuterTest : MonoBehaviour {

    [SerializeField]
    private UILabel m_label = null;

    public UILabel UILabel
    {
        get
        {
            return m_label;
        }
        set
        {
            m_label = value;
        }
    }

    [SerializeField]
    public Color startColor = Color.white;

    [SerializeField]
    public Color endColor = Color.white;
    void OnGUI()
    {
        float alpha = GUI.HorizontalSlider(new Rect(50, 50, 100, 50), UILabel.CurrentColor.a, 0.0f, 1.0f);
        UILabel.SetColorAndPropagate(Color.Lerp(startColor, endColor, alpha));
    }
}
