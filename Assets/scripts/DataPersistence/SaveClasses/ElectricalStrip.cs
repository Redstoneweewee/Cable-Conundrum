using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SerializableData {
    public class ElectricalStrip {
        private IntSize electricalStripSize;
        private List<SocketsRow> socketsActiveGrid;

        public ElectricalStrip(IntSize electricalStripSize, List<SocketsRow> socketsActiveGrid) {
            this.electricalStripSize = electricalStripSize;
            this.socketsActiveGrid = socketsActiveGrid;
        }

        public ElectricalStrip() {
            Debug.LogError("SerializableData.ElectricalStrip used the default constructor, which doesn't construct properly.");
            this.electricalStripSize = new IntSize(0, 0);
            this.socketsActiveGrid = new List<SocketsRow>();
        }


        public IntSize GetElectricalStripSize() {
            return electricalStripSize;
        }
        public List<SocketsRow> GetSocketsActiveGrid() {
            return socketsActiveGrid;
        }

        public void GetValues(out IntSize electricalStripSize, out List<SocketsRow> socketsActiveGrid) {
            electricalStripSize = this.electricalStripSize;
            socketsActiveGrid = this.socketsActiveGrid;
        }
    }
}

