using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMgr
{
    public static UIMgr Inst { get { return inst; } }
    private static UIMgr inst = new UIMgr();

    public Transform UIParent { get { return uiParent; } }
    private Transform uiParent;

    private Canvas canvas;

    private Dictionary<string, UIPanelBase> panelDict = new Dictionary<string, UIPanelBase>();

    public UIMgr()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        uiParent = GameObject.Find("UI Parent").transform;
        uiParent.SetParent(canvas.transform);
        GameObject.DontDestroyOnLoad(uiParent.gameObject);
    }

    /// <summary>
    /// 打开UI,如果没有创建则创建.
    /// </summary>
    public T OpenUIPanel<T>() where T : UIPanelBase
    {
        string panelName = typeof(T).Name;
        UIPanelBase panel;
        if (!panelDict.TryGetValue(panelName, out panel))
        {
            panel = FactoryPanel<T>();
            panelDict[panelName] = panel;
        }

        panel.Show();
        return panel as T;
    }

    /// <summary>
    /// 获取UI,如果没有就返回Null.
    /// </summary>
    public T GetUIPanel<T>() where T : UIPanelBase
    {
        string panelName = typeof(T).Name;
        UIPanelBase panel;
        panelDict.TryGetValue(panelName, out panel);
        return panel as T;
    }

    /// <summary>
    /// 隐藏UI.
    /// </summary>
    public void HideUIPanel<T>() where T : UIPanelBase
    {
        string panelName = typeof(T).Name;
        UIPanelBase panel;
        if (panelDict.TryGetValue(panelName, out panel))
        {
            panel.Hide();
        }
    }

    /// <summary>
    /// 独立创建UI
    /// 销毁需调用此类中的Destroy_CreateUIPanel函数.
    /// </summary>
    public T CreateUIPanel<T>() where T : UIPanelBase
    {
        string panelName = typeof(T).Name;
        T panel = FactoryPanel<T>();
        panel.Show();
        return panel as T;
    }

    /// <summary>
    /// 独立创建UI
    /// 销毁需调用此类中的Destroy_CreateUIPanel函数.
    /// </summary>
    public UIPanelBase CreateUIPanel(string panelName)
    {
        UIPanelBase panel = FactoryPanel(panelName);
        panel.Show();
        return panel;
    }

    /// <summary>
    /// 销毁独立创建的UI.
    /// </summary>
    public void Destroy_CreateUIPanel(UIPanelBase uiPanel)
    {
        if (uiPanel != null)
        {
            GameObject.Destroy(uiPanel.gameObject);
            uiPanel = null;
        }
    }

    /// <summary>
    /// 创建UI.
    /// </summary>
    private T FactoryPanel<T>() where T : UIPanelBase { return UIPanelBase.FactoryPanel<T>(); }

    /// <summary>
    /// 创建UI.
    /// </summary>
    private UIPanelBase FactoryPanel(string panelName) { return UIPanelBase.FactoryPanel(panelName); }
}