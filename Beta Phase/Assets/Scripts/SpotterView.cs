using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(MeshCollider))]
[ExecuteInEditMode]
public class SpotterView : MonoBehaviour {

    [SerializeField, Range(2, 50)] private int numSides = 3;
    [SerializeField] private float frontRadius;
    [SerializeField] private float backRadius;
    [SerializeField] private float length;

    [SerializeField] private Gradient gradient;   //vertex colors

    [SerializeField] protected Material material;

    protected List<Vector3> vertices;
    protected List<int> triangles;

    protected List<Vector3> normals;
    protected List<Vector4> tangents;
    protected List<Vector2> uvs;
    protected List<Color32> vertexColours;

    protected int numVertices;
    protected int numTriangles;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
    private Mesh mesh;

    protected void SetMeshNums()
    {
        //numSides vertices on each end, 4 on each length-side
        numVertices = 6 * numSides;
        //there are 3 * (numSides - 2) on each end 
        //and 6 on each length-side: 6*numSides
        numTriangles = 12 * (numSides - 1);
    }

    protected void SetVertices()
    {
        //building block vertices
        Vector3[] vs = new Vector3[2 * numSides];

        //Set the vs
        for (int i = 0; i < numSides; i++)
        {
            float angle = 2 * Mathf.PI * i / numSides;
            //one end
            vs[i] = new Vector3(frontRadius * Mathf.Cos(angle), frontRadius * Mathf.Sin(angle), 0);
            //other end
            vs[i + numSides] = new Vector3(backRadius * Mathf.Cos(angle), backRadius * Mathf.Sin(angle), length);
        }

        //set vertices - first end
        for (int i = 0; i < numSides; i++)
        {
            vertices.Add(vs[i]);
        }

        //middle verts
        for (int i = 0; i < numSides; i++)
        {
            vertices.Add(vs[i]);
            int secondIndex = i == 0 ? 2 * numSides - 1 : numSides + i - 1;
            vertices.Add(vs[secondIndex]);
            int thirdIndex = i == 0 ? numSides - 1 : i - 1;
            vertices.Add(vs[thirdIndex]);
            vertices.Add(vs[i + numSides]);
        }

        //other end
        for (int i = 0; i < numSides; i++)
        {
            vertices.Add(vs[i + numSides]);
        }
    }

    protected void SetTriangles()
    {
        //first end
        for (int i = 1; i < numSides - 1; i++)
        {
            triangles.Add(0);
            triangles.Add(i + 1);
            triangles.Add(i);
        }

        //middle
        for (int i = 1; i <= numSides; i++)
        {
            //There are numSides triangles in the first end, so start at numSides. 
            //On each loop, need to increase. 4*(i-1) does this correctly
            int val = numSides + 4 * (i - 1);

            triangles.Add(val);
            triangles.Add(val + 1);
            triangles.Add(val + 2);

            triangles.Add(val);
            triangles.Add(val + 3);
            triangles.Add(val + 1);
        }


        //other end - opposite way round so face points outwards
        for (int i = 1; i < numSides - 1; i++)
        {
            //There are numSides triangles in the first end, 
            //4 *numSides triangles in the middle, so this starts on 5*numSides
            triangles.Add(5 * numSides);
            triangles.Add(5 * numSides + i);
            triangles.Add(5 * numSides + i + 1);
        }
    }

    protected void SetVertexColours()
    {
        for (int i = 0; i < numVertices; i++)
        {
            //use the values in the gradient to colour
            vertexColours.Add(gradient.Evaluate((float)i / numVertices)); //take [0,1]
        }
    }

    void Update()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();

        meshRenderer.material = material;

        //initialise
        InitMesh();
        SetMeshNums();

        //create the mesh
        CreateMesh();
    }


    private bool ValidateMesh()
    {
        //build a string containing errors
        string errorStr = "";

        //check there are the correct number of triangles and vertices
        errorStr += vertices.Count == numVertices ? "" : "Should be " + numVertices + " vertices, but there are " + vertices.Count + ". ";
        errorStr += triangles.Count == numTriangles ? "" : "Should be " + numTriangles + " triangles, but there are " + triangles.Count + ". ";

        //Check there are the correct number of normals - there should be the same number of normals as there are vertices. If we're not manually calculating normals, there will be 0.
        //Similarly for tangents, uvs, vertexColours
        errorStr += (normals.Count == numVertices || normals.Count == 0) ? "" : "Should be " + numVertices + " normals, but there are " + normals.Count + ". ";
        errorStr += (tangents.Count == numVertices || tangents.Count == 0) ? "" : "Should be " + numVertices + " tangents, but there are " + tangents.Count + ". ";
        errorStr += (uvs.Count == numVertices || uvs.Count == 0) ? "" : "Should be " + numVertices + " uvs, but there are " + uvs.Count + ". ";
        errorStr += (vertexColours.Count == numVertices || vertexColours.Count == 0) ? "" : "Should be " + numVertices + " vertexColours, but there are " + vertexColours.Count + ". ";

        bool isValid = string.IsNullOrEmpty(errorStr);
        if (!isValid)
        {
            Debug.LogError("Not drawing mesh. " + errorStr);
        }

        return isValid;
    }

    private void InitMesh()
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();

        //optional
        normals = new List<Vector3>();
        tangents = new List<Vector4>();
        uvs = new List<Vector2>();
        vertexColours = new List<Color32>();
    }

    private void CreateMesh()
    {
        mesh = new Mesh();

        SetVertices();
        SetTriangles();
        SetVertexColours();

        if (ValidateMesh())
        {
            //This should always be done vertices first, triangles second - Unity requires this.
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);

            if (normals.Count == 0)
            {
                mesh.RecalculateNormals();
                normals.AddRange(mesh.normals);
            }

            mesh.SetNormals(normals);
            mesh.SetTangents(tangents);
            mesh.SetUVs(0, uvs);
            mesh.SetColors(vertexColours);

            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;
        }
    }
}
