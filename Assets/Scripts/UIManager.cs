using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button settingsMouseButton;
    [SerializeField] private Button doneButton;

    private AnimationManager _animationManager;

    private void Start()
    {
        _animationManager = CatManager.Instance.animationManager;
        _animationManager.InitializeAnimationsToMainMenuState();


        quitButton.onClick.AddListener(QuitGame);
        startButton.onClick.AddListener(StartGame);
        settingsMouseButton.onClick.AddListener(GameToMenu);
        doneButton.onClick.AddListener(Done);

        startButton.GetComponent<EventTrigger>().triggers[0].callback.AddListener((data) =>
        {
            _animationManager.JarButtonStartHoveredStart();
        });
        startButton.GetComponent<EventTrigger>().triggers[1].callback.AddListener((data) =>
        {
            _animationManager.JarButtonStartHoveredStop();
        });
        quitButton.GetComponent<EventTrigger>().triggers[0].callback.AddListener((data) =>
        {
            _animationManager.JarButtonQuitHoveredStart();
        });
        quitButton.GetComponent<EventTrigger>().triggers[1].callback.AddListener((data) =>
        {
            _animationManager.JarButtonQuitHoveredStop();
        });
        settingsMouseButton.GetComponent<EventTrigger>().triggers[0].callback.AddListener((data) =>
        {
            _animationManager.SettingsMouseHoveredStart();
        });
        settingsMouseButton.GetComponent<EventTrigger>().triggers[1].callback.AddListener((data) =>
        {
            _animationManager.SettingsMouseIdleStop();
        });


        CatManager.Instance.GameOverEvent += GameOver;

        doneButton.gameObject.SetActive(false);
    }

    private void StartGame()
    {
        _animationManager.SwitchToPlayState(() => CatManager.Instance.StartGame());
        ChangeMainMenuButtons(false);
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
// stop playmode
        EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    private void GameOver(object sender, EventArgs e)
    {
        Debug.Log("Game Over, in UIManager");
        // EnableDoneButton();
        _animationManager.SwitchToDoneState(EnableDoneButton);
    }

    private void EnableDoneButton()
    {
        Debug.Log("Enable Done Button");
        doneButton.gameObject.SetActive(true);
    }

    private void Done()
    {
        CatManager.Instance.ResetTimescale();
        _animationManager.SwitchFromDoneToMenu();
        doneButton.gameObject.SetActive(false);
        ChangeMainMenuButtons(true);
        CatManager.Instance.CleanUpGame();
    }

    private void GameToMenu()
    {
        CatManager.Instance.ResetTimescale();
        _animationManager.SwitchFromPlayToMenu();
        ChangeMainMenuButtons(true);
        CatManager.Instance.CleanUpGame();
    }

    private void ChangeMainMenuButtons(bool newState)
    {
        startButton.gameObject.SetActive(newState);
        quitButton.gameObject.SetActive(newState);
    }
}