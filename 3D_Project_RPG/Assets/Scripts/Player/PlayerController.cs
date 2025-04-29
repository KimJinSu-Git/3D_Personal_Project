using System;
using System.Collections;
using System.Collections.Generic;
using suntail;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Suntail
{
    public enum PlayerState
    {
        Idle,
        Walk,
        Run,
        Jump,
        Attack,
        ShieldWait,
        ShieldWalk,
        ShieldRun,
        ShieldAttack,
        Die,
        Talking
    }

    public class PlayerController : MonoBehaviour
    {
        [System.Serializable]
        public class GroundLayer
        {
            public string layerName;
            public Texture2D[] groundTextures;
            public AudioClip[] footstepSounds;
        }

        [Header("Movement")]
        [Tooltip("Basic controller speed")]
        [SerializeField] private float walkSpeed;
        [Tooltip("Running controller speed multiplier")]
        [SerializeField] private float runMultiplier;
        [Tooltip("Force of the jump")]
        [SerializeField] private float jumpForce;
        [Tooltip("Gravity")]
        [SerializeField] private float gravity = -9.81f;

        [Header("Player Information")] 
        // Player의 Level
        public int playerLevel = 1;
        public int currentExp = 0;
        public int requiredExp = 100;
        [SerializeField] private float expGrowthRate = 1.2f;
        [SerializeField] private int damageIncreasePerLevel = 5;
        // Player의 최대 HP
        public int playerMaxHp;
        // Player의 현재 HP
        public int playerCurrentHp;
        // Player의 검 데미지
        public int playerSwrodDamage;
        // Player의 방패 데미지
        public int playerShieldDamage;
        
        [Header("Respawn")]
        public Transform spawnPoint;
        
        [Header("Effects")]
        public Transform attackEffectSpawnPoint;
        
        [Header("Mouse Look")]
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float mouseSensivity;
        [SerializeField] private float mouseVerticalClamp;

        [Header("Keybinds")]
        [SerializeField] private KeyCode jumpKey = KeyCode.Space;
        [SerializeField] private KeyCode runKey = KeyCode.LeftShift;

        [Header("Footsteps")]
        [SerializeField] private AudioSource footstepSource;
        [SerializeField] private float groundCheckDistance = 0.3f;
        [SerializeField] [Range(1f, 2f)] private float footstepRate = 1f;
        [SerializeField] [Range(1f, 2f)] private float runningFootstepRate = 1.5f;
        public List<GroundLayer> groundLayers = new List<GroundLayer>();

        [Header("Animation")]
        public Animator playerAnimator;

        public PlayerState currentState;
        private Dictionary<PlayerState, PlayerBaseState> states;
        
        [Header("Collider")]
        public Collider playerSwordCollider;
        public Collider playerShieldCollider;
        
        public bool isHoldingWeapon; // 무기를 들고 있는가 ? 기본 true 값 설정
        
        [Header("Dialogue")]
        public DialogueUI dialogueUI;
        public bool isDialogue; // 대화중인가 ?

        public bool isInventoryOpen;
        public SettingManager settingManager;
        
        private float horizontalInput;
        private float verticalInput;
        private bool isRunning;
        private Vector3 velocity;

        private float verticalRotation;
        private float yAxis;
        private float xAxis;

        private Terrain terrain;
        private TerrainData terrainData;
        private TerrainLayer[] terrainLayers;
        private AudioClip previousClip;
        private Texture2D currentTexture;
        private RaycastHit groundHit;
        private float nextFootstep;
        
        private Coroutine regenCoroutine;
        
        public float WalkSpeed => walkSpeed;
        public float RunMultiplier => runMultiplier;
        public float JumpForce => jumpForce;
        public float Gravity => gravity;
        public bool IsGrounded { get; private set; }
        public float HorizontalInput => horizontalInput;
        public float VerticalInput
        {
            get => verticalInput;
            set => verticalInput = value;
        }

        public bool IsRunning => isRunning;
        public CharacterController CharacterController { get; private set; }

        public float FootstepRate => footstepRate;
        public float RunningFootstepRate => runningFootstepRate;
        public AudioSource FootstepSource => footstepSource;
        public Texture2D CurrentTexture => currentTexture;
        public AudioClip PreviousClip => previousClip;
        public void SetPreviousClip(AudioClip clip) => previousClip = clip;
        public KeyCode JumpKey => jumpKey;
        public KeyCode RunKey => runKey;

        private void Awake()
        {
            CharacterController = GetComponent<CharacterController>();
            GetTerrainData();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isHoldingWeapon = true;

            playerSwordCollider.enabled = false;
            playerShieldCollider.enabled = false;
            
            isDialogue = false;
            isInventoryOpen = false;
        }

        private void OnEnable()
        {
            regenCoroutine = StartCoroutine(RegenerateHealth());
        }

        private void OnDisable()
        {
            if (regenCoroutine != null)
                StopCoroutine(regenCoroutine);
        }

        private void Start()
        {
            states = new Dictionary<PlayerState, PlayerBaseState>();
            states.Add(PlayerState.Idle, new PlayerIdleState(this));
            states.Add(PlayerState.Walk, new PlayerWalkState(this));
            states.Add(PlayerState.Run, new PlayerRunState(this));
            states.Add(PlayerState.Jump, new PlayerJumpState(this));
            states.Add(PlayerState.Attack, new PlayerAttack(this));
            states.Add(PlayerState.ShieldWait, new ShieldWait(this));
            states.Add(PlayerState.ShieldWalk, new ShieldWalk(this));
            states.Add(PlayerState.ShieldRun, new ShieldRun(this));
            states.Add(PlayerState.ShieldAttack, new ShieldAttack(this));
            states.Add(PlayerState.Die, new PlayerDie(this));
            states.Add(PlayerState.Talking, new PlayerTalkingState(this));

            isInventoryOpen = false;
            
            ChangeState(PlayerState.Idle);
        }

        private void GetTerrainData()
        {
            if (Terrain.activeTerrain)
            {
                terrain = Terrain.activeTerrain;
                terrainData = terrain.terrainData;
                terrainLayers = terrain.terrainData.terrainLayers;
            }
        }

        private void Update()
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");
            isRunning = Input.GetKey(runKey);
            
            
            playerAnimator.SetFloat("Horizontal", horizontalInput);
            playerAnimator.SetFloat("Vertical", verticalInput);
            playerAnimator.SetBool("isMoving", Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f);
            playerAnimator.SetBool("isRunning", isRunning);
            playerAnimator.SetBool("isHoldingWeapon", isHoldingWeapon);
            
            MouseLook();
            GroundChecker();

            states[currentState].Update();
        }

        

        public void ChangeState(PlayerState newState)
        {
            if (currentState != newState)
            {
                if (states.ContainsKey(currentState))
                {
                    states[currentState].Exit();
                }
                currentState = newState;
                states[currentState].Enter();
            }
        }

        private void MouseLook()
        {
            if (isDialogue || isInventoryOpen || ShopManager.Instance.IsOpen || settingManager.isPanelOpen) return;
            
            xAxis = Input.GetAxis("Mouse X");
            yAxis = Input.GetAxis("Mouse Y");

            verticalRotation += -yAxis * mouseSensivity;
            verticalRotation = Mathf.Clamp(verticalRotation, -mouseVerticalClamp, mouseVerticalClamp);
            playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
            transform.rotation *= Quaternion.Euler(0, xAxis * mouseSensivity, 0);
        }

        private void FixedUpdate()
        {
            if (CharacterController.isGrounded && (horizontalInput != 0 || verticalInput != 0))
            {
                float currentFootstepRate = (isRunning ? runningFootstepRate : footstepRate);

                if (nextFootstep >= 100f)
                {
                    PlayFootstep();
                    nextFootstep = 0;
                }
                nextFootstep += (currentFootstepRate * walkSpeed);
            }
        }
        
        public void GainExp(int amount)
        {
            currentExp += amount;
            Debug.Log($"EXP 획득: {amount} | 현재 EXP: {currentExp} / {requiredExp}");

            while (currentExp >= requiredExp)
            {
                currentExp -= requiredExp;
                LevelUp();
            }
        }
        
        private void LevelUp()
        {
            playerLevel++;
            requiredExp = Mathf.RoundToInt(requiredExp * expGrowthRate);
            playerSwrodDamage += damageIncreasePerLevel;

            playerMaxHp += 20;
            
            playerCurrentHp = playerMaxHp;

            Debug.Log($"레벨 업! ▶ 레벨: {playerLevel}, 다음 EXP: {requiredExp}, 공격력: {playerSwrodDamage}");
        }

        private void GroundChecker()
        {
            Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
            IsGrounded = Physics.Raycast(ray, out groundHit, groundCheckDistance);
    
            if (IsGrounded)
            {
                if (groundHit.collider.GetComponent<Terrain>())
                {
                    currentTexture = terrainLayers[GetTerrainTexture(transform.position)].diffuseTexture;
                }
                else if (groundHit.collider.GetComponent<Renderer>())
                {
                    currentTexture = GetRendererTexture();
                }
            }
        }

        private void PlayFootstep()
        {
            for (int i = 0; i < groundLayers.Count; i++)
            {
                for (int k = 0; k < groundLayers[i].groundTextures.Length; k++)
                {
                    if (currentTexture == groundLayers[i].groundTextures[k])
                    {
                        footstepSource.PlayOneShot(RandomClip(groundLayers[i].footstepSounds));
                    }
                }
            }
        }

        private float[] GetTerrainTexturesArray(Vector3 controllerPosition)
        {
            terrain = Terrain.activeTerrain;
            terrainData = terrain.terrainData;
            Vector3 terrainPosition = terrain.transform.position;

            int positionX = (int)(((controllerPosition.x - terrainPosition.x) / terrainData.size.x) * terrainData.alphamapWidth);
            int positionZ = (int)(((controllerPosition.z - terrainPosition.z) / terrainData.size.z) * terrainData.alphamapHeight);

            float[,,] layerData = terrainData.GetAlphamaps(positionX, positionZ, 1, 1);

            float[] texturesArray = new float[layerData.GetUpperBound(2) + 1];
            for (int n = 0; n < texturesArray.Length; ++n)
            {
                texturesArray[n] = layerData[0, 0, n];
            }
            return texturesArray;
        }

        private int GetTerrainTexture(Vector3 controllerPosition)
        {
            float[] array = GetTerrainTexturesArray(controllerPosition);
            float maxArray = 0;
            int maxArrayIndex = 0;

            for (int n = 0; n < array.Length; ++n)
            {
                if (array[n] > maxArray)
                {
                    maxArrayIndex = n;
                    maxArray = array[n];
                }
            }
            return maxArrayIndex;
        }

        private Texture2D GetRendererTexture()
        {
            Texture2D texture;
            texture = (Texture2D)groundHit.collider.gameObject.GetComponent<Renderer>().material.mainTexture;
            return texture;
        }

        private AudioClip RandomClip(AudioClip[] clips)
        {
            int attempts = 2;
            footstepSource.pitch = Random.Range(0.9f, 1.1f);

            AudioClip selectedClip = clips[Random.Range(0, clips.Length)];

            while (selectedClip == previousClip && attempts > 0)
            {
                selectedClip = clips[Random.Range(0, clips.Length)];
                attempts--;
            }
            previousClip = selectedClip;
            return selectedClip;
        }
        
        private IEnumerator RegenerateHealth()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);

                if (playerCurrentHp < playerMaxHp)
                {
                    playerCurrentHp = Mathf.Min(playerCurrentHp + 1, playerMaxHp);
                }
            }
        }
    }
}