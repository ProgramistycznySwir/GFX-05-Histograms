using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static GFX_05_Histograms.Prelude;
namespace GFX_05_Histograms.Effects;

public static class Hist_Expand
{
    public static void Apply(Span<byte> bmp)
    {
        int[][] imgHist = Histogram.Calculate(bmp);
        (byte Min, byte Max)[] channelsMinMax = Histogram.Min(imgHist).Zip(Histogram.Max(imgHist)).ToArray();
        byte[][] remapLUT = CalculateRemap(channelsMinMax);

        Histogram.Remap.Apply(bmp, remapLUT);
    }

    private static byte[][] CalculateRemap((byte Min, byte Max)[] channelsMinMax)
    {
        byte[][] result = NewArr<byte>();

        for (int i = 0; i < 256; i++)
            for (int cha = 0; cha < 3; cha++)
            {
                (byte min, byte max) = channelsMinMax[cha];
                result[cha][i] = clampByte((int)((1f / (max - min)) * ((i - min) * 255)));
            }

        return result;
    }
}
