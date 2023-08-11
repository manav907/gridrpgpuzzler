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
}
