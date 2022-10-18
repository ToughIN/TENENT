using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobilePlatformEntity : EntityBase
{
    
    public bool _Enable;
    public float _MoveSpeed;
    public float _WaitTime;
   

    public override bool DontTakeNotes => true;

    
    private SpriteRenderer spriteRenderer;
    
    private Collider2D coll;
    private List<Vector3> movePoints=new List<Vector3>();

    private bool isWait;
    private float startWaitTime=-100;
    private int posIndex=0;
    private Transform playerParent;
    private float curTime;
    private float posLerp;
    
    

    protected override void Init()
    {
        
        coll = GetComponent<Collider2D>();
        
        spriteRenderer=GetComponentInChildren<SpriteRenderer>();

        
        
        Transform[] tempTransforms = GetComponentsInChildren<Transform>();
        foreach (var childTransform in tempTransforms)//将子物体中移动点的localPosition转换成世界坐标并存储坐标
        {
            if (!childTransform.CompareTag("movePoints")) continue;
            
            Vector3 offSet =new Vector3(childTransform.localPosition.x * transform.localScale.x,
                                     childTransform.localPosition.y * transform.localScale.y,
                                     childTransform.localPosition.z * transform.localScale.z);            
            movePoints.Add(transform.position + offSet);
        }
        


        base.Init();
    } 

    public void MobilePlatformMove()
    {
        curTime = TimeMgr.Inst.CurTime;
        posLerp = (Mathf.Sin(curTime) + 1) / 2;
        transform.position = Vector2.Lerp(movePoints[0], movePoints[1],posLerp);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;
        playerParent=collision.collider.transform.parent;
        collision.transform.parent = transform;

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;
        collision.transform.parent = playerParent;
    }
    private void Update()
    {
        MobilePlatformMove();
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
