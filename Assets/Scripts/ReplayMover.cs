using System;
using UnityEngine;

namespace DefaultNamespace
{
	[RequireComponent(typeof(PositionSaver))]
	public class ReplayMover : MonoBehaviour
	{
		private PositionSaver _save;

		private int _index;
		private PositionSaver.Data _prev;
		private float _duration;

		private void Start()
		{
            ////todo comment: зачем нужны эти проверки?
            //Проверки нужны для валидации зависимостей компонента. 
            if (!TryGetComponent(out _save) || _save.Records.Count == 0)
			{
				Debug.LogError("Records incorrect value", this);
                //todo comment: Для чего выключается этот компонент?
                //Компонент выключается, потому что без корректных данных воспроизведение невозможно.
                enabled = false;
			}
		}

		private void Update()
		{
			var curr = _save.Records[_index];
            //todo comment: Что проверяет это условие (с какой целью)?
            //Условие проверяет, настало ли время для перехода к следующей записи.
            if (Time.time > curr.Time)
			{
				_prev = curr;
				_index++;
                //todo comment: Для чего нужна эта проверка?
                //Проверка определяет, достигли ли мы конца списка записей.
                if (_index >= _save.Records.Count)
				{
					enabled = false;
					Debug.Log($"<b>{name}</b> finished", this);
				}
			}
            //todo comment: Для чего производятся эти вычисления (как в дальнейшем они применяются)?
            //Вычисляется интерполяционный коэффициент (0-1) между предыдущей и текущей позицией.
            var delta = (Time.time - _prev.Time) / (curr.Time - _prev.Time);
            //todo comment: Зачем нужна эта проверка?
            //Проверка предотвращает ошибки при делении на ноль или NaN значениях
            if (float.IsNaN(delta)) delta = 0f;
            //todo comment: Опишите, что происходит в этой строчке так подробно, насколько это возможно
            //Происходит линейная интерполяция позиции между предыдущей (_prev.Position) 
            //и текущей (curr.Position) точками на основе вычисленного коэффициента delta.
            //Transform.position плавно изменяется от предыдущей позиции к текущей.
            transform.position = Vector3.Lerp(_prev.Position, curr.Position, delta);
		}
	}
}