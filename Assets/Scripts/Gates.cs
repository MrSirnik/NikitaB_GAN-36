using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gates : MonoBehaviour
{
    [SerializeField] private string _tag = "Ball";
    private int _score = 0;


    void OnTriggerStay(Collider other)
    {
        // ��������� �������� ����� ��� ��� ����
        if (other.CompareTag(_tag))
        {
            Destroy(other.gameObject);
            _score++;
            //� ������� ��������� ������� ������� ���� -->
            Debug.Log($"����� - {_score}");
        }
    }
}
