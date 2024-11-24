using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum EGameState : byte
{
    Unknown,
    Gameplay,
    FadeIn,
    FadeHold,
    FadeOut
};

public class Game : MonoBehaviour
{
    private static Game sInstance;

    public MarioCamera marioCamera;

    public GameObject marioGameObject;
    public GameObject deadMarioPrefab;
    public GameObject mushroomPickupPrefab;
    public GameObject itemBoxPickupPrefab;
    public GameObject coinSwitchPrefab;
    public GameObject coinPickupPrefab;
    public GameObject breakableBlockPrefab;
    public GameObject breakableBlockBitPrefab;
    public GameObject fireballPrefab;
    public GameObject changeAnimationPrefab;


    private GameObject deadMario = null;
    private Vector2 marioSpawnLocation = Vector2.zero;
    private float localTimeScale = 1.0f;
    private float timeRemaining = GameConstants.DefaultGameDuration;
    private float blackOverlayAlpha = 0.0f;
    private float fadeInOutTimer = 0.0f;
    private float fadeHoldTimer = 0.0f;
    private float coinSwitchDuration = 0.0f;
    private EGameState state = EGameState.Unknown;
    private bool isGameOver = false;

    public static Game Instance
    {
        get { return sInstance; }
    }

    public GameObject MarioGameObject
    {
        get { return marioGameObject; }
    }

    public Mario GetMario
    {
        get { return marioGameObject.GetComponent<Mario>(); }
    }

    public MarioState GetMarioState
    {
        get { return marioGameObject.GetComponent<MarioState>(); }
    }

    public MarioMovement GetMarioMovement
    {
        get { return marioGameObject.GetComponent<MarioMovement>(); }
    }

    public float LocalTimeScale
    { 
        get { return localTimeScale; } 
    }

    public float TimeRemaining
    {
        get { return timeRemaining; }
    }

    public float BlackOverlayAlpha
    {
        get { return blackOverlayAlpha; }
    }

