using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    public static readonly int GENERATOR_PATHS_AMOUNT = 3;

    public static void AssignJumps(Room element)
    {
        List<Room> neighbours = element.GetNeighbours();
        element.jumpsFromStart = GetJumps(neighbours);
        foreach (var n in neighbours)
        {
            if (n.jumpsFromStart == -1 
                || Mathf.Abs(n.jumpsFromStart - element.jumpsFromStart) > 1)
                AssignJumps(n);
        }
    }

    private static int GetJumps(List<Room> neighbours)
    {
        int minJumps = -1;
        foreach (Room n in neighbours)
            if (minJumps == -1 || (n.jumpsFromStart > -1 && minJumps > n.jumpsFromStart))
                minJumps = n.jumpsFromStart;
        return minJumps + 1;
    }

    public static int[] PathRoomsAmount(int amountToGenerate, bool equal = false)
    {

        int[] output = new int[GENERATOR_PATHS_AMOUNT];
        int roomsAmount = amountToGenerate;
        //fill all but the last element
        for (int i = 0; i < GENERATOR_PATHS_AMOUNT - 1; i++)
        {
            if (equal)
                output[i] = amountToGenerate / GENERATOR_PATHS_AMOUNT;
            else
                output[i] = Random.Range(1, roomsAmount / 2);

            roomsAmount -= output[i];
        }
        //fill the last element
        output[GENERATOR_PATHS_AMOUNT - 1] = roomsAmount;
        //Debug.Log("Rooms per path: " + string.Join(", ",output));
        return output;
    }

    public static void SetPathToRoom(Room r, Room target)
    {
        if (!r || !target) return;

        List<Room> steps = r.GetShortestPathTo(target);
        //Debug.Log($"Steps: {steps.Count}\n{string.Join<Room>("\n", steps.ToArray())}");
        for (int i = 1; i < steps.Count; i++)
            steps[i].MarkAsPathToBossRoom();
    }

    public static Room FindFurthestRoom(List<Room> rooms)
    {
        int index = -1;
        int biggestDist = 0;
        for (int i = rooms.Count - 1; i >= 0; i--)
        {
            int dist = rooms[i].jumpsFromStart;

            if (dist > biggestDist)
            {
                index = i;
                biggestDist = dist;
            }
        }

        if (index != -1)
            return rooms[index];
        else
            return null;
    }

    public static Room[] FindPlaceFor2x2(List<Room> rooms, Room generatorRoom)
    {
        Room start = null;
        Room right = null;
        Room down = null;
        Room downRight = null;

        //search for a correct place on grid (can't place on generator, must find 4 rooms to replace)
        for (int i = 0; i < rooms.Count; i++)
        {
            right = rooms[i].roomDoors[(int)Room.Directions.right].leadsTo;
            down = rooms[i].roomDoors[(int)Room.Directions.down].leadsTo;

            if (right == null || down == null)
                continue;
            if (right == generatorRoom || down == generatorRoom)
                continue;

            downRight = down.roomDoors[(int)Room.Directions.right].leadsTo;

            if (downRight != null && downRight != generatorRoom)
            {
                start = rooms[i];
                break;
            }
        }
        return new Room[] { start, right, down, downRight };
    }
}
