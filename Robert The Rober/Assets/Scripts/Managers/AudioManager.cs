using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Scene Music")]
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip heistSceneMusic;
    [SerializeField] private AudioClip shopMusic;

    [Header("Player / Interaction SFX")]
    [SerializeField] private AudioClip doorLockedClip;
    [SerializeField] private AudioClip lockpickSuccessClip;
    [SerializeField] private AudioClip enterHouseClip;
    [SerializeField] private AudioClip moneyPickupClip;
    [SerializeField] private AudioClip leaveSceneClip;
    [SerializeField] private AudioClip buyItemClip;
    [SerializeField] private AudioClip cameraDetectedClip;
    [SerializeField] private AudioClip wireCutterUseClip;
    [SerializeField] private AudioClip dogBiteClip;
    [SerializeField] private AudioClip throwMeatClip;
    [SerializeField] private AudioClip bigLootPickupClip;
    [SerializeField] private AudioClip missingRequiredItemClip;
    [SerializeField] private AudioClip sackFullClip;
    [SerializeField] private AudioClip lockpickFailClip;

    [Header("SFX Cooldowns")]
    [SerializeField] private float defaultSfxCooldown = 1f;
    [SerializeField] private float doorLockedCooldown = 1f;
    [SerializeField] private float enterHouseCooldown = 1f;
    [SerializeField] private float lockpickSuccessCooldown = 1f;
    [SerializeField] private float moneyPickupCooldown = 1f;
    [SerializeField] private float leaveSceneCooldown = 1f;
    [SerializeField] private float buyItemCooldown = 1f;
    [SerializeField] private float cameraDetectedCooldown = 1f;
    [SerializeField] private float wireCutterUseCooldown = 1f;
    [SerializeField] private float dogBiteCooldown = 1f;
    [SerializeField] private float throwMeatCooldown = 1f;
    [SerializeField] private float bigLootPickupCooldown = 1f;
    [SerializeField] private float missingRequiredItemCooldown = 1f;
    [SerializeField] private float sackFullCooldown = 1f;
    [SerializeField] private float lockpickFailCooldown = 1f;
    private string currentSceneMusicKey = "";
    private readonly Dictionary<string, float> lastPlayTimeByKey = new();

    private const string DoorLockedKey = "door_locked";
    
    private const string LockpickSuccessKey = "lockpick_success";
    private const string EnterHouseKey = "enter_house";
    private const string MoneyPickupKey = "money_pickup";
    private const string LeaveSceneKey = "leave_scene";
    private const string BuyItemKey = "buy_item";
    private const string CameraDetectedKey = "camera_detected";
    private const string WireCutterUseKey = "wire_cutter_use";
    private const string DogBiteKey = "dog_bite";
    private const string ThrowMeatKey = "throw_meat";
    private const string BigLootPickupKey = "bigloot_pickup";
    private const string MissingRequiredItemKey = "missing_required_item";
    private const string SackFullKey = "sack_full";
    private const string LockpickFailKey = "lockpick_fail";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        if (musicSource == null)
        {
            Debug.LogWarning("AudioManager: musicSource no está asignado.");
            return;
        }

        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    public void PlayMusicForScene(string sceneName)
    {
        if (musicSource == null)
            return;

        AudioClip targetClip = GetMusicClipForScene(sceneName);
        string targetKey = sceneName;

        if (targetClip == null)
        {
            Debug.LogWarning($"AudioManager: no hay música asignada para la escena {sceneName}");
            return;
        }

        if (musicSource.clip == targetClip && musicSource.isPlaying && currentSceneMusicKey == targetKey)
            return;

        musicSource.clip = targetClip;
        musicSource.loop = true;
        musicSource.Play();
        currentSceneMusicKey = targetKey;
    }

    private AudioClip GetMusicClipForScene(string sceneName)
    {
        switch (sceneName)
        {
            case "MainMenu":
                return mainMenuMusic;

            case "HeistScene":
                return heistSceneMusic;

            case "Shop":
                return shopMusic;

            default:
                return null;
        }
    }

    public void PlaySfxWithCooldown(string soundKey, AudioClip clip, float cooldown = -1f, float volume = 1f)
    {
        if (sfxSource == null || clip == null || string.IsNullOrEmpty(soundKey))
            return;

        float usedCooldown = cooldown >= 0f ? cooldown : defaultSfxCooldown;

        if (lastPlayTimeByKey.TryGetValue(soundKey, out float lastTime))
        {
            if (Time.unscaledTime - lastTime < usedCooldown)
                return;
        }

        lastPlayTimeByKey[soundKey] = Time.unscaledTime;
        sfxSource.PlayOneShot(clip, volume);
    }

    public void PlayDoorLocked()
    {
        PlaySfxWithCooldown(DoorLockedKey, doorLockedClip, doorLockedCooldown);
    }

    public void PlayLockpickSuccess()
    {
        PlaySfxWithCooldown(LockpickSuccessKey, lockpickSuccessClip, lockpickSuccessCooldown);
    }

    public void PlayEnterHouse()
    {
        PlaySfxWithCooldown(EnterHouseKey, enterHouseClip, enterHouseCooldown);
    }

    public void PlayMoneyPickup()
    {
        PlaySfxWithCooldown(MoneyPickupKey, moneyPickupClip, moneyPickupCooldown);
    }

    public void PlayLeaveScene()
    {
        PlaySfxWithCooldown(LeaveSceneKey, leaveSceneClip, leaveSceneCooldown);
    }

    public void PlayBuyItem()
    {
        PlaySfxWithCooldown(BuyItemKey, buyItemClip, buyItemCooldown);
    }

    public void PlayCameraDetected()
    {
        PlaySfxWithCooldown(CameraDetectedKey, cameraDetectedClip, cameraDetectedCooldown);
    }

    public void PlayWireCutterUse()
    {
        PlaySfxWithCooldown(WireCutterUseKey, wireCutterUseClip, wireCutterUseCooldown);
    }

    public void PlayDogBite()
    {
        PlaySfxWithCooldown(DogBiteKey, dogBiteClip, dogBiteCooldown);
    }

    public void PlayThrowMeat()
    {
        PlaySfxWithCooldown(ThrowMeatKey, throwMeatClip, throwMeatCooldown);
    }
    
    public void PlayBigLootPickup()
    {
        PlaySfxWithCooldown(BigLootPickupKey, bigLootPickupClip, bigLootPickupCooldown);
    }

    public void PlayMissingRequiredItem()
    {
        PlaySfxWithCooldown(MissingRequiredItemKey, missingRequiredItemClip, missingRequiredItemCooldown);
    }

    public void PlaySackFull()
    {
        PlaySfxWithCooldown(SackFullKey, sackFullClip, sackFullCooldown);
    }

    public void PlayLockpickFail()
    {
        PlaySfxWithCooldown(LockpickFailKey, lockpickFailClip, lockpickFailCooldown);
    }
}