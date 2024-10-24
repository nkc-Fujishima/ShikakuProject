using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace StageDelaunayTriangles
{
    // 四角　必ず時計回りで登録すること
    public class Square
    {
        private Vector2Int _point1, _point2, _point3, _point4;

        public Square(Vector2Int point1, Vector2Int point2, Vector2Int point3, Vector2Int point4)
        {
            _point1 = point1;
            _point2 = point2;
            _point3 = point3;
            _point4 = point4;
        }

        public Triangle[] GetTriangle()
        {
            Triangle[] triangles = new Triangle[2];
            triangles[0] = new(_point1, _point2, _point4);
            triangles[1] = new(_point2, _point3, _point4);

            return triangles;
        }
    }

    public class DelaunayTriangles
    {
        // 三角形リスト
        public HashSet<Triangle> TriangleSet;

        /// <summary>
        /// 与えられた点のリストを基にDelaunay分割を行う
        /// </summary>
        /// <param name="pointList"></param>
        public DelaunayTriangles(List<Vector2Int> pointList, Square[] square = null)
        {
            HashSet<Triangle> triangles = new ();

            if (square != null)
            {
                HashSet<Vector2Int> pointHash = new();

                for (int i = 0; i < square.Length; ++i)
                {
                    // 三角形を設定
                    Triangle[] triangleSquare = square[i].GetTriangle();
                    triangles.Add(triangleSquare[0]);
                    triangles.Add(triangleSquare[1]);

                    // 三角形から頂点を取得
                    pointHash.AddRange(triangleSquare[0].GetPointsToVector2Int());
                    pointHash.AddRange(triangleSquare[1].GetPointsToVector2Int());
                }
                pointList.AddRange(pointHash);
            }

            DelaunayTriangulation(pointList, triangles.ToArray());

            if (square != null)
            {
                for (int i = 0; i < square.Length; ++i)
                {
                    Triangle[] triangleSqare = square[i].GetTriangle();
                    TriangleSet.Remove(triangleSqare[0]);
                    TriangleSet.Remove(triangleSqare[1]);
                }
            }
        }

        /// <summary>
        /// 点のリストを与えて、Delaunay三角分割を行う
        /// </summary>
        /// <param name="pointList"></param>
        public void DelaunayTriangulation(List<Vector2Int> pointList, Triangle[] triangles = null)
        {
            // 三角形リストを初期化
            TriangleSet = new ();

            // 巨大な外部三角形をリストに追加
            Triangle hugeTriangle = GetHugeTriangle(pointList);
            TriangleSet.Add(hugeTriangle);

            for (int i = 0; i < triangles.Length; ++i)
                TriangleSet.Add(triangles[i]);

            //三角形のまとまりを保持
            Triangle[] triangleSetOld;

            // 点を逐次追加し、反復的に三角分割を行う
            foreach (Vector2 point in pointList)
            {
                // 追加候補の三角形を保持する一時ハッシュ
                Dictionary<Triangle, bool> tmpTriangleSet = new ();

                // 三角形のまとまりを保持
                triangleSetOld = new Triangle[TriangleSet.Count];
                TriangleSet.CopyTo(triangleSetOld);

                // 現在の三角形リストから要素を一つずつ取り出して、
                // 与えられた点が各々の三角形の外接円の中に含まれるかどうか判定
                foreach (Triangle triangle in triangleSetOld)
                {
                    // 外接円を求める
                    Circle circle = GetCircumscribedCirclesOfTriangle(triangle);

                    // 追加された点が外接円内部に存在する場合、
                    // その外接円を持つ三角形をリストから除外し、新たに分割し直す
                    if (Vector2.Distance(circle.Center, point) <= circle.Radius)
                    {
                        // 新しい三角形を作り、一時ハッシュに入れる
                        AddElementToRedundanciesMap(tmpTriangleSet, new Triangle(point, triangle.P1, triangle.P2));
                        AddElementToRedundanciesMap(tmpTriangleSet, new Triangle(point, triangle.P2, triangle.P3));
                        AddElementToRedundanciesMap(tmpTriangleSet, new Triangle(point, triangle.P3, triangle.P1));

                        // 旧い三角形をリストから削除
                        TriangleSet.Remove(triangle);
                    }
                }

                // 一時ハッシュのうち、重複のないものを三角形リストに追加
                foreach (KeyValuePair<Triangle, bool> entry in tmpTriangleSet)
                {
                    if (entry.Value)
                        TriangleSet.Add(entry.Key);
                }
            }

            //三角形のまとまりを保持
            triangleSetOld = new Triangle[TriangleSet.Count];
            TriangleSet.CopyTo(triangleSetOld);


            foreach (Triangle t in triangleSetOld)
            {
                // 外部三角形の頂点を削除
                if (hugeTriangle.HasCommonPoints(t))
                {
                    TriangleSet.Remove(t);
                }

                // 潰れてる三角形を削除
                if (t.CheckSmashedPolygon())
                {
                    TriangleSet.Remove(t);
                }
            }
        }

        /// <summary>
        /// ある座標が含まれている三角形に隣接している頂点の座標を取得
        /// </summary>
        /// <param name="checkPosition"></param>
        /// <returns></returns>
        public Vector2Int[] CheckPositionConnect(Vector2Int checkPosition)
        {
            List<Vector2Int> _data = new ();

            foreach (Triangle node in TriangleSet)
            {
                // 座標が含まれてない三角形はスキップ
                if (!IsVertexOfTriangle(checkPosition, node))
                    continue;

                if (checkPosition != node.P1)
                    _data.Add(new Vector2Int((int)node.P1.x, (int)node.P1.y));
                if (checkPosition != node.P2)
                    _data.Add(new Vector2Int((int)node.P2.x, (int)node.P2.y));
                if (checkPosition != node.P3)
                    _data.Add(new Vector2Int((int)node.P3.x, (int)node.P3.y));
            }

            return _data.ToArray();
        }


        /// <summary>
        /// 一時ハッシュを使って重複判定
        /// </summary>
        /// <param name="hashMap">Key:三角形 - Value:重複していないかどうか</param>
        /// <param name="triangle"></param>
        private void AddElementToRedundanciesMap(Dictionary<Triangle, bool> hashMap, Triangle triangle)
        {
            Triangle[] hashMapSave = hashMap.Keys.ToArray();
            bool isDuplicate = false;

            foreach (Triangle key in hashMapSave)
            {
                if (triangle.Equals(key))
                {
                    // 重複あり : Keyに対応する値にFalseをセット
                    hashMap[key] = false;
                    isDuplicate = true;
                }
            }

            if (!isDuplicate)
                // 重複なし : 新規追加
                hashMap[triangle] = true;
        }
        
        /// <summary>
        /// 最初に必要な巨大三角形を求める
        /// </summary>
        /// <param name="pointList"></param>
        /// <returns></returns>
        private Triangle GetHugeTriangle(List<Vector2Int> pointList)
        {
            Vector2 minPos = new (float.MaxValue, float.MaxValue);
            Vector2 maxPos = new (float.MinValue, float.MinValue);

            foreach (Vector2 point in pointList)
            {
                if (point.x < minPos.x) minPos.x = point.x;
                if (point.y < minPos.y) minPos.y = point.y;

                if (point.x > maxPos.x) maxPos.x = point.x;
                if (point.y > maxPos.y) maxPos.y = point.y;
            }

            return GetHugeTriangle(minPos - Vector2.one * 1000, maxPos + Vector2.one * 1000);
        }

        /// <summary>
        /// 任意の矩形を包含する正三角形を求める
        /// 引数には矩形の左上座標および右下座標を与える
        /// </summary>
        /// <param name="startPos">矩形の左下座標</param>
        /// <param name="endPos">矩形の右上座標</param>
        /// <returns></returns>
        private Triangle GetHugeTriangle(Vector3 startPos, Vector3 endPos)
        {
            // 1 与えられた矩形を包含する円を求める
            Circle inclusiveCircle;
            {
                Vector2 circleCenter = new((endPos.x - startPos.x) / 2.0f, (endPos.y - startPos.y) / 2.0f);
                float circleRadius = Vector2.Distance(circleCenter, startPos) + 1.0f;
                inclusiveCircle = new(circleCenter, circleRadius);
            }

            // 2 その円に外接する正三角形を求める
            // 正三角形の高さ（垂直二等分線の長さ）は 2√3･r
            Vector2 triangleRightPoint;
            triangleRightPoint.x = inclusiveCircle.Center.x - Mathf.Sqrt(3) * inclusiveCircle.Radius;
            triangleRightPoint.y = inclusiveCircle.Center.y - inclusiveCircle.Radius;

            Vector2 triangleLeftPoint;
            triangleLeftPoint.x = inclusiveCircle.Center.x + Mathf.Sqrt(3) * inclusiveCircle.Radius;
            triangleLeftPoint.y = inclusiveCircle.Center.y - inclusiveCircle.Radius;

            Vector2 triangleTopPoint;
            triangleTopPoint.x = inclusiveCircle.Center.x;
            triangleTopPoint.y = inclusiveCircle.Center.y + 2 * inclusiveCircle.Radius;

            return new Triangle(triangleRightPoint, triangleLeftPoint, triangleTopPoint);
        }

        /// <summary>
        /// 三角形を与えてその外接円を求める
        /// </summary>
        /// <param name="triangle"></param>
        /// <returns></returns>
        private Circle GetCircumscribedCirclesOfTriangle(Triangle triangle)
        {
            float x1 = triangle.P1.x;
            float y1 = triangle.P1.y;
            float x2 = triangle.P2.x;
            float y2 = triangle.P2.y;
            float x3 = triangle.P3.x;
            float y3 = triangle.P3.y;

            float c = 2.0f * ((x2 - x1) * (y3 - y1) - (y2 - y1) * (x3 - x1));
            float x = ((y3 - y1) * (x2 * x2 - x1 * x1 + y2 * y2 - y1 * y1) +
                       (y1 - y2) * (x3 * x3 - x1 * x1 + y3 * y3 - y1 * y1)) / c;
            float y = ((x1 - x3) * (x2 * x2 - x1 * x1 + y2 * y2 - y1 * y1) +
                       (x2 - x1) * (x3 * x3 - x1 * x1 + y3 * y3 - y1 * y1)) / c;
            Vector2 center = new (x, y);

            // 外接円の半径 r は、半径から三角形の任意の頂点までの距離に等しい
            float r = Vector2.Distance(center, triangle.P1);
            return new Circle(center, r);
        }

        private bool IsVertexOfTriangle(Vector2 p, Triangle node)
        {
            return p == node.P1 || p == node.P2 || p == node.P3;
        }
    }
}