using System.Drawing;
using System.Text.Json.Serialization;

namespace POE2RuleTool.Models;

public sealed class PixelCondition
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Tolerance { get; set; } = 5;
    public bool HasSample { get; set; }
    public int SampleArgb { get; set; }

    [JsonIgnore]
    public Point Point => new(X, Y);

    [JsonIgnore]
    public Color SampleColor
    {
        get => Color.FromArgb(SampleArgb);
        set
        {
            SampleArgb = value.ToArgb();
            HasSample = true;
        }
    }

    [JsonIgnore]
    public string SampleDisplay => HasSample
        ? $"{SampleColor.R}, {SampleColor.G}, {SampleColor.B}"
        : "Not sampled";

    public bool HasChangedFromSample(Color currentColor)
    {
        return HasSample && !IsColorSimilar(currentColor, SampleColor, Tolerance);
    }

    public PixelCondition Clone()
    {
        return new PixelCondition
        {
            X = X,
            Y = Y,
            Tolerance = Tolerance,
            HasSample = HasSample,
            SampleArgb = SampleArgb
        };
    }

    private static bool IsColorSimilar(Color color1, Color color2, int tolerance)
    {
        return Math.Abs(color1.R - color2.R) <= tolerance
            && Math.Abs(color1.G - color2.G) <= tolerance
            && Math.Abs(color1.B - color2.B) <= tolerance;
    }
}
