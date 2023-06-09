using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeRenderer : MonoBehaviour
{

    [SerializeField]
    [Range(1,50)]
    private int width;

    [SerializeField]
    private float size = 1f; //size of the prefab (each cell)

    [SerializeField]
    [Range(1,50)]
    private int height;

    [SerializeField]
    private Transform wallPrefab = null;

    [SerializeField]
    private Transform floorPrefab = null;

    // Start is called before the first frame update
    void Start()
    {
        var maze = MazeGenerator.Generate(width, height);
        Draw(maze);
    }

    private void Draw(WallState[,] maze) 
    { 

        var floor = Instantiate(floorPrefab, transform);
        floor.localScale = new Vector3(size, 1, size);
        floor.position = new Vector3(-0.5f, 0, -0.5f);
        for(int i=0; i< width; ++i) {
            for (int j=0; j < height; ++j) {
                var cell = maze[i,j];
                var position = new Vector3(-width/2 + i, 0.5f, -height/2 + j); //Position of each cell would be offset

                //check if each of the walls exist
                if(cell.HasFlag(WallState.UP))
                {
                    var topWall = Instantiate(wallPrefab, transform) as Transform;
                    topWall.position = position + new Vector3(0, 0, 1f/2); //Offset
                    topWall.localScale = new Vector3(1f, topWall.localScale.y, topWall.localScale.z);
                }

                //check if cell has a left wall
                if(cell.HasFlag(WallState.LEFT))
                {
                    var leftWall = Instantiate(wallPrefab, transform) as Transform;
                    leftWall.position = position + new Vector3(-1f/2, 0, 0);
                    leftWall.localScale = new Vector3(1f, leftWall.localScale.y, leftWall.localScale.z);
                    leftWall.eulerAngles = new Vector3(0, 90, 0);
                }

                if(i == width - 1)
                {
                    if(cell.HasFlag(WallState.RIGHT))
                    {
                        var rightWall = Instantiate(wallPrefab, transform) as Transform;
                        rightWall.position = position + new Vector3(+1f/2, 0, 0);
                        rightWall.localScale = new Vector3(1f, rightWall.localScale.y, rightWall.localScale.z);
                        rightWall.eulerAngles = new Vector3(0, 90, 0);
                    }
                }

                if(j==0)
                {
                    if(cell.HasFlag(WallState.DOWN))
                    {
                        var bottomWall = Instantiate(wallPrefab, transform) as Transform;
                        bottomWall.position = position + new Vector3(0, 0, -1f/2);
                        bottomWall.localScale = new Vector3(1f, bottomWall.localScale.y, bottomWall.localScale.z);
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
