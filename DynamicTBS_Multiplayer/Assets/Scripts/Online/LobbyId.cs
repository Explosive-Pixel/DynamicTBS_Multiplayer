using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyId
{
    private int id = 0;
    private string name;

    public int Id { get { return id; } }
    public string Name { get { return name; } }
    public string FullId { get { return name + "#" + id; } }

    public bool IsNewLobby { get { return id == 0; } }

    public LobbyId(string name)
    {
        this.name = name;
    }

    public LobbyId(int id, string name)
    {
        this.id = id;
        this.name = name;
    }

    public override bool Equals(object obj)
    {
        if (obj is not LobbyId lobbyId)
        {
            return false;
        }

        return this.id == lobbyId.id && this.name == lobbyId.name;
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(id, name);
    }

    public override string ToString()
    {
        return FullId;
    }

    public static LobbyId FromFullId(string fullId)
    {
        try
        {
            string name = fullId.Split("#")[0];
            int id = Int32.Parse(fullId.Split("#")[1]);

            return new LobbyId(id, name);
        }
        catch(Exception)
        {
            return null;
        }
    }
}
