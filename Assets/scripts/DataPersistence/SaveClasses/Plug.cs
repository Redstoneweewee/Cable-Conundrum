using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SerializableData {
    public class Plug {
        private int plugId;
        private Index2D jointIndex;
        private bool isPluggedIn;
        private List<CableIndexAndDirection> cableIndexAndDirections;


        public Plug(int plugId, Index2D jointIndex, bool isPluggedIn, List<CableIndexAndDirection> cableIndexAndDirections) {
            this.plugId = plugId;
            this.jointIndex = jointIndex;
            this.isPluggedIn = isPluggedIn;
            this.cableIndexAndDirections = cableIndexAndDirections;
        }
        public Plug(int plugId, bool isPluggedIn) {
            this.plugId = plugId;
            this.isPluggedIn = isPluggedIn;
            this.jointIndex = new Index2D(-1, -1);
            this.cableIndexAndDirections = new List<CableIndexAndDirection>();
        }
        public Plug() {
            Debug.LogError("SerializableData.Plug used the default constructor, which doesn't construct properly.");
            this.plugId = 0;
            this.jointIndex = new Index2D(-1, -1);
            this.isPluggedIn = false;
            this.cableIndexAndDirections = new List<CableIndexAndDirection>();
        }

        public int GetPlugId() {
            return plugId;
        }
        public Index2D GetJointIndex() {
            return jointIndex;
        }
        public bool GetIsPluggedIn() {
            return isPluggedIn;
        }
        public List<CableIndexAndDirection> GetCableIndexAndDirections() {
            return cableIndexAndDirections;
        }

        public void GetValues(out int plugId, out Index2D jointIndex, out bool isPluggedIn, out List<CableIndexAndDirection> cableIndexAndDirections) {
            plugId = this.plugId;
            jointIndex = this.jointIndex;
            isPluggedIn = this.isPluggedIn;
            cableIndexAndDirections = this.cableIndexAndDirections;
        }
    }

    public struct CableIndexAndDirection {
        public int previousIndex;
        public Directions endingDirection;

        public CableIndexAndDirection(int previousIndex, Directions endingDirection) {
            this.previousIndex = previousIndex;
            this.endingDirection = endingDirection;
        }
    }
}

