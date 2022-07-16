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

    [HideInInspector]
    public bool collision;
    void Start()
    {
        if (!GetComponent<RoomGenerator>())
            body.color = new Color(Random.Range(.2f, .8f), Random.Range(.2f, .8f), Random.Range(.2f, .8f));
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        collision = true;
        //Debug.Log("hit");
    }

    public void AssignAllNeighbours(Vector2[] offsets)
    {
        for (int i = 0; i < roomDoors.Length; i++)
        {
            Vector2 offset = offsets[(int)roomDoors[i].direction];
            RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, offset, RoomGenerator.prefabsDistance);
            for (int j = 0; j < hit.Length; j++)
            {
                if (hit[j].collider != null && hit[j].collider.gameObject != this.gameObject)
                {
                    roomDoors[i].leadsTo = hit[j].collider.GetComponentInChildren<Room>();
                    roomDoors[i].active = true;
                    roomDoors[i].spriteR.enabled = true;
                }
            }
        }
    }

    public int GetActiveDoorsAmount()
    {
        int output = 0;
        foreach (Doors d in roomDoors)
            if (d.active)
                output++;
        return output;
    }
}
