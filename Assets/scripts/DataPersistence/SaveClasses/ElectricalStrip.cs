using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SerializableData {
    public class ElectricalStrip {
        private Vector2 electricalStripSize;
        private List<SocketsRow> socketsActiveGrid;

        public ElectricalStrip(Vector2 electricalStripSize, List<SocketsRow> socketsActiveGrid) {
            this.electricalStripSize = electricalStripSize;
            this.socketsActiveGrid = socketsActiveGrid;
        }

        public ElectricalStrip() {
            Debug.LogError("SerializableData.ElectricalStrip used the default constructor, which doesn't construct properly.");
            this.electricalStripSize = new Vector2(1, 1);
            this.socketsActiveGrid = new List<SocketsRow>();
        }


        public Vector2 GetElectricalStripSize() {
            return electricalStripSize;
        }
        public List<SocketsRow> GetSocketsActiveGrid() {
            return socketsActiveGrid;
        }

        public void GetValues(out Vector2 electricalStripSize, out List<SocketsRow> socketsActiveGrid) {
            electricalStripSize = this.electricalStripSize;
            socketsActiveGrid = this.socketsActiveGrid;
        }
    }
}

