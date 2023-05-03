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

    public void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteFixedString32(name);
        writer.WriteInt((int)role);
    }

    public static UserData Deserialize(DataStreamReader reader)
    {
        return new UserData
        {
            name = reader.ReadFixedString32().ToString(),
            role = (ClientType)reader.ReadInt()
        };
    }
}
