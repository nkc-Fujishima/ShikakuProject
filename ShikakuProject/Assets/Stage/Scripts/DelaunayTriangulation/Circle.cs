using UnityEngine;

namespace StageDelaunayTriangles
{
    public class Circle
    {
        // 中心座標と半径
        public Vector2 Center;
        public float Radius;

        /// <summary>
        /// 中心座標と半径を与えて円をつくる
        /// </summary>
        /// <param name="c"></param>
        /// <param name="r"></param>
        public Circle(Vector3 c, float r)
        {
            // 中心座標をPointクラスでラップ
            Center = c;
            Radius = r;
        }
    }
}