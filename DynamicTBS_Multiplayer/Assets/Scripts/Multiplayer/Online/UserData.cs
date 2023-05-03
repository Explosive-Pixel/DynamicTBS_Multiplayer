using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public class UserData
{
    private string name;
    public string Name { get { return name; } }

    private ClientType role;
    public ClientType Role { get { return role; } }

    public UserData(string name, ClientType role)
    {
        this.name = name;
        this.role = role;
    }

    public void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteFixedString32(name);
        writer.WriteInt((int)role);
    }

    public static UserData Deserialize(DataStreamReader reader)
    {
        return new UserData(
            reader.ReadFixedString32().ToString(),
            (ClientType)reader.ReadInt()
        );
    }
}
