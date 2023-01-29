using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoSingleton<TurnManager>
{
    public const int ACTIONS_PER_TURN = 1;

    private int actionsTakenInTurn = 0;

    public void ActionTaken()
    {
        ++actionsTakenInTurn;
        if(actionsTakenInTurn >= ACTIONS_PER_TURN)
        {
            PlantManager.instance.Step();
            HopperManager.instance.Step();
        }
    }
}
