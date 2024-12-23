using System;
using Game.Utils;
using UnityEngine;

namespace Game
{
    // Game logic is in separate file always. It's usually many files. Only one here for interview test task
    public class Logic
    {
        private static readonly HandDecision[] AllDecisions = EnumExt.GetValues<HandDecision>();
        
        public int PlayerScore { get; private set; }
        public int OpponentScore { get; private set; }

        private readonly System.Random _random = new(Environment.TickCount);

        public HandDecision MakeOpponentDecision()
        {
            return AllDecisions[_random.Next(0, AllDecisions.Length)];
        }

        public HandDecision MakeOpponentDecision(HandDecision decision, float aiDifficultyCoef)
        {
            if (Mathf.Approximately(aiDifficultyCoef, 1f))
            {
                return GetWinDecision(decision);
            }
            if (Mathf.Approximately(aiDifficultyCoef, 0f))
            {
                return GetLoseDecision(decision);
            }
            
            float current;

            if (PlayerScore != 0) // защита от деления на ноль
            {
                current = OpponentScore / (float) PlayerScore;
            }
            else
            {
                current = 1 - aiDifficultyCoef;
            }

            if (current < aiDifficultyCoef) // нужно победить
            {
                // ничья обеспечивает более медленное приближение к целевому значению
                float maybeCurrent = (OpponentScore + 1) / (float) (PlayerScore + 1);

                if (maybeCurrent < aiDifficultyCoef)
                {
                    // но если в результате мы не достигаем unhonestCoef, то двигаемся победой
                    return GetWinDecision(decision);
                }
                return decision; // двигаемся ничьей
            }
            if (current > aiDifficultyCoef) // нужно програть
            {
                //return GetOpponentDecisionToLose(decision);
                // ничья обеспечивает более медленное приближение к целевому значению
                float maybeCurrent = (OpponentScore + 1) / (float) (PlayerScore + 1);

                if (maybeCurrent > aiDifficultyCoef)
                {
                    // но если в результате мы не достигаем unhonestCoef, то двигаемся поражением
                    return GetLoseDecision(decision);
                }
                return decision; // двигаемся ничьей
            }
            return decision;
        }

        public GameResult GetRoundResult(HandDecision playerDecision, HandDecision opponentDecision)
        {
            if (playerDecision == opponentDecision)
            {
                return GameResult.Draw;
            }

            if (GetWinDecision(playerDecision) == opponentDecision)
            {
                return GameResult.PlayerLose;
            }
            
            return GameResult.PlayerWins;
        }

        public void UpdateScore(GameResult roundResult)
        {
            PlayerScore += roundResult != GameResult.PlayerLose ? 1 : 0;
            OpponentScore += roundResult != GameResult.PlayerWins ? 1 : 0;
        }

        private HandDecision GetWinDecision(HandDecision playerDecision)
        {
            switch (playerDecision)
            {
                case HandDecision.Paper:
                    return HandDecision.Scissors;
                case HandDecision.Scissors:
                    return HandDecision.Stone;
                case HandDecision.Stone:
                    return HandDecision.Paper;
                default:
                    throw new NotImplementedException(playerDecision.ToString());
            }
        }

        private HandDecision GetLoseDecision(HandDecision playerDecision)
        {
            switch (playerDecision)
            {
                case HandDecision.Scissors:
                    return HandDecision.Paper;
                case HandDecision.Stone:
                    return HandDecision.Scissors;
                case HandDecision.Paper:
                    return HandDecision.Stone;
                default:
                    throw new NotImplementedException(playerDecision.ToString());
            } 
        }
    }
}