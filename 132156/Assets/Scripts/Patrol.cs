using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    public bool ShouldPatrol = true;

    [Header("Main Patrol Settings")]
    public int PatrolPoints;
    public float PatrolRadius;
    public float PatrolSpeed;
    public float WaitAtPositionTime = 1.0f;
    public float ChangePatrolPointsTime = 10.0f;

    private Vector2 _origin = new Vector2();
    private List<Vector2> _patrolPoints = new List<Vector2>();
    private int _currentPositionIndex = 0;

    private float _changePatrolPointsTimer = 0.0f;
    private float _waitAtPositiontimer = 0.0f;
    private float _destinationOffset = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        _origin = transform.position;
        SetNewPatrolPoints(PatrolPoints);
    }

    // Update is called once per frame
    void Update()
    {
        if (ShouldPatrol)
        { 
            if (Time.time >= _changePatrolPointsTimer)
            {
                _changePatrolPointsTimer += ChangePatrolPointsTime;
                SetNewPatrolPoints(PatrolPoints);
            }

            if (Time.time >= _waitAtPositiontimer)
                PatrolBetweenPositions();
        }
    }

    public void PatrolBetweenPositions()
    {
        Vector2 nextPosition = _patrolPoints[_currentPositionIndex];

        bool isFacingRight = (nextPosition - (Vector2)transform.position).normalized.x > 0.0f;

        FlipSprite(isFacingRight);

        float distance = Vector2.Distance(transform.position, nextPosition);

        if (distance >= _destinationOffset)
        {
            transform.position = Vector2.MoveTowards(transform.position, nextPosition, PatrolSpeed * Time.deltaTime);
        }
        else
        {
            _currentPositionIndex++;
            _waitAtPositiontimer = Time.time + WaitAtPositionTime;

            if (_currentPositionIndex >= _patrolPoints.Count)
            {
                _currentPositionIndex = 0;
            }
        }

        _changePatrolPointsTimer = Time.time + ChangePatrolPointsTime;
    }

    public void SetNewPatrolPoints(int numberOfPoints)
    {
        for (int i = 0; i < numberOfPoints; i++)
        {
            Vector2 randomPosition = GenerateRandomPosition(PatrolRadius);
            _patrolPoints.Add(randomPosition);
        }
    }

    public Vector2 GenerateRandomPosition(float radiusAroundTarget)
    {
        float randomX = Random.Range(_origin.x - radiusAroundTarget, _origin.x + radiusAroundTarget);
        float randomY = Random.Range(_origin.y - radiusAroundTarget, _origin.y + radiusAroundTarget);

        Vector2 position = new Vector2(randomX, randomY);

        return position;
    }

    private void FlipSprite(float direction)
    {
        Vector3 localScale = transform.localScale;
        if (direction > 0.0f)
        {
            localScale.x = 1;
            transform.localScale = localScale;
        }
        else if (direction < 0.0f)
        {
            localScale.x = -1;
            transform.localScale = localScale;
        }
    }

    private void FlipSprite(bool isFacingRight)
    {
        GetComponent<SpriteRenderer>().flipX = !isFacingRight;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, PatrolRadius);
    }
}
