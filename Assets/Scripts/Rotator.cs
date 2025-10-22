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

        // Устанавливаем тело как кинематическое (если еще не установлено)
        rb.isKinematic = true;

        // Сохраняем начальное вращение
        Quaternion startRotation = rb.rotation;

        // Бесконечный цикл для вращения в каждом кадре
        while (true)
        {
            // Вращаем ОТНОСИТЕЛЬНО начального вращения, чтобы не конфликтовать с движением
            Quaternion currentRotation = rb.rotation;
            Quaternion deltaRotation = Quaternion.Euler(_rotate * Time.deltaTime);

            // Применяем вращение
            rb.MoveRotation(currentRotation * deltaRotation);

            // Ждем следующий кадр
            yield return null;
        }
    }
}