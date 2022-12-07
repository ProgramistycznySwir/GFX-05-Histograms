using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFX_05_Histograms.Effects;

/// <summary>
/// Selekcja iteratywna średniej (ang. Mean Iterative Selection),
/// </summary>
public static class Hist_Binarize_MinError
{
    public static void Apply(Span<byte> bmp)
    {
        int[][] imageHist = Histogram.Calculate(bmp);
        (byte, byte)[] channelsMinMax = Histogram.Min(imageHist).Zip(Histogram.Max(imageHist)).ToArray();
        byte[] thresholds = CalculateMinError(imageHist, channelsMinMax);
        byte[][] remapLUT = Histogram.Remap.Binarize(thresholds);

        Histogram.Remap.Apply(bmp, remapLUT);
    }

    public static byte[] CalculateMinError(int[][] hist, (byte Min, byte Max)[] channelsMinMax)
    {
        byte[] result = new byte[3];
        for(int cha = 0; cha < 3; cha++)
        {
            (int min, int max) = channelsMinMax[cha];
            float minSigma = float.PositiveInfinity;
            for (int i = min; i < max; i++)
            {
                float pixelBack = 0; float pixelFore = 0;
                float omegaBack = 0; float omegaFore = 0;
                for (int ii = min; ii <= i; ii++)
                {
                    pixelBack += hist[cha][ii];
                    omegaBack = omegaBack + ii * hist[cha][ii];
                }
                for (int ii = i + 1; ii <= max; ii++)
                {
                    pixelFore += hist[cha][ii];
                    omegaFore = omegaFore + ii * hist[cha][ii];
                }
                omegaBack = omegaBack / pixelBack;
                omegaFore = omegaFore / pixelFore;
                float SigmaBack = 0; float SigmaFore = 0;
                for (int ii = min; ii <= i; ii++)
                    SigmaBack = SigmaBack + (ii - omegaBack) * (ii - omegaBack) * hist[cha][ii];
                for (int ii = i + 1; ii <= max; ii++)
                    SigmaFore = SigmaFore + (ii - omegaFore) * (ii - omegaFore) * hist[cha][ii];

                if (SigmaBack == 0 || SigmaFore == 0)
                {
                    result[cha] = (byte)i;
                }
                else
                {
                    SigmaBack = MathF.Sqrt(SigmaBack / pixelBack);
                    SigmaFore = MathF.Sqrt(SigmaFore / pixelFore);
                    float sigma = pixelBack * MathF.Log(SigmaBack / pixelBack) + pixelFore * MathF.Log(SigmaFore / pixelFore) - pixelBack * MathF.Log(pixelBack) - pixelFore * MathF.Log(pixelFore);
                    if (sigma < minSigma)
                    {
                        minSigma = sigma;
                        result[cha] = (byte)i;
                    }
                }
            }
        }
        return result;
    }
}
