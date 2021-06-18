using Essentials.Saving;
using MessagePack;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

/// <summary>
/// When this script is added to an object, its position and rotation will be saved.
/// </summary>
public class PositionRotationSave : MonoBehaviour, ISaveable
{
	private void Awake()
	{
		SaveSystem.Register(this);
	}

	#region Saving
	public SaveData GetSave()
	{
		return new PositionRotationData(gameObject.name)
		{
			position = transform.position,
			rotation = transform.rotation
		};
	}

	public void Load(SaveData saveData)
	{
		if (saveData is PositionRotationData save)
		{
			if (transform.CompareTag("Player")) // Handle player position/rotation saving differently, as playercontroller forces the position every frame.
			{
				InteractableComponents.Instance.PlayerController.enabled = false;
				transform.SetPositionAndRotation(save.position, save.rotation);
				InteractableComponents.Instance.PlayerController.enabled = true;
			}
			else
			{
				transform.SetPositionAndRotation(save.position, save.rotation);
			}
		}
	}

	[Serializable, MessagePackObject, SuppressMessage("Usage", "CA2235:Mark all non-serializable fields", Justification = "Serialized using serialization surogates")]
	public class PositionRotationData : SaveData
	{
		[Key("Position")]
		public Vector3 position;
		[Key("Rotation")]
		public Quaternion rotation;

		public PositionRotationData() { }

		public PositionRotationData(string objName)
		{
			this.objName = objName;
		}
	}
	#endregion
}