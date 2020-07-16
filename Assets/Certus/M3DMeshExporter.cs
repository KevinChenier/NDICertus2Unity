using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M3DMeshExporter : MonoBehaviour
{

    public SkinnedMeshRenderer SMR;
    public Mesh M; 

    Vector3[] newVertices;
    Vector2[] newUV;
    int[] newTriangles;

    void LateUpdate()
    {
        M = SMR.sharedMesh;

        Mesh mesh = new Mesh();
        mesh.vertices = M.vertices;
        mesh.uv = M.uv;
        mesh.triangles = M.triangles;

        GetComponent<MeshFilter>().mesh = mesh;

        //Debug.Log(SMR.sharedMesh)
    }


}
