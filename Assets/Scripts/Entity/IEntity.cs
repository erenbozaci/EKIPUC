using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityState
{
    Idle,
    Running,
    Jumping,
    Falling,
    Dashing
}

public interface IEntity 
{
    void TakeDamage(int damage);
    void Heal(int amount);
    void Die();
    void Move(Vector3 direction);
    void Attack(Vector3 targetPosition);
}
