using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class DownMover : MonoBehaviour
{
    public IEnumerator Move(Transform obj, float endValue, float duration, Ease ease, Action onStart, Action onEnd)
    {
        yield return obj.DOLocalMoveY(endValue, duration, false)
                        .SetEase(ease)
                        .OnStart(() => onStart.Invoke())
                        .OnComplete(() => onEnd.Invoke())
                        .WaitForCompletion();
    }

    public IEnumerator Move(Transform obj, Vector3 endValue, float duration, Ease ease, Action onStart, Action onEnd)
    {
        yield return obj.DOLocalMove(endValue, duration, false)
                        .SetEase(ease)
                        .OnStart(() => onStart.Invoke())
                        .OnComplete(() => onEnd.Invoke())
                        .WaitForCompletion();
    }
}