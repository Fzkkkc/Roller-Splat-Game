using System.Collections.Generic;
using System.Linq;
using SwipeController;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class BallMovement : MonoBehaviour
{
    [SerializeField] private SwipeListener _swipeListener;
    [SerializeField] private LevelSettings _levelSettings;

    [SerializeField] private float _stepDuration = 0.1f;
    [SerializeField] private LayerMask _wallsAndRoadsLayer;
    private const float MAX_RAY_DISTANCE = 10f;

    private Vector3 _moveDirection;
    private bool _canMove = true;

    public UnityAction<List<RoadTile>, float> OnMoveStart;
    private List<RoadTile> _pathRoadTilesList = new List<RoadTile>();
    
    private void Start()
    {
        ChangeDefaultBallPosition();
        AddListeners();
    }

    private void ChangeDefaultBallPosition() =>
        transform.position = _levelSettings.DefaultBallRoadTile.Position;

    private void AddListeners()
    {
        _swipeListener.OnSwipe.AddListener(swipe =>
        {
            switch (swipe)
            {
                case "Right":
                    _moveDirection = Vector3.right;
                    break;
                case "Left":
                    _moveDirection = Vector3.left;
                    break;
                case "Up":
                    _moveDirection = Vector3.forward;
                    break;
                case "Down":
                    _moveDirection = Vector3.back;
                    break;
            }
            MoveBall();
        });
    }
    
    private void MoveBall()
    {
        if (_canMove)
        {
            _canMove = false;
            RaycastHit[] hits = Physics.RaycastAll(transform.position, _moveDirection,
                MAX_RAY_DISTANCE, _wallsAndRoadsLayer.value).OrderBy(hit => hit.distance).ToArray(); //raycast in the direction

            Vector3 targetPosition = transform.position;
            
            int steps = 0;

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.isTrigger)
                {
                    _pathRoadTilesList.Add(hits[i].transform.GetComponent<RoadTile>());
                }
                else
                {
                    if (i == 0)
                    {
                        _canMove = true;
                        return;
                    }

                    steps = i;
                    targetPosition = hits[i - 1].transform.position;
                    break;
                }
            }

            MoveBallToTargetPosition(targetPosition, steps);
        }
    }

    private void MoveBallToTargetPosition(Vector3 targetPosition, int steps)
    {
        float moveDuration = _stepDuration * steps;
        transform
            .DOMove(targetPosition, moveDuration)
            .SetEase(Ease.OutExpo)
            .OnComplete(() => _canMove = true);
        
        if(OnMoveStart != null)
            OnMoveStart.Invoke(_pathRoadTilesList, moveDuration);
    }
}