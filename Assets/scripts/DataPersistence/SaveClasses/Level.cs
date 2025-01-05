using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SerializableData {
    public class Level {

        //private BackgroundTypes background;
        private int levelNumber;
        private ElectricalStrip electricalStrip;
        private List<Obstacle>  allObstacles;
        private List<Plug>      allPlugs;

        public Level(int levelNumber, ElectricalStrip electricalStrip, List<Obstacle> allObstacles, List<Plug> allPlugs) {
            this.levelNumber = levelNumber;
            this.electricalStrip = electricalStrip;
            this.allObstacles = allObstacles;
            this.allPlugs = allPlugs;
        }
        public Level() {
            Debug.LogError("SerializableData.Level used the default constructor, which doesn't construct properly.");
            this.electricalStrip = new ElectricalStrip();
            this.allObstacles = new List<Obstacle>();
            this.allPlugs = new List<Plug>();
        }


        public int GetLevelNumber() {
            return levelNumber;
        }
        public ElectricalStrip GetElectricalStrip() {
            return electricalStrip;
        }
        public List<Obstacle> GetAllObstacles() {
            return allObstacles;
        }
        public List<Plug> GetAllPlugs() {
            return allPlugs;
        }

        public void GetValues(out int levelNumber, out ElectricalStrip electricalStrip, out List<Obstacle> allObstacles, out List<Plug> allPlugs) {
            levelNumber = this.levelNumber;
            electricalStrip = this.electricalStrip;
            allObstacles = this.allObstacles;
            allPlugs = this.allPlugs;
        }
    }
}
