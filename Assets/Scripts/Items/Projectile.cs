using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    OwnerTypeSO owner;

    [SerializeField]
    ProjectileStatsSO stats;

    [SerializeField]
    Rigidbody2D body;

    [SerializeField]
    DisableAfterTime disabler;

    public OwnerTypeSO Owner { get => owner; set => owner = value; }

    [Header("Owners")]
    [SerializeField]
    OwnerTypeSO playerOwner;
    [SerializeField]
    OwnerTypeSO neutralOwner;
    [SerializeField]
    OwnerTypeSO enemyOwner;

    [Header("Directions")]
    [SerializeField]
    DirectionSO up;
    [SerializeField]
    DirectionSO down;
    [SerializeField]
    DirectionSO left;
    [SerializeField]
    DirectionSO right;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    private void OnEnable()
    {
        disabler.SecondsToWait = stats.Lifetime;
    }

    void DisableIfNotPiercing()
    {
        if (!stats.IsPiercing)
        {
            DisableProjectile();
        }
    }

    public void DisableProjectile()
    {
        disabler.CallDisabledEvent();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Check that should be run on player-owned projectile
    /// </summary>
    /// <param name="other"></param>
    void PlayerCollisionCheck(Collider2D other)
    {
        Enemy enemy = other.gameObject.GetComponent<Enemy>();

        if (enemy)
        {
            enemy.TakeDamage(stats.Damage);
            DisableIfNotPiercing();
        }
    }

    /// <summary>
    /// Check that should be run on enemy-owned projectile
    /// </summary>
    /// <param name="other"></param>
    void EnemyCollisionCheck(Collider2D other)
    {
        Player player = other.gameObject.GetComponent<Player>();

        if (player)
        {
            player.TakeDamage();
            DisableIfNotPiercing();
        }
    }

    /// <summary>
    /// Check that should be run on neutral-owned projectile
    /// </summary>
    /// <param name="other"></param>
    void NeutralCollisionCheck(Collider2D other)
    {
        EnemyCollisionCheck(other);
        if (gameObject.activeInHierarchy)
        {
            PlayerCollisionCheck(other);
        }
    }

    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (owner == playerOwner)
        {
            PlayerCollisionCheck(other);
        }
        else if (owner == neutralOwner)
        {
            NeutralCollisionCheck(other);
        }
        else if (owner == enemyOwner)
        {
            EnemyCollisionCheck(other);
        }
    }

    /// <summary>
    /// Fires in one of the given directions
    /// </summary>
    /// <param name="directionSO"></param>
    public void SetDirection(DirectionSO directionSO)
    {
        if (directionSO == up)
        {
            body.velocity = new Vector2(0, stats.Speed);
        }
        else if (directionSO == down)
        {
            body.velocity = new Vector2(0, -stats.Speed);
        }
        else if (directionSO == left)
        {
            body.velocity = new Vector2(-stats.Speed, 0);
        }
        else if (directionSO == right)
        {
            body.velocity = new Vector2(stats.Speed, 0);
        }
    }

    /// <summary>
    /// Fires in the given direction
    /// </summary>
    /// <param name="direction"></param>
    public void SetDirection(Vector2 direction)
    {
        direction *= stats.Speed;
        body.velocity = direction;
    }
}
