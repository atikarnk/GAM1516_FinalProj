using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EBreakableBlockState : byte
{
    Unknown,
    Active,
    AnimUp,
    AnimDown,
    Breaking
}

public class BreakableBlock : MonoBehaviour
{
    private EBreakableBlockState state = EBreakableBlockState.Unknown;
    private Animator animator;
    private Vector2 start;
    private Vector2 target;
    private Vector2 initial;
    private float animationTimer;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.Play("BreakableBlock");

        SetState(EBreakableBlockState.Active);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == EBreakableBlockState.AnimUp)
        {
            animationTimer -= Time.deltaTime;

            float pct = 1.0f - (animationTimer / PickupConstants.ItemBoxAnimationDuration);
            float x = Mathf.Lerp(start.x, target.x, pct);
            float y = Mathf.Lerp(start.y, target.y, pct);
            transform.position = new Vector2(x, y);

            if (animationTimer <= 0.0f)
            {
                animationTimer = 0.0f;
                SetState(EBreakableBlockState.AnimDown);
            }
        }
        else if (state == EBreakableBlockState.AnimDown)
        {
            animationTimer -= Time.deltaTime;

            float pct = 1.0f - (animationTimer / PickupConstants.ItemBoxAnimationDuration);
            float x = Mathf.Lerp(start.x, target.x, pct);
            float y = Mathf.Lerp(start.y, target.y, pct);
            transform.position = new Vector2(x, y);

            if (animationTimer <= 0.0f)
            {
                animationTimer = 0.0f;
                SetState(EBreakableBlockState.Active);
            }
        }
    }

    public bool CanTransformToCoin()
    {
        return state == EBreakableBlockState.Active;
    }

    private void SetState(EBreakableBlockState newState)
    {
        if (state != newState)
        {
            state = newState;

            if (state == EBreakableBlockState.Active)
            {
                animator.Play("BreakableBlock");
            }
            else if (state == EBreakableBlockState.AnimUp)
            {
                initial = transform.position;
                start = initial;
                target = start + new Vector2(0.0f, 0.25f);

                animationTimer = PickupConstants.ItemBoxAnimationDuration;
            }
            else if (state == EBreakableBlockState.AnimDown)
            {
                start = target;
                target = initial;

                animationTimer = PickupConstants.ItemBoxAnimationDuration;
            }
            else if (state == EBreakableBlockState.Breaking)
            {
                Vector2 location = transform.position;
                const int kNumBits = 4;
                EBreakableBlockBitType[] types = new[] { EBreakableBlockBitType.Left, EBreakableBlockBitType.Left, EBreakableBlockBitType.Right, EBreakableBlockBitType.Right };

                for (int i = 0; i < kNumBits; i++)
                {
                    Game.Instance.SpawnBreakableBlockBits(location + GameConstants.BreakableBlockBitOffsets[i], GameConstants.BreakableBlockBitImpulses[i], types[i]);
                }

                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Mario"))
        {
            if (collision.contacts.Length > 0 && collision.contacts[0].normal.y >= 0.8f)
            {
                if (state == EBreakableBlockState.Active)
                {
                    if (Game.Instance.GetMarioState.Form == EMarioForm.Small)
                    {
                        SetState(EBreakableBlockState.AnimUp);
                    }
                    else
                    {
                        //Stop mario from jumping when breaking a block
                        collision.gameObject.GetComponent<MarioMovement>().StopJumping();

                        SetState(EBreakableBlockState.Breaking);
                    }
                }
            }
        }
    }
}
