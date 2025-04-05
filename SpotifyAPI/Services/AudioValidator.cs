using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Services;

public class AudioValidator : IAudioValidator
{
    public bool IsValidAudio(IFormFile audio)
    {
        string tempFile = Path.GetTempFileName();
        try
        {
            using (var stream = audio.OpenReadStream())
            using (var fileStream = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(fileStream);
            }

            var tfile = TagLib.File.Create(tempFile);
            return tfile.Properties.Duration.TotalSeconds > 0;
        }
        catch
        {
            return false;
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
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