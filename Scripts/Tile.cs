using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tile
{
    public Vector3 position = Vector3.zero;

    public int id = -1;

    public int left = -1;
    public int right = -1;
    public int top = -1;
    public int bottom = -1;

    public int topLeft = -1;
    public int topRight = -1;
    public int bottomLeft = -1;
    public int bottomRight = -1;

    public Tile(int _id)
    {
        id = _id;
    }

    public int GetByDir(Direction dir)
    {
        switch (dir)
        {
            case Direction.left:
                return left;
            case Direction.right:
                return right;
            case Direction.top:
                return top;
            case Direction.bottom:
                return bottom;

            case Direction.topLeft:
                return topLeft;
            case Direction.topRight:
                return topRight;
            case Direction.bottomLeft:
                return bottomLeft;
            case Direction.bottomRight:
                return bottomRight;
        }

        Debug.LogError("There is no direction: " + dir);
        return -1;
    }
}

public enum Direction
{
    left,
    right,
    top,
    bottom,

    topLeft,
    topRight,
    bottomLeft,
    bottomRight,
}
