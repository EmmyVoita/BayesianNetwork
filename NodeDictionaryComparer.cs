using System.Collections.Generic;

public class NodeDictionaryComparer : IEqualityComparer<Dictionary<Node, bool>>
{
    public bool Equals(Dictionary<Node, bool> x, Dictionary<Node, bool> y)
    {
        if (x == null || y == null)
        {
            return x == null && y == null;
        }

        if (x.Count != y.Count)
        {
            return false;
        }

        foreach (var kvp in x)
        {
            if (!y.TryGetValue(kvp.Key, out bool value) || kvp.Value != value)
            {
                return false;
            }
        }

        return true;
    }

    public int GetHashCode(Dictionary<Node, bool> obj)
    {
        int hash = 17;
        foreach (var kvp in obj)
        {
            hash = hash * 31 + kvp.Key.GetHashCode() + (kvp.Value ? 1 : 0);
        }
        return hash;
    }
}
