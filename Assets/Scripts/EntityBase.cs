using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityBase : MonoBehaviour
{
    public Dictionary<int, EntityTimeStatus> _TimeStatus;
    private Dictionary<int, EntityTimeStatus>.Enumerator timeStatusEnumerator;
    private EntityTimeStatus lastStatus;
    private bool containsLastStatus;

    public virtual bool DontTakeNotes { get { return false; } }
    public virtual bool IsReverse { get { return false; } }
    public int TODODoingCount { get { return todoDoingCount; } set { todoDoingCount = value; OnTODODoingCountChanged(todoDoingCount); } }
    private int todoDoingCount;

    public int LastTODOStatus { get { return lastTODOStatus; } }
    private int lastTODOStatus;

    public float LastTODOProgress { get { return lastTODOProgress; } }
    private float lastTODOProgress;

    private void OnDrawGizmos()
    {
        if (_TimeStatus == null) return;
        timeStatusEnumerator = _TimeStatus.GetEnumerator();
        while (timeStatusEnumerator.MoveNext())
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(timeStatusEnumerator.Current.Value.Position, new Vector3(0.02f, 0.2f, 0f));
        }
    }

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        OnUpdateAlways();
    }

    protected virtual void Init()
    {
        EntityMgr.Inst.OnEntityBorn(this);
    }

    protected virtual void OnDestroy()
    {
        EntityMgr.Inst.OnEntityDie(this);
        TODOList.Inst.RemoveEntityTodoEvent(this);
    }

    /// <summary>
    /// 返回当前实体时间状态.
    /// </summary>
    public EntityTimeStatus GetTimeStatus()
    {
        return CopyTimeStatus();
    }
    protected abstract EntityTimeStatus CopyTimeStatus();

    /// <summary>
    /// 时间改变时.
    /// </summary>
    public void UpdateTime(float curTime, float deltaTime, int curIntervalIndex)
    {
        if (DontTakeNotes) return;
        if (IsReverse == TimeMgr.Inst.IsReverse)
        {
            OnUpdateByController(curTime, deltaTime);
            return;
        }
        timeStatusEnumerator = _TimeStatus.GetEnumerator();
        EntityTimeStatus status = default;
        int index = -1;
        while (timeStatusEnumerator.MoveNext())
        {
            if (timeStatusEnumerator.Current.Key == curIntervalIndex)
            {
                status = timeStatusEnumerator.Current.Value;
                index = timeStatusEnumerator.Current.Key;
                containsLastStatus = true;
                break;
            }
        }
        if (index < 0)
        {
            if (containsLastStatus)
                OnUpdateByStatus(lastStatus);
            return;
        }
        _TimeStatus.Remove(index);
        lastStatus = status;
        OnUpdateByStatus(lastStatus);
    }
    protected abstract void OnUpdateByController(float curTime, float deltaTime);
    protected abstract void OnUpdateByStatus(EntityTimeStatus status);
    protected abstract void OnResetStatus(EntityTimeStatus status);
    protected virtual void OnUpdateAlways() { }

    /// <summary>
    /// 时间运行方向改变时.
    /// </summary>
    public void UpdateTimeRuningDirectionChanged(bool isReverse)
    {
        if (containsLastStatus)
        {
            OnResetStatus(lastStatus);
            containsLastStatus = false;
        }
        OnTimeRuningDirectionChanged(isReverse);
    }

    protected abstract void OnTimeRuningDirectionChanged(bool isReverse);

    /// <summary>
    /// 待办事项状态更新.
    /// </summary>
    public void TODOListStatusUpdate(int status, float progress)
    {
        lastTODOStatus = status;
        lastTODOProgress = progress;
        OnTODOListStatusUpdate(status, progress);
    }
    protected virtual void OnTODOListStatusUpdate(int status, float progress) { }

    protected virtual void OnTODODoingCountChanged(int curCount) { }
}
