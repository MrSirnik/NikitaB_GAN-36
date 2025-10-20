using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour
{
    [SerializeField] private Vector3 _rotate; // Поле для настройки вращения через инспектор

    // Итерационный метод Start
    public IEnumerator Start()
    {
        // Получаем физическое тело объекта
        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing!");
            yield break;
        }

        // Устанавливаем тело как кинематическое
        rb.isKinematic = true;

        // Бесконечный цикл для вращения в каждом кадре
        while (true)
        {
            // Выполняем вращение кинематического тела
            rb.MoveRotation(rb.rotation * Quaternion.Euler(_rotate * Time.deltaTime));

            // Ждем следующий кадр
            yield return null;
        }
    }
}