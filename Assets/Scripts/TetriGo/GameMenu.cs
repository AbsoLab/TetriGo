using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenu : MonoBehaviour { 

    public void SingleGame()
    {
        Application.LoadLevel("SingleGame");
    }

    public void TwoPlayerGame()
    {
        Application.LoadLevel("TwoPlayGame");
    }

    public void ComputerGame()
    {
        Application.LoadLevel("ComputerGame");
    }
}
