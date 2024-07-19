namespace SpotifyAPI.Services.Interfaces;

public interface IAudioValidator
{
    bool IsValidAudio(IFormFile audio);
    bool IsValidNullPossibleAudio(IFormFile? audio);
    bool IsValidAudios(IEnumerable<IFormFile> audios);
    bool IsValidNullPossibleAudios(IEnumerable<IFormFile>? audios);
}