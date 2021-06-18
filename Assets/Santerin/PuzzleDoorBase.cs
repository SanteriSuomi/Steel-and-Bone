using DG.Tweening;
using UnityEngine;

public abstract class PuzzleDoorBase : MonoBehaviour
{
	[SerializeField] protected GameObject m_gameObject = default;
	protected DownMover wallMover;
	protected AudioSource wallAudioSource;
	protected Coroutine wallMoveCoroutine;
	protected Vector3 originalPos;
	[SerializeField]
	protected Vector3 movePosOffset = new Vector3(0, 6, 0);
	protected Vector3 movePos;

	protected virtual void Awake()
	{
		wallMover =  m_gameObject.GetComponent<DownMover>();
		wallAudioSource = m_gameObject.GetComponent<AudioSource>();
	}

	protected virtual void Start()
	{
		originalPos = wallMover.transform.localPosition;
		movePos = originalPos + movePosOffset;
	}

	protected abstract void OnTriggerEnter(Collider _);

	protected abstract void OnTriggerExit(Collider _);

	protected void DisableDoor()
	{
		Move(movePos);
	}

	protected void EnableDoor()
	{
		Move(originalPos);
	}

	private void Move(Vector3 towards)
	{
		DOTween.Kill(wallMover, false);
		if (wallMoveCoroutine != null)
		{
			StopCoroutine(wallMoveCoroutine);
		}

		wallMoveCoroutine = StartCoroutine(wallMover.Move(wallMover.transform, towards, 7, Ease.Linear,
			() => wallAudioSource.Play(),
			() => wallAudioSource.Stop()));
	}
}