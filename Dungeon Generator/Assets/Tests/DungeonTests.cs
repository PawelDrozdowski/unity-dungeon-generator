using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class DungeonTests
    {
        [UnityTest]
        public IEnumerator T01_Generator_Active()
        {
            ////uncomment if you fail any test - work on the same seed for easier debugging
            //RoomGenerator.useSeed = true;
            SceneManager.LoadScene(0);
            yield return null;

            RoomGenerator generator = Object.FindObjectOfType<RoomGenerator>();

            bool generatorActive = generator.isActiveAndEnabled && generator.generatingRooms;
            
            Assert.AreEqual(true, generatorActive);
        }

        [UnityTest]
        public IEnumerator T02_Every_Room_Has_A_Door()
        {
            //SceneManager.LoadScene(0);
            //yield return null;

            RoomGenerator generator = Object.FindObjectOfType<RoomGenerator>();

            while (generator.generatingRooms)
                yield return new WaitForSeconds(0.15f);
            yield return new WaitForSeconds(0.05f);

            bool foundBrokenRoom = false;
            for (int i = 0; i < generator.rooms.Count; i++)
            {
                if (generator.rooms[i].GetActiveDoorsAmount() == 0)
                {
                    foundBrokenRoom = true;
                    Debug.Log(generator.rooms[i].name);
                    yield return new WaitForSeconds(3);
                    break;
                }
            }
            Assert.AreEqual(false, foundBrokenRoom);
        }

        [UnityTest]
        public IEnumerator T03_All_Active_Doors_Lead_Somewhere()
        {
            RoomGenerator generator = Object.FindObjectOfType<RoomGenerator>();

            while (generator.generatingRooms)
                yield return new WaitForSeconds(0.05f);

            bool foundBrokenDoor = false;
            List<Room> generatedRooms = generator.rooms;
            for (int i = 0; i < generatedRooms.Count && !foundBrokenDoor; i++)
            {
                Room.Doors[] doors = generatedRooms[i].roomDoors;
                for (int j = 0; j < doors.Length; j++)
                    if (doors[j].active && doors[j].leadsTo == null)
                    {
                        foundBrokenDoor = true;
                        break;
                    }
            }
            Assert.AreEqual(false, foundBrokenDoor);
        }

        [UnityTest]
        public IEnumerator T04_Two_Way_Connections()
        {
            RoomGenerator generator = Object.FindObjectOfType<RoomGenerator>();

            while (generator.generatingRooms)
                yield return new WaitForSeconds(0.05f);

            bool foundBrokenConnection = false;
            List<Room> generatedRooms = generator.rooms;
            for (int i = 0; i < generatedRooms.Count && !foundBrokenConnection; i++)
            {
                Room currentRoom = generatedRooms[i];
                Room.Doors[] doors = currentRoom.roomDoors;
                for (int j = 0; j < doors.Length; j++)
                {
                    if (doors[j].active)
                    {
                        Room.Doors[] neighbourDoors = doors[j].leadsTo.roomDoors;
                        bool canGoBack = false;
                        foreach (Room.Doors d in neighbourDoors)
                        {
                            if (d.leadsTo == currentRoom)
                                canGoBack = true;
                        }
                        if (!canGoBack)
                        {
                            foundBrokenConnection = true;
                            Debug.Log($"{currentRoom.name} - no two-way connection");
                            //yield return new WaitForSeconds(2);
                            break;
                        }
                    }
                }
            }

            Assert.AreEqual(false, foundBrokenConnection);
        }

        [UnityTest]
        public IEnumerator T05_Can_Visit_Every_Room()
        {
            RoomGenerator generator = Object.FindObjectOfType<RoomGenerator>();

            while (generator.generatingRooms)
                yield return new WaitForSeconds(0.05f);

            bool foundUnreachableRoom = false;
            List<Room> generatedRooms = generator.rooms;
            Room generatorRoom = generator.generatorRoom;
            for (int i = 0; i < generatedRooms.Count && !foundUnreachableRoom; i++)
            {
                List<Room> path = generatedRooms[i].GetShortestPathTo(generatorRoom);
                if (path == null)
                {
                    foundUnreachableRoom = true;
                    Debug.Log($"{generatedRooms[i].name} isn't connected to generator (start point)");
                    //yield return new WaitForSeconds(2);
                }
            }

            Assert.AreEqual(false, foundUnreachableRoom);
        }

        [UnityTest]
        public IEnumerator T06_Correct_Furthest_Room_Distance()
        {
            RoomGenerator generator = Object.FindObjectOfType<RoomGenerator>();

            while (generator.generatingRooms)
                yield return new WaitForSeconds(0.05f);

            Room generatorRoom = generator.generatorRoom;
            Room furthest = PathManager.FindFurthestRoom(generator.rooms);
            int pathLength = furthest.GetShortestPathTo(generatorRoom).Count;


            Assert.AreEqual(pathLength, furthest.jumpsFromStart);
        }

        [UnityTest]
        public IEnumerator T07_Same_Seed_Dungeons_Are_Equal()
        {
            string dungeonToString(RoomGenerator _generator)
            {
                Room[] generatedRooms = _generator.rooms.ToArray();
                List<Room> neighbours = new List<Room>();
                for (int i = 0; i < generatedRooms.Length; i++)
                {
                    Room.Doors[] doors = generatedRooms[i].roomDoors;
                    for (int j = 0; j < doors.Length; j++)
                        neighbours.Add(doors[j].leadsTo);
                }
                return string.Join("\n", neighbours);
            }

            string generation1;
            string generation2;
            RoomGenerator.useSeed = true;
            SceneManager.LoadScene(0);
            yield return null;

            RoomGenerator generator = Object.FindObjectOfType<RoomGenerator>();

            while (generator.generatingRooms)
                yield return new WaitForSeconds(0.05f);
            generation1 = dungeonToString(generator);

            SceneManager.LoadScene(0);
            yield return null;

            generator = Object.FindObjectOfType<RoomGenerator>();

            while (generator.generatingRooms)
                yield return new WaitForSeconds(0.05f);

            generation2 = dungeonToString(generator);

            Assert.AreEqual(generation1, generation2);
        }
    }
}
