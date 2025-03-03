using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector3 worldPosition; // Position of the node in the world
    public List<Node> neighbours; // Connected nodes (roads)

    public Node(Vector3 _worldPosition)
    {
        worldPosition = _worldPosition;
        neighbours = new List<Node>();
    }

    public int gridX, gridY; // Grid koordinatlarý
    public int gCost; // Baþlangýç noktasýndan bu noktaya olan maliyet
    public int hCost; // Bu noktadan hedef noktaya olan tahmini maliyet (heuristic)
    public Node parent; // Yolu takip etmek için ebeveyn düðüm

    public Node(Vector3 _worldPos, int _gridX, int _gridY)
    {
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int fCost // Toplam maliyet
    {
        get { return gCost + hCost; }
    }
}