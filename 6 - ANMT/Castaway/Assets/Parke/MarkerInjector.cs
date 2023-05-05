using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerInjector : MonoBehaviour{

    public Camera camera;
    public GameObject actor;
    private MeshCollider actorMeshCollider;

    struct Marker{
        public int[] triangleIndex;
        public GameObject markerObject;
        public Vector3 barycentricCoordinate;
    }



    private List<Marker> markers;


    public List<Vector3> MarkersGlobalCoords(){
        List<Vector3> points = new List<Vector3>();
        foreach(Marker marker in markers){
            points.Add(marker.markerObject.transform.position);
        }
        return points;
    }



    void Start(){
        markers = new List<Marker>();
        actorMeshCollider = actor.GetComponent<MeshCollider>();
    }
    
    void Update(){
        MarkerTracking();

       
    }
    
    public void CreateMarker(){
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit;

        if (actorMeshCollider.Raycast(ray, out raycastHit, 100000)){
            Marker marker = new Marker();
            
            marker.barycentricCoordinate = raycastHit.barycentricCoordinate;
            marker.triangleIndex = new int[3];
            for(int i =0; i<3; i++){
                marker.triangleIndex[i] =  actorMeshCollider.sharedMesh.triangles[raycastHit.triangleIndex * 3 + i];  
            }

            Vector3 worldPosition = CalculateWorldPosition(marker);
            marker.markerObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.markerObject.transform.position = worldPosition;
            marker.markerObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            markers.Add(marker);

            GetComponent<BowyerWatson>().Triangulation(MarkersGlobalCoords());

        }

    }

    Vector3 CalculateWorldPosition(Marker marker){

                Vector3[] vertices = new Vector3[3];
                for(int i = 0; i < 3; i++){
                    vertices[i] =  actorMeshCollider.sharedMesh.vertices[marker.triangleIndex[i]];
                }

                Vector3 localPosition = 
                    marker.barycentricCoordinate.x * vertices[0] +
                    marker.barycentricCoordinate.y * vertices[1] +
                    marker.barycentricCoordinate.z * vertices[2]; 
            
            Vector3 worldPosition = actorMeshCollider.transform.TransformPoint(localPosition);
            return worldPosition;
    }


    void MarkerTracking(){
        Mesh mesh = new Mesh();
        actor.GetComponent<SkinnedMeshRenderer>().BakeMesh(mesh);
        actorMeshCollider.sharedMesh = mesh;

        foreach(Marker marker in markers){
            Vector3 worldPosition = CalculateWorldPosition(marker);
            marker.markerObject.transform.position = worldPosition;
        }
    }

}
