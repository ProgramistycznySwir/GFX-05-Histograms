using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static GFX_05_Histograms.Prelude;
namespace GFX_05_Histograms.Effects;

public static class Hist_Equalize
{
    public static void Apply(Span<byte> bmp)
    {
        int[][] imageHist = Histogram.Calculate(bmp);

        // Nie jestem pewien czy w tym miejscu nie powinienem skorzystać z tego CalculateRemap, ale nie wiem czemu nie działa (wyświetla czarny obraz)
        //byte[][] remapLUT = CalculateRemap(imageHist, bmp.Length / 3);
        byte[][] remapLUT = Histogram.CumulativeDistribution(imageHist, bmp.Length / 3).Select(arr => arr.Select(invLerp).ToArray()).ToArray();
        Histogram.Remap.Apply(bmp, remapLUT);
    }

    private static byte[][] CalculateRemap(int[][] imageHist, int pixelCount)
    {
        byte[][] result = NewArr<byte>();

        float scale_factor = 255f / pixelCount;
        Span<long> channelSums = stackalloc long[3];

        for(int cha = 0; cha < 3; cha++)
            for (int i = 0; i < imageHist.Length; i++)
            {
                channelSums[cha] += imageHist[cha][i];
                int a =
                result[cha][i] = clampByte((int)(channelSums[cha] * scale_factor));
            }

        return result;
    }
}
