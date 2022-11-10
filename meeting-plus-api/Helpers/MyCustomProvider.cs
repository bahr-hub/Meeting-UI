using Microsoft.AspNetCore.SignalR;

public class MyCustomProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        
        
        return connection.User.Identity.Name;
    }
}