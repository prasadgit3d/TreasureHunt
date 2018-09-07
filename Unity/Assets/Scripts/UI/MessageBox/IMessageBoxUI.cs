
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SillyGames.TreasureHunt
{
    public interface IMessageBoxUI
    {
        void Dispose();

        string LowerText { get; set; }
        Sprite Image { get; set; }
        string MainText { get; set; }
        string TitleText { get; set; }
        void AppendButton(string a_text, Action a_calback, bool a_bUseLocalization = false);
        void PrependButton(string a_text, Action a_calback, bool a_bUseLocalization = false);
        void Show();
        void AppendButton(MessageBox.ButtonArgs buttonArg, bool a_bUseLocalization);

        void PrependButton(MessageBox.ButtonArgs buttonArg, bool a_bUseLocalization);
    }
}
