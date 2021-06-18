using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Input Actions Variable")]
public class InputActionsVar : ScriptableObject
{
    private InputActions inputActions;
    public InputActions InputActions
    {
        get
        {
            return inputActions ?? (inputActions = new InputActions());
        }
    }

    private void OnEnable()
    {
        InputActions.Enable();
		InputActions.Player.Move.performed += OnMovePerformed;
		InputActions.Player.Move.canceled += OnMoveCanceled;
    }

	[Flags]
	public enum InputDirection
	{
		None = 0,
		Up = 1,
		Down = 2,
		Left = 4,
		Right = 8
	}

    public InputDirection CurrentInputDirection { get; private set; }

	private void OnMovePerformed(InputAction.CallbackContext context)
	{
		Vector2 moveInputValueFloat = context.ReadValue<Vector2>();
		Vector2Int moveInputValue = new Vector2Int((int)moveInputValueFloat.x, (int)moveInputValueFloat.y);
		if (moveInputValue.x > 0)
		{
			CurrentInputDirection |= InputDirection.Right;
		}
		else
		{
			CurrentInputDirection |= InputDirection.Left;
		}

		if (moveInputValue.y > 0)
		{
			CurrentInputDirection |= InputDirection.Up;
		}
		else
		{
			CurrentInputDirection |= InputDirection.Down;
		}
	}

	private void OnMoveCanceled(InputAction.CallbackContext context)
	{
		CurrentInputDirection = InputDirection.None;
	}

    private void OnDisable()
    {
		InputActions.Player.Move.performed -= OnMovePerformed;
		InputActions.Player.Move.canceled -= OnMoveCanceled;
        InputActions.Disable();
    }
}