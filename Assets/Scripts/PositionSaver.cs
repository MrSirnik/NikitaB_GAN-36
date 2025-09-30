using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DefaultNamespace
{
	public class PositionSaver : MonoBehaviour
	{
        [System.Serializable]
        public struct Data
		{
			public Vector3 Position;
			public float Time;
		}
        [ReadOnly]
        [Tooltip("Для заполнения этого поля воспользуйтесь контекстным меню в инспекторе и командой 'Create File'")]
        [SerializeField] private TextAsset _json;

        [SerializeField, HideInInspector]
        private List<Data> _records;
        public List<Data> Records => _records;

        private void Awake()
		{
            //todo comment: Что будет, если в теле этого условия не сделать выход из метода?
            //Без выхода метод продолжит выполнение, что приведет к NullReferenceException 
            //при попытке десериализации из null _json.text.
            if (_json == null)
			{
				gameObject.SetActive(false);
				Debug.LogError("Please, create TextAsset and add in field _json");
				return;
			}
			
			JsonUtility.FromJsonOverwrite(_json.text, this);
            //todo comment: Для чего нужна эта проверка (что она позволяет избежать)?
            //Проверка инициализирует список Records если десериализация не удалась,
            //предотвращая NullReferenceException при последующем использовании списка.
            if (_records == null)
                _records = new List<Data>(10);
		}

		private void OnDrawGizmos()
		{
            //todo comment: Зачем нужны эти проверки (что они позволляют избежать)?
            //Проверки предотвращают ошибки при отрисовке Gizmos когда данных нет,
            //избегая исключений при обращении к пустому списку.
            if (_records == null || _records.Count == 0) return;
			var data = _records;
			var prev = data[0].Position;
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(prev, 0.3f);
            //todo comment: Почему итерация начинается не с нулевого элемента?
            //Итерация начинается с 1 потому что нулевой элемент уже обработан перед циклом
            //(prev = data[0].Position), и теперь нужно соединить его со следующими точками.
            for (int i = 1; i < data.Count; i++)
			{
				var curr = data[i].Position;
				Gizmos.DrawWireSphere(curr, 0.3f);
				Gizmos.DrawLine(prev, curr);
				prev = curr;
			}
		}

#if UNITY_EDITOR
		[ContextMenu("Create File")]
		private void CreateFile()
		{
			//todo comment: Что происходит в этой строке?
			//Создается файловый поток для создания нового файла "Path.txt" в папке Assets.
			var stream = File.Create(Path.Combine(Application.dataPath, "Path.txt"));
			//todo comment: Подумайте для чего нужна эта строка? (а потом проверьте догадку, закомментировав)
			//Stream.Dispose() освобождает файловый поток, позволяя Unity получить доступ к файлу.
			stream.Dispose();
			UnityEditor.AssetDatabase.Refresh();
			//В Unity можно искать объекты по их типу, для этого используется префикс "t:"
			//После нахождения, Юнити возвращает массив гуидов (которые в мета-файлах задаются, например)
			var guids = UnityEditor.AssetDatabase.FindAssets("t:TextAsset");
			foreach (var guid in guids)
			{
				//Этой командой можно получить путь к ассету через его гуид
				var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
				//Этой командой можно загрузить сам ассет
				var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);
				//todo comment: Для чего нужны эти проверки?
				//Проверки гарантируют, что найден корректный TextAsset с именем "Path"
				// и предотвращают присвоение null или неподходящего ассета.
				if(asset != null && asset.name == "Path")
				{
					_json = asset;
					UnityEditor.EditorUtility.SetDirty(this);
					UnityEditor.AssetDatabase.SaveAssets();
					UnityEditor.AssetDatabase.Refresh();
					//todo comment: Почему мы здесь выходим, а не продолжаем итерироваться?
					//Выход происходит потому что нужный ассет уже найден,
					// дальнейшая итерация бесполезна и может привести к непредсказуемому поведению.
					return;
				}
			}
		}

		private void OnDestroy()
		{
			if (_json != null && Records != null)
            {
                string jsonData = JsonUtility.ToJson(this, true);
                string path = UnityEditor.AssetDatabase.GetAssetPath(_json);
                File.WriteAllText(path, jsonData);
                UnityEditor.AssetDatabase.Refresh();
            }
		}
#endif
    }
}