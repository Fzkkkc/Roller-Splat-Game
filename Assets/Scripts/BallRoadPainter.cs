using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BallRoadPainter : MonoBehaviour
{
    [SerializeField] private LevelSettings _levelSettings;
    [SerializeField] private BallMovement _ballMovement;
    [SerializeField] private MeshRenderer _ballMeshRenderer;

    public int PaintedRoadTiles = 0;

    private void Start()
    {
        PaintBall();
        Paint(_levelSettings.DefaultBallRoadTile, 0.5f, 0f);
        PainBallRoad();
    }

    private void PaintBall() =>
        _ballMeshRenderer.material.color = _levelSettings.PaintColor;
    
    private void Paint(RoadTile roadTile, float duration, float delay)
    {
        roadTile.MeshRenderer.material
            .DOColor(_levelSettings.PaintColor, duration)
            .SetDelay(delay);

        roadTile.IsPainted = true;
        PaintedRoadTiles++;
    }
    
    private void PainBallRoad() =>
        _ballMovement.OnMoveStart += OnBallMoveStart;
    
    private void OnBallMoveStart(List<RoadTile> roadTiles, float totalDuration)
    {
        float stepDuration = totalDuration / roadTiles.Count;
        for (int i = 0; i < roadTiles.Count; i++)
        {
            RoadTile roadTile = roadTiles[i];
            if (!roadTile.IsPainted)
            {
                float duration = totalDuration / 2f;
                float delay = i * (stepDuration / 2f);
                Paint(roadTile, duration, delay);

                if (PaintedRoadTiles == _levelSettings.RoadTilesList.Count)
                {
                    LoadNextScene();
                }
            }
        }
    }
    
    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        
        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;
        
        SceneManager.LoadScene(nextSceneIndex);
    }
}
