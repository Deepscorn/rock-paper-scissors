using System;
using System.Diagnostics;

namespace Game
{
    // Localization manager.
    // Note This is for interview task. In real-world game need to read from external source
    public static class Localization
    {
        public const string YouWin = "You are wining!";
        public const string OpponentWin = "You are losing..";
        public const string NoOneWin = "There is a draw";
        public const string OpponentSelectsPaper = "Opponent selects paper";
        public const string OpponentSelectsScissors = "Opponent selects scissors";
        public const string OpponentSelectsStone = "Opponent selects stone";
        public const string SelectYourFigure = "Select your figure:";
        public const string PressToContinue = "Press left mouse button to continue...";
        
        public static string Localize(HandDecision handDecision)
        {
            switch (handDecision)
            {
                case HandDecision.Paper:
                    return OpponentSelectsPaper;
                case HandDecision.Scissors:
                    return OpponentSelectsScissors;
                case HandDecision.Stone:
                    return OpponentSelectsStone;
                default:
                    Debug.Assert(false, "value not localized: " + handDecision);
                    return handDecision.ToString();
            }
        }
    }
}