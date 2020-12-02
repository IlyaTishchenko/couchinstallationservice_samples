using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NewBlock
{
    public int startTileId = -1;

    public CellTypes cellType = CellTypes.Empty;

    public Vector3 position = Vector3.zero;

    [System.NonSerialized]
    public int currentTileId = -1;

    public void Initialize()
    {
        currentTileId = startTileId;
    }

    public void SetPosition(int targetTileId, Tiles tiles, Vector3 pos)
    {
        currentTileId = targetTileId;
        position = tiles.GetTilePos(currentTileId) - pos;
    }

    public int GetTileIdByDir(Direction dir, Tiles tiles)
    {
        return tiles.GetTileIdByDir(currentTileId, dir);
    }

#if UNITY_EDITOR

    public void SetPosFromStartId(Tiles tiles, Vector3 pos)
    {
        position = tiles.GetTilePos(startTileId) - pos;
    }

#endif
}