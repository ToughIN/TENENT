using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanelMask : UIPanelBase
{
    public Animator _Animator;

    private Coroutine cor;

    public void Show(Action onComplete)
    {
        if (cor != null)
            StopCoroutine(cor);
        cor = StartCoroutine(IShowAct(onComplete));
    }

    public override void Hide()
    {
        if (cor != null)
            StopCoroutine(cor);
        cor = StartCoroutine(IHideAct());
    }

    private IEnumerator IShowAct(Action onComplete)
    {
        _Animator.Play("Show", 0, 0);
        yield return new WaitForSeconds(0.5f);
        cor = null;
        onComplete?.Invoke();
    }

    private IEnumerator IHideAct()
    {
        _Animator.Play("Hide", 0, 0);
        yield return new WaitForSeconds(0.5f);
        cor = null;
        base.Hide();
    }
}
