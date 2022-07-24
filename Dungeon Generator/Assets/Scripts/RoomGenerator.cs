using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Room))]
public class RoomGenerator : MonoBehaviour
{
    public static bool useSeed = false;
    public static readonly int seed = 40;
    [SerializeField]
    int amountToGenerate = 32;

    public Room roomPrefab1x1;

    public static readonly float prefabsDistance = 1;
    public readonly Vector2[] offsets = new Vector2[]
    {
        Vector2.up * prefabsDistance,
        Vector2.right * prefabsDistance,
        Vector2.down * prefabsDistance,
        Vector2.left * prefabsDistance,
    };

    public List<Room> rooms;

    private Transform roomsContainer;
    public bool generatingRooms;
    public Room generatorRoom;

    private void Awake()
    {
        if (useSeed)
            Random.InitState(seed);
        rooms = new List<Room>();
        generatorRoom = GetComponent<Room>();
        roomsContainer = new GameObject("Rooms").transform;
    }

    IEnumerator Start()
    {
        StartCoroutine(GenerateRooms(roomPrefab1x1));
        while (generatingRooms)
            yield return new WaitForSeconds(0.05f);

        GenerateDoors();

        Room furthest = FindFurthestRoom();
        if(furthest != null)
            furthest.MarkAsBossRoom();
        SetPathToRoom(furthest);
    }
    //singular path, multi direction generation
    private IEnumerator GenerateRooms(Room prefab)
    {
        generatingRooms = true;
        Room.Directions dir;
        Vector2 offset;
        Vector2 last = transform.position;

        for (int i = 0; i < amountToGenerate; i++)
        {
            dir = (Room.Directions)Random.Range(0, 4);
            offset = offsets[(int)dir];
            Vector2 newRoomPos = last + offset;

            Room newRoom = Instantiate(prefab, newRoomPos, Quaternion.identity, roomsContainer);
            newRoom.gameObject.name = "Room " + rooms.Count;

            yield return new WaitForFixedUpdate();//best performance
            //yield return new WaitForSeconds(0.2f);//animated look

            last = newRoomPos;
            if (newRoom.collision)
            {
                newRoom.gameObject.SetActive(false);
                Destroy(newRoom.gameObject);
                i--;
                continue;
            }
            rooms.Add(newRoom);
            //Debug.Log($"Generated: {dir}");
        }
        generatingRooms = false;
        yield return null;
    }

    private void GenerateDoors()
    {
        generatorRoom.AssignAllNeighbours(offsets);

        for (int i = 0; i < rooms.Count; i++)
        {
            rooms[i].AssignAllNeighbours(offsets);
        }
    }
    private Room FindFurthestRoom()
    {
        List<Room> checkedRooms = new List<Room>();

        int index = -1;
        int biggestDist = 0;
        for (int i = rooms.Count - 1; i >= 0; i--)
        {
            if (checkedRooms.Contains(rooms[i]))
                continue;

            //check how many rooms have to be visited before reaching target
            List<Room> path = rooms[i].GetShortestPathTo(generatorRoom);
            if (path == null)
            {
                Debug.LogError($"Paths error - {rooms[i].name}can't lead to generator");
                break;
            }
            int dist = path.Count;
            if (dist > biggestDist)
            {
                index = i;
                biggestDist = dist;
            }

            //mark visited rooms as checked
            for (int j = 0; j < path.Count; j++)
                if (!checkedRooms.Contains(path[j]))
                    checkedRooms.Add(path[j]);

            //Debug.Log(checkedRooms.Count);
        }
        if (index != -1)
            return rooms[index];
        else
            return null;
    }

    private void SetPathToRoom(Room r)
    {
        if (r)
        {
            List<Room> steps = r.GetShortestPathTo(generatorRoom);
            //Debug.Log($"Steps: {steps.Count}\n{string.Join<Room>("\n", steps.ToArray())}");
            for (int i = 1; i < steps.Count; i++)
                steps[i].MarkAsPathToBossRoom();
        }
        else
            Debug.LogError("FindFurthestRoom() returned null - cannot set path.");
    }
    //based on physical distance
    //private Room FindFurthestRoom()
    //{
    //    int index = -1;
    //    float biggestDist = 0;
    //    for (int i = 0; i < rooms.Count; i++)
    //    {
    //        float dist = (transform.position - rooms[i].transform.position).sqrMagnitude;
    //        if (dist > biggestDist)
    //        {
    //            index = i;
    //            biggestDist = dist;
    //        }
    //    }
    //    if (index != -1)
    //        return rooms[index];
    //    else
    //        return null;
    //}
}
