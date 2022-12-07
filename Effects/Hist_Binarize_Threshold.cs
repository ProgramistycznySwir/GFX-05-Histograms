using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFX_05_Histograms.Effects;

public static class Hist_Binarize_Threshold
{
    public static void Apply(Span<byte> bmp, float value)
        => Histogram.Remap.Apply(bmp, Histogram.Remap.Binarize((byte)(value * byte.MaxValue)));
}
