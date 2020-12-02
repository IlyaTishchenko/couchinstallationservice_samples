using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Custom game grid class
public class Tiles : MonoBehaviour
{
    [SerializeField]
    public List<Tile> tilesList = null;

    [SerializeField]
    public List<int> disabledTileIndices = null;

    [Space(20)]
    [SerializeField]
    public int width;

    [SerializeField]
    public int height;

    [SerializeField]
    public float offsetX;

    [SerializeField]
    public float offsetY;

    [Space(20)]
    [SerializeField]
    public float noiseRange = 0.0f;

    public int GetTileIdByDir(int tileId, Direction dir)
    {
        foreach (var t in tilesList)
        {
            if (t.id == tileId)
                return t.GetByDir(dir);
        }

        Debug.Log("There is no tile with id: " + tileId);
        return -1;
    }

    public void SetTileIdByDir(int tileId, Direction dir, int setTileId)
    {
        foreach (var t in tilesList)
        {
            if (t.id == tileId)
            {
                switch (dir)
                {
                    case Direction.left:
                        t.left = setTileId;
                        break;
                    case Direction.right:
                        t.right = setTileId;
                        break;
                    case Direction.top:
                        t.top = setTileId;
                        break;
                    case Direction.bottom:
                        t.bottom = setTileId;
                        break;

                    case Direction.topLeft:
                        t.topLeft = setTileId;
                        break;
                    case Direction.topRight:
                        t.topRight = setTileId;
                        break;
                    case Direction.bottomLeft:
                        t.bottomLeft = setTileId;
                        break;
                    case Direction.bottomRight:
                        t.bottomRight = setTileId;
                        break;
                }
                return;
            }
        }
    }

    public Vector3 GetTilePos(int tileId)
    {
        foreach (var t in tilesList)
        {
            if (t.id == tileId)
                return transform.TransformPoint(t.position);
        }

        Debug.Log("There is no tile with id: " + tileId);
        return Vector3.zero;
    }

    public void SetTilePos(int tileId, Vector3 pos)
    {
        foreach (var t in tilesList)
        {
            if (t.id == tileId)
            {
                t.position = pos + new Vector3(Random.Range(-noiseRange, noiseRange), 0, Random.Range(-noiseRange, noiseRange));
                return;
            }
        }
    }

#if UNITY_EDITOR

    void OnDrawGizmos()
    {
        var oldColor = Gizmos.color;

        foreach (var t in tilesList)
        {
            var tilePos = transform.TransformPoint(t.position);
            var isDisabled = disabledTileIndices.IndexOf(t.id) != -1;

            if (isDisabled)
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.yellow;

            Gizmos.DrawWireCube(tilePos, Vector3.one / 8.0f);
            UnityEditor.Handles.Label(tilePos + new Vector3(0.2f, 0, 0.1f), t.id.ToString());

            Gizmos.color = Color.blue;

            if (t.left != -1 && !isDisabled && disabledTileIndices.IndexOf(t.left) == -1)
            {
                Gizmos.DrawLine(tilePos, GetTilePos(t.left));
            }
            if (t.right != -1 && !isDisabled && disabledTileIndices.IndexOf(t.right) == -1)
            {
                Gizmos.DrawLine(tilePos, GetTilePos(t.right));
            }
            if (t.top != -1 && !isDisabled && disabledTileIndices.IndexOf(t.top) == -1)
            {
                Gizmos.DrawLine(tilePos, GetTilePos(t.top));
            }
            if (t.bottom != -1 && !isDisabled && disabledTileIndices.IndexOf(t.bottom) == -1)
            {
                Gizmos.DrawLine(tilePos, GetTilePos(t.bottom));
            }

            if (t.topLeft != -1 && !isDisabled && disabledTileIndices.IndexOf(t.topLeft) == -1)
            {
                Gizmos.DrawLine(tilePos, GetTilePos(t.topLeft));
            }
            if (t.topRight != -1 && !isDisabled && disabledTileIndices.IndexOf(t.topRight) == -1)
            {
                Gizmos.DrawLine(tilePos, GetTilePos(t.topRight));
            }
            if (t.bottomLeft != -1 && !isDisabled && disabledTileIndices.IndexOf(t.bottomLeft) == -1)
            {
                Gizmos.DrawLine(tilePos, GetTilePos(t.bottomLeft));
            }
            if (t.bottomRight != -1 && !isDisabled && disabledTileIndices.IndexOf(t.bottomRight) == -1)
            {
                Gizmos.DrawLine(tilePos, GetTilePos(t.bottomRight));
            }
        }

        Gizmos.color = oldColor;
    }

