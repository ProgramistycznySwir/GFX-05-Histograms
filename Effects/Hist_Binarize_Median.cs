using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFX_05_Histograms.Effects;

/// <summary>
/// Selekcja iteratywna średniej (ang. Mean Iterative Selection),<br />
/// Tyle że się pomyliłem i zaimplementowałem medianę xd.
/// </summary>
public static class Hist_Binarize_Median
{
    public static void Apply(Span<byte> bmp)
    {
        int[][] imageHist = Histogram.Calculate(bmp);
        byte[] thresholds = Histogram.Median(imageHist, bmp.Length/3);
        byte[][] remapLUT = Histogram.Remap.Binarize(thresholds);

        Histogram.Remap.Apply(bmp, remapLUT);
    }
}
