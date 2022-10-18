using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableSwitchEntity : EntityBase
{
    public EnableSwitchBase _DoEntity;
    public Sprite _OnSpr;
    public Sprite _OffSpr;
    public bool _Enable;

    public override bool DontTakeNotes => true;

    private GameObject tipObject;
    private SpriteRenderer spriteRenderer;
    private bool curEnable;
    private bool playerInRange;

    protected override void Init()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        tipObject = transform.Find("Tip").gameObject;
        tipObject.SetActive(false);
        OnTODOListStatusUpdate(_Enable ? 1 : 0, 1);
        base.Init();
    }

    private void Start()
    {
        
    }

    protected override void OnUpdateAlways()
    {
        if (GlobalStatus.IsPlaying &&
            tipObject.activeSelf &&
            Input.GetKeyDown(KeyCode.J))
        {
            AddTODOEvent(!curEnable);
            SetInteractEnable(false);
        }
        base.OnUpdateAlways();
    }

    private void AddTODOEvent(bool enable)
    {
        if (_DoEntity == null)
            return;
        if (_DoEntity.DontTakeNotes)
        {
            TODOArgs todoArgs = new TODOArgs
            {
                Time = TimeMgr.Inst.CurTime,
                Status = enable ? 1 : 0,
                Duration = 0.6f,
                Entity = this,
                TimeDirection = TimeMgr.Inst.IsReverse ? -1 : 1,
            };
            todoArgs.TODOElse = new EntityBase[] { _DoEntity };
            TODOList.Inst.AddTodoEvent(todoArgs);
        }
        else _DoEntity.SetEnable(enable);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer("Player"))
            return;
        var player = col.GetComponent<PlayerEntity>();
        if (player.IsReverse == TimeMgr.Inst.IsReverse)
        {
            playerInRange = true;
            RefreshInteractStatus();
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer("Player"))
            return;
        var player = col.GetComponent<PlayerEntity>();
        if (player.IsReverse == TimeMgr.Inst.IsReverse)
        {
            playerInRange = false;
            RefreshInteractStatus();
        }
    }

    protected override void OnTODODoingCountChanged(int curCount)
    {
        RefreshInteractStatus();
    }

    private void RefreshInteractStatus()
    {
        SetInteractEnable(playerInRange && TODODoingCount == 0);
    }

    protected override EntityTimeStatus CopyTimeStatus()
    {
        return new EntityTimeStatus();
    }

    protected override void OnResetStatus(EntityTimeStatus status)
    {

    }

    protected override void OnTimeRuningDirectionChanged(bool isReverse)
    {
        SetInteractEnable(false);
    }

    protected override void OnUpdateByController(float curTime, float deltaTime)
    {

    }

    protected override void OnUpdateByStatus(EntityTimeStatus status)
    {

    }

    protected override void OnTODOListStatusUpdate(int status, float progress)
    {
        if (status == 0)
        {
            curEnable = progress <= 0;
        }
        else if (status == 1)
        {
            curEnable = progress > 0;
        }
        spriteRenderer.sprite = curEnable ? _OnSpr : _OffSpr;
    }

    private void SetInteractEnable(bool enable)
    {
        tipObject.SetActive(enable);
    }
    
}