    [ContextMenu("Setup Tiles")]
    void SetupTiles()
    {
        tilesList.Clear();

        var index = 0;
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                tilesList.Add(new Tile(index));
                index++;
            }
        }

        index = 0;
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                var tileId = tilesList[index].id;
                var pos = transform.position;
                pos.x = i + offsetX;
                pos.y = 0.0f;
                pos.z = -j + offsetY;
                SetTilePos(tileId, pos);

                SetTileIdByDir(tileId, Direction.left, -1);
                SetTileIdByDir(tileId, Direction.right, -1);
                SetTileIdByDir(tileId, Direction.top, -1);
                SetTileIdByDir(tileId, Direction.bottom, -1);
                SetTileIdByDir(tileId, Direction.topLeft, -1);
                SetTileIdByDir(tileId, Direction.topRight, -1);
                SetTileIdByDir(tileId, Direction.bottomLeft, -1);
                SetTileIdByDir(tileId, Direction.bottomRight, -1);

                var neighbourIndex = index - 1;
                if (neighbourIndex > -1 && neighbourIndex < tilesList.Count && tilesList[neighbourIndex] != null)
                {
                    if (index % width != 0)
                        SetTileIdByDir(tileId, Direction.left, tilesList[neighbourIndex].id);
                }

                neighbourIndex = index + 1;
                if (neighbourIndex > -1 && neighbourIndex < tilesList.Count && tilesList[neighbourIndex] != null)
                {
                    if (neighbourIndex % width != 0)
                        SetTileIdByDir(tileId, Direction.right, tilesList[neighbourIndex].id);
                }

                neighbourIndex = index - width;
                if (neighbourIndex > -1 && neighbourIndex < tilesList.Count && tilesList[neighbourIndex] != null)
                    SetTileIdByDir(tileId, Direction.top, tilesList[neighbourIndex].id);

                neighbourIndex = index + width;
                if (neighbourIndex > -1 && neighbourIndex < tilesList.Count && tilesList[neighbourIndex] != null)
                    SetTileIdByDir(tileId, Direction.bottom, tilesList[neighbourIndex].id);

                neighbourIndex = index + width - 1;
                if (neighbourIndex > -1 && neighbourIndex < tilesList.Count && tilesList[neighbourIndex] != null)
                {
                    if (index % width != 0)
                        SetTileIdByDir(tileId, Direction.bottomLeft, tilesList[neighbourIndex].id);
                }

                neighbourIndex = index + width + 1;
                if (neighbourIndex > -1 && neighbourIndex < tilesList.Count && tilesList[neighbourIndex] != null)
                {
                    if (neighbourIndex % width != 0)
                        SetTileIdByDir(tileId, Direction.bottomRight, tilesList[neighbourIndex].id);
                }

                neighbourIndex = index - width - 1;
                if (neighbourIndex > -1 && neighbourIndex < tilesList.Count && tilesList[neighbourIndex] != null)
                {
                    if (index % width != 0)
                        SetTileIdByDir(tileId, Direction.topLeft, tilesList[neighbourIndex].id);
                }

                neighbourIndex = index - width + 1;
                if (neighbourIndex > -1 && neighbourIndex < tilesList.Count && tilesList[neighbourIndex] != null)
                {
                    if (neighbourIndex % width != 0)
                        SetTileIdByDir(tileId, Direction.topRight, tilesList[neighbourIndex].id);
                }

                index++;
            }
        }

        UnityEditor.EditorUtility.SetDirty(this);
    }

#endif
}
