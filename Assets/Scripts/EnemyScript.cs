using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FildOfView))]
public class EnemyScript : Editor
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

    void Update()
    {
        //Handles.Circle.Draw
    }
}
