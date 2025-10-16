using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gates : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Проверяем материал через тег или слой
        if (other.CompareTag("Ball"))
        {
            Destroy(other.gameObject);
            //в консоль выводится текущий игровой счет -->
        }
    }
}
