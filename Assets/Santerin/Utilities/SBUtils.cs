using System.Runtime.InteropServices;
using UnityEngine;
using System;

[Flags]
public enum MouseClickEvents
{
	LEFTDOWN = 0x00000002,
	LEFTUP = 0x00000004,
	MIDDLEDOWN = 0x00000020,
	MIDDLEUP = 0x00000040,
	MOVE = 0x00000001,
	ABSOLUTE = 0x00008000,
	RIGHTDOWN = 0x00000008,
	RIGHTUP = 0x00000010
}

public static class SBUtils
{
	public static bool PosApproxEqual(Vector3 current, Vector3 goal, float minDifference)
	{
		return FastAbs(current.x - goal.x) <= minDifference
			   && FastAbs(current.y - goal.y) <= minDifference
			   && FastAbs(current.z - goal.z) <= minDifference;
	}

	public static float FastAbs(float value)
	{
		return value >= 0 ? value : -value;
	}

	public static float FastFloatLerp(float current, float goal, float t)
	{
		return (1 - t) * current + t * goal;
	}

	[DllImport("user32.dll")]
	private static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);

	public static void MouseClick(MouseClickEvents @event, Vector2 position)
	{
		System.Windows.Forms.Cursor.Position = new System.Drawing.Point((int)position.x, (int)position.y);
		mouse_event((uint)@event, 0, 0, 0, 0);
	}
}