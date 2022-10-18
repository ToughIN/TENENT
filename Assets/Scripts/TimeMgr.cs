using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeMgr
{
    private const float INTERVAL_COPYSTATUS = 0.05f;
    private const float INIT_TIME = 10000;

    public static TimeMgr Inst { get { return inst; } }
    private static TimeMgr inst = new TimeMgr();

    public bool IsReverse
    {
        get { return runingDirection < 0; }
        set
        {
            runingDirection = value ? -1 : 1;
            EntityMgr.Inst.OnTimeRuningDirectionChanged(value, lastIntervalCount);
        }
    }

    public float CurTime { get { return curTime - INIT_TIME; } }

    public int LastIntervalCount { get { return lastIntervalCount; } }

    private int runingDirection = -1;
    private float curTime = INIT_TIME;
    private int lastIntervalCount = 0;
    private int tempIntervalCount = 0;

    public void ResetTime(float time)
    {
        curTime = INIT_TIME + time;
        tempIntervalCount = tempIntervalCount = Mathf.FloorToInt(curTime / INTERVAL_COPYSTATUS);
        lastIntervalCount = tempIntervalCount;
    }

    public void Update()
    {
        Debug.Log(CurTime);
        curTime += Time.deltaTime * runingDirection;
        EntityMgr.Inst.OnEntitysTimeValueChanged(CurTime, Time.deltaTime, lastIntervalCount);

        tempIntervalCount = Mathf.FloorToInt(curTime / INTERVAL_COPYSTATUS);
        if (lastIntervalCount != tempIntervalCount)
        {
            EntityMgr.Inst.CopyEntitysTimeStatus(tempIntervalCount);
            lastIntervalCount = tempIntervalCount;
        }

        TODOList.Inst.OnTimeUpdate(CurTime, Time.deltaTime, runingDirection);

        //Time.timeScale = Input.anyKey ? 1 : 0;
    }
}
