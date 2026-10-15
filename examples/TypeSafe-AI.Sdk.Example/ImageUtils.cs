using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using TypeSafeAI.Sdk.Contracts;

namespace TypeSafeAI.Sdk.Example;

/// <summary>
/// Provides helper methods for image processing in examples.
/// </summary>
public static class ImageUtils
{
    /// <summary>
    /// Draws a red rectangle for the provided box edges and saves the result as a JPEG image.
    /// </summary>
    /// <param name="inputImagePath">The input image file path.</param>
    /// <param name="outputImagePath">The output image file path.</param>
    /// <param name="boxEdges">The normalized box edges in the range of 0 to 1.</param>
    /// <param name="strokeThickness">The rectangle stroke thickness in pixels.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that completes when the image is written.</returns>
    public static async Task DrawRedBoxAsync(
        string inputImagePath,
        string outputImagePath,
        BoxEdges boxEdges,
        int strokeThickness = 6,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(inputImagePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputImagePath);
        ArgumentNullException.ThrowIfNull(boxEdges);

        if (strokeThickness < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(strokeThickness), "Stroke thickness must be greater than zero.");
        }

        using var image = await Image.LoadAsync<Rgba32>(inputImagePath, cancellationToken);

        var left = (int)Math.Clamp(Math.Round(boxEdges.Left * image.Width), 0, image.Width - 1);
        var top = (int)Math.Clamp(Math.Round(boxEdges.Top * image.Height), 0, image.Height - 1);
        var right = (int)Math.Clamp(Math.Round(boxEdges.Right * image.Width), 0, image.Width - 1);
        var bottom = (int)Math.Clamp(Math.Round(boxEdges.Bottom * image.Height), 0, image.Height - 1);

        if (right < left)
        {
            (left, right) = (right, left);
        }

        if (bottom < top)
        {
            (top, bottom) = (bottom, top);
        }

        var red = new Rgba32(255, 0, 0, 255);

        image.ProcessPixelRows(accessor =>
        {
            for (var stroke = 0; stroke < strokeThickness; stroke++)
            {
                var yTop = Math.Min(top + stroke, accessor.Height - 1);
                var yBottom = Math.Max(bottom - stroke, 0);
                var rowTop = accessor.GetRowSpan(yTop);
                var rowBottom = accessor.GetRowSpan(yBottom);

                for (var x = left; x <= right; x++)
                {
                    rowTop[x] = red;
                    rowBottom[x] = red;
                }
            }

            for (var stroke = 0; stroke < strokeThickness; stroke++)
            {
                var xLeft = Math.Min(left + stroke, accessor.Width - 1);
                var xRight = Math.Max(right - stroke, 0);

                for (var y = top; y <= bottom; y++)
                {
                    var row = accessor.GetRowSpan(y);
                    row[xLeft] = red;
                    row[xRight] = red;
                }
            }
        });

        await image.SaveAsJpegAsync(outputImagePath, cancellationToken);
    }
}