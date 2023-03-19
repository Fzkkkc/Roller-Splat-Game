using System.Collections.Generic;
using UnityEngine;

public class LevelSettings : MonoBehaviour
{
    [Header("Level texture")] [SerializeField]
    private Texture2D _levelTexture;

    [Header("Tiles prefabs")] 
    [SerializeField] private GameObject _prefabWallTile;
    [SerializeField] private GameObject _prefabRoadTile;

    [Header("Ball and Road paint color")] public Color PaintColor;
    
    [HideInInspector] public List<RoadTile> RoadTilesList = new List<RoadTile>();
    [HideInInspector] public RoadTile DefaultBallRoadTile;

    private Color _colorWall = Color.white;
    private Color _colorRoad = Color.black;

    public float UnitPerPixel;

    private void Awake()
    {
        Generate();
        DefaultBallRoadTile = RoadTilesList[0];
    }

    private void Generate()
    {
        UnitPerPixel = _prefabWallTile.transform.lossyScale.x;
        float halfUnitPerPixel = UnitPerPixel / 2f;

        float width = _levelTexture.width;
        float height = _levelTexture.height;

        Vector3 offset = (new Vector3(width / 2f, 0f, height / 2f)) * UnitPerPixel
                         - new Vector3(halfUnitPerPixel, 0f, halfUnitPerPixel);

        CheckColor(offset);
    }

    private void CheckColor(Vector3 offset)
    {
        Color[] pixels = _levelTexture.GetPixels();
    
        for (int i = 0; i < pixels.Length; i++)
        {
            Color pixelColor = pixels[i];
            int x = i % _levelTexture.width;
            int y = i / _levelTexture.width;
            Vector3 spawnPos = ((new Vector3(x, 0f, y) * UnitPerPixel) - offset);

            if (pixelColor == _colorWall)
                Spawn(_prefabWallTile, spawnPos);
            else if (pixelColor == _colorRoad)
                Spawn(_prefabRoadTile, spawnPos);
        }
    }
    
    private void Spawn(GameObject prefabTile, Vector3 position)
    {
        position.y = prefabTile.transform.position.y;

        GameObject gameObject = Instantiate(prefabTile, position, Quaternion.identity, transform);
        
        if(prefabTile == _prefabRoadTile)
            RoadTilesList.Add(gameObject.GetComponent<RoadTile>());
    }
}
