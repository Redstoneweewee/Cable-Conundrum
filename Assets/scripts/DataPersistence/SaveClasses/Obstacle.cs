using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SerializableData {
    public class Obstacle {
        private int obstacleId;
        private ObstacleTypes obstacleType;
        private Index2D jointIndex;
        private List<CableIndexAndDirection> cableIndexAndDirections;

        public Obstacle(int obstacleId, ObstacleTypes obstacleType, Index2D jointIndex, List<CableIndexAndDirection> cableIndexAndDirections) {
            this.obstacleId = obstacleId;
            this.obstacleType = obstacleType;
            this.jointIndex = jointIndex;
            this.cableIndexAndDirections = cableIndexAndDirections;
        }

        public Obstacle() {
            Debug.LogError("SerializableData.Obstacle used the default constructor, which doesn't construct properly.");
            this.obstacleId = 0;
            this.obstacleType = ObstacleTypes.Plug;
            this.jointIndex = new Index2D(-1, -1);
            this.cableIndexAndDirections = new List<CableIndexAndDirection>();
        }


        public int GetObstacleId() {
            return obstacleId;
        }
        public ObstacleTypes GetObstacleType() {
            return obstacleType;
        }
        public Index2D GetJointIndex() {
            return jointIndex;
        }
        public List<CableIndexAndDirection> GetCableIndexAndDirections() {
            return cableIndexAndDirections;
        }

        public void GetValues(out int obstacleId, out ObstacleTypes obstacleType, out Index2D jointIndex, out List<CableIndexAndDirection> cableIndexAndDirections) {
            obstacleId = this.obstacleId;
            obstacleType = this.obstacleType;
            jointIndex = this.jointIndex;
            cableIndexAndDirections = this.cableIndexAndDirections;
        }
    }
}

