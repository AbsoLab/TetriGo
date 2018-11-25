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
        bool[,] board;              // 받아온 게임판
        GameObject currentMino;     // 현재 움직일 수 있는 블럭
        GameObject nextMino;        // 다음에 올 블럭
        GameSet gameSet = GameObject.FindObjectOfType<GameSet>();

        int selectedMove;           // 컴퓨터가 어떻게 움직일지 저장 (2bit단위로 읽는다)

        Model[] model = new Model[1024];  // 경우의 수
        int count;           // 만들어진 모델의 개수
        int bestBlank;       // 최적 모델의 빈칸 개수
        int bestHeight;      // 최적 모델의 최대 높이

        // 초기화
        public void SetComputer(bool[,] board, GameObject currentMino, GameObject nextMino)
        {
            this.board = board;
            this.currentMino = currentMino;
            this.nextMino = nextMino;

            count = 0;
            bestBlank = 999;
            bestHeight = 999;

            Calculate();        // 경우의 수 계산
            SelectModel();      // 모델 선택
        }

        // 경우의 수 계산
        void Calculate()
        {
            int move = 0;
            int rotate = 0;

            Vector2[] currentPos;

            // 회전 4번
            for (int i = 0; i < 4; i++)
            {
                // 좌측 탐색 (가운데 위치 포함)
                move = rotate;
                currentPos = TransMino(currentMino);

                do
                {
                    // 게임판 복사
                    bool[,] tempBoard = CopyBoard(board);       

                    // 테트로미노를 밑으로 내린 후 tempBoard에 더한다. 내린 테트로미노의 위치 반환
                    Vector2[] downPos = AddBoard(tempBoard, currentPos);

                    // 다음 테트로미노를 이용하여 모델 생성
                    NextModel(tempBoard, downPos, move);        

                    // 왼쪽으로 이동 추가 (2bit 왼쪽으로 shift 한 후 더한다)
                    move = SetMoveValue(move, 2);

                } while (CheckPos(board, MoveX(currentPos, -1)));   // 왼쪽으로 이동 후 위치 확인


                // 우측 탐색
                move = rotate;
                currentPos = TransMino(currentMino);    // 처음 가운데 위치 저장

                // 오른쪽으로 이동 후 위치 확인
                while (CheckPos(board, MoveX(currentPos, 1)))
                {
                    // 게임판 복사
                    bool[,] tempBoard = CopyBoard(board);

                    // 오른쪽으로 이동 추가 (2bit 왼쪽으로 shift 한 후 더한다)
                    move = SetMoveValue(move, 1);

                    // 테트로미노를 밑으로 내린 후 tempBoard에 더한다. 내린 테트로미노의 위치 반환
                    Vector2[] downPos = AddBoard(tempBoard, currentPos);

                    // 다음 테트로미노를 이용하여 모델 생성
                    NextModel(tempBoard, downPos, move);
                }

                // 회전
                Rotate(currentMino);
                rotate = SetMoveValue(rotate, 3);   // 회전 추가 (2bit 왼쪽으로 shift 한 후 더한다)
            }
        }

        // Tetromino의 상대 위치 반환
        Vector2[] TransMino(GameObject tetromino, int current = 0)
        {
            Vector2[] transPos = new Vector2[4];

            int i = 0;

            // currentMino인 경우
            if (current == 0)
            {
                foreach (Transform mino in tetromino.transform)
                {
                    transPos[i] = gameSet.Round(mino.position);
                    transPos[i].x -= 25;    // 오프셋 조정
                    i++;
                }
            }
            // nextMino인 경우
            else if (current == 1)
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
        Vector2[] MoveX(Vector2[] pos, int x)
        {
            for (int i = 0; i < 4; i++)
            {
                pos[i] += new Vector2(x, 0);
            }
            return pos;
        }

        // Y좌표 이동
        Vector2[] MoveY(Vector2[] pos, int y)
        {
            for (int i = 0; i < 4; i++)
            {
                pos[i] += new Vector2(0, y);
            }

            return pos;
        }

        // 아래로 쭉 이동
        Vector2[] MoveDown(bool[,] tempBoard, Vector2[] pos)
        {
            // 밑으로 이동 후 위치 확인
            while (CheckPos(tempBoard, MoveY(pos, -1)));
            MoveY(pos, 1);

            return pos;

            // return pos;
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

        // 이동값 더하기
        int SetMoveValue(int move, int v)
        {
            move *= 4;  // move << 2
            move += v;

            return move;
        }


        // 모델 생성 ------------------------------------------------------------
        void NextModel(bool[,] tempBoard, Vector2[] currentPos, int move)
        {
            Vector2[] nextPos;

            for (int i = 0; i < 4; i++)
            {
                nextPos = TransMino(nextMino, 1);

                // 좌측 탐색
                do
                {
                    bool[,] tempBoard2 = CopyBoard(tempBoard);

                    Vector2[] downPos = AddBoard(tempBoard2, nextPos);

                    // 모델 생성
                    MakeModel(tempBoard2, currentPos, downPos, move);

                } while (CheckPos(tempBoard, MoveX(nextPos, -1)));


                // 우측 탐색
                nextPos = TransMino(nextMino, 1);

                while (CheckPos(tempBoard, MoveX(nextPos, 1)))
                {
                    bool[,] tempBoard2 = CopyBoard(tempBoard);

                    Vector2[] downPos = AddBoard(tempBoard2, nextPos);

                    // 모델 생성
                    MakeModel(tempBoard2, currentPos, downPos, move);
                }

                // 회전
                Rotate(nextMino);
            }
        }

        void MakeModel(bool[,] tempBoard, Vector2[] currentPos, Vector2[] nextPos, int move)
        {
            Model newModel = new Model();
            newModel.move = move;

            int blank = 0;
            int height = 0;

            // 라인 지우기 추가 예정
            // DeleteLine(tempBoard);

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

            // 디버그용, 추후 삭제 예정
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
            if (count == 0)
            {
                ChangeModel(newModel);
            }
            else if (blank == 0)
            {
                if (bestBlank != 0)
                {
                    ChangeModel(newModel);
                }
                else if (bestBlank == 0)
                {
                    if (height < bestHeight)
                    {
                        ChangeModel(newModel);
                    }
                    else if (height == bestHeight)
                    {
                        model[count++] = newModel;
                    }
                }
            }
            else if (height < bestHeight)
            {
                if (blank <= bestBlank)
                {
                    ChangeModel(newModel);
                }
                else
                {
                    newModel = null;    // 나중에 제거
                }
            }
            else if (height == bestHeight)
            {
                if (blank < bestBlank)
                {
                    ChangeModel(newModel);
                }
                else if (blank == bestBlank)
                {
                    model[count++] = newModel;
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

        // 꽉찬 라인 지우기
        void DeleteLine(bool[,] tempBoard)
        {
            for (int y=0; y<20; y++)
            {
                if (IsFullLine(tempBoard, y))
                {
                    AllLineDown(tempBoard, y+1);
                    y--;
                }
            }
        }

        bool IsFullLine(bool[,] tempBoard, int y)
        {
            for (int x=0; x<10; x++)
            {
                if (tempBoard[x, y] == false)
                {
                    return false;
                }
            }

            return true;
        }

        void LineDown(bool[,] tempBoard, int y)
        {
            for (int x = 0; x < 10; x++)
            {
                tempBoard[x, y-1] = tempBoard[x, y];
            }
        }

        void AllLineDown(bool[,] tempBoard, int y)
        {
            for (int i = y; i<20; i++)
            {
                LineDown(tempBoard, i);
            }
        }

        // 빈칸 개수 반환
        int GetBlank(bool[,] tempBoard)
        {
            int blank = 0;

            for (int x = 0; x < 10; x++)
            {
                int weight = 0;     // 빈칸이 아닌 개수
                int top = 0;        // 높이

                for (int y = 0; y < 20; y++)
                {
                    if (tempBoard[x, y] == true)
                    {
                        weight++;
                        top = y + 1;
                    }
                }

                blank += top - weight;  // 빈칸 = 높이 - 빈칸이 아닌 개수
            }

            return blank;
        }

        // 최적모델 교체
        void ChangeModel(Model newModel)
        {
            count = 0;
            model[count++] = newModel;
            bestBlank = newModel.blank;
            bestHeight = newModel.height;
        }

        // 모델 선택 -----------------------------------------------------------
        void SelectModel()
        {
            // 바깥쪽일수록 좋다.

            int select = 0;
            int max = 0;
            for (int i = 0; i < count; i++)
            {
                if (max < model[i].distance)
                {
                    select = i;
                    max = model[i].distance;
                }
            }

            selectedMove = ChangeMove(model[select].move);
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