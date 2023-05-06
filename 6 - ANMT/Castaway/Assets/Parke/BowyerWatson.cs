using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


struct Triangle{
    public int P1;
    public int P2;
    public int P3;
};
    
public class BowyerWatson : MonoBehaviour
{
   
    public GameObject actor;
    List<Vector3> pointList = new List<Vector3>();
    List<Triangle> triangulation = new List<Triangle>();      
    List<GameObject> trackers = new List<GameObject>();

    HashSet<int> excludes = new HashSet<int>();

    GameObject faceMesh;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider = null;
    public Camera camera;

    bool wireframe = false;
    

    Vector3 offset  = new Vector3(2f, 0f, 0f);
    List<Vector3> supertTriangle = new List<Vector3>();


    List<List<Vector3>> keyframes = new List<List<Vector3>>();
    List<float> keyframeTimes = new List<float>();

private float timeSinceLastKeyframe;


bool parkeMode = false;
int currentKeyframeIndex =0;

public float keyframeLength = 0.1f;

   public void AddKeyframe(){

        keyframes.Add(GetComponent<MarkerInjector>().MarkersGlobalCoords());
        keyframeTimes.Add(Time.time);
   }   

    public void changeParkMode(){
       parkeMode = !parkeMode;
       keyframes.Clear();
       keyframeTimes.Clear();
    }


    void Start()
    {
        faceMesh = new GameObject("faceMesh");
        
        meshFilter = faceMesh.AddComponent<MeshFilter>();
        meshRenderer = faceMesh.AddComponent<MeshRenderer>();
        supertTriangle.Add(actor.transform.position + new Vector3(-1.5f, 0f, 0f));
        supertTriangle.Add(actor.transform.position + new Vector3(0f, 3f, 0f));
        supertTriangle.Add(actor.transform.position + new Vector3(1.5f, 0, 0f));
        AddSuperTriangle();
        createTrackers();
    
        createMesh();

    
    }

    public Vector3 GetFaceMeshPosition(){
        return faceMesh.transform.position;
    }

    
    void Update()
    {

     

        if(parkeMode){
            if(keyframes.Count>=2){



            int nextKeyframeIndex = (currentKeyframeIndex+1) % keyframes.Count;

            timeSinceLastKeyframe += Time.deltaTime;
            
            float deltaTime = (currentKeyframeIndex == keyframes.Count-1 )? 0:
            Mathf.Abs(keyframeTimes[nextKeyframeIndex] -keyframeTimes[currentKeyframeIndex]);
           
            if (timeSinceLastKeyframe >= deltaTime){
               timeSinceLastKeyframe = 0f;     
               currentKeyframeIndex = nextKeyframeIndex;
                
               return;
            }
                //Debug.Log(currentKeyframeIndex+"/"+keyframes.Count);


                for(int i = 3; i<pointList.Count; i++){
                    Debug.Log("i: "+i);
                    float phaseFraction = timeSinceLastKeyframe / deltaTime;
                    Vector3 difference = keyframes[nextKeyframeIndex][i-3] - keyframes[currentKeyframeIndex][i-3];
                    float C = (1f - Mathf.Cos(phaseFraction * Mathf.PI))/2f;
                    pointList[i] = (keyframes[currentKeyframeIndex][i-3] + C * difference) +offset;
                }
                meshFilter.mesh.vertices = pointList.ToArray();
            }
            
        } else {

            Reposition();
        }
        


    


    }

    public void switchShaderMode(){
            wireframe = !wireframe;
            meshRenderer.material = wireframe? new Material(Shader.Find("Custom/Wireframe")) : new Material(Shader.Find("Particles/Standard Unlit"))  ;
    }


    private void createTrackers(){
        for(int i = 0 ; i < pointList.Count; i++){
                GameObject tracker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                tracker.name = "Tracker-"+i;
                tracker.transform.position = pointList[i];
                tracker.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
                trackers.Add(tracker);
            }    
    }



