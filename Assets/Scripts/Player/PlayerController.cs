using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour, IDamageable
{
    [SerializeField] private PlayerData playerData;

    private float currentHP;
    private PlayerInput playerInput;
    private Vector2 moveInput;

    public GameManager gameManager;
    public int skor = 0;
    // ... kode gerak dari Tugas 1 ...

    // Variabel untuk menyimpan input serangan sebelumnya
    public GameObject bulletPrefab;
    // variabel untuk menentukan posisi spawn peluru
    public Transform bulletSpawnPoint;
    private float attackInput;
    // variabe untuk menyimpan input serangan sebelumnya agar bisa mendeteksi perubahan dari tidak menekan ke menekan
    private float previousAttackInput;
    public TextMeshProUGUI teksSkor;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        currentHP = playerData.maxHP;
        gameManager = FindFirstObjectByType<GameManager>();
        
    }

    void Shoot()
    {
        Debug.Log("Player is shooting!");

        if (bulletPrefab == null)
        {
            Debug.LogWarning("Bullet prefab not assigned!");
            return;
        }

        // Determine spawn position
        Vector3 spawnPos = bulletSpawnPoint != null ? bulletSpawnPoint.position : transform.position;

        // Get mouse position in world space for 2D
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0; // Ensure Z is 0 for 2D

        // Calculate direction from player to mouse
        Vector3 shootDirection = (mouseWorldPos - spawnPos).normalized;

        Debug.Log($"Spawn Pos: {spawnPos}, Mouse World Pos: {mouseWorldPos}, Direction: {shootDirection}");

        // Instantiate bullet
        // GameObject bulletObj = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        GameObject bulletObj = ObjectPooled.Instance.GetPooledObject();


        if (bulletObj != null)
        {
            // Atur posisi dan rotasi peluru
            bulletObj.transform.position = spawnPos;
            bulletObj.transform.rotation = Quaternion.identity;

            // Aktifkan peluru
            bulletObj.SetActive(true);



            // Set bullet direction
            Bullet bullet = bulletObj.GetComponent<Bullet>();

            if (bullet != null)
            {
                bullet.SetDirection(shootDirection);
                Debug.Log($"Bullet direction set to: {shootDirection}");
            }
            else
            {
                Debug.LogError("Bullet component not found on prefab!");
            }

            Debug.Log("Bullet spawned!");
        }
    }

    void Update()
    {
        if (playerInput == null) return;

        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
        // Baca input serangan
        attackInput = playerInput.actions["Attack"].ReadValue<float>();


        float h = moveInput.x;
        float v = moveInput.y;

        transform.Translate(new Vector3(h, v, 0) * playerData.moveSpeed * Time.deltaTime);

        // Ini untuk ngecek apakah tombol serang baru saja ditekan
        if (previousAttackInput == 0 && attackInput > 0)
        {
            Shoot();
        }

        previousAttackInput = attackInput;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        Debug.Log("Player HP: " + currentHP);

        if (currentHP <= 0)
        {
            GameManager.Instance.GameOver();
        }
    }

void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            
            // TODO: tambah skor sebanyak 1 (Kode Aslimu)
            skor += 1;
            
            // TODO: tampilkan skor ke Console (Kode Aslimu)
            teksSkor.text = "Total Coin : " + skor.ToString();
            Debug.Log("Skor: " + skor);

            // PENTING: Lapor ke GameManager agar dicek apakah sudah menang
            if (gameManager != null)
            {
                gameManager.AmbilKoin();
            }
        }
    }
}