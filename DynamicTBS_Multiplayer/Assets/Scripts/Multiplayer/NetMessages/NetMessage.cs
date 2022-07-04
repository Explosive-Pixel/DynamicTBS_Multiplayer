using Unity.Networking.Transport;

public class NetMessage
{
    public OperationCode Code { set; get; }

    public virtual void Serialize(ref DataStreamWriter writer) // Packaging message for sending.
    {
        writer.WriteByte((byte)Code);
    }

    public virtual void Deserialize(DataStreamReader reader) // Unpackaging and distributing message after receiving.
    {

    }

    public virtual void ReceivedOnClient() // Called when message is received on client side.
    {

    }

    public virtual void ReceivedOnServer(NetworkConnection cnn) // Called when message is received on server side, with identifier from whom it was sent.
    {

    }
}