using UnityEngine;

public class SimplePistol : MonoBehaviour
{
    public Transform m_raycastSpot;
    public float m_damage = 80.0f;
    public float m_forceToApply = 20.0f;
    public float m_weaponRange = 9999.0f;
    public int m_maxAmmo = 12;
    public int m_magazineCount = 5; // Cantidad de cargadores
    public int m_bulletsPerSecond = 2;
    public float m_accuracyDropPerShot = 5.0f;
    public float m_accuracyRecoveryPerSecond = 20.0f;

    public Animator muzzleAnimator;

    private int m_currentAmmo;
    private float m_timeBetweenShots;
    private float m_shotTimer = 0;
    private float m_currentAccuracy = 100.0f;
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        m_currentAmmo = m_maxAmmo;
        m_timeBetweenShots = 1f / m_bulletsPerSecond;

        // Inicializa la UI con la munición actual
        PlayerController.Instance.UpdateAmmoUI(m_currentAmmo, m_magazineCount);
    }

    private void Update()
    {
        // Evitar disparos o recarga si el juego está pausado
        if (Time.timeScale == 0f) return;

        m_shotTimer += Time.deltaTime;
        m_currentAccuracy = Mathf.Min(m_currentAccuracy + Time.deltaTime * m_accuracyRecoveryPerSecond, 100.0f);

        if (Input.GetButtonDown("Fire2")) // Recargar
        {
            Reload();
        }

        if (m_shotTimer >= m_timeBetweenShots && Input.GetButtonDown("Fire1") && m_currentAmmo > 0)
        {
            m_shotTimer = 0;
            Shoot();
        }
    }

    private void Reload()
    {
        if (m_magazineCount > 0 && m_currentAmmo < m_maxAmmo)
        {
            m_magazineCount--;
            m_currentAmmo = m_maxAmmo;
            SoundManager.Instance.PlayReloadSound();
            PlayerController.Instance.UpdateAmmoUI(m_currentAmmo, m_magazineCount);
        }
        else if (m_magazineCount == 0)
        {
            Debug.Log("No quedan cargadores.");
        }
    }

    private void Shoot()
    {
        m_currentAmmo--;
        PlayerController.Instance.UpdateAmmoUI(m_currentAmmo, m_magazineCount);

        if (muzzleAnimator != null)
        {
            GameObject muzzleObject = muzzleAnimator.gameObject;
            muzzleObject.SetActive(true);
            Invoke(nameof(DisableMuzzleFlash), 0.1f);
        }

        float accuracyModifier = (100 - m_currentAccuracy) / 1000;
        Vector3 shotDirection = m_raycastSpot.forward;
        shotDirection.x += Random.Range(-accuracyModifier, accuracyModifier);
        shotDirection.y += Random.Range(-accuracyModifier, accuracyModifier);

        Ray ray = new Ray(m_raycastSpot.position, shotDirection);
        Debug.DrawRay(m_raycastSpot.position, shotDirection * m_weaponRange, Color.green, 1);

        if (Physics.Raycast(ray, out RaycastHit hit, m_weaponRange))
        {
            Debug.Log("Hit detected: " + hit.transform.name);

            // Verifica si el enemigo fue impactado
            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(1); // Aplica 1 de daño al enemigo
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(ray.direction * m_forceToApply);
            }
        }

        m_currentAccuracy = Mathf.Max(m_currentAccuracy - m_accuracyDropPerShot, 0);
        SoundManager.Instance.PlayShotSound();
    }

    private void DisableMuzzleFlash()
    {
        if (muzzleAnimator != null)
        {
            muzzleAnimator.gameObject.SetActive(false);
        }
    }

    public void RestoreAmmoToMax()
    {
        m_currentAmmo = m_maxAmmo; // Restaurar balas al máximo
        m_magazineCount = 5;       // Restaurar cargadores al máximo
        PlayerController.Instance.UpdateAmmoUI(m_currentAmmo, m_magazineCount); // Actualizar la UI
    }

    public int GetCurrentAmmo() => m_currentAmmo;
    public int GetMagazineCount() => m_magazineCount;
}
