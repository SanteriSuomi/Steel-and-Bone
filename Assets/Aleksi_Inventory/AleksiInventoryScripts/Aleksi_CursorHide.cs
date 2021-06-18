using UnityEngine;

public class Aleksi_CursorHide : MonoBehaviour
{
	public GameObject Inventory;
	public GameObject LevelSelector;
	public bool LevelStats = false;

    private void Update()
    {
		if (Inventory.activeInHierarchy && !LevelSelector.activeInHierarchy && LevelStats)
		{			
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Confined;
		}
		else if (!Inventory.activeInHierarchy && !LevelSelector.activeInHierarchy && LevelStats)
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
    }
}