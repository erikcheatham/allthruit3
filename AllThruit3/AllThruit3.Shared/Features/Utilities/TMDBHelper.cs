namespace AllThruit3.Shared.Features.Utilities;

public static class TMDBHelper
{
    private const string BaseImageUrl = "https://image.tmdb.org/t/p/";

    public static string? GetImageUrl(string? path, string size = "w500")
    {
        if (string.IsNullOrEmpty(path))
        {
            return "images/placeholder.png";  // Fallback to a local asset in your Maui project (add to Resources/Images)
        }
        return $"{BaseImageUrl}{size}{path}";
    }
}
