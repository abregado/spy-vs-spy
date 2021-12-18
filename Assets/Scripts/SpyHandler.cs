    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.PlayerLoop;
    using Random = UnityEngine.Random;

    public class SpyHandler: MonoBehaviour {
        public Spy[] spies;
        
        private float _timeRemaining;

        private TimerUI _timerUI;
        
        public void Init() {
            _timerUI = FindObjectOfType<TimerUI>();
            
            foreach (Spy spy in spies) {
                spy.Init();
                spy.StartSpy();
            }
            
            ResetTime();
        }

        private void Update() {
            if (_timerUI != null && _timeRemaining > 0f) {
                _timeRemaining -= Time.deltaTime;
                _timerUI.DisplayTime(_timeRemaining);
                if (_timeRemaining <= 0f) {
                    KillAllSpiesExcept(-1);
                }
            }

            if (AtLeastOnePlayer() && AllSpiesDead()) {
                foreach (Spy spy in spies) {
                    if (spy.isPlaying && spy.isAlive == false) {
                        spy.Respawn();    
                    }
                }
            }
            
            
        }

        private bool AllSpiesDead() {
            foreach (Spy spy in spies) {
                if (spy.isAlive && spy.isPlaying) {
                    return false;
                }
            }

            return true;
        }

        private bool AtLeastOnePlayer() {
            foreach (Spy spy in spies) {
                if (spy.isPlaying) {
                    return true;
                }
            }

            return false;
        }

        public void ResetTime() {
            _timeRemaining = G.TIME_LIMIT;
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