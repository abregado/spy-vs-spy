
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class SpyHandler: MonoBehaviour {
        public Spy[] spies;
        
        void Awake() {
            
        }
        
        public Room GetEmptyRoomForRespawn() {
            List<Room> rooms = FindObjectsOfType<Room>().ToList();
            List<Room> respawnRooms = new List<Room>();

            foreach (Room room in rooms) {
                if (room.canSpawnHere) {
                    respawnRooms.Add(room);
                }
            }

            foreach (Spy spy in spies) {
                respawnRooms.Remove(spy.currentRoom);
            }

            if (respawnRooms.Count > 0) {
                return respawnRooms[Random.Range(0, respawnRooms.Count - 1)];
            }

            return FindObjectOfType<RoomCameraSystem>().rooms[2];
        }

        public void KillAllSpiesExcept(int playerIndex) {
            for (int i = 0; i < spies.Length; i++) {
                if (i != playerIndex) {
                    Spy spy = spies[i];
                    spy.KillByWin();
                }
            }
        }
    }