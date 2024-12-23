using System;
using Game.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    // Game logic is in separate file always. It's usually many files. Only one here for interview test task
    public class Logic
    {
        private static Logic instance;
        private readonly HandDecision[] allDecisions = EnumExt.GetValues<HandDecision>();

        public HandDecision MakeOpponentDecision()
        {
            return allDecisions[Random.Range(0, allDecisions.Length)];
        }

        public HandDecision MakeOpponentDecision(
            HandDecision decision, 
            float unhonestCoef,
            int playerScore,
            int opponentScore)
        {
            if (Mathf.Approximately(unhonestCoef, 1f))
            {
                return GetWinDecision(decision);
            }
            if (Mathf.Approximately(unhonestCoef, 0f))
            {
                return GetLoseDecision(decision);
            }
            
            float current;

            if (playerScore != 0) // защита от деления на ноль
            {
                current = opponentScore / (float) playerScore;
            }
            else
            {
                current = 1 - unhonestCoef;
            }

            if (current < unhonestCoef) // нужно победить
            {
                // ничья обеспечивает более медленное приближение к целевому значению
                float maybeCurrent = (opponentScore + 1) / (float) (playerScore + 1);

                if (maybeCurrent < unhonestCoef)
                {
                    // но если в результате мы не достигаем unhonestCoef, то двигаемся победой
                    return GetWinDecision(decision);
                }
                return decision; // двигаемся ничьей
            }
            if (current > unhonestCoef) // нужно програть
            {
                //return GetOpponentDecisionToLose(decision);
                // ничья обеспечивает более медленное приближение к целевому значению
                float maybeCurrent = (opponentScore + 1) / (float) (playerScore + 1);

                if (maybeCurrent > unhonestCoef)
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