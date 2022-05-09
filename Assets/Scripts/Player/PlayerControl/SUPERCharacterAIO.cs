//Original Code Author: Aedan Graves

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
#endif

namespace UntoldTracks.CharacterController
{
    [RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(CapsuleCollider))]
    public class SUPERCharacterAIO : MonoBehaviour, IFirstPersonController
    {
        #region Variables

        public bool controllerPaused = false;

        #region Camera Settings
        [Header("Camera Settings")]
        //
        //Public
        //
        //Both
        public Camera playerCamera;
        public bool  enableCameraControl = true, lockAndHideMouse = true, autoGenerateCrosshair = true, showCrosshairIn3rdPerson = false, drawPrimitiveUI = false;
        public Sprite crosshairSprite;
        public PerspectiveModes cameraPerspective = PerspectiveModes._1stPerson;
        //use mouse wheel to switch modes. (too close will set it to fps mode and attempting to zoom out from fps will switch to tps mode)
        public bool automaticallySwitchPerspective = true;
        #if ENABLE_INPUT_SYSTEM
        public Key perspectiveSwitchingKey = Key.Q;
        #else
        public KeyCode perspectiveSwitchingKey_L = KeyCode.None;
        #endif

        public MouseInputInversionModes mouseInputInversion;
        public float Sensitivity = 8;
        public float rotationWeight = 4;
        public float verticalRotationRange = 170.0f;
        public float standingEyeHeight = 0.8f;
        public float crouchingEyeHeight = 0.25f;

        //First person
        public ViewInputModes viewInputMethods;
        public float FOVKickAmount = 10; 
        public float FOVSensitivityMultiplier = 0.74f;

        //Third Person
        public bool rotateCharaterToCameraForward = false;
        public float maxCameraDistance = 8;
        public LayerMask cameraObstructionIgnore = -1;
        public float cameraZoomSensitivity = 5; 
        public float bodyCatchupSpeed = 2.5f;
        public float inputResponseFiltering = 2.5f;



        //
        //Internal
        //
    
        //Both
        Vector2 MouseXY;
        Vector2 viewRotVelRef;
        bool isInFirstPerson, isInThirdPerson, perspecTog;
        bool setInitialRot = true;
        Vector3 initialRot;
        Image crosshairImg;
        Image stamMeter, stamMeterBG;
        Image statsPanel, statsPanelBG;
        Image HealthMeter, HydrationMeter, HungerMeter;
        Vector2 normalMeterSizeDelta = new Vector2(175,12), normalStamMeterSizeDelta = new Vector2(330,5);
        float internalEyeHeight;

        //First Person
        float initialCameraFOV, FOVKickVelRef, currentFOVMod;

        //Third Person
        float mouseScrollWheel, maxCameraDistInternal, currentCameraZ, cameraZRef;
        Vector3 headPos, headRot, currentCameraPos, cameraPosVelRef;
        Quaternion quatHeadRot;
        Ray cameraObstCheck;
        RaycastHit cameraObstResult;
        [Space(20)]
        #endregion

        #region Movement
        [Header("Movement Settings")]
    
        //
        //Public
        //
        public bool enableMovementControl = true;

        //Walking/Sprinting/Crouching
        [Range(1.0f,650.0f)]public float walkingSpeed = 140, sprintingSpeed = 260, crouchingSpeed = 45;
        [Range(1.0f,400.0f)] public float decelerationSpeed=240;
        #if ENABLE_INPUT_SYSTEM
        public Key sprintKey = Key.LeftShift, crouchKey = Key.LeftCtrl, slideKey = Key.V;
        #else
        public KeyCode sprintKey_L = KeyCode.LeftShift, crouchKey_L = KeyCode.LeftControl, slideKey_L = KeyCode.V;
        #endif
        public bool canSprint=true, isSprinting, toggleSprint, sprintOveride, canCrouch=true, isCrouching, toggleCrouch, crouchOverride, isIdle;
        public Stances currentStance = Stances.Standing;
        public float stanceTransitionSpeed = 5.0f, crouchingHeight = 0.80f;
        public GroundSpeedProfiles currentGroundMovementSpeed = GroundSpeedProfiles.Walking;
        public LayerMask whatIsGround =-1;

        //Slope affectors
        public float hardSlopeLimit = 70, slopeInfluenceOnSpeed = 1, maxStairRise = 0.25f, stepUpSpeed=0.2f;

        //Jumping
        public bool canJump=true,holdJump=false, jumpEnhancements=true, Jumped;
        #if ENABLE_INPUT_SYSTEM
            public Key jumpKey = Key.Space;
        #else
            public KeyCode jumpKey_L  = KeyCode.Space;
        #endif
        [Range(1.0f,650.0f)] public float jumpPower = 40;
        [Range(0.0f,1.0f)] public float airControlFactor = 1;
        public float decentMultiplier = 2.5f, tapJumpMultiplier = 2.1f;
        float jumpBlankingPeriod;

        //Sliding
        public bool isSliding, canSlide = true;
        public float slidingDeceleration = 150.0f, slidingTransisionSpeed=4, maxFlatSlideDistance =10;
    

        //
        //Internal
        //

        //Walking/Sprinting/Crouching
        public GroundInfo currentGroundInfo = new GroundInfo();
        float standingHeight;
        float currentGroundSpeed;
        Vector3 InputDir;
        float HeadRotDirForInput;
        Vector2 MovInput;
        Vector2 MovInput_Smoothed;
        Vector2 _2DVelocity;
        float _2DVelocityMag, speedToVelocityRatio;
        PhysicMaterial _ZeroFriction, _MaxFriction;
        CapsuleCollider capsule;
        Rigidbody p_Rigidbody;
        bool crouchInput_Momentary, crouchInput_FrameOf, sprintInput_FrameOf,sprintInput_Momentary, slideInput_FrameOf, slideInput_Momentary;
        bool changingStances = false; 

        //Slope Affectors

        //Jumping
        bool jumpInput_Momentary, jumpInput_FrameOf;

        //Sliding
        Vector3 cachedDirPreSlide, cachedPosPreSlide;

        [Space(20)]
        #endregion
    
        #region Stamina System
        //Public
        public bool enableStaminaSystem = true, jumpingDepletesStamina = true;
        [Range(0.0f,250.0f)]public float Stamina = 50.0f, currentStaminaLevel = 0, s_minimumStaminaToSprint = 5.0f, s_depletionSpeed = 2.0f,  s_regenerationSpeed = 1.2f, s_JumpStaminaDepletion = 5.0f;
    
        //Internal
        bool staminaIsChanging;
        bool ignoreStamina = false;
        #endregion
    
        #region Footstep System
        [Header("Footstep System")]
        public bool enableFootstepSounds = true;
        public FootstepTriggeringMode footstepTriggeringMode = FootstepTriggeringMode.calculatedTiming;
        [Range(0.0f,1.0f)] public float stepTiming = 0.15f;
        public List<GroundMaterialProfile> footstepSoundSet = new List<GroundMaterialProfile>();
        bool shouldCalculateFootstepTriggers= true;
        float StepCycle = 0;
        AudioSource playerAudioSource;
        List<AudioClip> currentClipSet = new List<AudioClip>();
        [Space(18)]
        #endregion
    
        #region  Headbob
        //
        //Public
        //
        public bool enableHeadbob = true;
        [Range(1.0f,5.0f)] public float headbobSpeed = 0.5f, headbobPower = 0.25f;
        [Range(0.0f,3.0f)] public float ZTilt = 3;

        //
        //Internal
        //
        bool shouldCalculateHeadbob;
        Vector3 headbobCameraPosition;
        float headbobCyclePosition, headbobWarmUp;
        #endregion
    
        #region  Survival Stats
        //
        //Public
        //
        public bool enableSurvivalStats = true;
        public SurvivalStats defaultSurvivalStats = new SurvivalStats();
        public float statTickRate = 6.0f, hungerDepletionRate = 0.06f, hydrationDepletionRate = 0.14f;
        public SurvivalStats currentSurvivalStats = new SurvivalStats();

        //
        //Internal
        //
        float StatTickTimer;
        #endregion

        #region Animation
        //
        //Pulbic
        //

        //Firstperson
        public Animator _1stPersonCharacterAnimator;
        //ThirdPerson
        public Animator _3rdPersonCharacterAnimator;
        public string a_velocity, a_2DVelocity, a_Grounded, a_Idle, a_Jumped, a_Sliding, a_Sprinting, a_Crouching;
        public bool stickRendererToCapsuleBottom = true;

        #endregion
    
        [Space(18)]
        public bool enableGroundingDebugging = false, enableMovementDebugging = false, enableMouseAndCameraDebugging = false, enableVaultDebugging = false;
        #endregion

        void Start()
        {
            #region Camera
            maxCameraDistInternal = maxCameraDistance;
            initialCameraFOV = playerCamera.fieldOfView;
            headbobCameraPosition = Vector3.up*standingEyeHeight;
            internalEyeHeight = standingEyeHeight;
            if(lockAndHideMouse){
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if(autoGenerateCrosshair || drawPrimitiveUI){
                    Canvas canvas = playerCamera.gameObject.GetComponentInChildren<Canvas>();
                    if(canvas == null){canvas = new GameObject("AutoCrosshair").AddComponent<Canvas>();}
                    canvas.gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    canvas.pixelPerfect = true;
                    canvas.transform.SetParent(playerCamera.transform);
                    canvas.transform.position = Vector3.zero;
                if(autoGenerateCrosshair && crosshairSprite){
                    crosshairImg = new GameObject("Crosshair").AddComponent<Image>();
                    crosshairImg.sprite = crosshairSprite;
                    crosshairImg.rectTransform.sizeDelta = new Vector2(25,25);
                    crosshairImg.transform.SetParent(canvas.transform);
                    crosshairImg.transform.position = Vector3.zero;
                    crosshairImg.raycastTarget = false;
                }
                if(drawPrimitiveUI){
                    //Stam Meter BG
                    stamMeterBG = new GameObject("Stam BG").AddComponent<Image>();
                    stamMeterBG.rectTransform.sizeDelta = normalStamMeterSizeDelta;
                    stamMeterBG.transform.SetParent(canvas.transform);
                    stamMeterBG.rectTransform.anchorMin = new Vector2(0.5f,0);
                    stamMeterBG.rectTransform.anchorMax = new Vector2(0.5f,0);
                    stamMeterBG.rectTransform.anchoredPosition = new Vector2(0,22);
                    stamMeterBG.color = Color.gray;
                    stamMeterBG.gameObject.SetActive(enableStaminaSystem);
                    //Stam Meter
                    stamMeter = new GameObject("Stam Meter").AddComponent<Image>();
                    stamMeter.rectTransform.sizeDelta = normalStamMeterSizeDelta;
                    stamMeter.transform.SetParent(canvas.transform);
                    stamMeter.rectTransform.anchorMin = new Vector2(0.5f,0);
                    stamMeter.rectTransform.anchorMax = new Vector2(0.5f,0);
                    stamMeter.rectTransform.anchoredPosition = new Vector2(0,22);
                    stamMeter.color = Color.white;
                    stamMeter.gameObject.SetActive(enableStaminaSystem);
                    //Stats Panel
                    statsPanel = new GameObject("Stats Panel").AddComponent<Image>();
                    statsPanel.rectTransform.sizeDelta = new Vector2(3,45);
                    statsPanel.transform.SetParent(canvas.transform);
                    statsPanel.rectTransform.anchorMin = new Vector2(0,0);
                    statsPanel.rectTransform.anchorMax = new Vector2(0,0);
                    statsPanel.rectTransform.anchoredPosition = new Vector2(12,33);
                    statsPanel.color = Color.clear;
                    statsPanel.gameObject.SetActive(enableSurvivalStats);
                    //Stats Panel BG
                    statsPanelBG = new GameObject("Stats Panel BG").AddComponent<Image>();
                    statsPanelBG.rectTransform.sizeDelta = new Vector2(175,45);
                    statsPanelBG.transform.SetParent(statsPanel.transform);
                    statsPanelBG.rectTransform.anchorMin = new Vector2(0,0);
                    statsPanelBG.rectTransform.anchorMax = new Vector2(1,0);
                    statsPanelBG.rectTransform.anchoredPosition = new Vector2(87,22);
                    statsPanelBG.color = Color.white*0.5f;
                    //Health Meter
                    HealthMeter = new GameObject("Health Meter").AddComponent<Image>();
                    HealthMeter.rectTransform.sizeDelta = normalMeterSizeDelta;
                    HealthMeter.transform.SetParent(statsPanel.transform);
                    HealthMeter.rectTransform.anchorMin = new Vector2(0,0);
                    HealthMeter.rectTransform.anchorMax = new Vector2(1,0);
                    HealthMeter.rectTransform.anchoredPosition = new Vector2(87,6);
                    HealthMeter.color =new Color32(211,0,0, 255);
                    //Hydration Meter
                    HydrationMeter = new GameObject("Hydration Meter").AddComponent<Image>();
                    HydrationMeter.rectTransform.sizeDelta = normalMeterSizeDelta;
                    HydrationMeter.transform.SetParent(statsPanel.transform);
                    HydrationMeter.rectTransform.anchorMin = new Vector2(0,0);
                    HydrationMeter.rectTransform.anchorMax = new Vector2(1,0);
                    HydrationMeter.rectTransform.anchoredPosition = new Vector2(87,22);
                    HydrationMeter.color =new Color32(0,194,255, 255);
                    //Hunger Meter
                    HungerMeter = new GameObject("Hunger Meter").AddComponent<Image>();
                    HungerMeter.rectTransform.sizeDelta = normalMeterSizeDelta;
                    HungerMeter.transform.SetParent(statsPanel.transform);
                    HungerMeter.rectTransform.anchorMin = new Vector2(0,0);
                    HungerMeter.rectTransform.anchorMax = new Vector2(1,0);
                    HungerMeter.rectTransform.anchoredPosition = new Vector2(87,38);
                    HungerMeter.color = new Color32(142,54,0, 255);
                
                }
            }
            if(cameraPerspective == PerspectiveModes._3rdPerson && !showCrosshairIn3rdPerson){
                crosshairImg?.gameObject.SetActive(false);
            }
            initialRot = transform.localEulerAngles;
            #endregion 

            #region Movement
            p_Rigidbody = GetComponent<Rigidbody>();
            capsule = GetComponent<CapsuleCollider>();
            standingHeight = capsule.height;
            currentGroundSpeed = walkingSpeed;
            _ZeroFriction = new PhysicMaterial("Zero_Friction");
            _ZeroFriction.dynamicFriction =0f;
            _ZeroFriction.staticFriction =0;
            _ZeroFriction.frictionCombine = PhysicMaterialCombine.Minimum;
            _ZeroFriction.bounceCombine = PhysicMaterialCombine.Minimum;
            _MaxFriction = new PhysicMaterial("Max_Friction");
            _MaxFriction.dynamicFriction =1;
            _MaxFriction.staticFriction =1;
            _MaxFriction.frictionCombine = PhysicMaterialCombine.Maximum;
            _MaxFriction.bounceCombine = PhysicMaterialCombine.Average;
            #endregion

            #region Stamina System
            currentStaminaLevel = Stamina;
            #endregion
        
            #region Footstep
            playerAudioSource = GetComponent<AudioSource>();
            #endregion
        
        }

        public Carriage curCar;

        void Update(){
            if (Physics.Raycast(transform.position - Vector3.up*0.2f, -Vector3.up, out RaycastHit hit))
            {
                if (hit.transform.TryGetComponent(out Carriage carr))
                {
                    if (curCar != carr)
                    {
                        curCar = carr;
                        transform.parent = curCar.transform;
                    }
                }
            }

            if (!controllerPaused){
            #region Input
            #if ENABLE_INPUT_SYSTEM
                MouseXY.x = Mouse.current.delta.y.ReadValue()/50;
                MouseXY.y = Mouse.current.delta.x.ReadValue()/50;
            
                mouseScrollWheel = Mouse.current.scroll.y.ReadValue()/1000;
                if(perspectiveSwitchingKey!=Key.None)perspecTog = Keyboard.current[perspectiveSwitchingKey].wasPressedThisFrame;
                if(interactKey!=Key.None)interactInput = Keyboard.current[interactKey].wasPressedThisFrame;
                //movement

                 if(jumpKey!=Key.None)jumpInput_Momentary =  Keyboard.current[jumpKey].isPressed;
                 if(jumpKey!=Key.None)jumpInput_FrameOf =  Keyboard.current[jumpKey].wasPressedThisFrame;

                 if(crouchKey!=Key.None){
                    crouchInput_Momentary =  Keyboard.current[crouchKey].isPressed;
                    crouchInput_FrameOf = Keyboard.current[crouchKey].wasPressedThisFrame;
                 }
                 if(sprintKey!=Key.None){
                    sprintInput_Momentary = Keyboard.current[sprintKey].isPressed;
                    sprintInput_FrameOf = Keyboard.current[sprintKey].wasPressedThisFrame;
                 }
                 if(slideKey != Key.None){
                    slideInput_Momentary = Keyboard.current[slideKey].isPressed;
                    slideInput_FrameOf = Keyboard.current[slideKey].wasPressedThisFrame;
                 }
                #if SAIO_ENABLE_PARKOUR
                vaultInput = Keyboard.current[VaultKey].isPressed;
                #endif
                MovInput.x = Keyboard.current.aKey.isPressed ? -1 : Keyboard.current.dKey.isPressed ? 1 : 0;
                MovInput.y = Keyboard.current.wKey.isPressed ? 1 : Keyboard.current.sKey.isPressed ? -1 : 0;
            #else
                //camera
                MouseXY.x = Input.GetAxis("Mouse Y");
                MouseXY.y = Input.GetAxis("Mouse X");
                mouseScrollWheel = Input.GetAxis("Mouse ScrollWheel");
                perspecTog = Input.GetKeyDown(perspectiveSwitchingKey_L);
                //movement

                jumpInput_Momentary = Input.GetKey(jumpKey_L);
                jumpInput_FrameOf = Input.GetKeyDown(jumpKey_L);
                crouchInput_Momentary = Input.GetKey(crouchKey_L);
                crouchInput_FrameOf = Input.GetKeyDown(crouchKey_L);
                sprintInput_Momentary = Input.GetKey(sprintKey_L);
                sprintInput_FrameOf = Input.GetKeyDown(sprintKey_L);
                slideInput_Momentary = Input.GetKey(slideKey_L);
                slideInput_FrameOf = Input.GetKeyDown(slideKey_L);
                #if SAIO_ENABLE_PARKOUR

                vaultInput = Input.GetKeyDown(VaultKey_L);
                #endif
                MovInput = Vector2.up *Input.GetAxisRaw("Vertical") + Vector2.right * Input.GetAxisRaw("Horizontal");
#endif
                #endregion

                #region Camera
                if(enableCameraControl)
                {
                HeadbobCycleCalculator();
                FOVKick();
                RotateView(MouseXY, Sensitivity, rotationWeight);
                if(setInitialRot){
                    setInitialRot = false;
                    RotateView(initialRot,false);
                    InputDir = transform.forward;
                }
            }
            
        
            if(currentStance == Stances.Standing && !changingStances){
                internalEyeHeight = standingEyeHeight;
            }
            #endregion

            #region Movement
            InputDir = cameraPerspective == PerspectiveModes._1stPerson?  Vector3.ClampMagnitude((transform.forward*MovInput.y+transform.right * (viewInputMethods == ViewInputModes.Traditional ? MovInput.x : 0)),1) : Quaternion.AngleAxis(HeadRotDirForInput,Vector3.up) * (Vector3.ClampMagnitude((Vector3.forward*MovInput_Smoothed.y+Vector3.right * MovInput_Smoothed.x),1));
            GroundMovementSpeedUpdate();
            if(canJump && (holdJump? jumpInput_Momentary : jumpInput_FrameOf)){Jump(jumpPower);}
            #endregion

            #region Footstep
            CalculateFootstepTriggers();
            #endregion

            }
            else
            {
                jumpInput_FrameOf = false;
                jumpInput_Momentary = false;
            }
            #region Animation
            UpdateAnimationTriggers(controllerPaused);
            #endregion
        }

        void FixedUpdate()
        {
            if(!controllerPaused)
            {
                #region Movement
                if(enableMovementControl)
                {
                    GetGroundInfo();

                    MovePlayer(InputDir,currentGroundSpeed);
                }
                #endregion
            }
        }

        #region Camera Functions
        void RotateView(Vector2 yawPitchInput, float inputSensitivity, float cameraWeight){
        
            switch (viewInputMethods){
            
                case ViewInputModes.Traditional:{  
                    yawPitchInput.x *= ((mouseInputInversion==MouseInputInversionModes.X||mouseInputInversion == MouseInputInversionModes.Both) ? 1 : -1);
                    yawPitchInput.y *= ((mouseInputInversion==MouseInputInversionModes.Y||mouseInputInversion == MouseInputInversionModes.Both) ? -1 : 1);
                    switch(cameraPerspective){
                        case PerspectiveModes._1stPerson:{
                            Vector2 targetAngles = ((Vector2.right*playerCamera.transform.localEulerAngles.x)+(Vector2.up*transform.localEulerAngles.y));
                            float fovMod = FOVSensitivityMultiplier>0 && playerCamera.fieldOfView <= initialCameraFOV ? ((initialCameraFOV - playerCamera.fieldOfView)*(FOVSensitivityMultiplier/10))+1 : 1;
                            targetAngles = Vector2.SmoothDamp(targetAngles, targetAngles+(yawPitchInput*((inputSensitivity/fovMod)*Mathf.Pow(cameraWeight*fovMod,2))), ref viewRotVelRef,(Mathf.Pow(cameraWeight*fovMod,2))*Time.deltaTime);
                            targetAngles.x += targetAngles.x>180 ? -360 : targetAngles.x<-180 ? 360 :0;
                            targetAngles.x = Mathf.Clamp(targetAngles.x,-0.5f*verticalRotationRange,0.5f*verticalRotationRange);
                            playerCamera.transform.localEulerAngles = (Vector3.right * targetAngles.x) + (Vector3.forward* (enableHeadbob? headbobCameraPosition.z : 0));
                            transform.localEulerAngles = (Vector3.up*targetAngles.y);
                        }break;

                        case PerspectiveModes._3rdPerson:{
                            headPos = p_Rigidbody.position + Vector3.up *standingEyeHeight;
                            quatHeadRot = Quaternion.Euler(headRot);
                            headRot = Vector3.SmoothDamp(headRot,headRot+((Vector3)yawPitchInput*(inputSensitivity*Mathf.Pow(cameraWeight,2))),ref cameraPosVelRef ,(Mathf.Pow(cameraWeight,2))*Time.deltaTime);
                            headRot.y += headRot.y>180 ? -360 : headRot.y<-180 ? 360 :0;
                            headRot.x += headRot.x>180 ? -360 : headRot.x<-180 ? 360 :0;
                            headRot.x = Mathf.Clamp(headRot.x,-0.5f*verticalRotationRange,0.5f*verticalRotationRange);
                            cameraObstCheck= new Ray(headPos+(quatHeadRot*(Vector3.forward*capsule.radius)), quatHeadRot*-Vector3.forward);
                            if(enableMouseAndCameraDebugging){
                                Debug.Log(headRot);
                                Debug.DrawRay(cameraObstCheck.origin,cameraObstCheck.direction*maxCameraDistance,Color.red);
                                Debug.DrawRay(cameraObstCheck.origin,cameraObstCheck.direction*-currentCameraZ,Color.green);
                            }   
                            if(Physics.SphereCast(cameraObstCheck, 0.5f, out cameraObstResult,maxCameraDistInternal, cameraObstructionIgnore,QueryTriggerInteraction.Ignore)){
                                currentCameraZ = -(Vector3.Distance(headPos,cameraObstResult.point)*0.9f);
                
                            }else{
                                currentCameraZ = Mathf.SmoothDamp(currentCameraZ, -(maxCameraDistInternal*0.85f), ref cameraZRef ,Time.deltaTime,10);
                            }
                        }break;
                        
                    }
            
                }break;
            
                case ViewInputModes.Retro:{
                    yawPitchInput = Vector2.up * (Input.GetAxis("Horizontal") * ((mouseInputInversion==MouseInputInversionModes.Y||mouseInputInversion == MouseInputInversionModes.Both) ? -1 : 1));
                    Vector2 targetAngles = ((Vector2.right*playerCamera.transform.localEulerAngles.x)+(Vector2.up*transform.localEulerAngles.y));
                    float fovMod = FOVSensitivityMultiplier>0 && playerCamera.fieldOfView <= initialCameraFOV ? ((initialCameraFOV - playerCamera.fieldOfView)*(FOVSensitivityMultiplier/10))+1 : 1;
                    targetAngles = targetAngles+(yawPitchInput*((inputSensitivity/fovMod)));   
                    targetAngles.x = 0;
                    playerCamera.transform.localEulerAngles = (Vector3.right * targetAngles.x) + (Vector3.forward* (enableHeadbob? headbobCameraPosition.z : 0));
                    transform.localEulerAngles = (Vector3.up*targetAngles.y);
                }break;
            }
        
        }
        public void RotateView(Vector3 AbsoluteEulerAngles, bool SmoothRotation)
        {
            AbsoluteEulerAngles.x += AbsoluteEulerAngles.x>180 ? -360 : AbsoluteEulerAngles.x<-180 ? 360 :0;
            AbsoluteEulerAngles.x = Mathf.Clamp(AbsoluteEulerAngles.x,-0.5f*verticalRotationRange,0.5f*verticalRotationRange);

            if(SmoothRotation)
            {
                IEnumerator SmoothRot()
                {
                    Vector3 refVec = Vector3.zero, targetAngles = (Vector3.right * playerCamera.transform.localEulerAngles.x)+Vector3.up*transform.eulerAngles.y;
                    while(Vector3.Distance(targetAngles, AbsoluteEulerAngles)>0.1f)
                    { 
                        targetAngles = Vector3.SmoothDamp(targetAngles, AbsoluteEulerAngles, ref refVec, 25*Time.deltaTime);
                        targetAngles.x += targetAngles.x>180 ? -360 : targetAngles.x<-180 ? 360 :0;
                        targetAngles.x = Mathf.Clamp(targetAngles.x,-0.5f*verticalRotationRange,0.5f*verticalRotationRange);
                        playerCamera.transform.localEulerAngles = Vector3.right * targetAngles.x;
                        transform.eulerAngles = Vector3.up*targetAngles.y;
                        yield return null;
                    }
                }   
                StopCoroutine("SmoothRot");
                StartCoroutine(SmoothRot());
            }
            else
            {
                playerCamera.transform.eulerAngles = Vector3.right * AbsoluteEulerAngles.x;
                transform.eulerAngles = (Vector3.up*AbsoluteEulerAngles.y)+(Vector3.forward*AbsoluteEulerAngles.z);
            }
        }

        void FOVKick()
        {
            if(cameraPerspective == PerspectiveModes._1stPerson && FOVKickAmount>0){
                currentFOVMod = (!isIdle && isSprinting) ? initialCameraFOV+(FOVKickAmount*((sprintingSpeed/walkingSpeed)-1)) : initialCameraFOV;
                if(!Mathf.Approximately(playerCamera.fieldOfView, currentFOVMod) && playerCamera.fieldOfView >= initialCameraFOV){
                    playerCamera.fieldOfView = Mathf.SmoothDamp(playerCamera.fieldOfView, currentFOVMod,ref FOVKickVelRef, Time.deltaTime,50);
                }
            }
        }
        void HeadbobCycleCalculator()
        {
            if(enableHeadbob)
            {
                if(!isIdle && currentGroundInfo.isGettingGroundInfo && !isSliding)
                {
                    headbobWarmUp = Mathf.MoveTowards(headbobWarmUp, 1,Time.deltaTime*5);
                    headbobCyclePosition += (_2DVelocity.magnitude)*(Time.deltaTime * (headbobSpeed/10));

                    headbobCameraPosition.x = (((Mathf.Sin(Mathf.PI * (2*headbobCyclePosition + 0.5f)))*(headbobPower/50)))*headbobWarmUp;
                    headbobCameraPosition.y = ((Mathf.Abs((((Mathf.Sin(Mathf.PI * (2*headbobCyclePosition)))*0.75f))*(headbobPower/50)))*headbobWarmUp )+internalEyeHeight;
                    headbobCameraPosition.z = ((Mathf.Sin(Mathf.PI * (2*headbobCyclePosition))) * (ZTilt/3))*headbobWarmUp;
                }
                else
                {
                    headbobCameraPosition = Vector3.MoveTowards(headbobCameraPosition,Vector3.up*internalEyeHeight,Time.deltaTime/(headbobPower*0.3f ));
                    headbobWarmUp = 0.1f;
                }

                playerCamera.transform.localPosition = (Vector2)headbobCameraPosition;

                if(StepCycle>(headbobCyclePosition*3))
                {
                    StepCycle = headbobCyclePosition+0.5f;
                }
            }
        }
        #endregion

        #region Movement Functions

        void MovePlayer(Vector3 Direction, float Speed)
        {
           // GroundInfo gI = GetGroundInfo();
            isIdle = Direction.normalized.magnitude <=0;
            _2DVelocity = Vector2.right * p_Rigidbody.velocity.x + Vector2.up * p_Rigidbody.velocity.z;
            speedToVelocityRatio = (Mathf.Lerp(0, 2, Mathf.InverseLerp(0, (sprintingSpeed/50), _2DVelocity.magnitude)));
            _2DVelocityMag = Mathf.Clamp((walkingSpeed/50) / _2DVelocity.magnitude, 0f,2f);
    
            //Movement
            if((currentGroundInfo.isGettingGroundInfo) && !Jumped && !isSliding)
            {
                //Deceleration
                if(Direction.magnitude==0&& p_Rigidbody.velocity.normalized.magnitude>0.1f){
                    p_Rigidbody.AddForce(-new Vector3(p_Rigidbody.velocity.x,currentGroundInfo.isInContactWithGround? p_Rigidbody.velocity.y-  Physics.gravity.y:0,p_Rigidbody.velocity.z)*(decelerationSpeed*Time.fixedDeltaTime),ForceMode.Force); 
                }
                //normal speed
                else if((currentGroundInfo.isGettingGroundInfo) && currentGroundInfo.groundAngle<hardSlopeLimit && currentGroundInfo.groundAngle_Raw<hardSlopeLimit)
                {
                    p_Rigidbody.velocity = (Vector3.MoveTowards(p_Rigidbody.velocity,Vector3.ClampMagnitude(((Direction)*((Speed)*Time.fixedDeltaTime))+(Vector3.down),Speed/50),1));
                }
                capsule.sharedMaterial = InputDir.magnitude>0 ? _ZeroFriction : _MaxFriction;
            }
            //Sliding
            else if(isSliding)
            {
                p_Rigidbody.AddForce(-(p_Rigidbody.velocity-Physics.gravity)*(slidingDeceleration*Time.fixedDeltaTime),ForceMode.Force);
            }
            //Air Control
            else if(!currentGroundInfo.isGettingGroundInfo)
            {
                p_Rigidbody.AddForce((((Direction*(walkingSpeed))*Time.fixedDeltaTime)*airControlFactor*5)*currentGroundInfo.groundAngleMultiplier_Inverse_persistent,ForceMode.Acceleration);
                p_Rigidbody.velocity= Vector3.ClampMagnitude((Vector3.right*p_Rigidbody.velocity.x + Vector3.forward*p_Rigidbody.velocity.z) ,(walkingSpeed/50))+(Vector3.up*p_Rigidbody.velocity.y);

                if(!currentGroundInfo.potentialStair && jumpEnhancements)
                {
                    if(p_Rigidbody.velocity.y < 0 && p_Rigidbody.velocity.y> Physics.gravity.y*1.5f)
                    {
                        p_Rigidbody.velocity += Vector3.up*(Physics.gravity.y*(decentMultiplier)*Time.fixedDeltaTime);
                    }
                    else if(p_Rigidbody.velocity.y>0 && !jumpInput_Momentary)
                    {
                       p_Rigidbody.velocity += Vector3.up*(Physics.gravity.y*(tapJumpMultiplier-1)*Time.fixedDeltaTime);
                    }
                }
            }
        }

        void Jump(float Force)
        {
            if((currentGroundInfo.isInContactWithGround) && 
                (currentGroundInfo.groundAngle<hardSlopeLimit) && 
                ((enableStaminaSystem && jumpingDepletesStamina)? currentStaminaLevel>s_JumpStaminaDepletion*1.2f : true) && 
                (Time.time>(jumpBlankingPeriod+0.1f)) &&
                (currentStance == Stances.Standing && !Jumped))
            {
                Jumped = true;
                p_Rigidbody.velocity =(Vector3.right * p_Rigidbody.velocity.x) + (Vector3.forward * p_Rigidbody.velocity.z);
                p_Rigidbody.AddForce(Vector3.up*(Force/10),ForceMode.Impulse);
                capsule.sharedMaterial  = _ZeroFriction;
                jumpBlankingPeriod = Time.time;
            }
        }

        public void DoJump(float Force = 10.0f){
            if(
                (Time.time>(jumpBlankingPeriod+0.1f)) &&
                (currentStance == Stances.Standing)){
                    Jumped = true;
                    p_Rigidbody.velocity =(Vector3.right * p_Rigidbody.velocity.x) + (Vector3.forward * p_Rigidbody.velocity.z);
                    p_Rigidbody.AddForce(Vector3.up*(Force/10),ForceMode.Impulse);
                    capsule.sharedMaterial  = _ZeroFriction;
                    jumpBlankingPeriod = Time.time;
            }
        }

        void GetGroundInfo(){
            //to Get if we're actually touching ground.
            //to act as a normal and point buffer.
            currentGroundInfo.groundFromSweep = null;

            currentGroundInfo.groundFromSweep = Physics.SphereCastAll(transform.position,capsule.radius-0.001f,Vector3.down,((capsule.height/2))-(capsule.radius/2),whatIsGround);
    
            currentGroundInfo.isInContactWithGround = Physics.Raycast(transform.position, Vector3.down, out currentGroundInfo.groundFromRay, (capsule.height/2)+0.25f,whatIsGround);
        
            if(Jumped && (Physics.Raycast(transform.position, Vector3.down, (capsule.height/2)+0.1f,whatIsGround)||Physics.CheckSphere(transform.position-(Vector3.up*((capsule.height/2)-(capsule.radius-0.05f))),capsule.radius,whatIsGround)) &&Time.time>(jumpBlankingPeriod+0.1f)){
                Jumped=false;
            }
        
            //if(Result.isGrounded){
                if(currentGroundInfo.groundFromSweep!=null&&currentGroundInfo.groundFromSweep.Length!=0){
                    currentGroundInfo.isGettingGroundInfo=true;
                    currentGroundInfo.groundNormals_lowgrade.Clear();
                    currentGroundInfo.groundNormals_highgrade.Clear();
                    foreach(RaycastHit hit in currentGroundInfo.groundFromSweep){
                        if(hit.point.y > currentGroundInfo.groundFromRay.point.y && Vector3.Angle(hit.normal, Vector3.up)<hardSlopeLimit){
                            currentGroundInfo.groundNormals_lowgrade.Add(hit.normal);
                        }else{
                            currentGroundInfo.groundNormals_highgrade.Add(hit.normal);
                        }
                    }                
                    if(currentGroundInfo.groundNormals_lowgrade.Any()){
                        currentGroundInfo.groundNormal_Averaged = Average(currentGroundInfo.groundNormals_lowgrade);
                    }else{
                        currentGroundInfo.groundNormal_Averaged = Average(currentGroundInfo.groundNormals_highgrade);
                    }
                    currentGroundInfo.groundNormal_Raw = currentGroundInfo.groundFromRay.normal;
                    currentGroundInfo.groundRawYPosition = currentGroundInfo.groundFromSweep.Average(x=> (x.point.y > currentGroundInfo.groundFromRay.point.y && Vector3.Angle(x.normal,Vector3.up)<hardSlopeLimit) ? x.point.y :  currentGroundInfo.groundFromRay.point.y); //Mathf.MoveTowards(currentGroundInfo.groundRawYPosition, currentGroundInfo.groundFromSweep.Average(x=> (x.point.y > currentGroundInfo.groundFromRay.point.y && Vector3.Dot(x.normal,Vector3.up)<-0.25f) ? x.point.y :  currentGroundInfo.groundFromRay.point.y),Time.deltaTime*2);
                
                }else{
                    currentGroundInfo.isGettingGroundInfo=false;
                    currentGroundInfo.groundNormal_Averaged = currentGroundInfo.groundFromRay.normal;
                    currentGroundInfo.groundNormal_Raw = currentGroundInfo.groundFromRay.normal;
                    currentGroundInfo.groundRawYPosition = currentGroundInfo.groundFromRay.point.y;
                }

                if(currentGroundInfo.isGettingGroundInfo){currentGroundInfo.groundAngleMultiplier_Inverse_persistent = currentGroundInfo.groundAngleMultiplier_Inverse;}
                //{
                    currentGroundInfo.groundInfluenceDirection = Vector3.MoveTowards(currentGroundInfo.groundInfluenceDirection, Vector3.Cross(currentGroundInfo.groundNormal_Averaged, Vector3.Cross(currentGroundInfo.groundNormal_Averaged, Vector3.up)).normalized,2*Time.fixedDeltaTime);
                    currentGroundInfo.groundInfluenceDirection.y = 0;
                    currentGroundInfo.groundAngle = Vector3.Angle(currentGroundInfo.groundNormal_Averaged,Vector3.up);
                    currentGroundInfo.groundAngle_Raw = Vector3.Angle(currentGroundInfo.groundNormal_Raw,Vector3.up);
                    currentGroundInfo.groundAngleMultiplier_Inverse = ((currentGroundInfo.groundAngle-90)*-1)/90;
                    currentGroundInfo.groundAngleMultiplier = ((currentGroundInfo.groundAngle))/90;
               //
                currentGroundInfo.groundTag = currentGroundInfo.isInContactWithGround ? currentGroundInfo.groundFromRay.transform.tag : string.Empty;
                if( Physics.Raycast(transform.position+(Vector3.down*((capsule.height*0.5f)-0.1f)), InputDir,out currentGroundInfo.stairCheck_RiserCheck,capsule.radius+0.1f,whatIsGround)){
                    if(Physics.Raycast(currentGroundInfo.stairCheck_RiserCheck.point+(currentGroundInfo.stairCheck_RiserCheck.normal*-0.05f)+Vector3.up,Vector3.down,out currentGroundInfo.stairCheck_HeightCheck,1.1f)){
                        if(!Physics.Raycast(transform.position+(Vector3.down*((capsule.height*0.5f)-maxStairRise))+InputDir*(capsule.radius-0.05f), InputDir,0.2f,whatIsGround) ){
                            if(!isIdle &&  currentGroundInfo.stairCheck_HeightCheck.point.y> (currentGroundInfo.stairCheck_RiserCheck.point.y+0.025f) /* Vector3.Angle(currentGroundInfo.groundFromRay.normal, Vector3.up)<5 */ && Vector3.Angle(currentGroundInfo.groundNormal_Averaged, currentGroundInfo.stairCheck_RiserCheck.normal)>0.5f){
                                p_Rigidbody.position -= Vector3.up*-0.1f;
                                currentGroundInfo.potentialStair = true;
                            }
                        }else{currentGroundInfo.potentialStair = false;}
                    }
                }else{currentGroundInfo.potentialStair = false;}
             

                    currentGroundInfo.playerGroundPosition = Mathf.MoveTowards(currentGroundInfo.playerGroundPosition, currentGroundInfo.groundRawYPosition+ (capsule.height/2) + 0.01f,0.05f);
            //}

            if(currentGroundInfo.isInContactWithGround && enableFootstepSounds && shouldCalculateFootstepTriggers){
                if(currentGroundInfo.groundFromRay.collider is TerrainCollider){
                    currentGroundInfo.groundMaterial = null;
                    currentGroundInfo.groundPhysicMaterial = currentGroundInfo.groundFromRay.collider.sharedMaterial;
                    currentGroundInfo.currentTerrain = currentGroundInfo.groundFromRay.transform.GetComponent<Terrain>();
                    if(currentGroundInfo.currentTerrain){
                        Vector2 XZ = (Vector2.right* (((transform.position.x - currentGroundInfo.currentTerrain.transform.position.x)/currentGroundInfo.currentTerrain.terrainData.size.x)) * currentGroundInfo.currentTerrain.terrainData.alphamapWidth) + (Vector2.up* (((transform.position.z - currentGroundInfo.currentTerrain.transform.position.z)/currentGroundInfo.currentTerrain.terrainData.size.z)) * currentGroundInfo.currentTerrain.terrainData.alphamapHeight);
                        float[,,] aMap = currentGroundInfo.currentTerrain.terrainData.GetAlphamaps((int)XZ.x, (int)XZ.y, 1, 1);
                        for(int i =0; i < aMap.Length; i++){
                            if(aMap[0,0,i]==1 ){
                                currentGroundInfo.groundLayer = currentGroundInfo.currentTerrain.terrainData.terrainLayers[i];
                                break;
                            }
                        }
                    }else{currentGroundInfo.groundLayer = null;}                
                }else{
                    currentGroundInfo.groundLayer = null;
                    currentGroundInfo.groundPhysicMaterial = currentGroundInfo.groundFromRay.collider.sharedMaterial;
                    currentGroundInfo.currentMesh = currentGroundInfo.groundFromRay.transform.GetComponent<MeshFilter>().sharedMesh;
                    if(currentGroundInfo.currentMesh && currentGroundInfo.currentMesh.isReadable){
                        int limit = currentGroundInfo.groundFromRay.triangleIndex*3, submesh;
                        for(submesh = 0; submesh<currentGroundInfo.currentMesh.subMeshCount; submesh++){
                            int indices = currentGroundInfo.currentMesh.GetTriangles(submesh).Length;
                            if(indices>limit){break;}
                            limit -= indices;
                        }
                        currentGroundInfo.groundMaterial = currentGroundInfo.groundFromRay.transform.GetComponent<Renderer>().sharedMaterials[submesh];
                    }else{currentGroundInfo.groundMaterial = currentGroundInfo.groundFromRay.collider.GetComponent<MeshRenderer>().sharedMaterial; }
                }
            }else{currentGroundInfo.groundMaterial = null; currentGroundInfo.groundLayer = null; currentGroundInfo.groundPhysicMaterial = null;}
            #if UNITY_EDITOR
            if(enableGroundingDebugging){
                print("Grounded: "+currentGroundInfo.isInContactWithGround + ", Ground Hits: "+ currentGroundInfo.groundFromSweep.Length +", Ground Angle: "+currentGroundInfo.groundAngle.ToString("0.00") + ", Ground Multi: "+ currentGroundInfo.groundAngleMultiplier.ToString("0.00") + ", Ground Multi Inverse: "+ currentGroundInfo.groundAngleMultiplier_Inverse.ToString("0.00"));
                print("Ground mesh readable for dynamic foot steps: "+ currentGroundInfo.currentMesh?.isReadable);
                Debug.DrawRay(transform.position, Vector3.down*((capsule.height/2)+0.1f),Color.green);
                Debug.DrawRay(transform.position, currentGroundInfo.groundInfluenceDirection,Color.magenta);
                Debug.DrawRay(transform.position+(Vector3.down*((capsule.height*0.5f)-0.05f)) + InputDir*(capsule.radius-0.05f) ,InputDir*(capsule.radius+0.1f), Color.cyan);
                Debug.DrawRay(transform.position+(Vector3.down*((capsule.height*0.5f)-0.5f)) + InputDir*(capsule.radius-0.05f) ,InputDir*(capsule.radius+0.3f), new Color(0,.2f,1,1));
            }
            #endif
        }

        void GroundMovementSpeedUpdate(){
            #if SAIO_ENABLE_PARKOUR
            if(!isVaulting)
            #endif
            {
                switch (currentGroundMovementSpeed){
                    case GroundSpeedProfiles.Walking:{
                        if(isCrouching || isSprinting){
                            isSprinting = false;
                            isCrouching = false;
                            currentGroundSpeed = walkingSpeed;
                            StopCoroutine("ApplyStance");
                            StartCoroutine(ApplyStance(stanceTransitionSpeed,Stances.Standing));
                        }
                        #if SAIO_ENABLE_PARKOUR
                        if(vaultInput && canVault){VaultCheck();}
                        #endif
                        //check for state change call
                        if((canCrouch&&crouchInput_FrameOf)||crouchOverride){
                            isCrouching = true;
                            isSprinting = false;
                            currentGroundSpeed = crouchingSpeed;
                            currentGroundMovementSpeed = GroundSpeedProfiles.Crouching;
                            StopCoroutine("ApplyStance");
                            StartCoroutine(ApplyStance(stanceTransitionSpeed,Stances.Crouching));
                            break;
                        }else if((canSprint&&sprintInput_FrameOf && ((enableStaminaSystem && jumpingDepletesStamina)? currentStaminaLevel>s_minimumStaminaToSprint : true) && (enableSurvivalStats ? (!currentSurvivalStats.isDehydrated && !currentSurvivalStats.isStarving) : true))||sprintOveride){
                            isCrouching = false;
                            isSprinting = true;
                            currentGroundSpeed = sprintingSpeed;
                            currentGroundMovementSpeed = GroundSpeedProfiles.Sprinting;
                            StopCoroutine("ApplyStance");
                            StartCoroutine(ApplyStance(stanceTransitionSpeed,Stances.Standing));
                        }
                        break;
                    }
                
                    case GroundSpeedProfiles.Crouching:{
                        if(!isCrouching){
                            isCrouching = true;
                            isSprinting = false;
                            currentGroundSpeed = crouchingSpeed;
                            StopCoroutine("ApplyStance");
                            StartCoroutine(ApplyStance(stanceTransitionSpeed,Stances.Crouching));
                        }


                        //check for state change call
                        if((toggleCrouch ? crouchInput_FrameOf : !crouchInput_Momentary)&&!crouchOverride && OverheadCheck()){
                            isCrouching = false;
                            isSprinting = false;
                            currentGroundSpeed = walkingSpeed;
                            currentGroundMovementSpeed = GroundSpeedProfiles.Walking;
                            StopCoroutine("ApplyStance");
                            StartCoroutine(ApplyStance(stanceTransitionSpeed,Stances.Standing));
                            break;
                        }else if(((canSprint && sprintInput_FrameOf && ((enableStaminaSystem && jumpingDepletesStamina)? currentStaminaLevel>s_minimumStaminaToSprint : true)&&(enableSurvivalStats ? (!currentSurvivalStats.isDehydrated && !currentSurvivalStats.isStarving) : true))||sprintOveride) && OverheadCheck()){
                            isCrouching = false;
                            isSprinting = true;
                            currentGroundSpeed = sprintingSpeed;
                            currentGroundMovementSpeed = GroundSpeedProfiles.Sprinting;
                            StopCoroutine("ApplyStance");
                            StartCoroutine(ApplyStance(stanceTransitionSpeed,Stances.Standing));
                        }
                        break;
                    }

                    case GroundSpeedProfiles.Sprinting:{
                        //if(!isIdle)
                        {
                            if(!isSprinting){
                                isCrouching = false;
                                isSprinting = true;
                                currentGroundSpeed = sprintingSpeed;
                                StopCoroutine("ApplyStance");
                                StartCoroutine(ApplyStance(stanceTransitionSpeed,Stances.Standing));
                            } 
                            #if SAIO_ENABLE_PARKOUR
                            if((vaultInput || autoVaultWhenSpringing) && canVault){VaultCheck();}
                            #endif
                            else if((canCrouch && crouchInput_FrameOf)||crouchOverride){
                                isCrouching = true;
                                isSprinting = false;
                                currentGroundSpeed = crouchingSpeed;
                                currentGroundMovementSpeed = GroundSpeedProfiles.Crouching;
                                StopCoroutine("ApplyStance");
                                StartCoroutine(ApplyStance(stanceTransitionSpeed,Stances.Crouching));
                                break;
                                //Can't leave sprint in toggle sprint.
                            }else if((toggleSprint ? sprintInput_FrameOf : !sprintInput_Momentary)&&!sprintOveride){
                                isCrouching = false;
                                isSprinting = false;
                                currentGroundSpeed = walkingSpeed;
                                currentGroundMovementSpeed = GroundSpeedProfiles.Walking;
                                StopCoroutine("ApplyStance");
                                StartCoroutine(ApplyStance(stanceTransitionSpeed,Stances.Standing));
                            }
                            break;
                        }
                    }
                }
            }
        }

        IEnumerator ApplyStance(float smoothSpeed, Stances newStance)
        {
            currentStance = newStance;
            float targetCapsuleHeight = currentStance==Stances.Standing? standingHeight : crouchingHeight;
            float targetEyeHeight = currentStance == Stances.Standing? standingEyeHeight : crouchingEyeHeight;

            while(!Mathf.Approximately(capsule.height,targetCapsuleHeight))
            {
                changingStances = true;
                capsule.height = (smoothSpeed>0? Mathf.MoveTowards(capsule.height, targetCapsuleHeight, stanceTransitionSpeed*Time.fixedDeltaTime) : targetCapsuleHeight);
                internalEyeHeight = (smoothSpeed > 0 ? Mathf.MoveTowards(internalEyeHeight, targetEyeHeight, stanceTransitionSpeed * Time.fixedDeltaTime) : targetCapsuleHeight);
            
                if(currentStance == Stances.Crouching && currentGroundInfo.isGettingGroundInfo)
                {
                    p_Rigidbody.velocity = p_Rigidbody.velocity+(Vector3.down*2);
                    if(enableMovementDebugging)
                    {
                        print("Applying Stance and applying down force ");
                    }
                }

                yield return new WaitForFixedUpdate();
            }

            changingStances = false;
            yield return null;
        }

        bool OverheadCheck()
        {
            bool result = false;

            if(Physics.Raycast(transform.position,Vector3.up,standingHeight - (capsule.height/2),whatIsGround))
            {
                result = true;
            }

            return !result;
        }

        Vector3 Average(List<Vector3> vectors)
        {
            Vector3 returnVal = default(Vector3);
            vectors.ForEach(x=> {returnVal += x;});
            returnVal/=vectors.Count();
            return returnVal;
        }
    
        #endregion

        #region Footstep System
        void CalculateFootstepTriggers()
        {
            if(enableFootstepSounds&& footstepTriggeringMode == FootstepTriggeringMode.calculatedTiming && shouldCalculateFootstepTriggers)
            {
                if(_2DVelocity.magnitude>(currentGroundSpeed/100)&& !isIdle)
                {
                    if((enableHeadbob ? headbobCyclePosition : Time.time) > StepCycle && currentGroundInfo.isGettingGroundInfo && !isSliding)
                    {
                        //print("Steped");
                        CallFootstepClip();
                        StepCycle = enableHeadbob ? (headbobCyclePosition+0.5f) : (Time.time+((stepTiming*_2DVelocityMag)*2));
                    }
                }
            }
        }

        public void CallFootstepClip()
        {
            if(playerAudioSource)
            {
                if(enableFootstepSounds && footstepSoundSet.Any())
                {
                    for(int i = 0; i< footstepSoundSet.Count(); i++)
                    {
                        if(footstepSoundSet[i].profileTriggerType == MatProfileType.Material)
                        {
                            if(footstepSoundSet[i]._Materials.Contains(currentGroundInfo.groundMaterial))
                            {
                                currentClipSet = footstepSoundSet[i].footstepClips;
                                break;
                            }
                            else if(i == footstepSoundSet.Count-1)
                            {
                                currentClipSet = null;  
                            }
                        }

                        else if(footstepSoundSet[i].profileTriggerType == MatProfileType.physicMaterial)
                        {
                            if(footstepSoundSet[i]._Materials.Contains(currentGroundInfo.groundMaterial))
                            {
                                currentClipSet = footstepSoundSet[i].footstepClips;
                                break;
                            }
                            else if(i == footstepSoundSet.Count-1)
                            {
                                currentClipSet = null;  
                            }
                        }
                        else if(footstepSoundSet[i].profileTriggerType == MatProfileType.terrainLayer)
                        {
                            if(footstepSoundSet[i]._Layers.Contains(currentGroundInfo.groundLayer))
                            {
                                currentClipSet = footstepSoundSet[i].footstepClips;
                                break;
                            }
                            else if(i == footstepSoundSet.Count-1)
                            {
                                currentClipSet = null;  
                            }
                        }
                    }
                
                    if(currentClipSet!=null && currentClipSet.Any())
                    {
                        playerAudioSource.PlayOneShot(currentClipSet[Random.Range(0,currentClipSet.Count())]);
                    }
                }
            }
        }
        #endregion
   
        #region Animator Update
        void UpdateAnimationTriggers(bool zeroOut = false)
        {
            //Setup Fistperson animation triggers here.
        }
        #endregion

        #region Gizmos
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if(enableGroundingDebugging)
            {
                if(Application.isPlaying)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireSphere(transform.position-(Vector3.up*((capsule.height/2)-(capsule.radius+0.1f))),capsule.radius);
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(transform.position-(Vector3.up*((capsule.height/2)-(capsule.radius-0.5f))),capsule.radius);
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(new Vector3(transform.position.x,currentGroundInfo.playerGroundPosition,transform.position.z),0.05f);
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawSphere(new Vector3(transform.position.x,currentGroundInfo.groundRawYPosition,transform.position.z),0.05f);
                    Gizmos.color = Color.green;
                }
            }
        }
        #endif
        #endregion
        public void PausePlayer()
        {
            controllerPaused = true;
            p_Rigidbody.velocity = Vector3.zero;
            InputDir = Vector2.zero;
            MovInput = Vector2.zero;
            MovInput_Smoothed = Vector2.zero;
            capsule.sharedMaterial = _MaxFriction;
            lockAndHideMouse = true;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            UpdateAnimationTriggers(true);

            if(a_velocity != "")
            {
                _3rdPersonCharacterAnimator.SetFloat(a_velocity, 0);   
            }
        }

        public void UnpausePlayer()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            controllerPaused = false;

            if (p_Rigidbody == null)
            {
                p_Rigidbody = GetComponent<Rigidbody>();
            }


            p_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            p_Rigidbody.isKinematic = false;
        }

        public bool IsPointerLocked()
        {
            return false;
        }

        public void LockPointer()
        {
            UnpausePlayer();
        }

        public void UnlockPointer()
        {
            PausePlayer();
        }
    }

    #region Classes and Enums
    [System.Serializable]
    public class GroundInfo
    {
        public bool isInContactWithGround, isGettingGroundInfo, potentialStair;
        public float groundAngleMultiplier_Inverse = 1, groundAngleMultiplier_Inverse_persistent = 1, groundAngleMultiplier = 0, groundAngle, groundAngle_Raw, playerGroundPosition, groundRawYPosition;
        public Vector3 groundInfluenceDirection, groundNormal_Averaged, groundNormal_Raw;
        public List<Vector3> groundNormals_lowgrade = new List<Vector3>(), groundNormals_highgrade;
        public string groundTag;
        public Material groundMaterial;
        public TerrainLayer groundLayer;
        public PhysicMaterial groundPhysicMaterial;
        internal Terrain currentTerrain;
        internal Mesh currentMesh;
        internal RaycastHit groundFromRay, stairCheck_RiserCheck, stairCheck_HeightCheck;
        internal RaycastHit[] groundFromSweep;

    
    }

    [System.Serializable]
    public class GroundMaterialProfile
    {
        public MatProfileType profileTriggerType = MatProfileType.Material;
        public List<Material> _Materials;
        public List<PhysicMaterial> _physicMaterials;
        public List<TerrainLayer> _Layers;
        public List<AudioClip> footstepClips = new List<AudioClip>();
    }

    [System.Serializable]
    public class SurvivalStats
    {
        public float Health = 250.0f, Hunger = 100.0f, Hydration = 100f;
        public bool hasLowHealth, isStarving, isDehydrated;
    }

    public enum StatSelector{Health, Hunger, Hydration}
    public enum MatProfileType {Material, terrainLayer,physicMaterial}
    public enum FootstepTriggeringMode{calculatedTiming, calledFromAnimations}
    public enum PerspectiveModes{_1stPerson, _3rdPerson}
    public enum ViewInputModes{Traditional, Retro}
    public enum MouseInputInversionModes{None, X, Y, Both}
    public enum GroundSpeedProfiles{Crouching, Walking, Sprinting, Sliding}
    public enum Stances{Standing, Crouching}
    public enum PauseModes{MakeKinematic, FreezeInPlace,BlockInputOnly}
    #endregion

}