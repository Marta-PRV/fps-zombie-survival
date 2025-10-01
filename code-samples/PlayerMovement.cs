using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField, Range(1, 5)] private float moveSpeed;
    [SerializeField, Range(20, 360)] private float rotationSpeed;
    [SerializeField] private float rotationAngleXLimit = 40f;
    [SerializeField] private float jumpForce, verticalVelocity, terminalVelocity, gravity;
    [SerializeField] private bool isXInverted, isYInverted;
    [SerializeField] private float runSpeedMultiplier = 2f;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private Transform cameraTransform;

    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f; // Estamina máxima
    [SerializeField] private float stamina = 100f;   // Estamina actual
    [SerializeField] private float staminaDrainRate = 20f; // Velocidad de consumo al correr
    [SerializeField] private float staminaRegenRate = 10f; // Velocidad de regeneración
    [SerializeField] private float regenDelay = 2f;        // Tiempo de espera para regenerar
    [SerializeField] private UnityEngine.UI.Slider staminaBar; // Barra de estamina en UI

    private float regenTimer; // Temporizador para regeneración
    private bool isSprinting; // Indica si el jugador está corriendo

    private int _inversionX => isXInverted ? -1 : 1;
    private int _inversionY => isYInverted ? -1 : 1;

    private CharacterController _characterController;
    private Vector3 _playerCameraRotation;
    private Vector2 _cameraRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _characterController = GetComponent<CharacterController>();

        _playerCameraRotation = Vector3.zero;
        _cameraRotation = Vector2.zero;

        // Asegúrate de que la estamina comienza al máximo
        stamina = maxStamina;

        if (staminaBar != null)
        {
            staminaBar.maxValue = maxStamina;
            staminaBar.value = stamina;
        }
    }

    private void Update()
    {
        if (!_characterController.isGrounded)
        {
            ApplyGravity();
        }
        else
        {
            if (Input.GetAxisRaw("Jump") > 0.5f)
            {
                verticalVelocity = jumpForce;
            }
            else
            {
                verticalVelocity = -1;
            }
        }

        // Sprinting y consumo de estamina
        if (Input.GetKey(KeyCode.LeftShift) && stamina > 0)
        {
            isSprinting = true;
            stamina -= staminaDrainRate * Time.deltaTime; // Consume estamina
            if (stamina < 0) stamina = 0;
            regenTimer = 0; // Reinicia el temporizador de regeneración
        }
        else
        {
            isSprinting = false;
            regenTimer += Time.deltaTime;

            // Regeneración de estamina después de un retraso
            if (regenTimer >= regenDelay)
            {
                stamina += staminaRegenRate * Time.deltaTime;
                if (stamina > maxStamina) stamina = maxStamina;
            }
        }

        // Actualizar la barra de estamina
        if (staminaBar != null)
        {
            staminaBar.value = stamina; // Actualiza el slider
        }

        // Ajustar velocidad según estamina
        float currentSpeed = isSprinting ? moveSpeed * runSpeedMultiplier : moveSpeed;

        // Movimiento del jugador
        Vector3 movementDirection = transform.right * Input.GetAxis("Horizontal") +
            transform.forward * Input.GetAxis("Vertical");
        movementDirection.Normalize();
        _characterController.Move(movementDirection * (currentSpeed * Time.deltaTime) + Vector3.up * (verticalVelocity * Time.deltaTime));

        // Rotación de la cámara
        _cameraRotation.x = Input.GetAxis("Mouse Y") * _inversionY * mouseSensitivity;
        _cameraRotation.y = Input.GetAxis("Mouse X") * _inversionX * mouseSensitivity;

        Vector3 rotationVector = new Vector3(_cameraRotation.x, 0, 0);

        cameraTransform.Rotate(rotationVector);

        float angleTransformation = cameraTransform.rotation.eulerAngles.x > 180
            ? cameraTransform.localEulerAngles.x - 360
            : cameraTransform.localEulerAngles.x;

        rotationVector = new Vector3(Mathf.Clamp(angleTransformation, -rotationAngleXLimit, rotationAngleXLimit), 0, 0);

        cameraTransform.localEulerAngles = rotationVector;

        _playerCameraRotation.y = _cameraRotation.y;

        transform.Rotate(_playerCameraRotation * (rotationSpeed * Time.deltaTime));
    }

    private void ApplyGravity()
    {
        verticalVelocity -= gravity * Time.deltaTime;
        if (verticalVelocity <= -terminalVelocity)
        {
            verticalVelocity = -terminalVelocity;
        }
    }
}