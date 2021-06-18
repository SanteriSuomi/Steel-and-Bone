using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Aleksi_ItemClickHandler : MonoBehaviour
{
	private enum InventorySlotKeys
	{
		F1,
		F2,
		F3,
		F4,
		F5
	}

	public Aleksi_Inventory _Inventory;
	private Button _button;
	private Graphic graphic;
	[SerializeField]
	private Aleksi_ItemDragHandler dragHandler = default;
	public Aleksi_ItemDragHandler DragHandler => dragHandler;

	[SerializeField]
	private InputActionsVar inputActions = default;
	[SerializeField]
	private InventorySlotKeys keyToActivate = default;

	private static int currentSlotIndex;
	private static bool isModifyingIndex;

	private void Awake()
	{
		_button = GetComponent<Button>();
		graphic = GetComponent<Graphic>();
		SetStaticFields();
	}

	private static void SetStaticFields()
	{
		currentSlotIndex = 0;
		isModifyingIndex = false;
	}

	private void OnEnable()
	{
		inputActions.InputActions.Player.InventorySlots.performed += OnInventorySlotsPerformed;
	}

	private void OnInventorySlotsPerformed(InputAction.CallbackContext context)
	{
		try
		{
			if (Keyboard.current != null)
			{
				KeyboardCheck();
			}
			else if (Gamepad.current != null)
			{
				GamepadCheck(keyToActivate, () => DetermineSlotState(() => true));
			}
			else
			{
				Debug.LogError("No input devices found.");
			}
		}
		catch (NullReferenceException n)
		{
			Debug.Log(n);
		}
	}

	private void KeyboardCheck()
	{
		switch (keyToActivate)
		{
			case InventorySlotKeys.F1:
				DetermineSlotState(() => Keyboard.current?.f1Key.wasPressedThisFrame == true);
				break;

			case InventorySlotKeys.F2:
				DetermineSlotState(() => Keyboard.current?.f2Key.wasPressedThisFrame == true);
				break;

			case InventorySlotKeys.F3:
				DetermineSlotState(() => Keyboard.current?.f3Key.wasPressedThisFrame == true);
				break;

			case InventorySlotKeys.F4:
				DetermineSlotState(() => Keyboard.current?.f4Key.wasPressedThisFrame == true);
				break;

			case InventorySlotKeys.F5:
				DetermineSlotState(() => Keyboard.current?.f5Key.wasPressedThisFrame == true);
				break;
		}
	}

	private static void GamepadCheck(InventorySlotKeys key, Action determineState)
	{
		if (isModifyingIndex) return;
		isModifyingIndex = true;

		ModifyCurrentIndex();

		switch (currentSlotIndex)
		{
			case 0:
				if (key == InventorySlotKeys.F1)
				{
					determineState();
				}
				break;

			case 1:
				if (key == InventorySlotKeys.F2)
				{
					determineState();
				}
				break;

			case 2:
				if (key == InventorySlotKeys.F3)
				{
					determineState();
				}
				break;

			case 3:
				if (key == InventorySlotKeys.F4)
				{
					determineState();
				}
				break;

			case 4:
				if (key == InventorySlotKeys.F5)
				{
					determineState();
				}
				break;
		}

		isModifyingIndex = false;
	}

	private static void ModifyCurrentIndex()
	{
		if (Gamepad.current?.dpad.right.wasPressedThisFrame == true)
		{
			currentSlotIndex++;
		}
		else if (Gamepad.current?.dpad.left.wasPressedThisFrame == true)
		{
			currentSlotIndex--;
		}

		if (currentSlotIndex > 4)
		{
			currentSlotIndex = 4;
		}
		else if (currentSlotIndex < 0)
		{
			currentSlotIndex = 0;
		}
	}

	private void DetermineSlotState(Func<bool> statement)
	{
		if (statement())
		{
			ActivateSlot();
		}

		Invoke(nameof(DeactivateSlot), 0.25f);
	}

	private void ActivateSlot()
	{
		FadeToColor(_button.colors.pressedColor);
		_button.onClick.Invoke();
	}

	private void DeactivateSlot()
	{
		FadeToColor(_button.colors.normalColor);
	}

	private void FadeToColor(Color color)
	{
		graphic.CrossFadeColor(color, _button.colors.fadeDuration, true, true);
	}

	// Used by UI event
	public void OnItemClicked()
	{
		IInventoryItem item = dragHandler.Item;
		if (item != null)
		{
			_Inventory.UseItem(item, true);
			item.OnUse();
		}
	}

	public void OnItemClicked(IInventoryItem item)
	{
		_Inventory.UseItem(item, false);
		item.OnUse();
	}

	private void OnDisable()
	{
		inputActions.InputActions.Player.InventorySlots.performed -= OnInventorySlotsPerformed;
	}
}