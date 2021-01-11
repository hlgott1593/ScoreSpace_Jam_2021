using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour {
    private int score;
    private bool GameOver;
    private float m_ElapsedGameTime;
    private float m_ElapsedRealTime;


    [Header("Slowdown Settings")] [SerializeField]
    private float slowTimeScale;

    [SerializeField] private float normalizeTransitionLength;
    private float normalizeTransitionTimer;
    public bool TimeIsSlow;

    [Header("Managers and Parent objects")]
    public EnemyManager MyEnemyManager;

    public Transform TargettingNodes;
    public Transform BuildingsParent;

    [Header("UI Stuff")]
    public Text highscoreText;
    public Text scoreText;
    public Text finalScoreText;
    public GameObject GameOverScreen;

    // Popup Text
    [SerializeField] public GameObject popupTextPrefab;

    [Header("SFX")] [SerializeField] private AudioClip[] gameStart;
    [SerializeField] private AudioClip[] gameOver;
    private AudioSource audioSource;

    public ScoreData scoreData;
    public static string SCORE_FILENAME = "scoreData.json";

    public int BuildingCount {
        get { return BuildingsParent.childCount; }
    }

    // Start is called before the first frame update
    void Start() {
        Time.timeScale = 1f;
        audioSource = GetComponent<AudioSource>();
        GameOver = false;
        TargettingNodes = transform.Find("TargettingNodes");
        BuildingsParent = transform.Find("Buildings");
        audioSource.PlayOneShot(gameStart[Random.Range(0, gameStart.Length)]);
        // Load saved score data
        LoadScores();
    }

    // Update is called once per frame
    void Update() {
        UpdateTimers();
        UpdateSlowDown();
    }

    /// <summary>
    /// Updates the timers.
    /// </summary>
    private void UpdateTimers() {
        m_ElapsedGameTime += Time.deltaTime;
        m_ElapsedRealTime += Time.unscaledDeltaTime;
    }

    /// <summary>
    /// Mainly for handling the transition from slow to normal time.
    /// </summary>
    private void UpdateSlowDown() {
        if (!TimeIsSlow && normalizeTransitionTimer > 0f) {
            normalizeTransitionTimer -= Time.unscaledDeltaTime;
            float ratio = normalizeTransitionTimer / normalizeTransitionLength;
            if (ratio < 0f) {
                ratio = 0f;
            }

            Time.timeScale = Mathf.Lerp(1f, slowTimeScale, ratio);
        }
    }

    /// <summary>
    /// Checks if all buildings have been destroyed, and pulls up the game over screen if true.
    /// </summary>
    public void HandleGameOver() {
        if (GameOver) return;
        audioSource.PlayOneShot(gameOver[Random.Range(0, gameOver.Length)]);
        Time.timeScale = 0f;
        GameOver = true;
        GameOverScreen.SetActive(true);
        // set score texts
        Score finalScore = new Score() {
            player = "test",
            value = score
        };
        finalScoreText.text = finalScore.value.ToString();
        if (finalScore.value > scoreData.highscore.value)
        {
            scoreData.highscore = finalScore;
        }
        highscoreText.text = scoreData.highscore.value.ToString();
        // save scores
        scoreData.scores.Add(finalScore);
        SaveScores();
    }

    /// <summary>
    /// Immediately slows down time.
    /// </summary>
    public void SlowDownTime() {
        if (!TimeIsSlow) {
            Time.timeScale = slowTimeScale;
            TimeIsSlow = true;
        }
    }

    /// <summary>
    /// Begins the process of gradually bringing time back.
    /// </summary>
    public void NormalizeTime() {
        if (TimeIsSlow) {
            normalizeTransitionTimer = normalizeTransitionLength;
            TimeIsSlow = false;
        }
    }

    public void IncreaseScore(int _amount) {
        score += _amount;
        scoreText.text = "" + score;
    }

    /// <summary>
    /// Restarts the scene.
    /// </summary>
    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadScores()
    {
        string filePath = Path.Combine(Application.persistentDataPath, SCORE_FILENAME);

        if(File.Exists(filePath))
        {
            // Read the json from the file
            string json = File.ReadAllText(filePath);
            scoreData = JsonUtility.FromJson<ScoreData>(json);
        }
        else
        {
            Debug.LogError(SCORE_FILENAME + " not found: " + filePath);
            scoreData = new ScoreData();
            SaveScores();
        }
    }

    public void SaveScores()
    {
        string json = JsonUtility.ToJson (scoreData);
        // write to file
        string filePath = Path.Combine(Application.persistentDataPath, SCORE_FILENAME);
        File.WriteAllText(filePath, json);
    }

    public void DisplayText(string text, Vector3 location, Color color, int textSize = 180)
    {
        GameObject go = Instantiate(popupTextPrefab) as GameObject;
        var tos = go.GetComponent<TextOnSpot>();
        tos.TextPrefab.text = text;
        tos.SetFontSize(textSize);
        tos.SetColor(color);
        go.transform.position = location;
    }
}