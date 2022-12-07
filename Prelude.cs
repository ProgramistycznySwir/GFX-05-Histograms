using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFX_05_Histograms;

public static class Prelude
{
    public static byte clampByte(int num) => num > byte.MaxValue ? (byte)255 : num < 0 ? (byte)0 : (byte)num;
    public static byte invLerp(float t) => (byte)(t * byte.MaxValue);

    // Chanels:
    public const int ChaR = 2;
    public const int ChaG = 1;
    public const int ChaB = 0;

    public static int R(int i) => i + ChaR;
    public static int G(int i) => i + ChaG;
    public static int B(int i) => i + ChaB;

    [Flags]
    public enum Cha {
        R   = 1,
        G   = 2, 
        RG  = 3,
        B   = 4,
        RB  = 5,
        GB  = 6,
        RGB = 7
    }

    /// <summary> Returns jagged-array that is in rectangular shape </summary>
    public static T[][] NewArr<T>(int dim0 = 3, int dim1 = 256)
        => Enumerable.Range(0, dim0).Select(_ => new T[dim1]).ToArray();

    public static byte threshold(byte val, byte threshold)
        => val > threshold ? byte.MaxValue : byte.MinValue;
    public static Func<byte, byte> threshold(byte threshold)
        => (val) => Prelude.threshold(val, threshold);

    public static float standardDeviation(byte val, byte mid, byte min, byte max)
        => MathF.Sqrt((MathF.Pow(val - mid, 2) + MathF.Pow(min - mid, 2) + MathF.Pow(max - mid, 2)) / 2f);
}
