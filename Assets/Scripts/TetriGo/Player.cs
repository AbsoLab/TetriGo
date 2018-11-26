using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    class Player
    {
        public int level = 1;
        public int score = 0;
        public int line = 10;
        public double speed = 1;

  
        // 레벨업! ----------------------------------------------------
        public void LevelUp()
        {
            if (line <= 0)
            {
                speed *= 0.8;
                level++;
                line = 10;
            }
        }
        public void ForceLevelUp()
        {
            level++;
            speed *= 0.8;
        }

        // 점수 계산 --------------------------------------------------
        public void UpdateScore(int deleteLineNum)
        {
            // 지운 라인의 개수
            switch (deleteLineNum)
            {
                case 1:
                    Delete_1Line();
                    break;
                case 2:
                    Delete_2Line();
                    break;
                case 3:
                    Delete_3Line();
                    break;
                case 4:
                    Delete_4Line();
                    break;
            }

            deleteLineNum = 0;
        }

        void Delete_1Line()
        {
            score += 20;
            line -= 1;
        }

        void Delete_2Line()
        {
            score += 50;
            line -= 2;
        }

        void Delete_3Line()
        {
            score += 100;
            line -= 3;
        }

        void Delete_4Line()
        {
            score += 200;
            line -= 4;
        }
    }
}
        