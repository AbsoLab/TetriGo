using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    public bool allowRotate = true;     // ㅁ모양은 안돌아가게
    int player;                         // 플레이어

    GameSet gameSet;                    // 게임 관리

    // Use this for initialization
    void Start () {
       
    }
	
	// Update is called once per frame
	void Update () {

	}

    // 생성자 역할 ------------------------------------------------
    public void SetSetting(GameSet gameSet, int player)
    {
        this.player = player;
        this.gameSet = gameSet;
    }

    
    // 블럭 움직임 -------------------------------------------------
    public void MoveRotaion()
    {
        // ㅁ모양은 돌아가지 않는다.
        if (!allowRotate)
        {
            return;
        }

        transform.Rotate(0, 0, -90);

        if (!CheckPosition())
        {
            transform.Rotate(0, 0, 90);
        }
    }

    public void MoveLeft()
    {
        transform.position += new Vector3(-1, 0, 0);

        if (!CheckPosition())
        {
            transform.position += new Vector3(1, 0, 0);
        }
    }

    public void MoveRight()
    {
        transform.position += new Vector3(1, 0, 0);

        if (!CheckPosition())
        {
            transform.position += new Vector3(-1, 0, 0);
        }
    }

    public void MoveDown()
    {
        transform.position += new Vector3(0, -1, 0);

        if (!CheckPosition())
        {
            transform.position += new Vector3(0, 1, 0);
            try
            {
                gameSet.AddMino(transform, Offset(transform), player);     // 블럭 등록

            }
            catch (IndexOutOfRangeException)
            {
                gameSet.GameOver(player);
                enabled = false;
                return;
            }
            enabled = false;                // 그 위치에 멈춤

            gameSet.DeleteLine(player);     // 꽉 찬 라인이 있으면 지운다.

            gameSet.SpawnMino(player);  // 다음 테트로미노 소환
        }
    }

    // 맨 밑으로 바로 이동
    public void MoveDownDown()
    {
        while (CheckPosition())
        {
            transform.position += new Vector3(0, -1, 0);
        }

        transform.position += new Vector3(0, 1, 0);

        MoveDown();
    }


    // 블럭 위치 체크 ----------------------------------------------
    bool CheckPosition()
    {
        foreach (Transform mino in transform)   // 조각 하나하나씩 다 체크
        {
            Vector2 pos = gameSet.Round(mino.position);

            // 게임판 안을 벗어나면 false 반환
            if (!gameSet.CheckInside(Offset(pos), player))
            {
                return false;
            }
            // 이미 블럭이 있을 경우 false 반환
            if (gameSet.GetBoardStats(Offset(pos), player))
            {
                return false;
            }
        }

        return true;
    }


    // 플레이어에 따라 위치 조정 ------------------------------------
    Vector2 Offset(Vector2 pos)
    {
        if (player != 0)
        {
            pos.x -= 25;
        }
        return pos;
    }

    Vector2[] Offset(Transform transform)
    {
        Vector2[] pos = new Vector2[4];
        int i = 0;
        foreach(Transform mino in transform)
        {
            pos[i++] = Offset(mino.position);
        }
        return pos; 
    }
}
