using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableSwitchDoorEntity : EnableSwitchBase
{
    public override bool DontTakeNotes => true;

    private Animator animator;
    private BoxCollider2D boxCollider;

    protected override void Init()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponentInChildren<BoxCollider2D>();
        base.Init();
    }

    protected override void OnTODOListStatusUpdate(int status, float progress)
    {
        if (status == 0)
        {
            boxCollider.enabled = progress > 0;
            animator.Play("Close", 0, progress);
        }
        else if (status == 1)
        {
            boxCollider.enabled = progress <= 0;
            animator.Play("Open", 0, progress);
        }
    }

    public override void SetEnable(bool enable)
    {
        throw new System.NotImplementedException();
    }

    protected override EntityTimeStatus CopyTimeStatus()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnUpdateByController(float curTime, float deltaTime)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnUpdateByStatus(EntityTimeStatus status)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnResetStatus(EntityTimeStatus status)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnTimeRuningDirectionChanged(bool isReverse)
    {
        throw new System.NotImplementedException();
    }
}
