using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Room : MonoBehaviour
{
    public enum Directions
    {
        up,
        right,
        down,
        left
    }

    [System.Serializable]
    public struct Doors
    {
        [HideInInspector]
        public bool active;

        public Directions direction;
        public SpriteRenderer spriteR;
        public Room leadsTo;
    }

    [SerializeField]
    private SpriteRenderer body;

    [SerializeField]
    public SpriteRenderer centerDec;

    public Doors[] roomDoors = new Doors[4];
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
