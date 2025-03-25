using UnityEngine;

public class FlipMeshNormals : MonoBehaviour
{
    void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            FlipNormals(meshFilter.mesh);
        }
    }

    void FlipNormals(Mesh mesh)
    {
        Vector3[] normals = mesh.normals;
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = -normals[i]; // Invert normals
        }
        mesh.normals = normals;

        // Flip triangle winding order
        int[] triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int temp = triangles[i];
            triangles[i] = triangles[i + 1];
            triangles[i + 1] = temp;
        }
        mesh.triangles = triangles;
    }
}
