using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepButton : MonoBehaviour
{
    public void Step()
    {
        TurnManager.instance.ActionTaken();
    }
}
