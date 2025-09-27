using UnityEngine;

namespace DefaultNamespace
{
	
	[RequireComponent(typeof(PositionSaver))]
	public class EditorMover : MonoBehaviour
	{
		private PositionSaver _save;
		private float _currentDelay;

        //todo comment: Что произойдёт, если _delay > _duration?
        //если _delay больше чем оставшееся время _duration, то последняя запись не сохранится
        [Range(0.2f, 1.0f)]
        [SerializeField] private float _delay = 0.2f;//через каждые _delay секунд 
        [Min(0.2f)]
        [SerializeField] private float _duration = 20f;//в течении времени _duration

		private void Start()
		{
            //todo comment: Почему этот поиск производится здесь, а не в начале метода Update?
            //GetComponent - относительно дорогой вызов. В Start() он выполняется один раз, в Update() - каждый кадр
            _save = GetComponent<PositionSaver>();
			_save.Records.Clear();

            if (_duration <= _delay)
            {
                Debug.LogWarning($"Duration ({_duration}) is less or equal to delay ({_delay}). Setting duration to {_delay * 5f}");
                _duration = _delay * 5f; // Устанавливаем в 5 раз больше _delay
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
            //потому что мы забудем значение _delay
            _currentDelay -= Time.deltaTime;
			if (_currentDelay <= 0f)
			{
				_currentDelay = _delay;
				_save.Records.Add(new PositionSaver.Data
				{
					Position = transform.position,
                    //todo comment: Для чего сохраняется значение игрового времени?
                    //для фиксирования передвижения во времени
                    Time = Time.time,
				});
			}
		}
	}
}