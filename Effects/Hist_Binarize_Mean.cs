using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFX_05_Histograms.Effects;

/// <summary>
/// Selekcja iteratywna średniej (ang. Mean Iterative Selection),
/// </summary>
public static class Hist_Binarize_Mean
{
    public static void Apply(Span<byte> bmp)
    {
        int[][] imageHist = Histogram.Calculate(bmp);
        byte[] thresholds = Histogram.Mean(imageHist, bmp.Length/3);
        byte[][] remapLUT = Histogram.Remap.Binarize(thresholds);

        Histogram.Remap.Apply(bmp, remapLUT);
    }
}
