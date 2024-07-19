﻿using Model.Entities;

namespace SpotifyAPI.Models.Track;

public class TrackUpdateVm
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public IFormFile Audio { get; set; } = null!;

    public long AlbumId { get; set; }
}