using UnityEngine;

namespace StageDelaunayTriangles
{
    public class Triangle
    {
        // 頂点
        public Vector2 P1, P2, P3;  

        public Triangle(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            // Vector3からPointに変換して頂点を格納
            this.P1 = p1;
            this.P2 = p2;
            this.P3 = p3;
        }


        /// <summary>
        /// 同値判定
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        new public bool Equals(object obj)
        {
            try
            {
                Triangle triangle = (Triangle)obj;

                // 三角形の頂点の順番を考慮せず、頂点が全て同じであれば同じ三角形とみなす
                return (P1.Equals(triangle.P1) && P2.Equals(triangle.P2) && P3.Equals(triangle.P3)) ||
                       (P1.Equals(triangle.P2) && P2.Equals(triangle.P3) && P3.Equals(triangle.P1)) ||
                       (P1.Equals(triangle.P3) && P2.Equals(triangle.P1) && P3.Equals(triangle.P2)) ||

                       (P1.Equals(triangle.P3) && P2.Equals(triangle.P2) && P3.Equals(triangle.P1)) ||
                       (P1.Equals(triangle.P2) && P2.Equals(triangle.P1) && P3.Equals(triangle.P3)) ||
                       (P1.Equals(triangle.P1) && P2.Equals(triangle.P3) && P3.Equals(triangle.P2));
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// 他の三角形と共有点を持つか
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool HasCommonPoints(Triangle t)
        {
            return (P1.Equals(t.P1) || P1.Equals(t.P2) || P1.Equals(t.P3) ||
                    P2.Equals(t.P1) || P2.Equals(t.P2) || P2.Equals(t.P3) ||
                    P3.Equals(t.P1) || P3.Equals(t.P2) || P3.Equals(t.P3));
        }


        /// <summary>
        /// この三角形が潰れているか
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool CheckSmashedPolygon()
        {
            if (P1 == P2 || P2 == P3 || P3 == P1)
            { return true; }
            else 
            { return false; }
        }


        /// <summary>
        /// 座標をVector2Intで取得する
        /// </summary>
        /// <returns></returns>
        public Vector2Int[] GetPointsToVector2Int()
        {
            Vector2Int[] vectors=new Vector2Int[3];

            vectors[0] = new((int)P1.x, (int)P1.y);
            vectors[1] = new((int)P2.x, (int)P2.y);
            vectors[2] = new((int)P3.x, (int)P3.y);

            return vectors;
        }


        /// <summary>
        /// メッシュを作成する
        /// </summary>
        /// <returns></returns>
        public void CreateMesh()
        {
            Mesh mesh = new ();
            Vector3[] vertices = new Vector3[3];
            vertices[0] = new Vector2(P1.x, P1.y);
            vertices[1] = new Vector2(P2.x, P2.y);
            vertices[2] = new Vector2(P3.x, P3.y);

            int[] triangles = new int[3] { 0, 1, 2 };

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();


            // メッシュを作って出力
            GameObject triangleObject = new ("Triangle");
            MeshFilter meshFilter = triangleObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = triangleObject.AddComponent<MeshRenderer>();

            meshFilter.mesh = mesh;
            meshRenderer.material = new Material(Shader.Find("Standard"));

        }
    }
}