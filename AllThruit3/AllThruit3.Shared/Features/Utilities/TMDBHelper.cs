namespace AllThruit3.Shared.Features.Utilities;

public static class TMDBHelper
{
    private const string BaseImageUrl = "https://image.tmdb.org/t/p/";

    public static string? GetImageUrl(string? path, string size = "w500")
    {
        if (string.IsNullOrEmpty(path))
        {
            // Use TMDB's default no-image placeholder (external URL – reliable and no local file needed)
            return size switch
            {
                "w500" => "https://via.placeholder.com/500x750?text=No+Poster", // Or TMDB's actual no-poster: "https://image.tmdb.org/t/p/w500/wwemzKWzjKYJFfCeiB57q3r4Bcm.png" (generic)
                "w1280" => "https://via.placeholder.com/1280x720?text=No+Backdrop",
                _ => "https://via.placeholder.com/300x450?text=No+Image"
            };
            // Alternative: Local fallback – ensure file exists in AllThruit3.Web/wwwroot/images/placeholder.png (or AllThruit3.Shared/wwwroot for shared bundling)
            // return "_content/AllThruit3.Shared/images/placeholder.png"; // Blazor static asset path (if bundled in Shared)
        }
        // Trim leading '/' if present (TMDB paths already include it, but defensive)
        path = path.TrimStart('/');
        return $"{BaseImageUrl}{size}/{path}";
    }
}