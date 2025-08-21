using System;
using UnityEngine;

namespace Taki.Utility.Core
{
    internal static class Thrower
    {
        internal static void IfTrue(bool condition, string message)
        {
            if (condition)
            {
                throw new InvalidOperationException(message);
            }
        }

        internal static void IfNull<T>(T data, string paramName) where T : class
        {
            if (data == null)
            {
                throw new ArgumentNullException(
                    paramName, 
                    $"�p�����[�^�[��null�ł��B");
            }
        }

        internal static void IfNotMatch<TExpected, TActual>(string message)
        {
            if (typeof(TExpected) != typeof(TActual))
            {
                throw new InvalidCastException(message);
            }
        }

        internal static void IfOutOfRange(int index, int length)
        {
            if (index < 0 || index >= length)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(index), 
                    $"�C���f�b�N�X�� 0 ���� {length - 1} �͈͓̔��ł���K�v������܂��B");
            }
        }

        internal static void IfOutOfRange(Vector2Int index, Vector2Int size)
        {
            if (index.x < 0 || index.x >= size.x || index.y < 0 || index.y >= size.y)
            {
                throw new ArgumentOutOfRangeException(
                    $"x�� 0 ���� {size.x - 1}�A" +
                    $"y�� 0 ���� {size.y - 1} �͈͓̔��ł���K�v������܂��B");
            }
        }

        internal static void IfOutOfRange(Vector3Int index, Vector3Int size)
        {
            if (index.x < 0 || index.x >= size.x || index.y < 0 || index.y >= size.y || index.z < 0 || index.z >= size.z)
            {
                throw new ArgumentOutOfRangeException(
                    $"x�� 0 ���� {size.x - 1}�A" +
                    $"y�� 0 ���� {size.y - 1}�A" +
                    $"z�� 0 ���� {size.z - 1} �͈͓̔��ł���K�v������܂��B");
            }
        }

        internal static void IfOutOfRange<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value), 
                    $"�l�� {min} ���� {max} �͈͓̔��ł���K�v������܂��B");
            }
        }
    }
}