using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFX_05_Histograms.Effects;

public static class Hist_Otsu
{
    public static void Apply(Span<byte> bmp)
    {
        int[][] imageHist = Histogram.Calculate(bmp);
        byte[] means = Histogram.Mean(imageHist, bmp.Length/3);
        byte[] channelThresholds = CalculateThresholds(imageHist, means);
        byte[][] remapLUT = Histogram.Remap.Binarize(channelThresholds);

        Histogram.Remap.Apply(bmp, remapLUT);
    }

    private static byte[] CalculateThresholds(int[][] hist, byte[] means)
    {
        float bcv = 0;
        byte[] result = new byte[3];
        for(int cha = 0; cha < 3; cha++)
        {
            float[] hist_norm = Histogram.Normalize(hist[cha]);
            //float max = hist[cha].Max();
            //float[] hist_norm = hist[cha].Select(x => x/max).ToArray();

            for (int i = 0; i < 256; i++)
            {
                float cs = 0;
                float m = 0;
                for (int ii = 0; ii < i; ii++)
                {
                    cs += hist_norm[ii];
                    m += ii * hist_norm[ii];
                }

                if (cs == 0)
                    continue;

                float old_bcv = bcv;
                float new_bcv = MathF.Pow(means[cha] * cs - m, 2) / (cs * (1 - cs));
                bcv = MathF.Max(old_bcv, new_bcv);
                if (bcv > old_bcv)
                    result[cha] = (byte)i;
            }
        }
        return result;
    }
}
