using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreBoardController : MonoBehaviour
{
    public TMP_Text Team1Text;
    public TMP_Text Team2Text;

    public void Init()
    {
        // Bind OnScoreChange(...) to the script
    }

    void OnScoreChange(int? score1 = null, int? score2 = null)
    {
        if (score1 is int value1)
        {
            Team1Text.SetText(FormatScore(value1));
        }
        if (score2 is int value2)
        {
            Team1Text.SetText(FormatScore(value2));
        }
    }

    private void OnDestroy()
    {
        // Unbind OnScoreChange(...) from the script
    }

    private string FormatScore(int value)
    {
        value = Mathf.Min(99, value);
        string text = value.ToString();
        return text.PadLeft(Mathf.Max(0, 2 - text.Length), '0');
    }
}
