using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "StageWallGenerateData", menuName = "Stage/Generator/WallGenerateData")]
public class StageWallGenerateData : ScriptableObject
{
    private struct InstanceMap
    {
        public bool[,] _instanceMap { get; private set; }

        public int _mapX { get; private set; }
        public int _mapY { get; private set; }
        public int _allMapX { get; private set; }
        public int _allMapY { get; private set; }
        public int _spaceValue { get; private set; }

        public InstanceMap(int mapX, int mapY, int spaceValue)
        {
            _mapX = mapX;
            _mapY = mapY;
            _allMapX = mapX + spaceValue * 2;
            _allMapY = mapY + spaceValue * 2;
            _spaceValue = spaceValue;

            _instanceMap = new bool[_allMapX, _allMapY];

            for (int checkY = 0; checkY < mapY; ++checkY)
                for (int checkX = 0; checkX < mapX; ++checkX)
                    _instanceMap[checkX + _spaceValue, checkY + _spaceValue] = true;
        }

        public bool GetIsInstanceMap(int selectX, int selectY)
        {
            int checkX = selectX + _spaceValue;
            int checkY = -(selectY - _spaceValue);

            // �z��̊O��������I��
            if (checkX < 0 || checkX >= _allMapX) { Debug.Log("X"); return true; }
            if (checkY < 0 || checkY >= _allMapY) { Debug.Log("Y"); return true; }

            return _instanceMap[checkX, checkY];
        }

        public void DrawMap(int selectX, int selectY)
        {
            int checkX = selectX + _spaceValue;
            int checkY = -(selectY - _spaceValue);

            //Debug.Log(selectX+","+selectY+"�̏��Ł@"+checkX+","+checkY+"�ɓh��܂���");

            _instanceMap[checkX, checkY] = true;
        }
    }

    [SerializeField]
    private StageWallData[] _wallPrefab;

    [SerializeField]
    private int _spaceValue;

    public void GenerateSpiralWall(int mapX, int mapY, float tileScale)
    {
        InstanceMap instanceMap = new(mapX, mapY, _spaceValue);


        int x = -_spaceValue;
        int y = _spaceValue;
        int dx = 1;
        int dy = 0;
        int segmentLengthX = instanceMap._allMapX - 1;
        int segmentLengthY = instanceMap._allMapY;
        int segmentPassed = 0;

        int angle = 0;

        bool isTypeX = true;
        bool isFirst = true;

        // �}�b�v�͈͓̔��ɂȂ�܂ŌJ��Ԃ�
        while (!(0 <= x && x < mapX && -mapY < y && y <= 0))
        {
            //Debug.Log("���[�v���܂����������������������������������������������F��" + x + ",��" + y);

            //Debug.Log(!instanceMap.GetIsInstanceMap(x, y));

            // �����Ȃ��ꍇ����
            if (!instanceMap.GetIsInstanceMap(x, y))
            {
                int selectValue = Random.Range(0, _wallPrefab.Length);

                // �I�������v�f�����邩�𒲂ׂ�
                bool canPlace = true;
                Vector2Int upLeftPoint = _wallPrefab[selectValue].GetUpLeftPoint(new Vector2Int(x, y), angle);
                Vector2Int downRightPoint = new Vector2Int(upLeftPoint.x + _wallPrefab[selectValue].Scale.x,
                                                           upLeftPoint.y - _wallPrefab[selectValue].Scale.y);

                for (int checkY = upLeftPoint.y; checkY > downRightPoint.y ; --checkY)
                {
                    for (int checkX = upLeftPoint.x; checkX < downRightPoint.x; ++checkX)
                    {
                        if (instanceMap.GetIsInstanceMap(checkX, checkY))
                        {
                            canPlace = false;
                            break;
                        }
                    }
                    if (!canPlace) break;
                }

                if (canPlace)
                {
                    //Debug.Log("�������܂��F" + selectValue + " , " + new Vector3(x * tileScale, 0, y * tileScale));
                    // �u���b�N�𐶐�
                    Instantiate(_wallPrefab[selectValue], new Vector3(x * tileScale, 0, y * tileScale), Quaternion.Euler(0, angle, 0));

                    // �^���}�b�v�z���h��
                    for (int checkY = upLeftPoint.y; checkY > downRightPoint.y; --checkY)
                    {
                        for (int checkX = upLeftPoint.x; checkX < downRightPoint.x; ++checkX)
                        {
                            instanceMap.DrawMap(checkX, checkY);
                        }
                    }
                }
                else
                {
                    continue;
                }
            }

            bool conditionX = isTypeX && segmentPassed == segmentLengthX;
            bool conditionY = !isTypeX && segmentPassed == segmentLengthY;

            // �Z�O�����g�̒����ɒB�����������ύX
            if (conditionX || conditionY)
            {
                if (isTypeX) --segmentLengthY;
                else if (!isFirst) --segmentLengthX;

                if (!isTypeX) isFirst = false;

                isTypeX = !isTypeX;

                segmentPassed = 0;

                // �������E��90�x��]
                int temp = dy;
                dy = -dx;
                dx = temp;

                angle += 90;
                if (angle >= 360)
                    angle = 0;
            }

            // ���̈ʒu�Ɉړ�
            x += dx;
            y += dy;
            segmentPassed++;

        }
    }
}