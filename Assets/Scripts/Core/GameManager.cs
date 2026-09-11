using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject PausePanel;
    public GameObject WinPanel;

    public GameState currentState;

    public int totalKoin;
    private int koinTerkumpul = 0;

    [SerializeField] private int skor = 0;


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentState = GameState.Playing;
        // TODO: hitung jumlah koin di scene saat mulai
        totalKoin = GameObject.FindGameObjectsWithTag("Coin").Length;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Playing)
            {
                PauseGame();
                PausePanel.SetActive(true);
            }
            else if (currentState == GameState.Paused)
            {
                PausePanel.SetActive(false);
                ResumeGame();
            }
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        currentState = GameState.Paused;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        currentState = GameState.Playing;
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
        SceneManager.LoadScene("GameOver");
        Time.timeScale = 0f;
        currentState = GameState.GameOver;
    }

    public void AmbilKoin()
    {
        koinTerkumpul++;
        // TODO: jika koinTerkumpul == totalKoin, panggil Menang()
     if (koinTerkumpul == totalKoin) Menang();
    }
    void Menang()
    {
        Debug.Log("KAMU MENANG!");
        currentState = GameState.GameOver;
        Time.timeScale = 0f;
        WinPanel.SetActive(true);
    }

    
    void OnEnable()
    {
        Enemy.OnZombieMati += TambahSkorSaatZombieMati;
    }
 
    void OnDisable()
    {
        Enemy.OnZombieMati -= TambahSkorSaatZombieMati;
    }
 
    void TambahSkorSaatZombieMati(Enemy zombieYangMati)
    {
        skor += 10;
        Debug.Log("Skor: " + skor);
    }

}