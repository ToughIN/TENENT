using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : EntityBase
{
    public float _MoveSpeed = 1;
    public bool _IsReverse = false;

    public override bool IsReverse => _IsReverse;

    public bool IsDied { get; set; }

    private Animator animator;
    private Rigidbody2D rigid;
    private GameObject autoIconObj;
    private SpriteRenderer spriteRenderer;
    private Vector2 inputVec;

    protected override void Init()
    {
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        autoIconObj = transform.Find("AutoIcon").gameObject;
        rigid.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        IsDied = false;
        base.Init();
    }

    protected override void OnTimeRuningDirectionChanged(bool isReverse)
    {
        animator.enabled = isReverse == IsReverse;
        rigid.velocity = Vector2.zero;
        rigid.isKinematic = isReverse != IsReverse;
        autoIconObj.SetActive(isReverse == IsReverse);
    }

    protected override EntityTimeStatus CopyTimeStatus()
    {
        EntityTimeStatus status = new EntityTimeStatus()
        {
            Position = transform.position,
            FlipX = spriteRenderer.flipX,
            Status = 0,
            ModelSprite = spriteRenderer.sprite,
        };
        return status;
    }

    protected override void OnUpdateByController(float curTime, float deltaTime)
    {
        
        if (Input.GetKeyDown(KeyCode.K))
        {
            rigid.WakeUp();
            rigid.velocity = Vector2.zero;
            rigid.AddForce(Vector2.up * 160, ForceMode2D.Force);
        }
        animator.SetInteger("ForceV", Mathf.Abs(rigid.velocity.y) <= 0.01f ? 0 : (int)Mathf.Sign(rigid.velocity.y));

        inputVec.x = Input.GetAxis("Horizontal");
        if (inputVec.x == 0)
        {
            animator.SetBool("IsMove", false);
            animator.speed = 1;
            return;
        }

        animator.SetBool("IsMove", true);
        animator.speed = Mathf.Max(Mathf.Abs(inputVec.x), 0.3f);
        //rigid.position += inputVec * (_MoveSpeed * deltaTime);
        rigid.transform.localPosition += (Vector3)inputVec * (_MoveSpeed * deltaTime);
        spriteRenderer.flipX = inputVec.x < 0;
    }

    protected override void OnUpdateByStatus(EntityTimeStatus status)
    {
        transform.position = Vector2.Lerp(transform.position, status.Position, 0.34f);
        spriteRenderer.flipX = status.FlipX;
        spriteRenderer.sprite = status.ModelSprite;
    }

    protected override void OnResetStatus(EntityTimeStatus status)
    {
        transform.position = status.Position;
        spriteRenderer.flipX = status.FlipX;
        spriteRenderer.sprite = status.ModelSprite;
    }
}
