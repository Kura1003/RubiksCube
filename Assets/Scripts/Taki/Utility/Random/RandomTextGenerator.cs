using System;
using System.Collections.Generic;
using System.Text;
using Taki.Utility.Core;

namespace Taki.Utility
{
    [Flags]
    public enum CharType
    {
        LowerCase = 1 << 0,
        UpperCase = 1 << 1,
        Numeric = 1 << 2
    }

    internal static class RandomTextGenerator
    {
        private const string LowerCaseChars = "abcdefghijklmnopqrstuvwxyz";
        private const string UpperCaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string NumericChars = "0123456789";

        internal static string Generate(int length, CharType charType)
        {
            Thrower.IfOutOfRange(length, 1, int.MaxValue);
            Thrower.IfTrue(charType == 0, $"CharType���w�肵�Ă��������B");

            var charSet = new List<char>();
            if (charType.HasFlag(CharType.LowerCase))
            {
                charSet.AddRange(LowerCaseChars);
            }

            if (charType.HasFlag(CharType.UpperCase))
            {
                charSet.AddRange(UpperCaseChars);
            }

            if (charType.HasFlag(CharType.Numeric))
            {
                charSet.AddRange(NumericChars);
            }

            var random = SeedGenerator.GetRandom();
            var sb = new StringBuilder(length);

            for (var i = 0; i < length; i++)
            {
                var randomIndex = random.Next(charSet.Count);
                sb.Append(charSet[randomIndex]);
            }

            return sb.ToString();
        }

        internal static string GenerateUnique(int length, CharType charType)
        {
            Thrower.IfOutOfRange(length, 1, int.MaxValue);
            Thrower.IfTrue(charType == 0, $"CharType���w�肵�Ă��������B");

            var charSet = new List<char>();
            if (charType.HasFlag(CharType.LowerCase))
            {
                charSet.AddRange(LowerCaseChars);
            }

            if (charType.HasFlag(CharType.UpperCase))
            {
                charSet.AddRange(UpperCaseChars);
            }

            if (charType.HasFlag(CharType.Numeric))
            {
                charSet.AddRange(NumericChars);
            }

            Thrower.IfTrue(length > charSet.Count,
                $"�������悤�Ƃ��Ă��镶����̒���({length})���A" +
                $"�g�p�\�Ȉ�ӂȕ�����({charSet.Count})�𒴂��Ă��܂��B");

            var random = SeedGenerator.GetRandom();
            var sb = new StringBuilder(length);

            for (var i = 0; i < length; i++)
            {
                var randomIndex = random.Next(charSet.Count);
                sb.Append(charSet[randomIndex]);
                charSet.RemoveAt(randomIndex);
            }

            return sb.ToString();
        }
    }
}