using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertView : MonoBehaviour {

    public Text title;
    public Text msg;

    public Button restartBtn;
    public Button returnBtn;

    public int mode;

    private void Start()
    {
        restartBtn.onClick.AddListener(OnRestartButtonClick);
        returnBtn.onClick.AddListener(OnRetrunButtonClick);
    }

    public void SetText(string title, string msg)
    {
        this.title.text = title;
        this.msg.text = msg;
    }

    public void SetGameMode(int mode)
    {
        this.mode = mode;
    }

    public void OnRetrunButtonClick()
    {
        Application.LoadLevel("GameMenu");
    }

    public void OnRestartButtonClick()
    {
        switch(mode)
        {
            case 0:
                Application.LoadLevel("SingleGame");
                break;
            case 1:
                Application.LoadLevel("TwoPlayGame");
                break;
            case 2:
                Application.LoadLevel("ComputerGame");
                break;
        }
    }
}
