using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static GFX_05_Histograms.Prelude;
namespace GFX_05_Histograms.Effects;

// Actually it's not based on histogram
public static class Hist_Niblack
{
    public static void Apply(Span<byte> bmp, int width, int size = 3, float k = 0.25f)
    {
        IEnumerator<(byte Val, byte Mid, byte Max)> region = Histogram.AnalyzeRegions(bmp.ToArray(), width, size).GetEnumerator();
        for (int i = 0; i < bmp.Length; i++)
        {
            region.MoveNext();
            (byte val, byte min, byte max) = region.Current;

            byte mid = (byte)((min + max) / 2);
            float stdDev = standardDeviation(val, mid, min, max);

            float threshold = mid - k * stdDev;
            bmp[i] = Prelude.threshold(bmp[i], (byte)threshold);
        }
    }
}
