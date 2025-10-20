using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gates : MonoBehaviour
{
    [SerializeField] private string _tag = "Ball";
    private int _score = 0;


    void OnTriggerEnter(Collider other)
    {
        // ѕровер€ем материал через тег или слой
        if (other.CompareTag(_tag))
        {
            Destroy(other.gameObject);
            _score++;
            //в консоль выводитс€ текущий игровой счет -->
            Debug.Log($"√олов - {_score}");
        }
    }
}
