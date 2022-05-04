using UnityEngine;
using System.Collections;

public class PlayerControlController : MonoBehaviour, IPlayerControlController
{
    public PlayerManager playerManager;
    public FirstPersonLook look;
    public FirstPersonMovement move;

    public void FreeViewOff()
    {
        look.FreeViewOff();
    }

    public void FreeViewOn()
    {
        look.FreeViewOn();
    }

    public void LoseControl()
    {
        move.LoseControl();
    }

    public void GainControl()
    {
        move.GainControl();
    }
}
