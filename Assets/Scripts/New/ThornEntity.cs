using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornEntity : TrapEntityBase
{
    private Collider2D coll;


    protected override void Init()
    {
        coll=GetComponent<Collider2D>();
        base.Init();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        collision.GetComponent<PlayerEntity>().IsDied = true;
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
