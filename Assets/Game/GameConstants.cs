using System;
using UnityEngine;

namespace Game
{
    // Interview variant. In real game there is a style asset for it, that can change artists
    public static class GameConstants
    {
        public static readonly Color WinColor = ParseColorFromHexString("#184912");
        public static readonly Color LoseColor = ParseColorFromHexString("#941C1C");
        public static readonly Color DrawColor = ParseColorFromHexString("#323232");

        // Parses color from string containing hex value of color(same as in editor)
        // String must start from '#'
        private static Color ParseColorFromHexString(string hexString)
        {
            if (!ColorUtility.TryParseHtmlString(hexString, out var resultColor))
            {
                resultColor = Color.white;
            }

            return resultColor;
        }
    }
}