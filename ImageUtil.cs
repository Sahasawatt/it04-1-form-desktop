using System.Drawing;
using System.Text.RegularExpressions;

namespace IT04Form;

/// <summary>Converts between image files and "data:image/&lt;mime&gt;;base64,&lt;data&gt;" URLs.</summary>
public static class ImageUtil
{
    private static readonly Regex DataUrlRegex =
        new(@"^data:([^;]+);base64,(.+)$", RegexOptions.Singleline | RegexOptions.Compiled);

    /// <summary>Reads an image file and returns a data URL, e.g. "data:image/png;base64,iVBOR...".</summary>
    public static string ToDataUrl(string filePath)
    {
        var mime = Path.GetExtension(filePath).ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".webp" => "image/webp",
            _ => "image/png",
        };

        var base64 = Convert.ToBase64String(File.ReadAllBytes(filePath));
        return $"data:{mime};base64,{base64}";
    }

    /// <summary>Decodes a data URL back to an Image. Returns null when the string isn't a valid image data URL.</summary>
    public static Image? FromDataUrl(string dataUrl)
    {
        var match = DataUrlRegex.Match(dataUrl ?? "");
        if (!match.Success) return null;

        try
        {
            var bytes = Convert.FromBase64String(match.Groups[2].Value);
            using var stream = new MemoryStream(bytes);
            using var decoded = Image.FromStream(stream);
            // GDI+ requires the backing stream to stay alive for as long as the
            // Image from FromStream is used, but `stream`/`decoded` are disposed
            // when this method returns. Copy into a new Bitmap (deep pixel copy,
            // taken while stream/decoded are still alive) so the caller gets a
            // stream-independent bitmap instead of a dangling one.
            return new Bitmap(decoded);
        }
        catch (FormatException)
        {
            return null;
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    /// <summary>Returns the extension implied by a data URL's mime, e.g. "png"/"jpeg". Falls back to "png".</summary>
    public static string ExtensionFromDataUrl(string dataUrl)
    {
        var match = DataUrlRegex.Match(dataUrl ?? "");
        if (!match.Success) return "png";

        var mime = match.Groups[1].Value;
        var slashIndex = mime.IndexOf('/');
        return slashIndex >= 0 ? mime[(slashIndex + 1)..] : "png";
    }
}
