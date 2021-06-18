using UnityEngine;
/// <summary>
/// Hold some references for interactables such as potion/bottles to prevent needing to search the scene every time one gets spawned.
/// </summary>
public class InteractableComponents : Singleton<InteractableComponents>
{
    public SAMI_PlayerHealth PlayerHealth { get; private set; }
    public PlayerCharacterController PlayerController { get; private set; }
    public SAMI_PlayerStats PlayerStats { get; private set; }
    public WaitForSeconds ActionsRaycastEnableWFS { get; private set; }

    protected override void Awake()
    {
        PlayerHealth = FindObjectOfType<SAMI_PlayerHealth>();
        PlayerController = PlayerHealth.GetComponent<PlayerCharacterController>();
        PlayerStats = FindObjectOfType<SAMI_PlayerStats>();
        ActionsRaycastEnableWFS = new WaitForSeconds(1);
    }
}