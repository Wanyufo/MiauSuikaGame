using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Spine;
using Spine.Unity;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation settingsMouse;
    [SerializeField] private SkeletonAnimation jarBackground;
    [SerializeField] private SkeletonAnimation jarFront;
    [SerializeField] private SkeletonAnimation dropPaw;
    [SerializeField] private SkeletonAnimation nextPaw;
    [SerializeField] private SkeletonAnimation bubbleSpeak;
    [SerializeField] private SkeletonAnimation bubbleThink;

    // Settings Button
    private const string SettingsMouseHovered = "animation";

    // Jar
    private const string JarButtonOptionOff = "jar_button_options_off";
    private const string JarButtonOptionOn = "jar_button_options_on";
    private const string JarButtonQuitOff = "jar_button_quit_off";
    private const string JarButtonQuitOn = "jar_button_quit_on";
    private const string JarButtonStartOff = "jar_button_start_off";
    private const string JarButtonStartOn = "jar_button_start_on";
    private const string JarDone = "jar_done";
    private const string JarDoneToMenu = "jar_done_to_menu";
    private const string JarGameToMenu = "jar_game_to_menu";
    private const string JarMenu = "jar_menu";
    private const string JarMenuToStart = "jar_menu_to_start";
    private const string JarStaticGame = "jar_statick_game";
    private const string JarStaticMenu = "jar_statick_menu";

    // Drop Paw
    private const string DropPawEnter = "coming";
    private const string DropPawLeave = "leave";
    private const string DropPawDrop = "up_paw_drop";

    // Next Paw
    private const string NextPawEnter = "paw_entering";
    private const string NextPawLeave = "paw_leaving";
    private const string NextPawIdle = "paw_idle";

    // Bubble Speak
    private const string BubbleSpeakEnter = "entering";

    private const string BubbleSpeakIdle = "idle";

    // Bubble Think
    private const string BubbleThinkEnter = "entering";
    private const string BubbleThinkIdle = "idle";


    public void InitializeAnimationsToMainMenuState()
    {
        settingsMouse.AnimationState.SetEmptyAnimation(0, 0);
        SetJarAnimation(JarMenu, 0, true);

        TrackEntry entryDropPaw = dropPaw.AnimationState.SetAnimation(0, DropPawLeave, false);
        entryDropPaw.TimeScale = 100f;
        TrackEntry entryNextPaw = nextPaw.AnimationState.SetAnimation(0, NextPawLeave, false);
        entryNextPaw.TimeScale = 100f;


        bubbleThink.AnimationState.SetAnimation(0, BubbleThinkEnter, false);
        bubbleSpeak.AnimationState.SetAnimation(0, BubbleSpeakEnter, false);
        bubbleSpeak.AnimationState.GetCurrent(0).Complete += (track) => { CatManager.Instance.ShowHighScore(); };
    }

    public void SwitchToPlayState(Action onComplete)
    {
        SetJarFromMenuToStart();
        jarFront.AnimationState.GetCurrent(0).Complete += (track) =>
        {
            Debug.Log("Jar animation completed");
            TrackEntry entry = PlayPawsEnter();
            entry.Complete += (_) =>
            {
                Debug.Log("Paws animation completed, Start game");
                onComplete();
            };
        };
    }

    public void SwitchFromDoneToMenu()
    {
        SetJarDoneToMenu();
    }

    public void SwitchFromPlayToMenu()
    {
        SetJarFromStartToMenu();
        PlayPawsLeave();
    }

    public void SwitchToDoneState(Action onComplete)
    {
        SetJarDone();
        PlayPawsLeave();
        jarFront.state.GetCurrent(0).Complete += (track) => { onComplete(); };
    }


    private TrackEntry PlayPawsEnter()
    {
        nextPaw.AnimationState.SetAnimation(0, NextPawEnter, false);
        return dropPaw.AnimationState.SetAnimation(0, DropPawEnter, false);
    }

    private void PlayPawsLeave()
    {
        dropPaw.AnimationState.SetAnimation(0, DropPawLeave, false);
        nextPaw.AnimationState.SetAnimation(0, NextPawLeave, false);
        // TODO make cats leave with paws or just disappear
    }


    public void SettingsMouseHoveredStart()
    {
        settingsMouse.AnimationState.SetAnimation(0, SettingsMouseHovered, true);
    }

    public void SettingsMouseIdleStop()
    {
        settingsMouse.AnimationState.SetEmptyAnimation(0, 0.5f);
    }

    // methods for turning jar to hover of buttons on and off
    public void JarButtonStartHoveredStart()
    {
        SetJarAnimation(JarButtonStartOn, 0, false);
    }

    public void JarButtonStartHoveredStop()
    {
        SetJarAnimation(JarButtonStartOff, 0, false);
    }

    public void JarButtonOptionsHoveredStart()
    {
        SetJarAnimation(JarButtonOptionOn, 0, false);
    }

    public void JarButtonOptionsHoveredStop()
    {
        SetJarAnimation(JarButtonOptionOff, 0, false);
    }

    public void JarButtonQuitHoveredStart()
    {
        SetJarAnimation(JarButtonQuitOn, 0, false);
    }

    public void JarButtonQuitHoveredStop()
    {
        SetJarAnimation(JarButtonQuitOff, 0, false);
    }

    private void SetJarFromMenuToStart()
    {
        SetJarAnimation(JarMenuToStart, 0, false);
    }

    private void SetJarFromStartToMenu()
    {
        SetJarAnimation(JarGameToMenu, 0, false);
    }

    // jar done animation
    private void SetJarDone()
    {
        SetJarAnimation(JarDone, 0, false);
    }

    // done to menu animation
    private void SetJarDoneToMenu()
    {
        SetJarAnimation(JarDoneToMenu, 0, false);
    }


    private void SetJarAnimation(string animationName, int trackIndex = 0, bool loop = false)
    {
        jarBackground.AnimationState.SetAnimation(trackIndex, animationName, loop);
        jarFront.AnimationState.SetAnimation(trackIndex, animationName, loop);
    }
}