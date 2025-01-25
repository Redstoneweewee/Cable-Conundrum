using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SerializableData {
    public class Level {

        //private BackgroundTypes background;
        private int levelNumber;
        private ElectricalStrip electricalStrip;
        private List<PlugObstacle>  allPlugObstacles;
        private List<Plug>      allPlugs;

        public Level(int levelNumber, ElectricalStrip electricalStrip, List<PlugObstacle> allPlugObstacles, List<Plug> allPlugs) {
            this.levelNumber = levelNumber;
            this.electricalStrip = electricalStrip;
            this.allPlugObstacles = allPlugObstacles;
            this.allPlugs = allPlugs;
        }
        public Level() {
            Debug.LogError("SerializableData.Level used the default constructor, which doesn't construct properly.");
            this.electricalStrip = new ElectricalStrip();
            this.allPlugObstacles = new List<PlugObstacle>();
            this.allPlugs = new List<Plug>();
        }


        public int GetLevelNumber() {
            return levelNumber;
        }
        public ElectricalStrip GetElectricalStrip() {
            return electricalStrip;
        }
        public List<PlugObstacle> GetAllPlugObstacles() {
            return allPlugObstacles;
        }
        public List<Plug> GetAllPlugs() {
            return allPlugs;
        }

        public void GetValues(out int levelNumber, out ElectricalStrip electricalStrip, out List<PlugObstacle> allPlugObstacles, out List<Plug> allPlugs) {
            levelNumber = this.levelNumber;
            electricalStrip = this.electricalStrip;
            allPlugObstacles = this.allPlugObstacles;
            allPlugs = this.allPlugs;
        }
    }
}
