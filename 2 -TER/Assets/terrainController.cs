using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class terrainController : MonoBehaviour
{
    
    private Terrain terrain;
    private TerrainData terrainData;
    private float[,] mapHeights;
    [SerializeField] private Texture2D map;
    private Texture2D colorTexture;

    private float[,] originMapHeights;
    private int originX = 128;
    private int originY = 256;
    private int length = 128;

    [Range(0f,1f)]
    public float baseRatio = 0.5f;

    [Range(0f,1f)]
    public float amplitude = 0.3f;
    public float frequency = 2f;
    public float speed = 4f;


    void Start(){
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;
        mapHeights = getMapHeightsFromTexture();
        

        terrainData.SetHeights(0,0, mapHeights);
        originMapHeights = terrainData.GetHeights(originX, originY, length, length);
    }

    
    void Update(){
        animateTerrain();
    }


    float[,] getMapHeightsFromTexture(){

        float [,] mapHeights = new float[map.width, map.height];

        for(int x = 0; x < map.width; x++){
            for(int y =0; y < map.height; y++){
                mapHeights[x,y] = map.GetPixel(y,x).grayscale; //x i y odwrócone, bo obrazek jest odbity
            }
        }
        
        return mapHeights;
    }

    void animateTerrain(){
            mapHeights = terrainData.GetHeights(originX, originY, length, length);


            for(int x = 0; x < length; x++){
                for(int y =0; y < length; y++){
                    if(originMapHeights[x,y] > 0.8f)
                        mapHeights[x,y] = 
                            baseRatio * originMapHeights[x,y] + 
                        
                    
                    (1f - baseRatio) * amplitude * ((Mathf.Sin(((float)y/(float)length) * Mathf.PI * frequency+ Time.time *speed) +1f)/2f) ;

                }
            }   

            terrainData.SetHeights(originX,originY, mapHeights);
    }

   
}


