//using System;
//using UnityEngine;

//namespace DefaultNamespace
//{
//	[RequireComponent(typeof(PositionSaver))]
//	public class ReplayMover : MonoBehaviour
//	{
//		private PositionSaver _save;

//		private int _index = 0;
//		private PositionSaver.Data _prev;
//		private float _duration;

//        [SerializeField, Range(0.1f, 1f)]
//        private float _speed = 0.5f; // Коэффициент замедления
//        private Vector3 _currentPosition;

//        private void Start()
//		{
//            ////todo comment: зачем нужны эти проверки?
//            //это защитные проверки
//            if (!TryGetComponent(out _save) || _save.Records.Count == 0)
//			{
//				Debug.LogError("Records incorrect value", this);
//                //todo comment: Для чего выключается этот компонент?
//                //чтобы предотвратить ошибки в Update()
//                enabled = false;
//			}
//            else
//            {
//                _currentPosition = transform.position;
//            }
//        }

//		private void Update()
//		{
//			var curr = _save.Records[_index];
//            //todo comment: Что проверяет это условие (с какой целью)?
//            //это тайминг-условие для перехода к следующей точке записи
//            //if (Time.time > curr.Time)
//            if (slowedTime > curr.Time)
//            {
//				_prev = curr;
//				_index++;
//                //todo comment: Для чего нужна эта проверка?
//                //это защита от выхода за границы массива и окончание воспроизведения
//                if (_index >= _save.Records.Count)
//                {
//                    enabled = false;
//                    Debug.Log($"<b>{name}</b> finished", this);
//                    return;
//                }
//            }
//            //todo comment: Для чего производятся эти вычисления (как в дальнейшем они применяются)?
//            //это нормализованное время для интерполяции между двумя точками
//            var delta = (slowedTime - _prev.Time) / (curr.Time - _prev.Time);
//            //todo comment: Зачем нужна эта проверка?
//            //это защита от деления на ноль и некорректных значений
//            if (float.IsNaN(delta)) delta = 0f;
//            //todo comment: Опишите, что происходит в этой строчке так подробно, насколько это возможно
//            //в этой строчке пишется комментарий с вопросом, но она пропускается при выполнении кода, поэтому ничего не происходит с ней или без нее
//            //transform.position = Vector3.Lerp(_prev.Position, curr.Position, delta);
//            Vector3 targetPosition = Vector3.Lerp(_prev.Position, curr.Position, delta);
//            _currentPosition = Vector3.Lerp(_currentPosition, targetPosition, Time.deltaTime * 5f);
//            transform.position = _currentPosition;
//        }
//	}
//}
using System;
using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(PositionSaver))]
    public class ReplayMover : MonoBehaviour
    {
        private PositionSaver _save;
        private int _index = 0;
        private PositionSaver.Data _prev;

        [SerializeField, Range(0.1f, 1f)]
        private float _speed = 0.5f; // Коэффициент замедления
        private Vector3 _currentPosition;

        private void Start()
        {
            if (!TryGetComponent(out _save) || _save.Records.Count == 0)
            {
                Debug.LogError("Records incorrect value", this);
                enabled = false;
            }
            else
            {
                _currentPosition = transform.position;
            }
        }

        private void Update()
        {
            var curr = _save.Records[_index];

            // Замедляем время в 2 раза (0.5f) или другое значение
            float slowedTime = Time.time * _speed;

            if (slowedTime > curr.Time)
            {
                _prev = curr;
                _index++;

                if (_index >= _save.Records.Count)
                {
                    enabled = false;
                    Debug.Log($"<b>{name}</b> finished", this);
                    return;
                }
            }

            var delta = (slowedTime - _prev.Time) / (curr.Time - _prev.Time);
            if (float.IsNaN(delta)) delta = 0f;

            // Плавная интерполяция к целевой позиции
            Vector3 targetPosition = Vector3.Lerp(_prev.Position, curr.Position, delta);
            _currentPosition = Vector3.Lerp(_currentPosition, targetPosition, Time.deltaTime * 5f);
            transform.position = _currentPosition;
        }
    }
}