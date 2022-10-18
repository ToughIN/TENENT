using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMgr
{
    public static EntityMgr Inst { get { return inst; } }
    private static EntityMgr inst = new EntityMgr();

    public List<EntityBase> Entitys { get { return entitys; } }
    private List<EntityBase> entitys = new List<EntityBase>();

    private EntityTimeStatus tempEntityStatus;

    public void OnEntityBorn(EntityBase entity)
    {
        if (!entitys.Contains(entity))
        {
            entity._TimeStatus = new Dictionary<int, EntityTimeStatus>();
            if (!entity.DontTakeNotes)
                entity.UpdateTimeRuningDirectionChanged(TimeMgr.Inst.IsReverse);
            entitys.Add(entity);
        }
    }

    public void OnEntityDie(EntityBase entity)
    {
        entitys.Remove(entity);
    }

    public void OnEntitysTimeValueChanged(float curTime, float deltaTime, int curIntervalCount)
    {
        for (int i = 0; i < entitys.Count; i++)
            entitys[i].UpdateTime(curTime, deltaTime, curIntervalCount);
    }

    public void OnTimeRuningDirectionChanged(bool isReverse, int curIntervalCount)
    {
        for (int i = 0; i < entitys.Count; i++)
        {
            if (entitys[i].DontTakeNotes)
                continue;
            entitys[i].UpdateTimeRuningDirectionChanged(isReverse);
            tempEntityStatus = entitys[i].GetTimeStatus();
            entitys[i]._TimeStatus[curIntervalCount] = tempEntityStatus;
        }
    }

    public void CopyEntitysTimeStatus(int curIntervalCount)
    {
        for (int i = 0; i < entitys.Count; i++)
        {
            if (entitys[i].DontTakeNotes)
                continue;
            if (entitys[i].IsReverse != TimeMgr.Inst.IsReverse)
                continue;
            tempEntityStatus = entitys[i].GetTimeStatus();
            entitys[i]._TimeStatus[curIntervalCount] = tempEntityStatus;
        }
    }
}
