using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static GFX_05_Histograms.Prelude;
namespace GFX_05_Histograms.Effects;

public static class Histogram
{
    /// <summary> Calculate histogram from image </summary>
    public static int[][] Calculate(Span<byte> bmp)
    {
        // Basically count sort, but without sotring.
        int[][] result = NewArr<int>();

        for (int i = 0; i < bmp.Length; i++)
            result[i % 3][bmp[i]]++;

        return result;
    }

    /// <summary>
    /// Converts from min..max range to 0f..1f range.
    /// </summary>
    public static float[][] Normalize(int[][] hists, int min = 0, int? max = null)
        => hists.Select(hist => Normalize(hist, min, max)).ToArray();
    public static float[] Normalize(int[] hist, int min = 0, int? max = null)
    {
        max ??= hist.Max();
        return hist.Select(x => x / (float)max).ToArray();
    }

    /// <summary> Dystrybuanta. </summary>
    public static float[][] CumulativeDistribution(int[][] hists, int? pixelCount = null)
        => hists.Select(hist => CumulativeDistribution(hist, pixelCount)).ToArray();
    public static float[] CumulativeDistribution(int[] hist, int? pixelCount= null)
    {
        pixelCount ??= hist.Sum();
        float[] result = new float[hist.Length];
        result[0] = hist[0] / (float)pixelCount;
        for (int i = 1; i < hist.Length; i++)
            result[i] = result[i-1] + hist[i] / (float)pixelCount;
        return result;
    }

    /// <summary> Średnia. </summary>
    public static byte[] Mean(int[][] hists, int? pixelCount = null)
        => hists.Select(hist => Mean(hist, pixelCount)).ToArray();
    public static byte Mean(int[] hist, int? pixelCount= null)
    {
        pixelCount ??= hist.Sum();
        long weighedSum = 0;
        for (int i = 0; i < 256; i++)
            weighedSum += hist[i] * i;
        return (byte)(weighedSum / pixelCount);
    }

    /// <summary> Mediana wartości na obrazie reprezentowanym przez histogram. </summary>
    public static byte[] Median(int[][] hists, int? pixelCount = null)
        => hists.Select(hist => Median(hist, pixelCount)).ToArray();
    public static byte Median(int[] hist, int? pixelCount = null)
    {
        pixelCount ??= hist.Sum();
        byte result = 0;
        int pixelsFromMedian = pixelCount.Value / 2;
        for (int i = 0; i < 256; i++)
        {
            if (pixelsFromMedian <= 0)
                break;

            result = (byte)i;
            pixelsFromMedian -= hist[i];
        }
        return result;
    }
    /// <summary> Minimalna wartość na obrazie reprezentowanym przez histogram. </summary>
    public static byte[] Min(int[][] hists)
        => hists.Select(Min).ToArray();
    public static byte Min(int[] hist)
    {
        int min = 0;
        for (; min <= byte.MaxValue && hist[min] is 0; min++) ;

        return clampByte(min);
    }
    /// <summary> Maksymalna wartość na obrazie reprezentowanym przez histogram. </summary>
    public static byte[] Max(int[][] hists)
        => hists.Select(Max).ToArray();
    public static byte Max(int[] hist)
    {
        int max = byte.MaxValue;
        for (; max >= 0 && hist[max] is 0; max--) ;

        return clampByte(max);
    }

    /// <summary> Analizuje obraz pixel po pixel'u podając minimalną i maksymalną wartość w obszarze size wokół pixel'u. </summary>
    public static IEnumerable<(byte Val, byte Mid, byte Max)> AnalyzeRegions(byte[] bmp, int width, int size = 3)
    {
        int height = (bmp.Length / 3) / width;
        Func<int, int, int> xy = (x, y) => (x + y * width) * 3;
        // Loop over all pixels:
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                // Loop over channels:
                for (int cha = 0; cha < 3; cha++)
                {
                    byte min = byte.MinValue, max = byte.MaxValue;
                    // Loop over all neighbours
                    for (int xx = -size; xx <= size; xx++)
                        for (int yy = -size; yy <= size; yy++)
                        {
                            (int X, int Y) = (x + xx, y + yy);
                            // Check if position is in image boundaries.
                            if (X < 0 || X >= width || Y < 0 || Y >= height)
                                continue;
                            min = Math.Min(min, bmp[xy(X, Y) + cha]);
                            max = Math.Max(max, bmp[xy(X, Y) + cha]);
                        }

                    yield return (bmp[xy(x, y) + cha], min, max);
                }
    }


    public static class Remap
    {
        public static void Apply(Span<byte> bmp, byte[][] remaps)
        {
            for (int i = 0; i < bmp.Length; i++)
                bmp[i] = remaps[i % 3][bmp[i]];
        }

        /// <summary> Lookup table zawierający wartości zbinaryzowane przy pomocy threshold. </summary>
        public static byte[] BinarizeChannel(byte threshold)
            => Enumerable.Range(0, 256).Select(i => i > threshold ? byte.MaxValue : byte.MinValue).ToArray();
        /// <summary> Lookup table zawierający wartości zbinaryzowane przy pomocy threshold. </summary>
        public static byte[][] Binarize(byte[] thresholds)
            => thresholds.Select(threshold => BinarizeChannel(threshold)).ToArray();
        /// <summary> Lookup table zawierający wartości zbinaryzowane przy pomocy threshold. </summary>
        public static byte[][] Binarize(byte threshold)
            => Enumerable.Range(0, 3).Select(_ => BinarizeChannel(threshold)).ToArray();
    }
}
