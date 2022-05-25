using UnityEngine;

namespace UntoldTracks.Player
{
    public abstract class PlayerComponent : MonoBehaviour
    {
        protected PlayerManager _playerManager;

        internal void InternalInit(PlayerManager playerManager)
        {
            _playerManager = playerManager;
            Initiate();
        }
        
        //internal void InternalBeforeWake() => BeforeWake();
        //internal void InternalBeforeSleep() => BeforeSleep();
        internal void InternalRun(float deltaTime) => Run(deltaTime);
        internal void InternalLateRun() => LateRun();
        
        protected virtual void Initiate() { }
        //protected virtual void BeforeWake() { }
        //protected virtual void BeforeSleep() { }
        protected virtual void Run(float deltaTime) { }
        protected virtual void LateRun() { }
    }
}