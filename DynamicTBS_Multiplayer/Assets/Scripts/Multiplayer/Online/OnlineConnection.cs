using Unity.Networking.Transport;

public record OnlineConnection
{
    public NetworkConnection NetworkConnection { get; set; }
    public string Name { get; set; }
    public ClientType Role { get; set; }
    public bool IsAdmin { get; set; }
    public PlayerType? Side { get; set; }

    public OnlineConnection(NetworkConnection cnn, UserData userData)
    {
        this.NetworkConnection = cnn;
        this.Name = userData.Name;
        this.Role = userData.Role;

        IsAdmin = false;
    }
}
