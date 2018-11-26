using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Assets.Scripts;

public class GameSet : MonoBehaviour
{
    Player [] player = new Player[2];       // 플레이어 최대 두 명
    GameBoard[] board = new GameBoard[2];   // 그래서 게임판도 두 개!
    Computer computer;                      // 컴퓨터

    public int gameMode = 0;    // 0:1P, 1:2P, 2:Com

    // 미노들 배열로 합칠 생각
    GameObject currentMino1;    // 현재 블럭
    GameObject currentMino2;

    GameObject ghostMino1;      // 고스트 블럭
    GameObject ghostMino2;

    GameObject nextMino1;       // 다음 블럭
    GameObject nextMino2;

    double time1 = 0;
    double time2 = 0;
    double comMoveSpeed = 0;

    public Text[] UI_1 = new Text[3];   // 플레이어1의 UI들
    public Text[] UI_2 = new Text[3];   // 플레이어2의 UI들

    // Use this for initialization
    void Start()
    {
        player[0] = new Player();
        board[0] = new GameBoard();
        SpawnNextMino(0);
        SpawnMino(0);

        if (gameMode != 0)
        {
            player[1] = new Player();
            board[1] = new GameBoard();

            if(gameMode == 2)
            {
                computer = new Computer();
            }

            SpawnNextMino(1);
            SpawnMino(1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        MoveTetromino();
    }


    // 다음 조각 생성 --------------------------------------------
    public void SpawnMino(int mode)
    {
        // 왼쪽 플레이어
        if (mode == 0)
        {
            //currentMino1 = (GameObject)Instantiate(Resources.Load(GetRandomName(), typeof(GameObject)), new Vector2(4, 20), Quaternion.identity);
            currentMino1 = (GameObject)Instantiate(nextMino1, new Vector2(4, 20), Quaternion.identity);
            currentMino1.AddComponent<Tetromino>();
            currentMino1.GetComponent<Tetromino>().SetSetting(this, 0);

            SpawnNextMino(mode);

            // 고스트 생성
            SpawnGhostMino(mode);
        }
        // 오른쪽 플레이어
        else
        {
            //currentMino2 = (GameObject)Instantiate(Resources.Load(GetRandomName(), typeof(GameObject)), new Vector2(29, 20), Quaternion.identity);
            currentMino2 = (GameObject)Instantiate(nextMino2, new Vector2(29, 20), Quaternion.identity);
            currentMino2.AddComponent<Tetromino>();
            currentMino2.GetComponent<Tetromino>().SetSetting(this, mode);

            SpawnNextMino(mode);

            // 고스트 생성
            SpawnGhostMino(mode);

            if (gameMode == 2)
            {
                computer.SetComputer(board[1].GetBoardBoolType(), currentMino2, nextMino2);
            }
        }        
    }
    
    string GetRandomName()
    {
        int rand = Random.Range(0, 7);  // 난수 생성 [0, 7)
        string randName = null;         // 테트로미노 이름

        switch (rand)
        {
            case 0:
                randName = "Tetromino/Tetromino_I";
                break;
            case 1:
                randName = "Tetromino/Tetromino_J";
                break;
            case 2:
                randName = "Tetromino/Tetromino_L";
                break;
            case 3:
                randName = "Tetromino/Tetromino_O";
                break;
            case 4:
                randName = "Tetromino/Tetromino_S";
                break;
            case 5:
                randName = "Tetromino/Tetromino_T";
                break;
            case 6:
                randName = "Tetromino/Tetromino_Z";
                break;
        }
        return randName;
    }
    
    public void SpawnNextMino(int mode)
    {   if(mode == 0)
        {
            if (nextMino1 != null)
            {
                Destroy(nextMino1);
            }
            nextMino1 = (GameObject)Instantiate(Resources.Load(GetRandomName(), typeof(GameObject)), new Vector2(14, 10), Quaternion.identity);
            Destroy(nextMino1.GetComponent<Tetromino>());
        }
        else
        {
            if (nextMino2 != null)
            {
                Destroy(nextMino2);
            }
            nextMino2 = (GameObject)Instantiate(Resources.Load(GetRandomName(), typeof(GameObject)), new Vector2(20, 10), Quaternion.identity);
            Destroy(nextMino2.GetComponent<Tetromino>());
        }
    }

    public void SpawnGhostMino(int mode)
    {
        if(mode == 0)
        {
            // 기존 고스트 삭제
            if (ghostMino1 != null)
            {
                Destroy(ghostMino1);
            }
            // 똑같이 생긴 블럭 생성
            ghostMino1 = (GameObject)Instantiate(currentMino1, new Vector2(4, 20), Quaternion.identity);
            Destroy(ghostMino1.GetComponent<Tetromino>());  // 컴포넌트 바꾸기
            ghostMino1.AddComponent<GhostTetromino>();
            ghostMino1.GetComponent<GhostTetromino>().player = mode;
            ghostMino1.GetComponent<GhostTetromino>().UpdatePosition();
        }
        else
        {
            // 기존 고스트 삭제
            if(ghostMino2 != null)
            {
                Destroy(ghostMino2);
            }
            // 똑같이 생긴 블럭 생성
            ghostMino2 = (GameObject)Instantiate(currentMino2, new Vector2(29, 20), Quaternion.identity);
            Destroy(ghostMino2.GetComponent<Tetromino>());  // 컴포넌트 바꾸기
            ghostMino2.AddComponent<GhostTetromino>();
            ghostMino2.GetComponent<GhostTetromino>().player = mode;
            ghostMino2.GetComponent<GhostTetromino>().UpdatePosition();
        }
    }


    // 게임판에 블록 등록 -----------------------------------------
    public void AddMino(Transform transform, Vector2[] pos, int mode)
    {
        board[mode].AddMino(transform, pos);
    }


    // 블럭 이동 -------------------------------------------------
    public void MoveTetromino()
    {
        if (currentMino1 == null)
        {
            // 플레이어가 죽었을 때
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentMino1.GetComponent<Tetromino>().MoveRotaion();
            ghostMino1.GetComponent<GhostTetromino>().UpdatePosition();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentMino1.GetComponent<Tetromino>().MoveLeft();
            ghostMino1.GetComponent<GhostTetromino>().UpdatePosition();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentMino1.GetComponent<Tetromino>().MoveRight();
            ghostMino1.GetComponent<GhostTetromino>().UpdatePosition();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Time.time - time1 >= player[0].speed)
        {
            currentMino1.GetComponent<Tetromino>().MoveDown();
            time1 = Time.time;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && gameMode != 1)
        {
            currentMino1.GetComponent<Tetromino>().MoveDownDown();
            time1 = Time.time;
        }
        else if (Input.GetKeyDown(KeyCode.RightShift) && gameMode == 1)
        {
            currentMino1.GetComponent<Tetromino>().MoveDownDown();
            time1 = Time.time;
        }
        
        
        if (currentMino2 == null)
        {
            // 플레이어가 죽었을 때 or 1인, 컴퓨터 모드일 때
        }
        else if(gameMode == 2)
        {
            if (Time.time - time2 >= player[1].speed)
            {
                currentMino2.GetComponent<Tetromino>().MoveDown();
                time2 = Time.time;
            }
            else if (Time.time - comMoveSpeed >= 0.01)
            {
                computer.Move();
                ghostMino2.GetComponent<GhostTetromino>().UpdatePosition();
                comMoveSpeed = Time.time;
            }
            
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            currentMino2.GetComponent<Tetromino>().MoveRotaion();
            ghostMino2.GetComponent<GhostTetromino>().UpdatePosition();
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            currentMino2.GetComponent<Tetromino>().MoveLeft();
            ghostMino2.GetComponent<GhostTetromino>().UpdatePosition();
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            currentMino2.GetComponent<Tetromino>().MoveRight();
            ghostMino2.GetComponent<GhostTetromino>().UpdatePosition();
        }
        else if (Input.GetKeyDown(KeyCode.S) || Time.time - time2 >= player[1].speed)
        {
            currentMino2.GetComponent<Tetromino>().MoveDown();
            time2 = Time.time;
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            currentMino2.GetComponent<Tetromino>().MoveDownDown();
            time2 = Time.time;
        }
    }


    // 꽉 찬 라인이 있으면 지운다 ---------------------------------
    public void DeleteLine(int mode)
    {
        board[mode].DeleteLine();

        // 지운 라인이 있으면
        if (board[mode].deleteLineNum >= 1)
        {
            player[mode].UpdateScore(board[mode].deleteLineNum);

            player[mode].LevelUp();

            UpdateUI(mode);

            board[mode].deleteLineNum = 0;
        }
    }


    // UI 업데이트 ------------------------------------------------
    public void UpdateUI(int mode)
    {
        if (mode == 0)
        {
            UI_1[0].text = player[mode].score.ToString();
            UI_1[1].text = player[mode].level.ToString();
            UI_1[2].text = player[mode].line.ToString();
        }
        else
        {
            UI_2[0].text = player[mode].score.ToString();
            UI_2[1].text = player[mode].level.ToString();
            UI_2[2].text = player[mode].line.ToString();
        }
        
    }


    // 해당위치의 보드판의 상태를 반환 -----------------------------
    public bool GetBoardStats(Vector2 pos, int mode)
    {
        return board[mode].GetBoardStats(pos);
    }


    // 게임판 내부를 벗어났는지 확인 -------------------------------
    public bool CheckInside(Vector2 pos, int mode)
    {
        return board[mode].CheckInside(pos);
    } 


    // 게임오버! --------------------------------------------------
    public void GameOver(int mode)
    {
        // 해당 사용자 종료
        if(mode == 0)
        {
            currentMino1 = null;
            comMoveSpeed = 0.01;
        }
        else
        {
            currentMino2 = null;
        }

        // 게임이 끝났는지 확인
        if(currentMino1 == null && currentMino2 == null)
        {
            GameObject alertView = (GameObject)Instantiate(Resources.Load("Board/AlertView"));

            string title = null;
            string msg = null;

            if(gameMode == 0)
            {
                title = "GameOver";
                msg = "다시 하시겠습니까?";     
            }
            else if (gameMode == 1)
            {
                int score1 = player[0].score;
                int score2 = player[1].score;
                
                if (score1 > score2)
                {
                    title = "Victory!";
                    msg = "플레이어1이 " + score1.ToString() + "점으로 승리했습니다.";
                }
                else if(score1 < score2)
                {
                    title = "Victory!";
                    msg = "플레이어2가 " + score2.ToString() + "점으로 승리했습니다.";
                }
                else
                {
                    title = "Draw!";
                    msg = "비겼습니다. 다시 하시겠습니까?";
                }
            }
            else
            {
                int score1 = player[0].score;
                int score2 = player[1].score;
                
                if(score1 > score2)
                {
                    title = "Victory!";
                    msg = "Player가 " + score1.ToString() + "점으로 승리했습니다.";
                }
                else if (score1 < score2)
                {
                    title = "Defeat!";
                    msg = "컴퓨터가 " + score2.ToString() + "점으로 승리했습니다.";
                }
                else
                {
                    title = "Draw!";
                    msg = "비겼습니다. 다시 하시겠습니까?";
                }
            }

            alertView.GetComponent<AlertView>().SetText(title, msg);
            alertView.GetComponent<AlertView>().SetGameMode(gameMode);
        }
    }


    // 블럭 반환 (Ghost에게 넘겨주는 용도) --------------------------
    public GameObject getTetromino(int mode)
    {
        if(mode == 0)
        {
            return currentMino1;
        }
        else
        {
            return currentMino2;
        }
    }


    // 반올림
    public Vector2 Round(Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

}