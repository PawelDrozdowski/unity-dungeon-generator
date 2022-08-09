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
    private SpriteRenderer body;

    [SerializeField]
    public SpriteRenderer centerDec;

    public Doors[] roomDoors = new Doors[4];

    [HideInInspector]
    public bool collision;

    public int jumpsFromStart = - 1;

    private void OnTriggerEnter2D(Collider2D col)
    {
        collision = true;
        //Debug.Log("hit");
    }

    public void AssignAllNeighbours(Vector2[] offsets)
    {
        for (int i = 0; i < roomDoors.Length; i++)
        {
            int dir = (int)roomDoors[i].direction;
            Vector2 offset = offsets[dir];

            RaycastHit2D[] hit = Physics2D.RaycastAll(roomDoors[i].roomPart.position, offset, RoomGenerator.prefabsDistance);
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

    public List<Room> GetShortestPathTo(in Room target, List<Room> steps = null, List<Room> shortest = null)
    {
        bool CanChangeShortest(in List<Room> _steps, in List<Room> _shortest)
        {
            return _shortest == null || _shortest.Count > _steps.Count;
        }

        if (steps == null)
            steps = new List<Room>();
        steps.Add(this);
        if (shortest != null && steps.Count > shortest.Count)
            return null;

        Room selected = GetClosestToStartNeighbour();

        if (selected == target)
        {
            if (CanChangeShortest(steps, shortest))
                shortest = new List<Room>(steps);
        }
        //tell closest neighbour to look for the target
        List<Room> result = selected.GetShortestPathTo(target, new List<Room>(steps), shortest);
        if (result != null)
            if (CanChangeShortest(result, shortest))
                shortest = result;

        return shortest;
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

    public bool IsColliding(float dist)
    {
        bool collision;
        RaycastHit2D[] hit = Physics2D
            .RaycastAll(transform.position, Vector2.one, dist);
        collision = hit.Length > 1;
        return collision;
    }
}
