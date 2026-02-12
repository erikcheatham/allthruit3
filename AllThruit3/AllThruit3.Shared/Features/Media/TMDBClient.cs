namespace AllThruit3.Shared.Features.Media;

public class TMDBClient
{
    public HttpClient Client { get; }
    public TMDBClient(HttpClient client) => Client = client;
}
