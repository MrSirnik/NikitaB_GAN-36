using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveScript : MonoBehaviour
{
    [SerializeField] private Transform _pointA;
    [SerializeField] private Transform _pointB;
    [SerializeField] private float duration;//продолжительность
    [SerializeField] private Ease changeCurve;

    void Start()
    {
        transform.DOMove(_pointA.position,
            Vector3.Distance(transform.position, _pointA.position) / duration).SetEase(changeCurve);//перемщение в начало анимации
    }

    void Update()
    {

        if (Vector3.Distance(transform.position, _pointA.position) < 0.2f)//достаточно ли близко объект к точке
        {
            transform.DOMove(_pointB.position, duration).SetEase(changeCurve);
            transform.DOScaleY(2f, 1f);//изменение формы
            transform.DOScaleZ(1f, 1f);
        }
        else if (Vector3.Distance(transform.position, _pointB.position) < 0.2f)
        {
            transform.DOMove(_pointA.position, duration).SetEase(changeCurve);
            transform.DOScaleY(1f, 1f);
            transform.DOScaleZ(2f, 1f);
        }
    }
}
