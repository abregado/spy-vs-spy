    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.PlayerLoop;
    using Random = UnityEngine.Random;

    public class SpyHandler: MonoBehaviour {
        public Spy[] spies;

        public void Init() {
            foreach (Spy spy in spies) {
                spy.Init();
                spy.StartSpy();
            }
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

            Debug.Assert(false, "Didnt find a respawn room");
            return null;
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