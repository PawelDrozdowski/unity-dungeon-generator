using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Room))]
public class RoomGenerator : MonoBehaviour
{
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

            //yield return new WaitForFixedUpdate();//best performance
            yield return new WaitForSeconds(0.2f);//animated look

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
}
