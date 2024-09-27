using System.Collections.Generic;
using UnityEngine;

namespace ObjectPool
{
    public class BulletObjectPool : MonoBehaviour
    {
        private GameObject _prefab;
        private readonly Queue<GameObject> _pool = new();


        // ��������֐�
        public void Initialize(GameObject prefab, int initialSize)
        {
            _prefab = prefab;

            AddObjects(initialSize);
        }

        // ���X�g����O���ăI�u�W�F�N�g��n���֐�
        public GameObject Get()
        {
            if (_pool.Count == 0)
            {
                AddObjects(1);
            }
            return _pool.Dequeue();
        }

        // �I�u�W�F�N�g���A�N�e�B�u�����ă��X�g�ɒǉ�����֐�
        public void Return(GameObject objectToReturn)
        {
            objectToReturn.SetActive(false);
            _pool.Enqueue(objectToReturn);
        }

        // �v�f������Ȃ��ꍇ�ɁA�������Ēǉ�����p�̊֐�
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