using DG.Tweening;
using UnityEngine;

/// <summary>
/// The purpose of this script is to initialize DOTween and make sure all tweens are killed when exiting.
/// </summary>
public class DoTweenManager : MonoBehaviour
{
    private void Awake()
    {
        DOTween.Init(false, false, LogBehaviour.ErrorsOnly).SetCapacity(100, 25);
    }

    private void OnDestroy()
    {
        DOTween.Clear();
    }
}