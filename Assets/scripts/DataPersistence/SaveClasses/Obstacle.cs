using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SerializableData {
    public class PlugObstacle {
        private int obstacleId;
        private ObstacleTypes obstacleType;
        private Index2D socketsIndex;
        private List<CableIndexAndDirection> cableIndexAndDirections;

        public PlugObstacle(int obstacleId, ObstacleTypes obstacleType, Index2D socketsIndex, List<CableIndexAndDirection> cableIndexAndDirections) {
            if(obstacleType != ObstacleTypes.Plug) {
                Debug.LogError("A non-plug obstacle was saved as a plug obstacle!!");
            }
            this.obstacleId = obstacleId;
            this.obstacleType = obstacleType;
            this.socketsIndex = socketsIndex;
            this.cableIndexAndDirections = cableIndexAndDirections;
        }

        public PlugObstacle() {
            Debug.LogError("SerializableData.Obstacle used the default constructor, which doesn't construct properly.");
            this.obstacleId = 0;
            this.obstacleType = ObstacleTypes.Plug;
            this.socketsIndex = new Index2D(-1, -1);
            this.cableIndexAndDirections = new List<CableIndexAndDirection>();
        }


        public int GetObstacleId() {
            return obstacleId;
        }
        public ObstacleTypes GetObstacleType() {
            return obstacleType;
        }
        public Index2D GetSocketsIndex() {
            return socketsIndex;
        }
        public List<CableIndexAndDirection> GetCableIndexAndDirections() {
            return cableIndexAndDirections;
        }

        public void GetValues(out int obstacleId, out ObstacleTypes obstacleType, out Index2D socketsIndex, out List<CableIndexAndDirection> cableIndexAndDirections) {
            obstacleId = this.obstacleId;
            obstacleType = this.obstacleType;
            socketsIndex = this.socketsIndex;
            cableIndexAndDirections = this.cableIndexAndDirections;
        }
    }
}

