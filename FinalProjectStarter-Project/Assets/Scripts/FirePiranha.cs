using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EFirePiranhaState : byte
{
    Unknown,
    Hiding,
    AnimatingUp,
    Active,
    Firing,
    FiringDelay,
    AnimatingDown
};

public enum EFirePiranhaDirection : byte
{
    Unknown,
    TopLeft,
    TopRight,
    DownLeft,
    DownRight
};


public class FirePiranha : Enemy
{
    private EFirePiranhaState state = EFirePiranhaState.Unknown;
    private EFirePiranhaDirection direction = EFirePiranhaDirection.Unknown;
    private Vector2 hidingLocation = Vector2.zero;
    private Vector2 activeLocation = Vector2.zero;
    private float holdTimer = 0.0f;
    private float firingTimer = 0.0f;
    private float firingDelayTimer = 0.0f;
    private float animationTimer = 0.0f;
    private int fireballsFired = 0;

    public Sprite mouthClosedUpSprite;
    public Sprite mouthClosedDownSprite;
    public Sprite mouthOpenUpSprite;
    public Sprite mouthOpenDownSprite;
    public int fireballsToFire = 1;

    public EFirePiranhaState State
    {
        get { return state; }
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // Set the enemy type
        enemyType = EEnemyType.FirePiranha;

        // Capture the starting location (hiding) and from that calculate the active (on-screen) location
        hidingLocation = transform.position;
        activeLocation = hidingLocation + new Vector2(0.0f, EnemyConstants.FirePlantOffsetY);

        // Set the state to hiding
        SetDirection(EFirePiranhaDirection.DownLeft);
        SetState(EFirePiranhaState.Hiding);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == EFirePiranhaState.Hiding)
        {
            holdTimer -= Time.deltaTime * Game.Instance.LocalTimeScale;

            if (holdTimer <= 0.0f)
            {
                holdTimer = 0.0f;
                SetState(EFirePiranhaState.AnimatingUp);
            }
        }
        else if (state == EFirePiranhaState.AnimatingUp)
        {
            animationTimer -= Time.deltaTime * Game.Instance.LocalTimeScale;

            float pct = 1.0f - (animationTimer / EnemyConstants.FirePlantAnimationDuration);
            float locationX = Mathf.Lerp(hidingLocation.x, activeLocation.x, pct);
            float locationY = Mathf.Lerp(hidingLocation.y, activeLocation.y, pct);
            transform.position = new Vector2(locationX, locationY);

            if (animationTimer <= 0.0f)
            {
                animationTimer = 0.0f;
                SetState(EFirePiranhaState.Active);
            }
        }
        else if (state == EFirePiranhaState.Active)
        {
            holdTimer -= Time.deltaTime * Game.Instance.LocalTimeScale;

            if (holdTimer <= 0.0f)
            {
                holdTimer = 0.0f;
                SetState(EFirePiranhaState.Firing);
            }
        }
        else if (state == EFirePiranhaState.Firing)
        {
            firingTimer -= Time.deltaTime * Game.Instance.LocalTimeScale;

            if (firingTimer <= 0.0f)
            {
                firingTimer = 0.0f;
                SpawnFireball();
                SetState(EFirePiranhaState.FiringDelay);
            }
        }
        else if (state == EFirePiranhaState.FiringDelay)
        {
            firingDelayTimer -= Time.deltaTime * Game.Instance.LocalTimeScale;

            if (firingDelayTimer <= 0.0f)
            {
                firingDelayTimer = 0.0f;


                if (fireballsFired >= fireballsToFire)
                {
                    SetState(EFirePiranhaState.AnimatingDown);
                }
                else
                {
                    SetState(EFirePiranhaState.Firing);
                }
            }
        }
        else if (state == EFirePiranhaState.AnimatingDown)
        {
            animationTimer -= Time.deltaTime * Game.Instance.LocalTimeScale;

            float pct = 1.0f - (animationTimer / EnemyConstants.FirePlantAnimationDuration);
            float locationX = Mathf.Lerp(activeLocation.x, hidingLocation.x, pct);
            float locationY = Mathf.Lerp(activeLocation.y, hidingLocation.y, pct);
            transform.position = new Vector2(locationX, locationY);

            if (animationTimer <= 0.0f)
            {
                animationTimer = 0.0f;
                SetState(EFirePiranhaState.Hiding);
            }
        }

