using UnityEngine;

public interface IFirstPersonLook
{
    public bool IsPointerLocked();
    public void LockPointer();
    public void UnlockPointer();
}
