﻿using Network;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    Ready,
    Playing,
    GameOver,
}

public enum PlayingMode
{
    Free = 0,
    AI = 1,
    Network = 2,
}


public enum SpeedMode
{
    Easy = 0,
    Normal = 1,
    Hard = 2,
}

public class GameManager : MonoBehaviour {
    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public GameState CurrentGameState
    {
        get
        {
            return _gameState;
        }
        set
        {
            _gameState = value;
        }
    }

    public float ScrollSpeed
    {
        get
        {
            return scrollSpeed[_speedMode];
        }
    }

    private Dictionary<SpeedMode, float> scrollSpeed = new Dictionary<SpeedMode, float>()
    {
        { SpeedMode.Easy, -2f}, { SpeedMode.Normal, -4}, { SpeedMode.Hard, -8}
    };

    private const string BEST_SCORE_PREPREF = "BEST_SCORE_PREPREF";
    private GameState _gameState;
    public Text scoreText;
    public Text finalScoreText;
    public Text bestScoreText;
    public Button restartButton;
    public Button startGameButton;
    public Dropdown playingModeDropdown;
    public Dropdown speedModeDropdown;
    public GameObject readyStatePanel;
    public GameObject gameOverStatePanel;
    private static PlayingMode _lastPlayingMode;
    private static GameManager _instance;
    private static PlayingMode _playingMode = PlayingMode.Free;
    private static SpeedMode _speedMode = SpeedMode.Normal;
    private bool _gameOver = false;
    private int _score = 0;
    private int _bestScore = 0;

    private void Awake()
    {
        Application.runInBackground = true;
        _instance = this;
    }

    private void Start()
    {
        if (_lastPlayingMode == PlayingMode.Network)
        {
            _playingMode = _lastPlayingMode;
            _gameState = GameState.Playing;
        }
        else
        {
            _gameState = GameState.Ready;
            _bestScore = PlayerPrefs.GetInt(BEST_SCORE_PREPREF, 0);
            InitPlayingModeDropdown();
            InitSpeedModeDropdown();
            restartButton.onClick.AddListener(OnRestartButtonClicked);
            startGameButton.onClick.AddListener(OnStartGameButtonClicked);
            playingModeDropdown.onValueChanged.AddListener(OnPlayingModeDropdownValueChanged);
            speedModeDropdown.onValueChanged.AddListener(OnSpeedModeDropdownValueChanged);
        }
    }

    private void InitPlayingModeDropdown()
    {
        List<string> options = new List<string>();
        foreach (PlayingMode item in Enum.GetValues(typeof(PlayingMode)))
        {
            options.Add(item.ToString());
        }
        playingModeDropdown.AddOptions(options);
        speedModeDropdown.value = (int)_playingMode;
    }

    private void InitSpeedModeDropdown()
    {
        List<string> options = new List<string>();
        foreach (SpeedMode item in Enum.GetValues(typeof(SpeedMode)))
        {
            options.Add(item.ToString());
        }
        speedModeDropdown.AddOptions(options);
        speedModeDropdown.value = (int)_speedMode;
    }

    private void Update () {
        switch (_gameState)
        {
            case GameState.Ready:
                ReadyState();
                break;
            case GameState.Playing:
                PlayingState();
                CheckPlayingMode();
                break;
            case GameState.GameOver:
                if (_playingMode != PlayingMode.Network)
                {
                    GameOverState();
                    SetScoreTexts();
                    EnvironmentObserver.Instance.StopMonitoring();
                }
                else
                {
                    RestartGame();
                }
                break;
            default:
                break;
        }
    }

    private void OnApplicationQuit()
    {
        EnvironmentObserver.Instance.StopMonitoring();
    }

    private void ReadyState()
    {
        _score = 0;
        scoreText.gameObject.SetActive(true);
        readyStatePanel.SetActive(true);
        gameOverStatePanel.SetActive(false);
        SetBoxScoreTexts();
    }

    private void PlayingState()
    {
        scoreText.gameObject.SetActive(true);
        readyStatePanel.SetActive(false);
        playingModeDropdown.gameObject.SetActive(false);
        speedModeDropdown.gameObject.SetActive(false);
        gameOverStatePanel.SetActive(false);
        SetBoxScoreTexts();
    }

    private void CheckPlayingMode()
    {
        if(_playingMode == PlayingMode.Network)
        {
            speedModeDropdown.gameObject.SetActive(false);
            playingModeDropdown.gameObject.SetActive(false);
            readyStatePanel.SetActive(false);
            gameOverStatePanel.SetActive(false);
            scoreText.gameObject.SetActive(false);
        }
    }

    private void GameOverState()
    {
        scoreText.gameObject.SetActive(false);
        readyStatePanel.SetActive(false);
        gameOverStatePanel.SetActive(true);
    }

    private void SetBoxScoreTexts()
    {
        scoreText.text = _score.ToString();
    }

    private void SetScoreTexts()
    {
        finalScoreText.text = _score.ToString();
        if (_score > _bestScore)
        {
            _bestScore = _score;
            PlayerPrefs.SetInt(BEST_SCORE_PREPREF, _bestScore);
        }
        bestScoreText.text = _bestScore.ToString();
    }

    public void BirdScored()
    {
        _score += 1;
        SetScoreTexts();
    }

    private void OnStartGameButtonClicked()
    {
        if (_playingMode == PlayingMode.Network)
            EnvironmentObserver.Instance.StartMonitoring();
        _gameState = GameState.Playing;
    }

    private void OnRestartButtonClicked()
    {
        RestartGame();
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnPlayingModeDropdownValueChanged(int index)
    {
        PlayingMode model = (PlayingMode)index;
        _playingMode = model;
        _lastPlayingMode = model;
    }

    private void OnSpeedModeDropdownValueChanged(int index)
    {
        _speedMode = (SpeedMode)index;
    }

}
