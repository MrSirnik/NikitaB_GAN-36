using UnityEngine;

namespace DefaultNamespace
{
	
	[RequireComponent(typeof(PositionSaver))]
	public class EditorMover : MonoBehaviour
	{
		private PositionSaver _save;
		private float _currentDelay;

        //todo comment: Что произойдёт, если _delay > _duration?
        //Записи будут добавляться слишком редко относительно общего времени записи,
        //что приведет к очень маленькому количеству сохраненных позиций.
        [Range(0.2f, 1.0f)]
        [SerializeField] private float _delay = 0.5f;

        [Min(0.2f)]
        [SerializeField] private float _duration = 5f;

        private void Start()
		{
            //todo comment: Почему этот поиск производится здесь, а не в начале метода Update?
            //Поиск выполняется в Start для оптимизации - чтобы не искать компонент каждый кадр,
            //а получить ссылку один раз при инициализации.
            _save = GetComponent<PositionSaver>();
			_save.Records.Clear();

            if (_duration <= _delay)
            {
                _duration = _delay * 5f;
                Debug.LogWarning($"Duration adjusted to {_duration} because it was less than or equal to delay", this);
            }
        }

		private void Update()
		{
			_duration -= Time.deltaTime;
			if (_duration <= 0f)
			{
				enabled = false;
				Debug.Log($"<b>{name}</b> finished", this);
				return;
			}

            //todo comment: Почему не написать (_delay -= Time.deltaTime;) по аналогии с полем _duration?
            //Потому что _delay - это интервал между записями (константа),
            //а _currentDelay - текущий отсчет до следующей записи (переменная).
            _currentDelay -= Time.deltaTime;
			if (_currentDelay <= 0f)
			{
				_currentDelay = _delay;
				_save.Records.Add(new PositionSaver.Data
				{
					Position = transform.position,
                    //todo comment: Для чего сохраняется значение игрового времени?
                    //Время сохраняется для последующего воспроизведения записей в правильной
					//временной последовательности в компоненте ReplayMover.
                    Time = Time.time,
				});
			}
		}
	}
}