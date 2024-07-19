using Model.Entities;
using NAudio.Wave;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Services;

public class AudioService(
    IConfiguration _configuration
    ) : IAudioService
{

    public async Task<string> SaveAudioAsync(IFormFile audio)
    {
        using MemoryStream ms = new();
        await audio.CopyToAsync(ms);
        string fileName = await SaveAudioAsync(ms.ToArray());
        return fileName;
    }

    public async Task<List<string>> SaveAudiosAsync(IEnumerable<IFormFile> audios)
    {
        List<string> result = new();

        try
        {
            foreach (var audio in audios)
            {
                result.Add(await SaveAudioAsync(audio));
            }
        }
        catch (Exception)
        {
            result.ForEach(DeleteAudioIfExists);
            throw;
        }

        return result;
    }

    public async Task<string> SaveAudioAsync(byte[] bytes)
    {
        string audioName = $"{Path.GetRandomFileName()}.mp3";

        string path = Path.Combine(AudioDir, audioName);
        await File.WriteAllBytesAsync(path, bytes);

        return audioName;
    }

    public async Task<List<string>> SaveAudiosAsync(IEnumerable<byte[]> bytesArrays)
    {
        List<string> result = new();

        try
        {
            foreach (var bytes in bytesArrays)
            {
                result.Add(await SaveAudioAsync(bytes));
            }
        }
        catch (Exception)
        {
            result.ForEach(DeleteAudioIfExists);
            throw;
        }

        return result;
    }

    public async Task<byte[]> LoadBytesAsync(string name)
    {
        return await File.ReadAllBytesAsync(Path.Combine(AudioDir, name));
    }

    public string AudioDir => Path.Combine(
        Directory.GetCurrentDirectory(),
        _configuration["DataDir"] ?? throw new NullReferenceException("DataDir"),
        _configuration["AudioDir"] ?? throw new NullReferenceException("Audio")
    );

    public void DeleteAudio(string nameWithFormat)
    {
        File.Delete(Path.Combine(AudioDir, nameWithFormat));
    }

    public void DeleteAudios(IEnumerable<string> audios)
    {
        foreach (var audio in audios)
            DeleteAudio(audio);
    }

    public void DeleteAudioIfExists(string nameWithFormat)
    {
        string path = Path.Combine(AudioDir, nameWithFormat);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public void DeleteAudiosIfExists(IEnumerable<string> audios)
    {
        foreach (var audio in audios)
            DeleteAudioIfExists(audio);
    }

    public int GetAudioDuration(string name)
    {
        using var reader = new Mp3FileReader(Path.Combine(AudioDir, name));
        return (int)reader.TotalTime.TotalSeconds;
    }
}