using UnityEngine;

public class UIPanelBase : MonoBehaviour
{
    public bool IsShow { get { return isShow; } }
    protected bool isShow;

    private void Awake() { Initialize(); }

    protected virtual void Initialize() { Show(); }

    public virtual void Show()
    {
        if (isShow)
            return;
        gameObject.SetActive(true);
        isShow = true;
    }

    public virtual void Hide()
    {
        if (!isShow)
            return;
        gameObject.SetActive(false);
        isShow = false;
    }

    public static T FactoryPanel<T>() where T : UIPanelBase
    {
        string panelName = typeof(T).Name;
        GameObject obj = Resources.Load<GameObject>("GUI/" + panelName);
        obj = GameObject.Instantiate(obj, UIMgr.Inst.UIParent);
        T cmp = obj.GetComponent<T>();
        System.Diagnostics.Debug.Assert(ReferenceEquals(cmp, null),
            string.Format("error: ui {0} not append component.", panelName));
        return cmp;
    }

    public static UIPanelBase FactoryPanel(string panelName)
    {
        GameObject obj = Resources.Load<GameObject>("GUI/" + panelName);
        obj = GameObject.Instantiate(obj, UIMgr.Inst.UIParent);
        UIPanelBase cmp = obj.GetComponent<UIPanelBase>();
        System.Diagnostics.Debug.Assert(ReferenceEquals(cmp, null),
            string.Format("error: ui {0} not append component.", panelName));
        return cmp;
    }

    protected virtual void OnDestroy()
    {
        StopAllCoroutines();
        CancelInvoke();
    }
}