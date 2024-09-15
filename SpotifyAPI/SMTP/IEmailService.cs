namespace SpotifyAPI.SMTP;

public interface IEmailService
{
    Task SendAsync(Message messageData);
}