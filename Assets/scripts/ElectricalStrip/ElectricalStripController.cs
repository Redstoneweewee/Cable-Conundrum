using System;
using System.Collections;
using UnityEngine;

public class ElectricalStripController : MonoBehaviour, IDebugC {
    public DebugC DebugC { get ; set; }
    public Transform[,] SocketsGrid { get; set; }
    public int[,] PlugsGrid { get; set; }  //contains the plug ids, starting from 1. A value of 0 means there is no plug.
    public int[,] AllCablesGrid { get; set; }  //contains the number of cables at each index. A value of 0 means there are no cables.
    private JointsController jointsController;
    private ElectricalStripValues electricalStripValues;
    private ElectricalStripSizeController electricalStripSizeController;


    // Start is called before the first frame update
    void Start() {
        DebugC = DebugC.Get();
        jointsController = FindObjectOfType<JointsController>();
        electricalStripValues         = GetComponent<ElectricalStripValues>();
        electricalStripSizeController = electricalStripValues.ElectricalStripSizeController;
        electricalStripSizeController.RenewSockets();
        StartCoroutine(InitializeAllCableGrids());
        StartCoroutine(InitializePlugsGrid());
    }

    public void RenewPlugsGrid() {
        Transform[,] jointsGrid = jointsController.JointsGrid;
        PlugsGrid = new int[jointsGrid.GetLength(0), jointsGrid.GetLength(1)];
        Plug[] allPlugs = FindObjectsOfType<Plug>();
        foreach(Plug plug in allPlugs) {
            if(plug.isPluggedIn) {
                foreach(Vector2 localPlugPositionsTakenUp in plug.localJointPositionsTakenUp) {
                    Vector3 actualPosition = plug.transform.position + new Vector3(localPlugPositionsTakenUp.x, localPlugPositionsTakenUp.y, 0);
                    float   subJointLength  = Constants.jointDistance/2;
                    Vector2 distanceFromTopLeftJoint = new Vector2(actualPosition.x - jointsGrid[0,0].position.x, jointsGrid[0,0].position.y - actualPosition.y);
                    Index2D gridIndex  = new Index2D(((int)(distanceFromTopLeftJoint.x/subJointLength)+1)/2, ((int)(distanceFromTopLeftJoint.y/subJointLength)+1)/2);
                    gridIndex          = new Index2D(Math.Clamp(gridIndex.y, 0, jointsGrid.GetLength(0)-1), Math.Clamp(gridIndex.x, 0, jointsGrid.GetLength(1)-1));
                    PlugsGrid[gridIndex.x, gridIndex.y] = plug.id;
                }
            }
        }
        
        string text = "";
        for(int i=0; i<PlugsGrid.GetLength(0); i++) {
            text += "| ";
            for(int j=0; j<PlugsGrid.GetLength(1); j++) {
                if(PlugsGrid[i,j] > 0) { text += "*  "; }
                else { text += "-  "; }
            }
            text += " |\n";
        }
        Debug.Log("PlugsGrid: \n"+text);
    }

    private IEnumerator InitializeAllCableGrids() {
        yield return new WaitForSeconds(0.01f);
        RenewAllCableGrids();
    }
    private IEnumerator InitializePlugsGrid() {
        yield return new WaitForSeconds(0.01f);
        RenewPlugsGrid();
    }

    public void RenewAllCableGrids() {
        CableParentAttributes[] allCableAttributes = FindObjectsOfType<CableParentAttributes>();

        AllCablesGrid = new int[jointsController.JointsGrid.GetLength(0), jointsController.JointsGrid.GetLength(1)];
        foreach(CableParentAttributes cableParentAttribute in allCableAttributes) {
            if(cableParentAttribute.cableGrid == null) { DebugC.Log($"CableGrid of {cableParentAttribute.transform.name} is null."); continue; }
            if(cableParentAttribute.cableGrid[0,0] == null) { DebugC.Log($"CableGrid of {cableParentAttribute.transform.name} is null."); continue; }
            for(int i=0; i<cableParentAttribute.cableGrid.GetLength(0); i++) {
                for(int j=0; j<cableParentAttribute.cableGrid.GetLength(1); j++) {
                    if(cableParentAttribute.cableGrid[i,j].hasCable) { AllCablesGrid[i,j] += 1; }
                }
            }
        }
        string text = "";
        for(int i=0; i<AllCablesGrid.GetLength(0); i++) {
            text += "| ";
            for(int j=0; j<AllCablesGrid.GetLength(1); j++) {
                text += AllCablesGrid[i,j]+" ";
            }
            text += " |\n";
        }
        Debug.Log("AllCablesGrid: \n"+text);
    }
}