        if (state == EFirePiranhaState.Active || state == EFirePiranhaState.FiringDelay || state == EFirePiranhaState.AnimatingDown || state == EFirePiranhaState.AnimatingUp)
        {
            Vector2 marioLocation = Game.Instance.MarioGameObject.transform.position;
            Vector2 firePlantHeadLocation = transform.position + new Vector3(0.0f, EnemyConstants.FirePlantHeadOffsetY, 0.0f);

            if (marioLocation.x < firePlantHeadLocation.x)
            {
                if (marioLocation.y > firePlantHeadLocation.y)
                {
                    SetDirection(EFirePiranhaDirection.TopLeft);
                }
                else
                {
                    SetDirection(EFirePiranhaDirection.DownLeft);
                }
            }
            else
            {
                if (marioLocation.y > firePlantHeadLocation.y)
                {
                    SetDirection(EFirePiranhaDirection.TopRight);
                }
                else
                {
                    SetDirection(EFirePiranhaDirection.DownRight);
                }
            }
        }
    }

    private void SetState(EFirePiranhaState newState)
    {
        if (state != newState)
        {
            state = newState;

            if (state == EFirePiranhaState.Hiding)
            {
                transform.position = hidingLocation;
                holdTimer = UnityEngine.Random.Range(EnemyConstants.FirePlantHiddenDurationMin, EnemyConstants.FirePlantHiddenDurationMax);
            }
            else if (state == EFirePiranhaState.AnimatingUp)
            {
                // Get Mario's location
                Vector2 marioLocation = Game.Instance.MarioGameObject.transform.position;

                // Mario is on top of the pipe, don't spawn the piranha plant
                if (Mathf.Abs(activeLocation.y - marioLocation.y) < 0.15f)
                {
                    SetState(EFirePiranhaState.Hiding);
                    return;
                }

                animationTimer = EnemyConstants.FirePlantAnimationDuration;
            }
            else if (state == EFirePiranhaState.Active)
            {
                transform.position = activeLocation;
                holdTimer = EnemyConstants.FirePlantActiveDuration;
                fireballsFired = 0;
            }
            else if (state == EFirePiranhaState.Firing)
            {
                firingTimer = EnemyConstants.FirePlantFireDuration;
            }
            else if (state == EFirePiranhaState.FiringDelay)
            {
                firingDelayTimer = EnemyConstants.FirePlantFireDelayDuration;
            }
            else if (state == EFirePiranhaState.AnimatingDown)
            {
                animationTimer = EnemyConstants.FirePlantAnimationDuration;
            }

            UpdateSprite();
        }
    }
    private void SetDirection(EFirePiranhaDirection newDirection)
    {
        if (direction != newDirection)
        {
            direction = newDirection;

            if (direction == EFirePiranhaDirection.TopLeft || direction == EFirePiranhaDirection.DownLeft)
            {
                Vector3 scale = transform.localScale;
                scale.x = 1.0f;
                transform.localScale = scale;
            }
            else if (direction == EFirePiranhaDirection.TopRight || direction == EFirePiranhaDirection.DownRight)
            {
                Vector3 scale = transform.localScale;
                scale.x = -1.0f;
                transform.localScale = scale;
            }

            UpdateSprite();
        }
    }

    private void UpdateSprite()
    {
        if (state == EFirePiranhaState.Firing || state == EFirePiranhaState.FiringDelay)
        {
            spriteRenderer.sprite = GetMouthOpenSprite();
        }
        else
        {
            spriteRenderer.sprite = GetMouthClosedSprite();
        }
    }

    private void SpawnFireball()
    {
        Vector2 location = transform.position + new Vector3(0.0f, EnemyConstants.FirePlantHeadOffsetY, 0.0f);
        Vector2 direction = GetDirectionVector();
        Vector2 velocity = direction * EnemyConstants.FirePlantFireballSpeed;

        Game.Instance.SpawnFireball(location, velocity);
        fireballsFired++;
    }

    private Vector2 GetDirectionVector()
    {
        Vector2 directionVector = Vector2.zero;
        float scalar = MathF.Sqrt(2.0f) / 2.0f;

        if (direction == EFirePiranhaDirection.TopLeft)
        {
            directionVector = new Vector2(-scalar, scalar);
        }
        else if (direction == EFirePiranhaDirection.TopRight)
        {
            directionVector = new Vector2(scalar, scalar);
        }
        else if (direction == EFirePiranhaDirection.DownLeft)
        {
            directionVector = new Vector2(-scalar, -scalar);
        }
        else if (direction == EFirePiranhaDirection.DownRight)
        {
            directionVector = new Vector2(scalar, -scalar);
        }

        return directionVector;
    }

    private Sprite GetMouthOpenSprite()
    {
        if (direction == EFirePiranhaDirection.TopLeft || direction == EFirePiranhaDirection.TopRight)
        {
            return mouthOpenUpSprite;
        }

        return mouthOpenDownSprite;
    }

    private Sprite GetMouthClosedSprite()
    {
        if (direction == EFirePiranhaDirection.TopLeft || direction == EFirePiranhaDirection.TopRight)
        {
            return mouthClosedUpSprite;
        }

        return mouthClosedDownSprite;
    }
}
