using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movable : MonoBehaviour
{
    public static float moveSpeed = 4.0f;
    public static float rotationSpeed = 720.0f;

    public virtual void MoveImmediate() {}
}
