using UnityEngine;

[CreateAssetMenu(fileName = "StageWallGenerateData", menuName = "Stage/Generator/WallGenerateData")]
public class StageWallGenerateData : ScriptableObject
{
    private struct InstanceMapData
    {
        public bool[,] InstanceMap { get; private set; }

        public int MapX { get; private set; }
        public int MapY { get; private set; }
        public int AllMapX { get; private set; }
        public int AllMapY { get; private set; }
        public int SpaceValue { get; private set; }

        public InstanceMapData(int mapX, int mapY, int spaceValue)
        {
            MapX = mapX;
            MapY = mapY;
            AllMapX = mapX + spaceValue * 2;
            AllMapY = mapY + spaceValue * 2;
            SpaceValue = spaceValue;

            InstanceMap = new bool[AllMapX, AllMapY];

            for (int checkY = 0; checkY < mapY; ++checkY)
                for (int checkX = 0; checkX < mapX; ++checkX)
                    InstanceMap[checkX + SpaceValue, checkY + SpaceValue] = true;
        }

        public readonly bool GetIsInstanceMap(int selectX, int selectY)
        {
            int checkX = selectX + SpaceValue;
            int checkY = -(selectY - SpaceValue);

            // �z��̊O��������I��
            if (checkX < 0 || checkX >= AllMapX) return true;
            if (checkY < 0 || checkY >= AllMapY) return true;

            return InstanceMap[checkX, checkY];
        }

        public readonly void DrawMap(int selectX, int selectY)
        {
            int checkX = selectX + SpaceValue;
            int checkY = -(selectY - SpaceValue);

            InstanceMap[checkX, checkY] = true;
        }
    }

    [SerializeField]
    private StageWallData[] _wallPrefab;

    [SerializeField]
    private int _spaceValue;

    public void GenerateSpiralWall(int mapX, int mapY, float tileScale, PhysicMaterial physicMaterial)
    {
        GameObject wallObject = new("WallObject");

        InstanceMapData instanceMap = new(mapX, mapY, _spaceValue);


        int x = -_spaceValue;
        int y = _spaceValue;
        int dx = 1;
        int dy = 0;
        int segmentLengthX = instanceMap.AllMapX - 1;
        int segmentLengthY = instanceMap.AllMapY;
        int segmentPassed = 0;

        int angle = 0;

        bool isTypeX = true;
        bool isFirst = true;

        // �}�b�v�͈͓̔��ɂȂ�܂ŌJ��Ԃ�
        while (!(0 <= x && x < mapX && -mapY < y && y <= 0))
        {
            // �����Ȃ��ꍇ����
            if (!instanceMap.GetIsInstanceMap(x, y))
            {
                int selectValue = Random.Range(0, _wallPrefab.Length);

                // �I�������v�f�����邩�𒲂ׂ�
                bool canPlace = true;
                Vector2Int upLeftPoint = _wallPrefab[selectValue].GetUpLeftPoint(new (x, y), angle);
                Vector2Int downRightPoint = new (upLeftPoint.x + _wallPrefab[selectValue].Scale.x,
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
                    // �u���b�N�𐶐�
                    StageWallData instanceObject = Instantiate(_wallPrefab[selectValue], new Vector3(x * tileScale, 0, y * tileScale), Quaternion.Euler(0, angle, 0));

                    instanceObject.transform.parent = wallObject.transform;

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


        // �R���C�_�[�𐶐�
        GameObject wallColliderObject = new ("WallColliderObject");
        wallColliderObject.transform.parent = wallObject.transform;

        for (int i = 0; i < 4; i++)
        {
            GameObject childObject = new ("ChildObject" + i);
            childObject.transform.parent = wallColliderObject.transform;
            BoxCollider boxCollider = childObject.AddComponent<BoxCollider>();
            boxCollider.material = physicMaterial;

            // �ʒu�ƃT�C�Y��ݒ�
            switch (i)
            {
                case 0:
                    childObject.transform.localPosition = new(tileScale * mapX / 2 - tileScale / 2, 0, tileScale);
                    boxCollider.size = new (tileScale * mapX, tileScale * 3, tileScale);
                    break;
                case 1:
                    childObject.transform.localPosition = new(tileScale * mapX / 2 - tileScale / 2, 0, -tileScale * mapY);
                    boxCollider.size = new(tileScale * mapX, tileScale * 3, tileScale);
                    break;
                case 2:
                    childObject.transform.localPosition = new(-tileScale, 0, -(tileScale * mapY / 2 - tileScale / 2));
                    boxCollider.size = new(tileScale, tileScale * 3, tileScale * mapY);
                    break;
                case 3:
                    childObject.transform.localPosition = new(tileScale * mapX, 0, -(tileScale * mapY / 2 - tileScale / 2));
                    boxCollider.size = new(tileScale, tileScale * 3, tileScale * mapY);
                    break;
            }
        }
    }
}