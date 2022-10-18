using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMgr
{
    public static LevelMgr Inst { get { return inst; } }
    private static LevelMgr inst = new LevelMgr();

    private Dictionary<string, Dictionary<int, EntityTimeStatus>> saveStatus = new Dictionary<string, Dictionary<int, EntityTimeStatus>>();
    private List<TODOArgs> saveTodoList = new List<TODOArgs>();
    private Dictionary<string, LastTODOStatus> lastTODOStatus = new Dictionary<string, LastTODOStatus>();
    private int saveTODOHasCode;
    private float saveTime;

    private bool containsCache;
    private GameObject levelObj;

    public void StartLevel(GameObject levelPrefab)
    {
        if (levelObj != null)
            GameObject.DestroyImmediate(levelObj);

        TimeMgr.Inst.ResetTime(saveTime);
        levelObj = GameObject.Instantiate(levelPrefab);
        if (containsCache)
        {
            TimeMgr.Inst.IsReverse = false;
            ReloadCache();
        }
        else
        {
            TimeMgr.Inst.IsReverse = true;
        }
    }

    private void ReloadCache()
    {
        var enumerator = saveStatus.GetEnumerator();
        GameObject obj;
        EntityBase entity;
        while (enumerator.MoveNext())
        {
            obj = GameObject.Find(enumerator.Current.Key);
            if (obj != null &&
                (entity = obj.GetComponent<EntityBase>()) != null)
            {
                foreach (var v in enumerator.Current.Value)
                    entity._TimeStatus[v.Key] = v.Value;
            }
        }

        TODOArgs todoArgs;
        for (int i = 0; i < saveTodoList.Count; i++)
        {
            todoArgs = saveTodoList[i];
            obj = GameObject.Find(todoArgs.EntityName);
            if (obj != null &&
                (entity = obj.GetComponent<EntityBase>()) != null)
            {
                todoArgs.Entity = entity;
                if (todoArgs.TODOElseNames != null)
                {
                    for (int o = 0; o < todoArgs.TODOElseNames.Length; o++)
                    {
                        obj = GameObject.Find(todoArgs.TODOElseNames[o]);
                        if (obj == null ||
                            (entity = obj.GetComponent<EntityBase>()) == null)
                        {
                            Debug.LogError("error");
                            break;
                        }
                        todoArgs.TODOElse[o] = entity;
                    }
                }
                TODOList.Inst.TodoList.Add(todoArgs);
            }
        }

        LastTODOStatus lastTodo;
        var e = lastTODOStatus.GetEnumerator();
        while (e.MoveNext())
        {
            lastTodo = e.Current.Value;
            obj = GameObject.Find(e.Current.Key);
            if (obj != null &&
                (entity = obj.GetComponent<EntityBase>()) != null)
                entity.TODOListStatusUpdate(lastTodo.Status, lastTodo.Progress);
        }
    }

    public void StopLevel()
    {
        if (levelObj != null)
            GameObject.Destroy(levelObj);
    }

    public void SaveCache()
    {
        ClearCache();
        containsCache = true;
        var entitys = EntityMgr.Inst.Entitys;
        EntityBase entity;
        Dictionary<int, EntityTimeStatus> status;
        for (int i = 0; i < entitys.Count; i++)
        {
            entity = entitys[i];
            if (!entity.IsReverse || entity._TimeStatus.Count == 0)
                continue;
            if (!saveStatus.TryGetValue(entity.name, out status))
                status = saveStatus[entity.name] = new Dictionary<int, EntityTimeStatus>();
            foreach (var v in entity._TimeStatus)
                status[v.Key] = v.Value;
        }

        var todoList = TODOList.Inst.TodoList;
        TODOArgs todoArgs;
        for (int i = 0; i < todoList.Count; i++)
        {
            todoArgs = todoList[i];
            todoArgs.EntityName = todoArgs.Entity.name;
            if (todoArgs.TODOElse != null)
            {
                todoArgs.TODOElseNames = new string[todoArgs.TODOElse.Length];
                for (int o = 0; o < todoArgs.TODOElse.Length; o++)
                {
                    todoArgs.TODOElseNames[o] = todoArgs.TODOElse[o].name;
                }
            }
            saveTodoList.Add(todoArgs);
            saveTODOHasCode = Mathf.Max(todoList[i].HashCode, saveTODOHasCode);
        }

        for (int i = 0; i < saveTodoList.Count; i++)
        {
            todoArgs = saveTodoList[i];
            lastTODOStatus[todoArgs.EntityName] = new LastTODOStatus
            {
                Status = todoArgs.Entity.LastTODOStatus,
                Progress = todoArgs.Entity.LastTODOProgress,
            };
            if (todoArgs.TODOElseNames != null)
            {
                for (int o = 0; o < todoArgs.TODOElseNames.Length; o++)
                {
                    lastTODOStatus[todoArgs.TODOElseNames[o]] = new LastTODOStatus
                    {
                        Status = todoArgs.TODOElse[o].LastTODOStatus,
                        Progress = todoArgs.TODOElse[o].LastTODOProgress,
                    };
                }
            }
        }

        saveTODOHasCode = Mathf.Max(saveTODOHasCode, 1);
        saveTime = TimeMgr.Inst.CurTime;
        TODOList.Inst.RemoveAllTodoList();
    }

    public void ClearCache()
    {
        containsCache = false;
        saveTime = 0;
        saveTODOHasCode = 1;
        saveStatus.Clear();
        saveTodoList.Clear();
        lastTODOStatus.Clear();
    }

    private struct LastTODOStatus
    {
        public int Status;
        public float Progress;
    }
}
