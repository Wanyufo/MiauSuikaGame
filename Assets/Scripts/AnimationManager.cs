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


    private List<string> _jarFolderNames = new List<string>()
    {
        "slot_0",
        "slot_1",
        "slot_2",
        "slot_3",
        "slot_4",
    };

    private List<string> _jarSkinNames = new List<string>()
    {
        "0",
        "1",
        "2",
        "3",
        "4",
        "5",
        "6",
        "7",
        "8",
        "9"
    };


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
        SetJarAnimation(JarButtonStartOn, loop: false);
    }

    public void JarButtonStartHoveredStop()
    {
        SetJarAnimation(JarButtonStartOff, loop: false, onComplete: () => { SetJarAnimation(JarMenu, loop: true); });
    }

    public void JarButtonOptionsHoveredStart()
    {
        SetJarAnimation(JarButtonOptionOn, loop: false);
    }

    public void JarButtonOptionsHoveredStop()
    {
        SetJarAnimation(JarButtonOptionOff, loop: false, onComplete: () => { SetJarAnimation(JarMenu, loop: true); });
    }

    public void JarButtonQuitHoveredStart()
    {
        SetJarAnimation(JarButtonQuitOn, loop: false);
    }

    public void JarButtonQuitHoveredStop()
    {
        SetJarAnimation(JarButtonQuitOff, loop: false, onComplete: () => { SetJarAnimation(JarMenu, loop: true); });
    }

    private void SetJarFromMenuToStart()
    {
        SetJarAnimation(JarMenuToStart, loop: false);
    }

    private void SetJarFromStartToMenu()
    {
        SetJarAnimation(JarGameToMenu, loop: false, onComplete: () => { SetJarAnimation(JarMenu, loop: true); });
    }

    // jar done animation
    private void SetJarDone()
    {
        SetJarAnimation(JarDone, loop: false);
    }

    // done to menu animation
    private void SetJarDoneToMenu()
    {
        SetJarAnimation(JarDoneToMenu, loop: false, onComplete: () => { SetJarAnimation(JarMenu, loop: true); });
    }

    // for testing ScoreLabel
    // private void Start()
    // {
    //     SetScoreToLabel(12345);
    // }

    public void SetScoreToLabel(int score)
    {
        int[] scoreDigits =
        {
            0, 0, 0, 0, 0
        };
        for (int i = scoreDigits.Length - 1; i >= 0; i--)
        {
            scoreDigits[i] = score % 10;
            score /= 10;
        }

        // use skins of Jar for score
        SkeletonAnimation skeletonAnimation = jarFront;
        Skeleton skeleton = skeletonAnimation.skeleton;
        SkeletonData skeletonData = skeleton.Data;
        Skin mixSkin = new Skin("MixSkin");

        for (int digitIndex = 0; digitIndex < scoreDigits.Length; digitIndex++)
        {
            int digit = scoreDigits[digitIndex];
            string folderName = _jarFolderNames[digitIndex];
            string skinName = _jarSkinNames[digit];
            Debug.Log(" Digit: " + digit + " in Slot: " + folderName + " Attachment Name: " + skinName);
            string skinNameFull = folderName + "/" + skinName;
            mixSkin.AddSkin(skeletonData.FindSkin(skinNameFull));
        }

        skeleton.SetSkin(mixSkin);
    }


    private void SetJarAnimation(string animationName, int trackIndex = 0, bool loop = false, Action onComplete = null)
    {
        jarBackground.AnimationState.SetAnimation(trackIndex, animationName, loop);
        TrackEntry entry = jarFront.AnimationState.SetAnimation(trackIndex, animationName, loop);
        if (onComplete != null)
        {
            entry.Complete += (_) => { onComplete(); };
        }
    }
}