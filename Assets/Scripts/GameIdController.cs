using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameIdController : MonoBehaviour
{
   public static string gameId;
   public static int RoundNumber = 0;

    public static void SetGameId(string id)
    {
        gameId = id;
    }

    public static void IncrementRoundNumber()
    {
        RoundNumber += 1;
    }
    
    public static void SetRoundNumber(int round)
    {
        RoundNumber = round;
    }
}
