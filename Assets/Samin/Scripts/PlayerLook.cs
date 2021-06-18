using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] public float m_MouseSensitivity = default;
    [SerializeField] private float mouseSensMultiplier = 0.75f;
    [SerializeField] private Transform m_pBody = default;

    private float xAxisClamp;
    public Camera m_Camera = default;
    public Transform m_MainCamera = default;
    public Transform m_CameraMovePos = default;
    public Transform m_CameraOrginalPos = default;
    public Transform m_KickPos = default;
    public bool m_KickingCamera = false;
    private GameObject levelPickerMenu;

    [SerializeField]
    private InputActionsVar inputActions = default;

    private void Start()
    {
        m_MainCamera = Camera.main.transform;
        xAxisClamp = 0.0f;
        levelPickerMenu = FindObjectOfType<LevelMenuScript>().gameObject;
    }

    private void Update()
    {
        // Don't allow camera rotation during ability menu or while executing some player ability
        if (levelPickerMenu.activeSelf) return;

        //if (isLooking)
        //{
        //    CameraRotation();
        //}

        CameraRotation();
    }

    private void CameraRotation()
    {
        Vector2 mouseInput = inputActions.InputActions.Player.Look.ReadValue<Vector2>();
        float mouseX = mouseInput.x * m_MouseSensitivity
            * mouseSensMultiplier * Time.deltaTime / 15;
        float mouseY = mouseInput.y * m_MouseSensitivity
            * mouseSensMultiplier * Time.deltaTime / 15;
        xAxisClamp += mouseY;

        if (xAxisClamp > 90.0f)
        {
            xAxisClamp = 90.0f;
            mouseY = 0.0f;
            ClampAxisRotationToValue(270.0f);
        }
        else if (xAxisClamp < -60.0f)
        {
            xAxisClamp = -60.0f;
            mouseY = 0.0f;
            ClampAxisRotationToValue(60.0f);
        }

        transform.Rotate(Vector3.left * mouseY);
        m_pBody.Rotate(Vector3.up * mouseX);
    }

    private void ClampAxisRotationToValue(float value)
    {
        Vector3 eulerRotation = transform.eulerAngles;
        eulerRotation.x = value;
        transform.eulerAngles = eulerRotation;
    }
}