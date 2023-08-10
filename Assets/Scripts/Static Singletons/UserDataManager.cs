using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UserDataManager
{
    public static LevelDataSO currentLevel;
    public static float waitAI = 0.25f;
    public static float waitAction = 0.4f;
    public static bool skipWaitTime = false;
    public static bool skipAnimations = false;
    public static bool Snap = false;
    public static bool SmartPosistioning = false;
    public static void SetSetting()
    {
        //UserDataManager.Snap = false;
#if UNITY_EDITOR
        if (GameEvents.current.inGameUI.enableEditorQuickMode)
        {
            Snap = true;
            skipAnimations = true;
            skipWaitTime = true;
        }
#endif
    }
}
