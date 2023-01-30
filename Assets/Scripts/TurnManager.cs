using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoSingleton<TurnManager>
{
    public const int ACTIONS_PER_TURN = 1;

    public Text text;
    public int ActionsTakenInTurn
    {
        get => actionsTakenInTurn;
        protected set
        {
            actionsTakenInTurn = value;
            UpdateUIValues();
        }
    }
    private int actionsTakenInTurn = 0;

    protected override void OnCreation()
    {
        base.OnCreation();
        UpdateUIValues();
    }

    public void ActionTaken()
    {
        ++ActionsTakenInTurn;
        if(actionsTakenInTurn >= ACTIONS_PER_TURN)
        {
            PlantManager.instance.Step();
            HopperManager.instance.Step();
            ActionsTakenInTurn = 0;
        }
    }

    private void UpdateUIValues()
    {
        text.text = string.Format("{0}/{1}", ACTIONS_PER_TURN - actionsTakenInTurn, ACTIONS_PER_TURN);
    }
}
