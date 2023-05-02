using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperTriangle : MonoBehaviour
{

    private void Start()
    {
        // Utwórz nowy pusty obiekt i sparentuj go z oryginalnym obiektem
        GameObject superTriangle = new GameObject("Super Triangle");
        superTriangle.transform.SetParent(this.transform);

        // Ustaw pozycję punktu trójkąta jako środek nowego obiektu
        Vector3 trianglePoint = superTriangle.transform.position = this.transform.position;

        // Utwórz trójkąt równoboczny z rozmiarem określonym przez użytkownika
        Mesh triangleMesh = new Mesh();
        Vector3[] vertices = new Vector3[3];
        vertices[0] = new Vector3(-2f, 0f, 0f);
        vertices[1] = new Vector3(0f, 3f, 0f);
        vertices[2] = new Vector3(2f, 0f, 0f);
        int[] triangles = new int[3] { 0, 1, 2 };
        triangleMesh.vertices = vertices;
        triangleMesh.triangles = triangles;
        triangleMesh.RecalculateNormals();

        // Dodaj komponent MeshFilter i MeshRenderer do nowego obiektu trójkąta
        MeshFilter meshFilter = superTriangle.AddComponent<MeshFilter>();
        meshFilter.mesh = triangleMesh;
        MeshRenderer meshRenderer = superTriangle.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Standard"));


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
