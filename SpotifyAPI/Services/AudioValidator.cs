using NAudio.Wave;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Services;

public class AudioValidator : IAudioValidator
{
    public bool IsValidAudio(IFormFile audio)
    {
        using var stream = audio.OpenReadStream();

        try
        {
            using var mp3Reader = new Mp3FileReader(stream);
            var waveFormat = mp3Reader.WaveFormat;

            return waveFormat != null;
        }
        catch
        {
            return false;
        }
    }

    public bool IsValidNullPossibleAudio(IFormFile? audio)
    {
        if (audio is null)
            return true;

        return IsValidAudio(audio);
    }

    public bool IsValidAudios(IEnumerable<IFormFile> audios)
    {
        return audios.All(IsValidAudio);
    }

    public bool IsValidNullPossibleAudios(IEnumerable<IFormFile>? audios)
    {
        if (audios is null)
            return true;

        return IsValidAudios(audios.Where(a => a is not null));
    }
}