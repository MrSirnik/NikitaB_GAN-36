using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [Tooltip("Отслеживание позиции и ориентации противника")]
    [SerializeField]
    private Transform _positionTracking;
    [Tooltip("Радиус обзора (поле зрение) противника")]
    [SerializeField, Range(0, 360)]
    private float _viewingRadius = 110f;
    [Tooltip("Центр обзора противника")]
    [SerializeField]
    private Vector3 _centerObzora;
    [SerializeField]
    private Player _player;
    [SerializeField]
    private float _rangeVisibility = 10f;


    void Start()
    {

    }

    void Update()
    {

    }
}
