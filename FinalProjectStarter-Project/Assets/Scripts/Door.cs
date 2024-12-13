using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EDoorType : byte
{
    Unknown,
    Exit,
    Entrance,
    TwoWay
}

public class Door : MonoBehaviour
{
    public Door counterpart;
    public EDoorType type;

    public void DoorAction()
    {
        if (type == EDoorType.Exit)
        {
            Game.Instance.NextRoom(counterpart);
        }
        if (type == EDoorType.TwoWay)
        {
            Game.Instance.NextRoom(counterpart);
        }
    }
}
