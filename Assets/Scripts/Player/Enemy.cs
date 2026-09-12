using System;
using UnityEngine;
using UnityEngine.UIElements;


public class Enemy : MonoBehaviour
{

    [SerializeField] public int hp = 100;

    [Header("Pengaturan State Machine")]
    [SerializeField] private float jarakDeteksi = 6f; // masuk CHASE
    [SerializeField] private float jarakSerang = 1.2f; // masuk ATTACK
    [SerializeField] private float jedaSerang = 1f; // detik antar serang

    public static event Action<Enemy> OnZombieMati;

    // Patrol Random Place 

    [SerializeField] private float radiusPatrol = 3f;
    private Vector2 titikAwal; // pusat area keliling
    private Vector2 tujuanPatrol; // titik yang sedang dituju

    // state sekarang -- mulai dari IDLE
    private StateZombie state = StateZombie.IDLE;
    private float waktuSerangTerakhir;

    public float ms = 2f;
    [SerializeField] private int damageSaatTabrakan = 20;
    
    protected Transform player;
    protected virtual void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // tambahkan di dalam Start() yang sudah ada:
        titikAwal = transform.position;
        PilihTujuanPatrolBaru();
    }

    // Update is called once per frame
    void Update()
    {
        // LANGKAH A: tentukan state (aturan pindah)
        PeriksaTransisi();
        // LANGKAH B: jalankan perilaku sesuai state sekarang
        switch (state)
        {
            case StateZombie.IDLE: PerilakuIdle(); break;
            case StateZombie.PATROL: PerilakuPatrol(); break;
            case StateZombie.CHASE: PerilakuChase(); break;
            case StateZombie.ATTACK: PerilakuAttack(); break;
        }
    }

    public void Kejar()
    {
        if (player == null) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            ms * Time.deltaTime
        );
    }

    public virtual void Serang()
    {
        Debug.Log("Enemy menyerang!");

    }

    void PerilakuIdle() { }
    void PerilakuPatrol()
    {
        transform.position = Vector2.MoveTowards(
        transform.position, tujuanPatrol, ms * 0.5f * Time.deltaTime);
        if (Vector2.Distance(transform.position, tujuanPatrol) < 0.1f)
            PilihTujuanPatrolBaru();
    }
    void PilihTujuanPatrolBaru()
    {
        Vector2 acak = UnityEngine.Random.insideUnitCircle * radiusPatrol;
        tujuanPatrol = titikAwal + acak;
    }
    void PerilakuChase() { Kejar(); }
    void PerilakuAttack()
    { // menyerang berkala, tidak tiap frame
        if (Time.time >= waktuSerangTerakhir + jedaSerang)
        {
            Serang(); // method dari OOP
            waktuSerangTerakhir = Time.time;
        }
    }

    void PeriksaTransisi()
    {
        float JarakKePlayer()
        {
            if (player == null) return Mathf.Infinity; // biar aman kalau player belum ketemu
            return Vector2.Distance(transform.position, player.position);
        }

        float jarak = JarakKePlayer(); // sudah ada dari materi OOP!
        if (jarak <= jarakSerang)
            state = StateZombie.ATTACK; // sangat dekat -> serang
        else if (jarak <= jarakDeteksi)
            state = StateZombie.CHASE; // terlihat -> kejar
        else
            state = StateZombie.PATROL; // jauh -> keliling
    }

    public void KenaDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Mati();
    }
    }

    protected virtual void Mati()
    {
    Debug.Log(name + " kalah!");
 
    // '?.Invoke' -> aman walau belum ada yang mendengarkan (tidak error)
    OnZombieMati?.Invoke(this);   // kirim 'this' = info diri sendiri
 
    Destroy(gameObject);

    }

void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            IDamageable playerScript = other.GetComponent<IDamageable>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(damageSaatTabrakan);
            }
        }
    }


}