    public bool IsGameOver
    {
        get { return isGameOver; }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Setup the static instance of the Game class
        if (sInstance != null && sInstance != this)
        {
            Destroy(this);
        }
        else
        {
            sInstance = this;
        }

        // Get Mario's spawn location
        marioSpawnLocation = marioGameObject.transform.position;

        // Set the game's state to gameplay
        SetState(EGameState.FadeHold);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == EGameState.Gameplay)
        {
            if (deadMario != null)
            {
                if (deadMario.transform.position.y < GameConstants.DestroyActorAtY)
                {
                    Destroy(deadMario);
                    deadMario = null;

                    UnpauseActors();

                    SetState(EGameState.FadeIn);
                }
            }

            // If the coin switch block has been triggered, handle the timer
            if (coinSwitchDuration > 0.0f)
            {
                coinSwitchDuration -= Time.deltaTime;

                if (coinSwitchDuration <= 0.0f)
                {
                    coinSwitchDuration = 0.0f;
                    EndToggleCoinsAndBlocks();
                }
            }

            // Countdown the time remaining timer
            timeRemaining -= Time.deltaTime;

            if (timeRemaining < 0.0f)
            {
                timeRemaining = 0.0f;
                GetMario.HandleDamage(true); // Mario is dead
            }
        }
        else if (state == EGameState.FadeIn)
        {
            fadeInOutTimer -= Time.deltaTime;

            float elapsed = Mathf.Max(0.0f, GameConstants.BlackOverlayFadeInOutDuration - fadeInOutTimer);
            float alpha = elapsed / GameConstants.BlackOverlayFadeInOutDuration;

            blackOverlayAlpha = alpha;

            if (fadeInOutTimer <= 0.0f)
            {
                fadeInOutTimer = 0.0f;
                SetState(EGameState.FadeHold);
            }
        }
        else if (state == EGameState.FadeHold)
        {
            fadeHoldTimer -= Time.deltaTime;

            if (fadeHoldTimer <= 0.0f)
            {
                fadeHoldTimer = 0.0f;
                SetState(EGameState.FadeOut);
            }
        }
        else if (state == EGameState.FadeOut)
        {
            fadeInOutTimer -= Time.deltaTime;

            float elapsed = Mathf.Max(0.0f, GameConstants.BlackOverlayFadeInOutDuration - fadeInOutTimer);
            float alpha = 1.0f - (elapsed / GameConstants.BlackOverlayFadeInOutDuration);

            blackOverlayAlpha = alpha;

            if (fadeInOutTimer <= 0.0f)
            {
                fadeInOutTimer = 0.0f;
                SetState(EGameState.Gameplay);
            }
        }
    }

    public void NextRoom(Door door)
    {
        marioSpawnLocation = door.transform.position;
        SetState(EGameState.FadeIn);
    }

    public void PauseActors()
    {
        localTimeScale = 0.0f;

        // get root objects in scene
        List<GameObject> gameObjects = new List<GameObject>();
        SceneManager.GetActiveScene().GetRootGameObjects(gameObjects);

        // iterate root objects and do something
        for (int i = 0; i < gameObjects.Count; ++i)
        {
            if (gameObjects[i].CompareTag("Mario"))
            {
                gameObjects[i].GetComponent<MarioMovement>().Pause();
            }
            else
            {
                Animator animator = gameObjects[i].GetComponent<Animator>();

                if (animator != null)
                    animator.speed = 0.0f;
            }
        }
    }

    public void UnpauseActors()
    {
        localTimeScale = 1.0f;

        // get root objects in scene
        List<GameObject> gameObjects = new List<GameObject>();
        SceneManager.GetActiveScene().GetRootGameObjects(gameObjects);

        // iterate root objects and do something
        for (int i = 0; i < gameObjects.Count; ++i)
        {
            if (gameObjects[i].CompareTag("Mario"))
            {
                gameObjects[i].GetComponent<MarioMovement>().Unpause();
            }
            else
            {
                Animator animator = gameObjects[i].GetComponent<Animator>();

                if (animator != null)
                    animator.speed = 1.0f;
            }
        }
    }

    public void MarioHasDied(bool spawnDeadMario)
    {
        // Get Mario's player state and decrease the Lives value by one
        MarioState marioState = GetMarioState;

        if (marioState != null)
        {
            if (marioState.Lives > 0)
            {
                marioState.Lives--;

                // Do we spawn dead mario or not?
                if (spawnDeadMario)
                {
                    SpawnDeadMario(marioGameObject.transform.position);
                }
                else
                {
                    // If not fade out immediately
                    SetState(EGameState.FadeIn);
                }
            }
            else
            {
                isGameOver = true;
            }
        }
    }

    public void SpawnMushroomPickup(Vector2 location)
    {
        if (mushroomPickupPrefab != null)
        {
            GameObject mushroomObject = Instantiate(mushroomPickupPrefab, new Vector3(location.x, location.y, 1.0f), Quaternion.identity);
            MushroomPickup mushroomPickup = mushroomObject.GetComponent<MushroomPickup>();
            mushroomPickup.Spawn();
        }
    }

    public void SpawnItemBoxCoin(Vector2 location)
    {
        if (itemBoxPickupPrefab != null)
        {
            Instantiate(itemBoxPickupPrefab, new Vector3(location.x, location.y, 1.0f), Quaternion.identity);
        }
    }

    public void SpawnCoinSwitch(Vector2 location)
    {
        if (coinSwitchPrefab != null)
        {
            Instantiate(coinSwitchPrefab, new Vector3(location.x, location.y, 0.0f), Quaternion.identity);

            SpawnChangeAnimation(location);
        }
    }

    public void SpawnCoinPickup(Vector2 location, bool useStaticCoin = false)
    {
        if (coinPickupPrefab != null)
        {
            GameObject coinObject = Instantiate(coinPickupPrefab, new Vector3(location.x, location.y, 0.0f), Quaternion.identity);
            CoinPickup coinPickup = coinObject.GetComponent<CoinPickup>();

            if (useStaticCoin)
                coinPickup.UseStaticCoin();
        }
    }

    public void SpawnBreakableBlock(Vector2 location)
    {
        if (breakableBlockPrefab != null)
        {
            Instantiate(breakableBlockPrefab, new Vector3(location.x, location.y, 0.0f), Quaternion.identity);
        }
    }

    public void SpawnBreakableBlockBits(Vector2 location, Vector2 impulse, EBreakableBlockBitType type)
    {
        if (breakableBlockBitPrefab != null)
        {
            GameObject breakableBlockBitObject = Instantiate(breakableBlockBitPrefab, new Vector3(location.x, location.y, -1.0f), Quaternion.identity);
            BreakableBlockBit breakableBlockBit = breakableBlockBitObject.GetComponent<BreakableBlockBit>();
            breakableBlockBit.Spawn(type, impulse);
        }
    }

    public void SpawnFireball(Vector2 location, Vector2 velocity)
    {
        if (fireballPrefab != null)
        {
            GameObject fireballObject = Instantiate(fireballPrefab, new Vector3(location.x, location.y, 1.0f), Quaternion.identity);
            Fireball fireball = fireballObject.GetComponent<Fireball>();
            fireball.Spawn(velocity);
        }
    }

    public void SpawnChangeAnimation(Vector2 location)
    {
        if (changeAnimationPrefab != null)
        {
            Instantiate(changeAnimationPrefab, new Vector3(location.x, location.y, -1.0f), Quaternion.identity);
        }
    }

    public void OnCoinSwitch()
    {
        coinSwitchDuration = GameConstants.BreakableBlockCoinSwitchDuration;
        BeginToggleCoinsAndBlocks();
    }

    private void SpawnDeadMario(Vector2 location)
    {
        if (deadMario == null)
        {
            PauseActors();

            if (deadMarioPrefab != null)
            {
                deadMario = Instantiate(deadMarioPrefab, new Vector3(location.x, location.y, -1.5f), Quaternion.identity);
            }
        }
    }

    private void BeginToggleCoinsAndBlocks()
    {
        CoinPickup[] coinPickupObjects = FindObjectsOfType<CoinPickup>();
        BreakableBlock[] breakableBlockObjects = FindObjectsOfType<BreakableBlock>();

        for (int i = 0; i < coinPickupObjects.Length; i++)
        {
            Vector2 location = coinPickupObjects[i].transform.position;
            Destroy(coinPickupObjects[i].gameObject);

            SpawnBreakableBlock(location);
        }

        for (int i = 0; i < breakableBlockObjects.Length; i++)
        {
            if (breakableBlockObjects[i].CanTransformToCoin())
            {
                Vector2 location = breakableBlockObjects[i].transform.position;
                Destroy(breakableBlockObjects[i].gameObject);

                SpawnCoinPickup(location, true);
            }
        }
    }

    private void EndToggleCoinsAndBlocks()
    {
        CoinPickup[] coinPickupObjects = FindObjectsOfType<CoinPickup>();
        BreakableBlock[] breakableBlockObjects = FindObjectsOfType<BreakableBlock>();

        for (int i = 0; i < coinPickupObjects.Length; i++)
        {
            Vector2 location = coinPickupObjects[i].transform.position;
            Destroy(coinPickupObjects[i].gameObject);

            SpawnBreakableBlock(location);
        }

        for (int i = 0; i < breakableBlockObjects.Length; i++)
        {
            if (breakableBlockObjects[i].CanTransformToCoin())
            {
                Vector2 location = breakableBlockObjects[i].transform.position;
                Destroy(breakableBlockObjects[i].gameObject);

                SpawnCoinPickup(location);
            }
        }
    }

    private void SetState(EGameState newState)
    {
        if (state != newState)
        {
            state = newState;

            if (state == EGameState.Gameplay)
            {
                blackOverlayAlpha = 0.0f;
            }
            else if (state == EGameState.FadeIn)
            {
                blackOverlayAlpha = 0.0f;
                fadeInOutTimer = GameConstants.BlackOverlayFadeInOutDuration;
            }
            else if (state == EGameState.FadeHold)
            {
                blackOverlayAlpha = 1.0f;
                fadeHoldTimer = GameConstants.BlackOverlayFadeHoldDuration;
            }
            else if (state == EGameState.FadeOut)
            {
                blackOverlayAlpha = 1.0f;

                if (GetMarioState.State == EMarioState.Dead)
                {
                    timeRemaining = GameConstants.DefaultGameDuration;
                }

                // Respawn Mario
                GetMario.ResetMario(marioSpawnLocation);

                // Set the Camera to mario's location
                marioCamera.SetCameraLocation(marioSpawnLocation);

                // Set the fade out timer
                fadeInOutTimer = GameConstants.BlackOverlayFadeInOutDuration;
            }
        }
    }
}
