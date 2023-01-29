using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoSingleton<ScoreManager>
{
    private int score = 0;
    public int Score
    {
        get { return score; }
        set
        {
            score = value;
            UpdateUIValues();
        }
    }

    public Text text;
    protected override void OnCreation()
    {
        base.OnCreation();
        UpdateUIValues();
    }

    private void UpdateUIValues()
    {
        text.text = string.Format("{0}", Score);
    }
}
