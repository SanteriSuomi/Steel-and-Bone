using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Aleksi_InventoryShowHide : MonoBehaviour
{
	public GameObject m_Inventory;
	public GameObject m_MainCamera;
	public bool HasPressed;
	public List<GameObject> m_InventorySlots;
	public AudioClip firstAudioClip;
	public AudioSource audioSource;
	private AbilityBarHandler m_Abilitybar;
	private Aleksi_ItemClickHandler[] clickHandlers;
	private Vector3 inventoryOriginalScale;
	[SerializeField]
	private InputActionsVar inputActions = default;

	private bool isTweening;

	private void Awake()
	{
		m_Abilitybar = FindObjectOfType<AbilityBarHandler>();
		inventoryOriginalScale = m_Inventory.transform.localScale;
		clickHandlers = new Aleksi_ItemClickHandler[m_InventorySlots.Count];
		for (int i = 0; i < clickHandlers.Length; i++)
		{
			clickHandlers[i] = m_InventorySlots[i].GetComponentInChildren<Aleksi_ItemClickHandler>();
		}
	}

	private void OnEnable()
	{
		inputActions.InputActions.Player.Inventory.performed += OnInventoryPerformed;
	}

	private void OnInventoryPerformed(InputAction.CallbackContext context)
	{
		if (isTweening) return;

		if (!HasPressed)
		{
			TweenInventoryIn();
			ActivateSlots(true, InventoryItemBase.CurrentlyEquippedItem);

			m_Abilitybar.HideAbilityBar();
			audioSource.PlayOneShot(firstAudioClip);
		}
		else
		{
			TweenInventoryOut();
			ActivateSlots(false, InventoryItemBase.CurrentlyEquippedItem);

			m_Abilitybar.ShowAbilityBar();
			audioSource.PlayOneShot(firstAudioClip);
		}
	}

	private void TweenInventoryIn()
	{
		isTweening = true;
		m_Inventory.SetActive(true);
		m_Inventory.transform.DOScale(inventoryOriginalScale, 0.45f)
				.SetRecyclable(true)
				.SetEase(Ease.OutExpo)
				.OnComplete(() =>
				{
					if (!HasPressed)
					{
						HasPressed = true;
					}

					if (isTweening)
					{
						isTweening = false;
					}
				});
	}

	private void TweenInventoryOut()
	{
		isTweening = true;
		m_Inventory.transform.DOScale(0, 0.45f)
				.SetRecyclable(true)
				.SetEase(Ease.Flash)
				.OnComplete(() =>
				{
					if (m_Inventory.activeSelf)
					{
						m_Inventory.SetActive(false);
					}

					if (HasPressed)
					{
						HasPressed = false;
					}

					if (isTweening)
					{
						isTweening = false;
					}
				});
	}

	private void ActivateSlots(bool value, ItemType currentlyEquippedItem)
	{
		for (int i = 0; i < m_InventorySlots.Count; i++)
		{
			if (value)
			{
				Aleksi_ItemClickHandler clickHandler = clickHandlers[i];
				if (clickHandler.DragHandler.Item != null
					&& clickHandler.DragHandler.Item.ItemType != ItemType.HealthPotion
					&& clickHandler.DragHandler.Item.ItemType != ItemType.None
					/*&& clickHandler.DragHandler.Item.ItemType == currentlyEquippedItem*/)
				{
					clickHandler.OnItemClicked(clickHandler.DragHandler.Item);
				}

				m_InventorySlots[i].SetActive(value);
			}
			else
			{
				m_InventorySlots[i].SetActive(value);
			}
		}
	}

	private void OnDisable()
	{
		inputActions.InputActions.Player.Inventory.performed -= OnInventoryPerformed;
	}
}