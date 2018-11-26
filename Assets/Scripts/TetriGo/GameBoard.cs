using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    class GameBoard
    {
        static int boardWidth = 10;     // 게임판의 너비
        static int boardHeight = 20;    // 게임판의 높이
        Transform[,] board = new Transform[boardWidth, boardHeight];  // 게임판

        public int deleteLineNum = 0;  // 한 번에 지운 라인의 수

        int penaltyNum = 0;

        // 게임판에 블록 등록 -----------------------------------------
        public void AddMino(Transform transform, Vector2[] offsetPos)
        {
            int i = 0;
            foreach (Transform mino in transform)
            {
                Vector2 pos = Round(offsetPos[i++]);
                board[(int)pos.x, (int)pos.y] = mino;
            }
        }


        // 라인 지우기 -----------------------------------------------
        public void DeleteLine()
        {
            for (int i = penaltyNum; i < 20; i++)
            {
                if (IsFullLine(i))
                {
                    DeleteMino(i);
                    AllLineDown(i + 1);
                    i--;

                    deleteLineNum++;
                }
            }
        }

        bool IsFullLine(int y)
        {
            int temp = 0;

            for (int x = 0; x < 10; x++)
            {
                if (board[x, y] != null)
                {
                    temp++;
                }
            }

            if (temp == 10)
            {
                return true;
            }

            return false;
        }

        void DeleteMino(int y)
        {
            for (int x = 0; x < 10; x++)
            {
                Object.Destroy(board[x, y].gameObject);
                board[x, y] = null;
            }
        }

        void LineDown(int y)
        {
            for (int x = 0; x < 10; x++)
            {
                if (board[x, y] != null)
                {
                    board[x, y - 1] = board[x, y];
                    board[x, y] = null;
                    board[x, y - 1].position += new Vector3(0, -1, 0);
                }
            }
        }

        void AllLineDown(int y)
        {
            for (int i = y; i < 20; i++)
            {
                LineDown(i);
            }
        }


        // 대전 모드 -------------------------------------------------
        public void AddPenalty(Transform transform, int offset)
        {
            int i = 0;

            foreach (Transform brick in transform)
            {
                Vector2 pos = Round(Round(brick.position));
                board[(int)pos.x - offset, (int)pos.y] = brick;
            }

            penaltyNum++;
        }

        public void AllLineUp()
        {
            for(int y=18; y>=0; y--)
            {
                for(int x=0; x<10; x++)
                {
                    if (board[x, y] != null)
                    {
                        board[x, y + 1] = board[x, y];
                        board[x, y + 1].position += new Vector3(0, 1, 0);
                    }
                    
                }
            }
        }

        // 해당위치의 보드판의 상태를 반환 -----------------------------
        public bool GetBoardStats(Vector2 pos)
        {
            // 배열 크기 초과 방지
            if (pos.y > boardHeight - 1)
            {
                return false;
            }

            if (board[(int)pos.x, (int)pos.y] != null)
            {
                return true;
            }
            return false;

        }


        // 게임판 내부를 벗어났는지 확인 -------------------------------
        public bool CheckInside(Vector2 pos)
        {
            if (pos.x < 0 || pos.x > boardWidth - 1)
            {
                return false;
            }
            if (pos.y < 0)
            {
                return false;
            }
            return true;
        }


        // 게임판을 반환
        public bool[,] GetBoardBoolType()
        {
            bool[,] temp = new bool[boardWidth, boardHeight];

            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 20; y++)
                {
                    temp[x, y] = (board[x, y] != null);
                }
            }

            return temp;
        }

        // 반올림
        public Vector2 Round(Vector2 pos)
        {
            return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
        }

    }
}

