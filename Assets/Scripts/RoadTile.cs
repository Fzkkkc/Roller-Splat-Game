using UnityEngine;

public class RoadTile : MonoBehaviour
{
    public MeshRenderer MeshRenderer;
    public Vector3 Position;
    public bool IsPainted;

    private void Awake()
    {
        Position = transform.position;
        IsPainted = false;
    }
}