    public void Triangulation(List<Vector3> points){
        excludes.Clear();
        foreach (GameObject tracker in trackers) {
            Destroy(tracker);
        }
        trackers.Clear();
        AddSuperTriangle();
        

        
        pointList.AddRange(points);

        for(int i = 0; i < pointList.Count; i++){
            pointList[i] +=offset;
        }
        createTrackers();

        for(int pointIndex = 3; pointIndex < pointList.Count; pointIndex++){
             List<Triangle> badTriangles  = new List<Triangle>();

            foreach(Triangle triangle in triangulation){
                float R = CalcCircumCircleRadius(triangle);  
                Vector2 circumCenter = CalcCircumCircleCenter(triangle);
                if(isInsideCricle(pointList[pointIndex], R, circumCenter) == true){
                    badTriangles.Add(triangle);
                }
            } 

            int i = 0;
            HashSet<(int, int)> poligon = new HashSet<(int, int)>();
            foreach(Triangle triangle in badTriangles){
                if(isEdgeSharded(badTriangles, i, triangle.P1, triangle.P2) == false){
                    poligon.Add((triangle.P1, triangle.P2));
                } 
                if(isEdgeSharded(badTriangles, i, triangle.P1, triangle.P3) == false){
                    poligon.Add((triangle.P1, triangle.P3));
                } 
                if(isEdgeSharded(badTriangles, i, triangle.P2, triangle.P3) == false){
                    poligon.Add((triangle.P2, triangle.P3));
                } 
                i++;
            }

            foreach(Triangle triangle in badTriangles){
                triangulation.Remove(triangle);
            }

            foreach((int A, int B) edge in poligon){
                Triangle newTriangle = new Triangle(){
                    P1 = edge.A,
                    P2 = edge.B,
                    P3 = pointIndex
                };


                Vector3 normal = CalculateNormal(pointList[newTriangle.P1], pointList[newTriangle.P2], pointList[newTriangle.P3]);
                if (Vector3.Dot(normal, Vector3.back) < 0){
                    // Kolejność wierzchołków jest nieprawidłowa, trzeba ją zmienić.
                    int temp = newTriangle.P2;
                    newTriangle.P2 = newTriangle.P3;
                    newTriangle.P3 = temp;
                }
                triangulation.Add(newTriangle);
            }       
        }
       
        createMesh();
    }

    Vector3 CalculateNormal(Vector3 p1, Vector3 p2, Vector3 p3){
        Vector3 v1 = p2 - p1;
        Vector3 v2 = p3 - p1;
        return Vector3.Cross(v1, v2).normalized;
    }



     bool isInsideCricle(Vector3 point, float radius, Vector2 circumCenter){
            return (Vector2.Distance(point, circumCenter) < radius) ? true : false;
    }

    bool isEdgeSharded(List<Triangle> badTriangles, int index, int A, int B)
    {

        int i = 0;
        foreach(Triangle badTriangle in badTriangles){
            if(index !=i){

                bool isShared = 
                    (A == badTriangle.P1 && B == badTriangle.P2) ||
                    (A == badTriangle.P2 && B == badTriangle.P1) ||
                    (A == badTriangle.P1 && B == badTriangle.P3) ||
                    (A == badTriangle.P3 && B == badTriangle.P1) ||
                    (A == badTriangle.P2 && B == badTriangle.P3) ||
                    (A == badTriangle.P3 && B == badTriangle.P2);

                if(isShared){
                     return true;   
                }
            }
            i ++;
        }
        return false;
    }

    float CalcCircumCircleRadius(Triangle triangle){
        float a = Vector2.Distance(pointList[triangle.P2], pointList[triangle.P3]);
        float b = Vector2.Distance(pointList[triangle.P3], pointList[triangle.P1]);
        float c = Vector2.Distance(pointList[triangle.P1], pointList[triangle.P2]);
        float s = (a + b + c) / 2f; //połowa obwodu trójkąta
        float P = Mathf.Sqrt(s * (s - a) * (s - b) * (s - c)); // pole trójkąta (wzór Herona)
        float R = (a*b*c) / (4*P);    //promień okręgu opisanego
        return R;
    }

    Vector2 CalcCircumCircleCenter(Triangle triangle){
    Vector2 A = new Vector2(pointList[triangle.P1].x, pointList[triangle.P1].y);
    Vector2 B = new Vector2(pointList[triangle.P2].x, pointList[triangle.P2].y);
    Vector2 C = new Vector2(pointList[triangle.P3].x, pointList[triangle.P3].y);

    Vector2 midAB = (A + B) / 2f;
    Vector2 midBC= (B + C) / 2f;
    float x, y;

    float m1 = -((B.x - A.x) / (B.y - A.y));
    float m2 = -((C.x - B.x) / (C.y - B.y));

    if(A.y == B.y){
        x = midAB.x;
         if(B.x == C.x){
            y = midBC.y;
        } else {
            y = m2* (x - midBC.x) + midBC.y;
        }
    } else if(A.x == B.x){
        y = midAB.y;
        if(B.y == C.y){
            x = midBC.x;
        } else {
            x = (y + m2 * midBC.x - midBC.y) / m2;
        }
    } else if(B.y == C.y){
        x = midBC.x;
        y = m1* (x - midAB.x) + midAB.y;
    } else if(B.x == C.x){
        y = midBC.y;
        x = (y + m1 * midAB.x - midAB.y) / m1;
    } else {
        x = (m1 * midAB.x - m2 * midBC.x + midBC.y - midAB.y) / (m1 - m2);
        y =  m1 * (x - midAB.x) + midAB.y;
    }
    return new Vector2(x, y);
}

