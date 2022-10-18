using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGame : MonoBehaviour
{
    private const float LONGPRESS_TIME = 0.3f;

    public static MainGame Inst;

    public Text _TimeText;
    public GameObject[] _Levels;

    private float curDuration;
    private bool buttonStay;

    private void Awake()
    {
        Inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        LevelMgr.Inst.StartLevel(_Levels[0]);
    }

    // Update is called once per frame
    void Update()
    {
        
        TimeMgr.Inst.Update();
        _TimeText.text = "Time:" + TimeMgr.Inst.CurTime.ToString("f2");

        //if (Input.GetMouseButtonDown(1) ||
        //    Input.GetKeyDown(KeyCode.Tab) ||
        //    Input.GetKeyDown(KeyCode.Space))
        //    TimeMgr.Inst.IsReverse = !TimeMgr.Inst.IsReverse;

        if (buttonStay)
            curDuration += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.R))
        {
            curDuration = 0;
            buttonStay = true;
        }
        else if (Input.GetKeyUp(KeyCode.R))
        {
            buttonStay = false;
            LevelMgr.Inst.StartLevel(_Levels[0]);
        }
        else if (buttonStay &&
            curDuration >= LONGPRESS_TIME)
        {
            buttonStay = false;
            LevelMgr.Inst.ClearCache();
            var uiPanel = UIMgr.Inst.OpenUIPanel<UIPanelMask>();
            uiPanel.Show(Restart);
        }

        if (Input.GetKeyDown(KeyCode.Return) &&
            TimeMgr.Inst.IsReverse)
        {
            LevelMgr.Inst.SaveCache();
            var uiPanel = UIMgr.Inst.OpenUIPanel<UIPanelMask>();
            uiPanel.Show(Restart);
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            var uiPanel = UIMgr.Inst.GetUIPanel<UIPanelHelp>();
            if (uiPanel == null || !uiPanel.IsShow)
                UIMgr.Inst.OpenUIPanel<UIPanelHelp>();
            else UIMgr.Inst.HideUIPanel<UIPanelHelp>();
        }
    }

    private void Restart()
    {
        LevelMgr.Inst.StartLevel(_Levels[0]);
        UIMgr.Inst.HideUIPanel<UIPanelMask>();
    }
}