using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameIdController : MonoBehaviour
{
   public static string gameId;

    public static void SetGameId(string id)
    {
        gameId = id;
    }
}
