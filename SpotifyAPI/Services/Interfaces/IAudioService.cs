using NAudio.Wave;

namespace SpotifyAPI.Services.Interfaces;

public interface IAudioService
{
    Task<string> SaveAudioAsync(IFormFile audio);
    Task<List<string>> SaveAudiosAsync(IEnumerable<IFormFile> audios);
    Task<string> SaveAudioAsync(byte[] bytes);
    Task<List<string>> SaveAudiosAsync(IEnumerable<byte[]> bytesArrays);
    Task<byte[]> LoadBytesAsync(string name);
    void DeleteAudio(string nameWithFormat);
    void DeleteAudios(IEnumerable<string> audios);
    void DeleteAudioIfExists(string nameWithFormat);
    void DeleteAudiosIfExists(IEnumerable<string> audios);
    int GetAudioDuration(string name);

    string AudioDir { get; }
}