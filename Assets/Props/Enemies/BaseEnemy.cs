using System;
using UnityEngine;
using UnityEngine.Events;

public class BaseEnemy : MonoBehaviour
{
    public enum EnemyTypes // TODO : Replace this by real Enemy class
    {
        LesFDPQuiCourts,
        LesFDPQuiTirent,
        LesDronesDeFDP
    }

    [SerializeField] private EnemyTypes _enemyType;
    public EnemyTypes EnemyType => _enemyType;

    public event UnityAction OnDeathCallback;

    virtual protected void OnDestroy()
    {
        OnDeathCallback?.Invoke();
    }
}
