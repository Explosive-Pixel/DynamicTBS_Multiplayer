using System;
using System.Collections.Generic;

public class RecentMessageIds
{
    private readonly int maxSize;
    private readonly HashSet<string> set = new();
    private readonly Queue<string> queue = new();

    public RecentMessageIds(int maxSize)
    {
        this.maxSize = maxSize;
    }

    /// <summary>
    /// Fügt eine Message-ID hinzu.
    /// Gibt false zurück, wenn sie bereits vorhanden war.
    /// </summary>
    public bool Add(string id)
    {
        if (set.Contains(id))
            return false;

        set.Add(id);
        queue.Enqueue(id);

        if (queue.Count > maxSize)
        {
            string oldest = queue.Dequeue();
            set.Remove(oldest);
        }

        return true;
    }

    public bool Contains(string id)
    {
        return set.Contains(id);
    }

    public void Clear()
    {
        set.Clear();
        queue.Clear();
    }

    public int Count => set.Count;
}
