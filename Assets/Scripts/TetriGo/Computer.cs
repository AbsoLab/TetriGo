using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    class Model
    {
        public int blank = 0;   // 빈칸 개수
        public int height = 0;  // 놓으려는 블럭의 높이
        public int move;        // 움직임 (2비트 단위)
        public int distance;

        public int[] debug = new int[10];
    }

    class Computer
    {
        bool[,] board;                  // 받아온 게임판
        GameObject currentMino;         // 현재 움직일 수 있는 블럭
        GameObject nextMino;
        GameSet gameSet = GameObject.FindObjectOfType<GameSet>();

        int selectedMove;

        Model[] model = new Model[1024];  // 경우의 수
        int index;
        int maxBlank;
        int maxHeight;

        // 초기화
        public void SetComputer(bool[,] board, GameObject currentMino, GameObject nextMino)
        {
            this.board = board;
            this.currentMino = currentMino;
            this.nextMino = nextMino;

            index = 0;
            maxBlank = 999;
            maxHeight = 999;

            Calculate();
            SelectModel();
        }

        // 경우의 수 계산
        void Calculate()
        {
            int move = 0;
            int rotate = 0;

            Vector2[] currentPos;

            for (int i = 0; i < 4; i++)
            {
                // 좌측 탐색
                move = rotate;
                currentPos = TransMino(currentMino);

                do
                {
                    bool[,] tempBoard = CopyBoard(board);

                    Vector2[] pos = AddBoard(tempBoard, currentPos);

                    NextModel(tempBoard, pos, move);

                    move = SetMoveValue(move, 2);

                } while (CheckPos(null, MoveX(currentPos, -1)));


                // 우측 탐색
                move = rotate;
                currentPos = TransMino(currentMino);

                while (CheckPos(null, MoveX(currentPos, 1)))
                {
                    bool[,] tempBoard = CopyBoard(board);

                    move = SetMoveValue(move, 1);

                    Vector2[] pos = AddBoard(tempBoard, currentPos);

                    NextModel(tempBoard, pos, move);
                }

                // 회전
                Rotate(currentMino);
                rotate = SetMoveValue(rotate, 3);
            }
        }

        // Tetromino의 위치 정보 반환
        Vector2[] TransMino(GameObject tetromino, int offset = 0)
        {
            Vector2[] transPos = new Vector2[4];

            int i = 0;
            if (offset == 0)
            {
                foreach (Transform mino in tetromino.transform)
                {
                    transPos[i] = gameSet.Round(mino.position);
                    transPos[i].x -= 25;    // 오프셋 조정
                    i++;
                }
            }
            else
            {
                foreach (Transform mino in tetromino.transform)
                {
                    transPos[i] = gameSet.Round(mino.position);
                    transPos[i].x -= 16;    // 오프셋 조정
                    transPos[i].y += 10;
                    i++;
                }
            }


            return transPos;
        }

        // 게임판 복사
        bool[,] CopyBoard(bool[,] input)
        {
            bool[,] tempBoard = new bool[10, 20];

            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 20; y++)
                {
                    tempBoard[x, y] = input[x, y];
                }
            }

            return tempBoard;
        }


        // 테트로 미노 움직임 ---------------------------------------------------
        // 회전
        Vector2[] Rotate(GameObject tetromino)
        {
            tetromino.transform.transform.Rotate(0, 0, -90);

            return TransMino(tetromino);
        }

        // X좌표 이동
        Vector2[] MoveX(Vector2[] pos, int v)
        {
            for (int i = 0; i < 4; i++)
            {
                pos[i] += new Vector2(v, 0);
            }
            return pos;
        }

        // Y좌표 이동
        Vector2[] MoveY(Vector2[] pos, int v)
        {
            for (int i = 0; i < 4; i++)
            {
                pos[i] += new Vector2(0, v);
            }

            return pos;
        }

        // 아래로 쭉 이동
        Vector2[] MoveDown(bool[,] tempBoard, Vector2[] pos)
        {
            while (true)
            {
                MoveY(pos, -1);
                if (!CheckPos(tempBoard, pos))
                {
                    MoveY(pos, 1);
                    return pos;
                }
            }
        }

        // 올바른 이동 확인
        bool CheckPos(bool[,] tempBoard, Vector2[] pos)
        {
            // 게임판을 벗어났는가?
            for (int i = 0; i < 4; i++)
            {
                if (pos[i].x < 0 || pos[i].x >= 10)
                {
                    return false;
                }

                if (pos[i].y < 0)
                {
                    return false;
                }
            }

            if (tempBoard == null)
            {
                return true;
            }

            // 게임판과 겹치는 부분이 있는가?
            for (int i = 0; i < 4; i++)
            {
                if (pos[i].y >= 20)
                {
                    // IndexOutOfRange
                }
                else if (tempBoard[(int)pos[i].x, (int)pos[i].y] == true)
                {
                    return false;
                }
            }

            return true;
        }

        // 이동값 변경
        int SetMoveValue(int move, int v)
        {
            move *= 4;
            move += v;

            return move;
        }


        // 모델 생성 ------------------------------------------------------------
        void NextModel(bool[,] tempBoard, Vector2[] currentPos, int move)
        {
            Vector2[] nextPos;

            for (int i = 0; i < 4; i++)
            {
                nextPos = TransMino(nextMino, 1);        // WARNING

                // 좌측 탐색
                do
                {
                    bool[,] tempBoard2 = CopyBoard(tempBoard);

                    MakeModel(tempBoard2, currentPos, nextPos, move);

                } while (CheckPos(null, MoveX(nextPos, -1)));


                // 우측 탐색
                nextPos = TransMino(nextMino, 1);

                while (CheckPos(null, MoveX(nextPos, 1)))
                {
                    bool[,] tempBoard2 = CopyBoard(tempBoard);

                    MakeModel(tempBoard2, currentPos, nextPos, move);
                }

                // 회전
                Rotate(nextMino);
            }
        }

        void MakeModel(bool[,] tempBoard, Vector2[] currentPos, Vector2[] nextPos, int move)
        {
            int blank = 0;
            int height = 0;

            nextPos = AddBoard(tempBoard, nextPos);

            Model newModel = new Model();
            newModel.move = move;


            // 현재 블럭의 최고 높이 계산
            for (int i = 0; i < 4; i++)
            {
                if (height < currentPos[i].y)
                {
                    height = (int)currentPos[i].y;
                }
                if (height < nextPos[i].y)
                {
                    height = (int)nextPos[i].y;
                }
            }

            height++;   // index + 1이 실제 높이
            newModel.height = height;


            for (int x = 0; x < 10; x++)
            {
                int temp = 0;

                for (int y = 0; y < 20; y++)
                {
                    if (tempBoard[x, y] == true)
                    {
                        temp = y + 1;
                    }
                    newModel.debug[x] = temp;
                }
            }


            // 빈칸 계산 
            blank = GetBlank(tempBoard) - GetBlank(board);
            newModel.blank = blank;


            // 거리 계산
            newModel.distance = GetDistance(currentPos);


            // 최적 모델 탐색
            if (index == 0)
            {
                ChangeModel(newModel);
            }
            else if (blank == 0)
            {
                if (maxBlank != 0)
                {
                    ChangeModel(newModel);
                }
                else if (maxBlank == 0)
                {
                    if (height < maxHeight)
                    {
                        ChangeModel(newModel);
                    }
                    else if (height == maxHeight)
                    {
                        model[index++] = newModel;
                    }
                }
            }
            else if (height < maxHeight)
            {
                if (blank <= maxBlank)
                {
                    ChangeModel(newModel);
                }
                else
                {
                    newModel = null;    // 나중에 제거
                }
            }
            else if (height == maxHeight)
            {
                if (blank < maxBlank)
                {
                    ChangeModel(newModel);
                }
                else if (blank == maxBlank)
                {
                    model[index++] = newModel;
                }
                else
                {
                    newModel = null;
                }
            }
        }

        // 게임판에 블럭 등록
        Vector2[] AddBoard(bool[,] tempBoard, Vector2[] pos)
        {
            Vector2[] tempPos = new Vector2[4];

            for (int i = 0; i < 4; i++)
            {
                tempPos[i] = pos[i];
            }

            tempPos = MoveDown(tempBoard, tempPos);

            for (int i = 0; i < 4; i++)
            {
                tempBoard[(int)tempPos[i].x, (int)tempPos[i].y] = true;
            }

            return tempPos;
        }

        // 빈칸 개수 반환
        int GetBlank(bool[,] tempBoard)
        {
            int blank = 0;

            for (int x = 0; x < 10; x++)
            {
                int weight = 0;
                int top = 0;

                for (int y = 0; y < 20; y++)
                {
                    if (tempBoard[x, y] == true)
                    {
                        weight++;
                        top = y + 1;
                    }
                }
                blank += top - weight;
            }

            return blank;
        }

        // 최적모델 교체
        void ChangeModel(Model newModel)
        {
            index = 0;
            model[index++] = newModel;
            maxBlank = newModel.blank;
            maxHeight = newModel.height;
        }

        // 모델 선택 -----------------------------------------------------------
        void SelectModel()
        {
            // 바깥쪽일수록 좋다.

            int select = 0;
            int max = 0;
            for (int i = 0; i < index; i++)
            {
                if (max < model[i].distance)
                {
                    select = i;
                    max = model[i].distance;
                }
            }


            // 지워지는 라인이 있는가
            selectedMove = ChangeMove(model[select].move);

            //selectedMove = ChangeMove(model[0].move);
        }

        // 가운데로 부터 얼마나 떨어져있는지 반환
        int GetDistance(Vector2[] pos)
        {
            int x = 0;

            for (int i = 0; i < 4; i++)
            {
                int temp = (int)pos[i].x;

                if (temp <= 4)
                {
                    temp = 4 - temp;
                    if (temp > x)
                    {
                        x = temp;
                    }
                }
                else
                {
                    temp = temp - 5;
                    if (temp > x)
                    {
                        x = temp;
                    }
                }
            }

            return x;
        }

        public void Move()
        {
            int move = selectedMove % 4;
            selectedMove /= 4;

            if (move == 0)
            {
                currentMino.GetComponent<Tetromino>().MoveDownDown();
            }
            else if (move == 1)
            {
                currentMino.GetComponent<Tetromino>().MoveRight();
            }
            else if (move == 2)
            {
                currentMino.GetComponent<Tetromino>().MoveLeft();
            }
            else if (move == 3)
            {
                currentMino.GetComponent<Tetromino>().MoveRotaion();
            }
        }

        int ChangeMove(int move)
        {
            int[] temp = new int[10];
            int index = 0;
            while (move != 0)
            {
                temp[index++] = move % 4;
                move /= 4;
            }

            for (int i = 0; i < index; i++)
            {
                move *= 4;
                move += temp[i];
            }

            return move;
        }
    }
}