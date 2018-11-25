using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTetromino : MonoBehaviour {

    static GameSet gameSet;
    public int player = 0;

    // Use this for initialization
    void Start () {
        // 반투명화
        foreach (Transform mino in transform)
        {
            mino.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, .2f);
        }
    }
	
	// Update is called once per frame
	void Update () {

	}

    // 움직임 확인
    public void UpdatePosition ()
    {
        gameSet = FindObjectOfType<GameSet>();

        GameObject currentMino = gameSet.getTetromino(player);
        if (currentMino == null)
        {
            return;
        }
        transform.position = currentMino.transform.position;
        transform.rotation = currentMino.transform.rotation;

        MoveDownDown();
    }

    // 맨 밑으로 바로 이동
    void MoveDownDown()
    {
        while(CheckPosition())
        {
            transform.position += new Vector3(0, -1, 0);
        }

        transform.position += new Vector3(0, 1, 0);
    }

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

    public Vector2 Offset(Vector2 pos)
    {
        if (player != 0)
        {
            pos.x -= 25;
        }
        return pos;
    }
}
