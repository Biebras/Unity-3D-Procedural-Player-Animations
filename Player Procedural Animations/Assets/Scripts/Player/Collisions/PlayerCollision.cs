using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PlayerSides
{
    public Vector3 BotPos, TopPos;
    public Vector3 ForwardPos, BackPos;
    public Vector3 RightPos, LeftPos;
    public Vector3 Center;
}

public class PlayerCollision : MonoBehaviour
{
    public CollisionInfo BumperCollider;
    public CollisionInfo GroundCollider;
    [SerializeField] private LayerMask _obstacleMask;

    private PlayerSides _playerSides;
    private Bounds _bounds;
    private float _highestYPoint;

    private Transform _transform;
    private Collider _collider;

    private void Awake()
    {
        _transform = transform;
        _collider = GetComponent<Collider>();
    }

    public void HandleCollisions(Vector3 nextPos, ref Vector3 move)
    {
        UpdatePlayerSidePositions();

        HandleBumperCollider(nextPos, ref move);
        HandleGroundCollider(nextPos, ref move);
    }

    private void HandleBumperCollider(Vector3 nextPos, ref Vector3 move)
    {
        var player = _playerSides;
        var checkPos = BumperCollider.GetPos(nextPos);

        var colliders = Physics.OverlapSphere(checkPos, BumperCollider.radius, _obstacleMask);

        if (colliders.Length == 0)
            return;

        var direction = GetXAndZDirection(move);
        var offsetPlayer =  - (player.RightPos.x - player.Center.x);
        var hitPoint = colliders[0].ClosestPointOnBounds(player.Center);

        hitPoint.x += BumperCollider.radius;

        var pos = _transform.position;
        pos.x = hitPoint.x;
        _transform.position = pos;
        //var gap = hitPoint - nextPos;
    }

    private void HandleGroundCollider(Vector3 nextPos, ref Vector3 move)
    {
        var player = _playerSides;
        var checkPos = GroundCollider.GetPos(nextPos);

        var colliders = Physics.OverlapSphere(checkPos, GroundCollider.radius, _obstacleMask);

        if (colliders.Length == 0)
            return;

        var highestPoint = colliders[0].ClosestPointOnBounds(player.Center);

        for (int i = 1; i < colliders.Length; i++)
        {
            var closestHitPoint = colliders[i].ClosestPointOnBounds(player.Center);

            if (closestHitPoint.y > highestPoint.y)
                highestPoint = closestHitPoint;
        }

        var gap = highestPoint - nextPos;

        move.y += gap.y;
    }

    private void UpdatePlayerSidePositions()
    {
        UpdateBounds();

        _playerSides.TopPos = new Vector3(_bounds.center.x, _bounds.max.y, _bounds.center.z);
        _playerSides.BotPos = new Vector3(_bounds.center.x, _bounds.min.y, _bounds.center.z);
        _playerSides.RightPos = new Vector3(_bounds.max.x, _bounds.center.y, _bounds.center.z);
        _playerSides.LeftPos = new Vector3(_bounds.min.x, _bounds.center.y, _bounds.center.z);
        _playerSides.ForwardPos = new Vector3(_bounds.center.x, _bounds.center.y, _bounds.max.z);
        _playerSides.BackPos = new Vector3(_bounds.center.x, _bounds.center.y, _bounds.min.z);
        _playerSides.Center = _bounds.center;
    }

    private Vector3 GetXAndZDirection(Vector3 movement)
    {
        var xDirection = movement.z == 0 ? 0 : movement.z > 0 ? 1 : 0;
        var zDirection = movement.z == 0 ? 0 : movement.z > 0 ? 1 : 0;

        return new Vector3(xDirection, 0, zDirection);
    }

    private void UpdateBounds()
    {
        _bounds = _collider.bounds;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = BumperCollider.gizmosColor;
        Gizmos.DrawWireSphere(BumperCollider.GetPos(transform.position), BumperCollider.radius);

        Gizmos.color = GroundCollider.gizmosColor;
        Gizmos.DrawWireSphere(GroundCollider.GetPos(transform.position), GroundCollider.radius);
    }
}
