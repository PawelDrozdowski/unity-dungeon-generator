using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Room : MonoBehaviour
{
    public enum Directions
    {
        right,
        left,
        up,
        down,
        hexRightUp,
        hexRightDown,
        hexLeftDown,
        hexLeftUp
    }

    [System.Serializable]
    public struct Doors
    {
        [HideInInspector]
        public bool active;

        public Transform roomPart;
        public Directions direction;
        public SpriteRenderer spriteR;
        public Room leadsTo;
    }

    [SerializeField]
    public SpriteRenderer body;

    [SerializeField]
    public SpriteRenderer centerDec;

    private BoxCollider2D myCollider;

    public Doors[] roomDoors = new Doors[4];

    [HideInInspector]
    public bool collision;

    public int jumpsFromStart = - 1;

    private void Awake()
    {
        myCollider = GetComponent<BoxCollider2D>();
        myCollider.isTrigger = true;
        //GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
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
            //if (roomDoors[i].active) continue; //already set by a neighbour

            int dir = (int)roomDoors[i].direction;
            Vector2 offset = offsets[dir];

            RaycastHit2D[] hit = Physics2D.RaycastAll(roomDoors[i].roomPart.position, offset, RoomGenerator.prefabsDistance);
            for (int j = 0; j < hit.Length; j++)
            {
                if (hit[j].collider != null && hit[j].collider.gameObject != this.gameObject)
                {
                    Room neighbour = hit[j].collider.GetComponentInChildren<Room>();
                    OpenDoor(i, neighbour);
                }
            }
        }
    }

    private void OpenDoor(int i, Room neighbour)
    {
        roomDoors[i].leadsTo = neighbour;
        roomDoors[i].active = true;
        roomDoors[i].spriteR.enabled = true;
    }

    public void SetRandomBodyColor()
    {
        body.color = new Color(Random.Range(.2f, .8f), Random.Range(.2f, .8f), Random.Range(.2f, .8f));
    }

    public void MarkAsBossRoom()
    {
        centerDec.color = Color.red;
        centerDec.transform.localScale *= 1.2f;
    }
    public void MarkAsPathToBossRoom()
    {
        centerDec.color = Color.red;
        body.color = Color.grey;
    }

    public List<Room> GetShortestPathTo(in Room target)
    {
        List<Room> steps = new List<Room>();
        steps.Add(this);

        Room next = GetClosestToStartNeighbour();
        while (next != target)
        {
            if (next.jumpsFromStart == 0 && next != target) {
                Debug.LogError($"Bad pathfinding for: {next.gameObject.name}");
                return steps;
            }

            steps.Add(next);
            next = next.GetClosestToStartNeighbour();
        }

        return steps;
    }

    public int GetActiveDoorsAmount()
    {
        int output = 0;
        foreach (Doors d in roomDoors)
            if (d.active)
                output++;
        return output;
    }

    public List<Room> GetNeighbours()
    {
        List<Room> output = new List<Room>();
        foreach (Doors d in roomDoors)
            if (d.active)
                output.Add(d.leadsTo);
        return output;
    }

    public Room GetClosestToStartNeighbour()
    {
        //return this if called on start room
        Room output = this;

        foreach (Doors d in roomDoors)
            if (d.active)
                if (output.jumpsFromStart >= d.leadsTo.jumpsFromStart)
                    output = d.leadsTo;
        return output;
    }
    public Room GetFurthestFromStartNeighbour()
    {
        //return this if called on start room
        Room output = this;

        foreach (Doors d in roomDoors)
            if (d.active)
                if (output.jumpsFromStart < d.leadsTo.jumpsFromStart)
                    output = d.leadsTo;
        return output;
    }

    public bool IsCollidingForPooled(List<int> chunk, List<Room> rooms, Vector2 generatorPosition)
    {
        bool roomsCollision = false;
        Vector2 me = transform.position;
        for (int i = chunk.Count-1; i >= 0; i--)
        {
            Vector2 target = rooms[chunk[i]].transform.position;
            if (Mathf.Abs(target.y - me.y) > 0.01f
                || Mathf.Abs(target.x - me.x) > 0.01f) continue;

            if (rooms[chunk[i]] == this) continue;

            //Debug.Log((transform.position - rooms[i].transform.position).sqrMagnitude);
            if ((me - target).sqrMagnitude < 0.2f)
            {
                roomsCollision = true;
                break;
            }
        }
        bool generatorColission = (me - generatorPosition).sqrMagnitude < 0.01f;
        return roomsCollision || generatorColission;
    }

    public static Directions GetOppositeDirection(Directions d)
    {
        //a quick way of finding the opposite direction (useful in doors pairing)
        //the is a simpler way of it if you don't want both hex and square rooms
        Directions output;
        switch (d)
        {
            case Directions.right:
                output = Directions.left;
                break;
            case Directions.left:
                output = Directions.right;
                break;
            case Directions.up:
                output = Directions.down;
                break;
            case Directions.down:
                output = Directions.up;
                break;
            case Directions.hexRightUp:
                output = Directions.hexLeftDown;
                break;
            case Directions.hexRightDown:
                output = Directions.hexLeftUp;
                break;
            case Directions.hexLeftDown:
                output = Directions.hexRightUp;
                break;
            case Directions.hexLeftUp:
                output = Directions.hexRightDown;
                break;
            default:
                output = Directions.up;
                break;
        }
        return output;     
    }

    public static int GetIndexOfMatchingNeighbourDoor(int directionAsInt, int doorsAmount)
    {
        //return directionAsInt if a square, otherwise reduce by 2 and then return
        return doorsAmount > 4 ? directionAsInt - 2 : directionAsInt;
    }

    //public bool IsColliding(float dist)
    //{
    //    bool collision;
    //    RaycastHit2D[] hit = Physics2D
    //        .RaycastAll(transform.position, Vector2.one, dist);
    //    collision = hit.Length > 1;
    //    return collision;
    //}
}
