using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static GFX_05_Histograms.Prelude;
namespace GFX_05_Histograms.Effects;

// Actually it's not based on histogram
public static class Hist_Bernsen
{
    public static void Apply(Span<byte> bmp, int width, int size = 3, int limit = 15)
    {
        IEnumerator<(byte Val, byte Mid, byte Max)> region = Histogram.AnalyzeRegions(bmp.ToArray(), width, size).GetEnumerator();
        for (int i = 0; i < bmp.Length; i++)
        {
            region.MoveNext();
            (byte val, byte min, byte max) = region.Current;

            byte mid = (byte)((min + max) / 2);
            byte contrast = (byte)(max - min);

            float threshold =
                contrast < limit
                    ? byte.MaxValue / 2
                    : mid;
            bmp[i] = Prelude.threshold(bmp[i], (byte)threshold);
        }
    }
}
