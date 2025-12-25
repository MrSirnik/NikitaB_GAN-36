using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyScript))]
public class RotatableBeamEditor : Editor
{
    private void OnSceneGUI()
    {
        EnemyScript enemy = target as EnemyScript;
        if (enemy == null) return;

        // Рисуем луч
        Vector3 direction = Quaternion.Euler(0, enemy.horizontalAngle, 0) * Vector3.forward;
        Vector3 endPoint = enemy.transform.position + direction * enemy._viewingRadius;

        Handles.color = enemy.handleColor;
        Handles.DrawLine(enemy.transform.position, endPoint, 3f);

        // 1. Ручка для изменения угла (на окружности)
        float radius = enemy._viewingRadius * 0.7f;
        Vector3 angleHandlePos = enemy.transform.position + direction * radius;

        EditorGUI.BeginChangeCheck();
        Vector3 newAnglePos = Handles.FreeMoveHandle(
            angleHandlePos,
            Quaternion.identity,
            0.2f,
            Vector3.zero,
            Handles.CircleHandleCap
        );

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(enemy, "Change Angle");
            Vector3 offset = newAnglePos - enemy.transform.position;
            enemy.horizontalAngle = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;
        }

        // 2. Ручка для изменения радиуса (на конце луча)
        EditorGUI.BeginChangeCheck();
        Vector3 newRadiusPos = Handles.FreeMoveHandle(
            endPoint,
            Quaternion.identity,
            0.2f,
            Vector3.zero,
            Handles.SphereHandleCap
        );

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(enemy, "Change Radius");
            enemy._viewingRadius = Vector3.Distance(enemy.transform.position, newRadiusPos);
        }

        // Отображение информации
        Handles.Label(enemy.transform.position + Vector3.up * 1.5f,
            $"Angle: {enemy.horizontalAngle:F1}°\nRadius: {enemy._viewingRadius:F1}");

        // Рисуем окружность радиуса
        Handles.color = new Color(enemy.handleColor.r, enemy.handleColor.g, enemy.handleColor.b, 0.1f);
        Handles.DrawSolidDisc(enemy.transform.position, Vector3.up, enemy._viewingRadius);

        Handles.color = enemy.handleColor;
        Handles.DrawWireDisc(enemy.transform.position, Vector3.up, enemy._viewingRadius);
    }
}

public class EnemyScript : MonoBehaviour
{
    [SerializeField] private Transform _positionTracking;
    [SerializeField, Range(0f, 360f)] public float _viewingRadius = 10f;
    [SerializeField] private Player _player;

    public Color handleColor = new Color(1, 0, 0, 0.5f);
    [Range(0, 360)] public float horizontalAngle = 0f;
    [Range(1, 180)] public float fieldOfView = 60f; // Добавил FOV

    void OnDrawGizmos()
    {
        Gizmos.color = handleColor;

        // Основной луч
        Vector3 direction = Quaternion.Euler(0, horizontalAngle, 0) * Vector3.forward;
        Gizmos.DrawRay(transform.position, direction * _viewingRadius);

        // Сектор обзора
        float halfFOV = fieldOfView / 2f;
        Vector3 leftDir = Quaternion.Euler(0, horizontalAngle - halfFOV, 0) * Vector3.forward;
        Vector3 rightDir = Quaternion.Euler(0, horizontalAngle + halfFOV, 0) * Vector3.forward;

        Gizmos.DrawLine(transform.position, transform.position + leftDir * _viewingRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightDir * _viewingRadius);

        // Дуга сектора
        int segments = 20;
        Vector3 prevPoint = transform.position + leftDir * _viewingRadius;

        for (int i = 0; i <= segments; i++)
        {
            float angle = horizontalAngle - halfFOV + (fieldOfView * i / segments);
            Vector3 point = transform.position +
                Quaternion.Euler(0, angle, 0) * Vector3.forward * _viewingRadius;

            Gizmos.DrawLine(prevPoint, point);
            prevPoint = point;
        }
    }
}