using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TODOList
{
    public static TODOList Inst { get { return inst; } }
    private static TODOList inst = new TODOList();

    public List<TODOArgs> TodoList { get { return todoList; } }
    private List<TODOArgs> todoList = new List<TODOArgs>();

    public int HashCode { get { return hashCode; } set { hashCode = value; } }
    private int hashCode = 1;

    private List<TODOArgs> doingList = new List<TODOArgs>();
    private float tempRatio;
    private TODOArgs tempArgs;

    /// <summary>
    /// 时间更新时
    /// </summary>
    public void OnTimeUpdate(float curTime, float deltaTime, int timeDirection)
    {
        for (int i = 0; i < todoList.Count; i++)
        {
            tempArgs = todoList[i];
            if (IsDoing(tempArgs)) continue;

            //首先判断该事件是否处于时间范围内
            tempRatio = ComputeEventTimeRatio(tempArgs, curTime, timeDirection);

            if (tempRatio > 0 && tempRatio < 1)
            {
                //事件1 时间1 = 0 | 事件1 时间-1 = 1 | 事件-1 时间1 = 1 | 事件-1 时间-1
                //发现：如果时间方向相同，那首次出现的进度必定是0，否则就是1
                tempRatio = tempArgs.TimeDirection == timeDirection ? 0 : 1;
                tempArgs.Entity.TODOListStatusUpdate(tempArgs.Status, tempRatio);
                doingList.Add(tempArgs);
                tempArgs.Entity.TODODoingCount++;

                if (tempArgs.TODOElse != null)
                {
                    for (int o = 0; o < tempArgs.TODOElse.Length; o++)
                    {
                        tempArgs.TODOElse[o].TODOListStatusUpdate(tempArgs.Status, tempRatio);
                        tempArgs.TODOElse[o].TODODoingCount++;
                    }
                }
            }
        }

        for (int i = 0; i < doingList.Count; i++)
        {
            tempArgs = doingList[i];
            tempRatio = ComputeEventTimeRatio(tempArgs, curTime, timeDirection);
            tempArgs.Entity.TODOListStatusUpdate(tempArgs.Status, Mathf.Clamp01(tempRatio));
            if (tempArgs.TODOElse != null)
                for (int o = 0; o < tempArgs.TODOElse.Length; o++)
                    tempArgs.TODOElse[o].TODOListStatusUpdate(tempArgs.Status, tempRatio);

            if (tempRatio < 0 || tempRatio > 1)
            {
                tempArgs.Entity.TODODoingCount--;
                if (tempArgs.TODOElse != null)
                    for (int o = 0; o < tempArgs.TODOElse.Length; o++)
                        tempArgs.TODOElse[o].TODODoingCount--;
                if (tempRatio < 0)
                {
                    if (GlobalStatus.IsPlaying)
                        RemoveTodoEvent(tempArgs);
                    i--;
                }
                else doingList.RemoveAt(i--);
            }
        }
    }

    /// <summary>
    /// 计算事件当前进度比例
    /// </summary>
    private float ComputeEventTimeRatio(TODOArgs todoArgs, float curTime, int timeDirection)
    {
        if (tempArgs.TimeDirection > 0)
            tempRatio = (curTime - tempArgs.Time) / tempArgs.Duration;
        else tempRatio = (tempArgs.Time - curTime) / tempArgs.Duration;
        //if (tempArgs.TimeDirection != timeDirection)
        //    tempRatio = 1 - tempRatio;
        return tempRatio;
    }

    private bool IsDoing(TODOArgs todoArgs)
    {
        for (int i = 0; i < doingList.Count; i++)
            if (doingList[i].HashCode == todoArgs.HashCode)
                return true;
        return false;
    }

    public TODOArgs AddTodoEvent(TODOArgs todoArgs)
    {
        todoArgs.HashCode = hashCode++;
        todoList.Add(todoArgs);
        return todoArgs;
    }

    public void RemoveTodoEvent(int hashCode)
    {
        for (int i = 0; i < todoList.Count; i++)
        {
            if (todoList[i].HashCode == hashCode)
            {
                todoList.RemoveAt(i);
                break;
            }
        }
        for (int i = 0; i < doingList.Count; i++)
        {
            if (doingList[i].HashCode == hashCode)
            {
                doingList.RemoveAt(i);
                break;
            }
        }
    }

    public void RemoveTodoEvent(TODOArgs todoArgs)
    {
        RemoveTodoEvent(todoArgs.HashCode);
    }

    public void RemoveEntityTodoEvent(EntityBase entity)
    {
        for (int i = 0; i < todoList.Count; i++)
        {
            if (todoList[i].Entity == entity)
                todoList.RemoveAt(i--);
        }
        for (int i = 0; i < doingList.Count; i++)
        {
            if (doingList[i].Entity == entity)
            {
                doingList.RemoveAt(i--);
                entity.TODODoingCount--;
            }
        }
    }

    public void RemoveAllTodoList()
    {
        todoList.Clear();
        doingList.Clear();
    }
}

public struct TODOArgs
{
    public EntityBase Entity;
    public int Status;
    public float Time;
    public int TimeDirection;
    public float Duration;

    //仅连锁一层
    public EntityBase[] TODOElse;

    //由TODOList脚本自行管理
    public int HashCode;

    public string EntityName;
    public string[] TODOElseNames;
}
