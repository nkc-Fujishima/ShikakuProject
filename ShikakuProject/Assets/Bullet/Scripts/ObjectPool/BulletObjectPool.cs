using System.Collections.Generic;
using UnityEngine;

namespace ObjectPool
{
    public class BulletObjectPool : MonoBehaviour
    {
        private GameObject _prefab;
        private readonly Queue<GameObject> _pool = new();


        // 生成する関数
        public void Initialize(GameObject prefab, int initialSize)
        {
            _prefab = prefab;

            AddObjects(initialSize);
        }

        // リストから外してオブジェクトを渡す関数
        public GameObject Get()
        {
            if (_pool.Count == 0)
            {
                AddObjects(1);
            }
            return _pool.Dequeue();
        }

        // オブジェクトを非アクティブ化してリストに追加する関数
        public void Return(GameObject objectToReturn)
        {
            objectToReturn.SetActive(false);
            _pool.Enqueue(objectToReturn);
        }

        // 要素が足りない場合に、生成して追加する用の関数
        private void AddObjects(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var newObject = Instantiate(_prefab);
                newObject.SetActive(false);
                _pool.Enqueue(newObject);
            }
        }
    }
}