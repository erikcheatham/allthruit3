using System;
using System.Collections.Generic;
using System.Text;

namespace AllThruit3.Shared.Configuration;

public class AppSettings
{
    public string TMDBUrl { get; set; } = "https://api.themoviedb.org/3/";
    public string TMDBBearerToken { get; set; } = "eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiJjZWJiMWRmMDk3MjY1MDljMTdjZDBjNjIxZDU0MDkwYSIsIm5iZiI6MTc2MDg5NTMwMS4wMzksInN1YiI6IjY4ZjUyMTQ1NDI5NmNmMjRiNmY5OWY2MSIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.5j0B4CHgvIFrgeLJLV8DddIrQD5l7ObDbo3u5hxqnlM";
    public string ApiBaseUrl { get; set; } = "https://localhost:7199/";
}