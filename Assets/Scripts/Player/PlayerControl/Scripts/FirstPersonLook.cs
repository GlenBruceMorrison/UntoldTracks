using UnityEngine;

namespace UntoldTracks.CharacterController
{
    public class FirstPersonLook : MonoBehaviour, IFirstPersonController
    {
        [SerializeField]
        Transform character;
        public float sensitivity = 2;
        public float smoothing = 1.5f;

        Vector2 velocity;
        Vector2 frameVelocity;

        private bool _pointerLocked = false;


        void Reset()
        {
            // Get the character from the FirstPersonMovement in parents.
            character = GetComponentInParent<FirstPersonMovement>().transform;
        }

        void Start()
        {
            LockPointer();
        }

        void Update()
        {
            if (!_pointerLocked)
            {
                // Get smooth velocity.
                Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
                Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
                frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
                velocity += frameVelocity;
                velocity.y = Mathf.Clamp(velocity.y, -90, 90);

                // Rotate camera up-down and controller left-right from velocity.
                transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
                character.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);
            }
        }

        public bool IsPointerLocked()
        {
            return _pointerLocked;
        }

        public void LockPointer()
        {
            _pointerLocked = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void UnlockPointer()
        {
            _pointerLocked = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}