    void Reposition(){
        List<Vector3> markers = GetComponent<MarkerInjector>().MarkersGlobalCoords();
        for(int i = 3 ; i < pointList.Count; i++){
            trackers[i].transform.position = markers[i-3]+=offset;
        }    

        Vector3[] positions = new Vector3[trackers.Count];
        for (int i = 0; i < trackers.Count; i++){
            positions[i] = trackers[i].transform.position;
        }


        meshFilter.mesh.vertices = positions;

      

    }



    void AddSuperTriangle(){
        pointList.Clear();
        triangulation.Clear();
         Triangle triangle = new Triangle(){
            P1 = 0,
            P2 = 1,
            P3 = 2
        };
        pointList.AddRange(supertTriangle);
        triangulation.Add(triangle);
        
    }



     void createMesh(){
        Mesh mesh = new Mesh();
    
       

        Vector3[] positions = new Vector3[trackers.Count];
        for (int i = 0; i < trackers.Count; i++){
            positions[i] = trackers[i].transform.position;
        }


        mesh.vertices = positions;

   

        List<Triangle> faceTriangles = new List<Triangle>();
        foreach(Triangle triangle in triangulation){
            bool isSuperTriangleChild = triangle.P1 == 0 || triangle.P1 == 1 || triangle.P1 == 2 || triangle.P2 == 0 || triangle.P2 == 1 || triangle.P2 == 2 || triangle.P3 == 0 || triangle.P3 == 1 || triangle.P3 == 2; 
            if(!isSuperTriangleChild){
                faceTriangles.Add(triangle);
            }
        }

        int[] triangulationArray = new int[faceTriangles.Count * 3];
        for (int i = 0; i < faceTriangles.Count; i++) {
            triangulationArray[i * 3] = faceTriangles[i].P1;
            triangulationArray[i * 3 + 1] = faceTriangles[i].P2;
            triangulationArray[i * 3 + 2] = faceTriangles[i].P3;
        }
 
        



        // int[] triangulationArray = new int[triangulation.Count * 3];
        // for (int i = 0; i < triangulation.Count; i++) {
        //     triangulationArray[i * 3] = triangulation[i].P1;
        //     triangulationArray[i * 3 + 1] = triangulation[i].P2;
        //     triangulationArray[i * 3 + 2] = triangulation[i].P3;
        // }

     
        mesh.triangles = triangulationArray;
        

      




        List<int> filteredTriangles = new List<int>();
        
     
            for(int i = 0; i < faceTriangles.Count; i ++){
              
                    if(!excludes.Contains(i)){
                        Debug.Log("Not excluded: "+i);
                        filteredTriangles.Add(triangulationArray[3*i]);
                        filteredTriangles.Add(triangulationArray[3*i + 1]);
                        filteredTriangles.Add(triangulationArray[3*i + 2]);
                    } else {
                          Debug.Log("Excluded: "+i);
                    }
            }

        
         

         Color[] colors = new Color[pointList.Count]; // Nowa tablica kolorów
        for (int i = 0; i < pointList.Count; i++) {
            
            colors[i] = new Color(Random.value, Random.value, Random.value);
           // Debug.Log(colors[i]);
        }


        if(faceTriangles.Count != excludes.Count){
        Mesh mesh2 = new Mesh();
        mesh2.vertices = positions;
        mesh2.colors = colors; 
        mesh2.triangles = filteredTriangles.ToArray();

        

        meshFilter.mesh = mesh2;
        meshRenderer.material = wireframe? new Material(Shader.Find("Custom/Wireframe")) : new Material(Shader.Find("Particles/Standard Unlit"));

        }
    

       //Debug.Log(excludes.Count+"/"+faceTriangles.Count);


    if(meshCollider != null){
        Destroy(meshCollider); 
    }
    //meshFilter.mesh.triangles.Length !=0
      if(faceTriangles.Count != 0){
                meshCollider = faceMesh.AddComponent<MeshCollider>();
                  meshCollider.sharedMesh = mesh;
            }
            
           
        

       

    }




    public void exclude(){
        if(pointList.Count>=6){
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit raycastHit;
                if (meshCollider.Raycast(ray, out raycastHit, 100000)){
                    
                        if (excludes.Contains(raycastHit.triangleIndex)){

                            excludes.Remove(raycastHit.triangleIndex);
                            Debug.Log("Usuwam: "+raycastHit.triangleIndex);
                        } else{
                            excludes.Add(raycastHit.triangleIndex); 
                            Debug.Log("Dodaje: "+raycastHit.triangleIndex);
                        }

                    createMesh();
                }
        }
    }

}
