﻿namespace SpotifyAPI.Models.Genre;

public class GenreVm
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
}
/*
namespace SpotifyAPI.Models.Genre
{
    public class GenreCreateVm
    {
        public string Name { get; set; } = null!;
    }

    public class GenreUpdateVm
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
    }

    public class GenreVm
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
    }
}

*/
