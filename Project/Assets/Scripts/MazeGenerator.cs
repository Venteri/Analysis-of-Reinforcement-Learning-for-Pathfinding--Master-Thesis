using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Flags]
public enum WallState
{
    // 0000 -> NO WALLS
    // 1111 -> LEFT,RIGHT,UP,DOWN
    LEFT = 1,       //0001
    RIGHT = 2,      //0010
    UP = 4,         //0100
    DOWN = 8,       //1000

    VISITED = 128, //1000 0000
}

//Keeps track of the coordinates, while the maze is generating
public struct Position
{
    public int X;
    public int Y;
}

//Keeps track of which wall is breaking
public struct Neighbour
{
    public Position Position;
    public WallState SharedWall;
}

public static class MazeGenerator 
{

    //Generate Walls
    public static WallState[,] Generate(int width, int height)
    {
        WallState[,] maze = new WallState[width, height];
        WallState initial = WallState.RIGHT | WallState.LEFT | WallState.UP | WallState.DOWN;
        for (int i=0; i < width; ++i) {
            for (int j=0; j < height; ++j) 
            {
                maze[i,j] = initial; //1111
            }
        }


        return ApplyRecursiveBacktracker(maze, width, height);
    }

    //recursive backtracker (for destroying walls to create a maze)
    public static WallState[,] ApplyRecursiveBacktracker(WallState[,] maze, int width, int height)
    {
        //making RANDOM changes on the maze
        var rng = new System.Random(/*seed*/); 
        var positionStack = new Stack<Position>();
        var position = new Position { X = rng.Next(0,width), Y= rng.Next(0,height)};

        maze[position.X, position.Y] |= WallState.VISITED;
        positionStack.Push(position);

        while (positionStack.Count > 0)
        {
            //start at the current node
            var current = positionStack.Pop();
            var neighbours = GetUnvisitedNeighbours(current, maze, width, height); //minus neighbour

            if(neighbours.Count > 0)
            {
                positionStack.Push(current);

                //go to a random neighbour
                var randIndex = rng.Next(0, neighbours.Count);
                var randomNeighbour = neighbours[randIndex];

                var nPosition = randomNeighbour.Position;
                //remove shared wall between neighbours
                maze[current.X, current.Y] &= ~randomNeighbour.SharedWall;
                maze[nPosition.X, nPosition.Y] &= ~GetOppositeWall(randomNeighbour.SharedWall);

                //mark neighbour as visited
                maze[nPosition.X, nPosition.Y] |= WallState.VISITED;

                positionStack.Push(nPosition);
            }
        }

        return maze;
    }

    //returns all neighbours of the current position, which are unvisited
    private static List<Neighbour> GetUnvisitedNeighbours(Position p, WallState[,] maze, int width, int height)
    {
        var list = new List<Neighbour>();

        if(p.X > 0) //checking to the left
        {
            if(!maze[p.X -1, p.Y].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        X = p.X -1,
                        Y = p.Y
                    },
                    SharedWall = WallState.LEFT
                });
            }
        }

        if(p.Y > 0) //checking to the bottom (down)
        {
            if(!maze[p.X, p.Y - 1].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        X = p.X,
                        Y = p.Y - 1
                    },
                    SharedWall = WallState.DOWN
                });
            }
        }

        if(p.Y < height - 1) //checking to top
        {
            if(!maze[p.X, p.Y + 1].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        X = p.X,
                        Y = p.Y + 1
                    },
                    SharedWall = WallState.UP
                });
            }
        }

        if(p.X < width -1) //checking to the right
        {
            if(!maze[p.X + 1, p.Y].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        X = p.X +1,
                        Y = p.Y
                    },
                    SharedWall = WallState.RIGHT
                });
            }
        }
        return list;
    }

    //takes wallstate and returns opposite wall
    private static WallState GetOppositeWall(WallState wall)
    {
        switch(wall)
        {
            case WallState.RIGHT: return WallState.LEFT;
            case WallState.LEFT: return WallState.RIGHT;
            case WallState.UP: return WallState.DOWN;
            case WallState.DOWN: return WallState.UP;
            default: return WallState.LEFT;
        }
    }
}
