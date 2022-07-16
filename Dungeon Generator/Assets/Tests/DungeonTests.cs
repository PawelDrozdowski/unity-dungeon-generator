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

            bool foundBrokenRoom = false;
            List<Room> generatedRooms = generator.rooms;
            for (int i = 0; i < generatedRooms.Count; i++)
            {
                if (generatedRooms[i].GetActiveDoorsAmount() == 0)
                {
                    foundBrokenRoom = true;
                    Debug.Log(generatedRooms[i].name);
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
    }
}
