using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellTypes : byte
{
    Wall,
    Empty,

    Player,

    Sofa1,
    Sofa2,
    Sofa3,
    Sofa4,
    Sofa5,
    Sofa6,
    Sofa7,
    Sofa8,

    SolvedSofa1,
    SolvedSofa2,
    SolvedSofa3,
    SolvedSofa4,
    SolvedSofa5,
    SolvedSofa6,
    SolvedSofa7,
    SolvedSofa8,
}

public class CellType
{
    public static bool IsSofa(CellTypes cellType)
    {
        if (cellType == CellTypes.Sofa1 ||
            cellType == CellTypes.Sofa2 ||
            cellType == CellTypes.Sofa3 ||
            cellType == CellTypes.Sofa4 ||
            cellType == CellTypes.Sofa5 ||
            cellType == CellTypes.Sofa6 ||
            cellType == CellTypes.Sofa7 ||
            cellType == CellTypes.Sofa8
        )
            return true;
        else
            return false;
    }
}
