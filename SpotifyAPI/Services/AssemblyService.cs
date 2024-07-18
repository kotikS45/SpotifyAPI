using System.Reflection;

namespace SpotifyAPI.Services;

public static class AssemblyService
{
    public static string GetAssemblyName()
    {
        return Assembly.GetExecutingAssembly().GetName().Name
            ?? throw new NullReferenceException("AssemblyName");
    }
}
