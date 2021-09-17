using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] private List<Transform> menuPositions = new List<Transform>();
    private int position = 1;


    private void Awake() => position = 1;
    private void Update() => transform.position = Vector2.LerpUnclamped(transform.position,
        menuPositions[position].position, 4.5f * Time.deltaTime);
    
    public void MoveMenu(int _pos) => position = _pos;
}
