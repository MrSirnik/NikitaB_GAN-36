using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour
{
    [SerializeField] private Vector3 _start; // Начальная точка
    [SerializeField] private Vector3 _end;   // Конечная точка
    [SerializeField] private float _speed;   // Скорость движения
    [SerializeField] private float _delay;   // Задержка в конечных точках

    // Итерационный метод Start
    public IEnumerator Start()
    {
        // Получаем физическое тело объекта
        Rigidbody rb = GetComponent<Rigidbody>();

        // Проверяем, есть ли компонент Rigidbody
        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing!");
            yield break;
        }

        // Настраиваем физическое тело как кинематическое
        rb.isKinematic = true;

        // Бесконечный цикл движения
        while (true)
        {
            // Движение от _start к _end
            yield return StartCoroutine(MoveToPosition(rb, _start, _end));

            // Пауза в конечной точке
            yield return new WaitForSeconds(_delay);

            // Движение от _end к _start
            yield return StartCoroutine(MoveToPosition(rb, _end, _start));

            // Пауза в начальной точке
            yield return new WaitForSeconds(_delay);
        }
    }

    // Вспомогательная корутина для движения к позиции
    private IEnumerator MoveToPosition(Rigidbody rb, Vector3 from, Vector3 to)
    {
        float distance = Vector3.Distance(from, to);
        float duration = distance / _speed;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.fixedDeltaTime;
            float t = elapsedTime / duration;

            // Плавное движение с использованием Lerp
            Vector3 newPosition = Vector3.Lerp(from, to, t);

            // Перемещаем кинематическое физическое тело
            rb.MovePosition(newPosition);

            // Ждем следующий физический кадр
            yield return new WaitForFixedUpdate();
        }

        // Гарантируем точное позиционирование в конечной точке
        rb.MovePosition(to);
    }
}
