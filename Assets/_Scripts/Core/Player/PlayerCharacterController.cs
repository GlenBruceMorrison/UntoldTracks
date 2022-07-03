using UnityEngine;
using System.Collections;
using UntoldTracks.Player;
using KinematicCharacterController.Examples;
using KinematicCharacterController;
using UntoldTracks.Managers;

namespace UntoldTracks.CharacterController
{
    public class PlayerCharacterController : MonoBehaviour
    {
        public PlayerBody Character;
        public ExampleCharacterCamera CharacterCamera;

        private const string MouseXInput = "Mouse X";
        private const string MouseYInput = "Mouse Y";
        private const string MouseScrollInput = "Mouse ScrollWheel";
        private const string HorizontalInput = "Horizontal";
        private const string VerticalInput = "Vertical";

        private bool _isLocked = false;

        public bool IsMoving
        {
            get
            {
                return Character.Motor.Velocity.x != 0 || Character.Motor.Velocity.z != 0;
            }
        }

        public bool IsPointerLocked()
        {
            return !(Cursor.lockState == CursorLockMode.Locked);
        }

        public bool IsFrozen
        {
            get
            {
                return _isLocked && IsPointerLocked();
            }
        }

        #region Life Cycle
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;

            // Tell camera to follow transform
            CharacterCamera.SetFollowTransform(Character.CameraFollowPoint);
        }

        private void Update()
        {
            if (!_isLocked)
            {
                HandleCharacterInput();
            }
        }

        private void LateUpdate()
        {
            // Handle rotating the camera along with physics movers
            if (CharacterCamera.RotateWithPhysicsMover && Character.Motor.AttachedRigidbody != null)
            {
                CharacterCamera.PlanarDirection = Character.Motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation * CharacterCamera.PlanarDirection;
                CharacterCamera.PlanarDirection = Vector3.ProjectOnPlane(CharacterCamera.PlanarDirection, Character.Motor.CharacterUp).normalized;
            }

            HandleCameraInput();
        }
        #endregion

        private void HandleCameraInput()
        {
            // Create the look input vector for the camera
            float mouseLookAxisUp = Input.GetAxisRaw(MouseYInput);
            float mouseLookAxisRight = Input.GetAxisRaw(MouseXInput);
            Vector3 lookInputVector = new(mouseLookAxisRight, mouseLookAxisUp, 0f);

            // Prevent moving the camera while the cursor isn't locked
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                lookInputVector = Vector3.zero;
            }

            // Input for zooming the camera (disabled in WebGL because it can cause problems)
            float scrollInput = -Input.GetAxis(MouseScrollInput);
#if UNITY_WEBGL
        scrollInput = 0f;
#endif

            // Apply inputs to the camera
            CharacterCamera.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);

            // Handle toggling zoom level
            if (Input.GetMouseButtonDown(1))
            {
                CharacterCamera.TargetDistance = (CharacterCamera.TargetDistance == 0f) ? CharacterCamera.DefaultDistance : 0f;
            }
        }

        private void HandleCharacterInput()
        {
            PlayerCharacterInputs characterInputs = new()
            {
                // Build the CharacterInputs struct
                MoveAxisForward = Input.GetAxisRaw(VerticalInput),
                MoveAxisRight = Input.GetAxisRaw(HorizontalInput),
                CameraRotation = CharacterCamera.Transform.rotation,
                JumpDown = Input.GetKeyDown(KeyCode.Space),
                CrouchDown = Input.GetKeyDown(KeyCode.C),
                CrouchUp = Input.GetKeyUp(KeyCode.C)
            };

            // Apply inputs to character
            Character.SetInputs(ref characterInputs);
        }

        public void LockPointer()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void UnlockPointer()
        {
            Cursor.lockState = CursorLockMode.None;
        }

        public void Freeze()
        {
            UnlockPointer();
            _isLocked = true;
        }

        public void UnFreeze()
        {
            LockPointer();
            _isLocked = false;
        }
    }
}