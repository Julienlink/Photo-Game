using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    PlayerInput playerInput;
    Rigidbody rb;
    private float verticalRotation = 0f;
    public GameObject cameraPivot;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void OnEnable()
    {

        playerInput.actions["Look"].performed += look;

        playerInput.actions["Look"].canceled += look;
    }

    private void OnDisable()
    {

        playerInput.actions["Look"].performed -= look;

        playerInput.actions["Look"].canceled -= look;
    }

    void look(InputAction.CallbackContext ctx)
    {
        Vector2 lookInput = ctx.ReadValue<Vector2>();
        // Rotation horizontale
        transform.Rotate(Vector3.up * lookInput.x);

        // Rotation verticale
        verticalRotation -= lookInput.y;
        verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f);

        cameraPivot.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    private void FixedUpdate()
    {
        Vector2 moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
        Vector3 moveDir = transform.right * moveInput.x + transform.forward * moveInput.y;
        rb.MovePosition(rb.position + moveDir * 10 * Time.fixedDeltaTime);
    }
}
