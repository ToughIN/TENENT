using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassAreaDetect : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer("Player"))
            return;
        var player = col.GetComponent<PlayerEntity>();
        if (!player.IsReverse && !TimeMgr.Inst.IsReverse)
        {
            var uiPanel = UIMgr.Inst.OpenUIPanel<UIPanelMask>();
            uiPanel.Show(Restart);
        }
    }

    private void Restart()
    {
        LevelMgr.Inst.ClearCache();
        LevelMgr.Inst.StartLevel(MainGame.Inst._Levels[0]);
        UIMgr.Inst.HideUIPanel<UIPanelMask>();
    }
}
