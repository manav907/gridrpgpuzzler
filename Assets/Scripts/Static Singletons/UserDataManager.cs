using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UserDataManager
{
    public static LevelDataSO currentLevel;
    public static float waitAI = 0.25f;
    public static float waitTurn = 0.5f;
    public static float waitAction = 0.4f;
    public static bool skipWaitTime = true;
    public static bool skipAnimations = true;
}
