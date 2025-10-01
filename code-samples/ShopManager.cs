using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private GameObject shopPanel; // Panel de la tienda
    [SerializeField] private Button buyHealthButton; // Botón para comprar salud
    [SerializeField] private Button buyAmmoButton; // Botón para comprar balas
    [SerializeField] private Button buyAKButton; // Botón para comprar el AK
    [SerializeField] private int healthCost = 100; // Precio de salud
    [SerializeField] private int ammoCost = 25; // Precio de balas
    [SerializeField] private int akCost = 250; // Precio del AK
    [SerializeField] private GameObject akObject; // Referencia al objeto del AK
    [SerializeField] private MonoBehaviour cameraController; // Script que controla la cámara

    private PlayerController playerController; // Referencia al jugador
    private bool isShopOpen = false; // Indica si la tienda está abierta

    private void Start()
    {
        // Referencia al PlayerController
        playerController = PlayerController.Instance;

        // Asegurarse de que el panel de la tienda esté cerrado al inicio
        shopPanel.SetActive(false);

        // Asegurarse de que el AK esté desactivado al inicio
        if (akObject != null)
        {
            akObject.SetActive(false);
        }

        // Asignar funciones a los botones
        buyHealthButton.onClick.AddListener(BuyHealth);
        buyAmmoButton.onClick.AddListener(BuyAmmo);
        buyAKButton.onClick.AddListener(BuyAK);
    }

    private void Update()
    {
        // Abrir y cerrar la tienda con la tecla Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleShopMenu();
        }
    }

    private void ToggleShopMenu()
    {
        isShopOpen = !isShopOpen; // Alternar el estado del menú
        shopPanel.SetActive(isShopOpen); // Activar o desactivar el panel

        // Pausar o reanudar el juego
        Time.timeScale = isShopOpen ? 0f : 1f;

        // Gestionar el cursor
        Cursor.lockState = isShopOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isShopOpen;

        // Deshabilitar el controlador de la cámara si la tienda está abierta
        if (cameraController != null)
        {
            cameraController.enabled = !isShopOpen;
        }
    }

    private void BuyHealth()
    {
        if (playerController.totalCoins >= healthCost)
        {
            playerController.totalCoins -= healthCost;
            playerController.Heal(50); // Aumenta la salud
            playerController.UpdateCoinsUI();
            Debug.Log("Has comprado salud.");
        }
        else
        {
            Debug.Log("No tienes suficientes monedas para comprar salud.");
        }
    }

    private void BuyAmmo()
    {
        if (playerController.totalCoins >= ammoCost)
        {
            playerController.totalCoins -= ammoCost;

            GameObject armaActual = FindObjectOfType<Cambio>().GetArmaActual();
            if (armaActual != null)
            {
                var pistol = armaActual.GetComponent<SimplePistol>();
                var ak = armaActual.GetComponent<SimpleAK>();

                if (pistol != null)
                {
                    pistol.RestoreAmmoToMax(); // Restaurar munición de la pistola
                }
                else if (ak != null)
                {
                    ak.RestoreAmmoToMax(); // Restaurar munición del AK
                }
            }

            playerController.UpdateCoinsUI();
            Debug.Log("Has comprado balas.");
        }
        else
        {
            Debug.Log("No tienes suficientes monedas para comprar balas.");
        }
    }

    private void BuyAK()
    {
        if (playerController.totalCoins >= akCost)
        {
            playerController.totalCoins -= akCost;

            // Habilitar el AK
            if (akObject != null)
            {
                akObject.SetActive(true);
                FindObjectOfType<Cambio>().UnlockAK(); // Desbloquear el AK en el cambio de armas
            }

            playerController.UpdateCoinsUI();
            Debug.Log("Has comprado el AK.");
        }
        else
        {
            Debug.Log("No tienes suficientes monedas para comprar el AK.");
        }
    }
}
