using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;


public class UnitOrders : MonoBehaviour
{
 ////////////// Structs,Enums
    enum DestinationType
    {
        Line,Mine,Warehouse 
    }     




 ////////////// Variables
    private (Vector3 Position, DestinationType Type) _CurrentDestiantion;
    public NavMeshAgent _NavMeshAgent;

    [HideInInspector] public OrderManager.Line? AssignedLine;                                                                  // null if unit is worker (no combat lines assigned)
    [HideInInspector] public bool IsCarryingOre = false;
    public GameObject GoldOreMesh;
    public float speed = 0.5f;



 ////////////// Functions
    public void UpdateCurrentDestiantion(bool StartMoving)
    {
        // if assigned to a line, target it
        if (AssignedLine.HasValue)
        {
            //drop any items
            PickupItem(false);

            // go to random point on line
            int RandomMarkerOnLine = UnityEngine.Random.Range(0, AssignedLine.Value.Markers.Count -1 );
            _CurrentDestiantion.Position = AssignedLine.Value.Markers[RandomMarkerOnLine].transform.position;
            _CurrentDestiantion.Type = DestinationType.Line;
        }

        // otherwise target mine or warehouse
        else  
        {   
            var Result = FindNearbyResourcess();
            Vector3 MinePosition = Result.Mine.transform.position;
            Vector3 WarehousePosition = Result.Warehouse.transform.position;

            _CurrentDestiantion.Position = IsCarryingOre ? WarehousePosition : MinePosition ;
            _CurrentDestiantion.Type = IsCarryingOre ? DestinationType.Warehouse : DestinationType.Mine ;
        }

        // if argument is true execute movement aswell
        if (StartMoving) 
        {
            _NavMeshAgent.SetDestination(_CurrentDestiantion.Position);
            _NavMeshAgent.isStopped = false;
        };
    }

    /// <summary> Pick up or Drop item,currently used only for ore,extend later </summary>
    public void PickupItem(bool PickUp)
    {
            IsCarryingOre = PickUp;
            GoldOreMesh.GetComponent<MeshRenderer>().enabled = PickUp;
    }

    public (GameObject Mine, GameObject Warehouse) FindNearbyResourcess()
    {
        // Get mines and warhouses

        List<GameObject> Mines = GameObject.FindGameObjectsWithTag("_Mine").ToList();
        List<GameObject> Warehouses = GameObject.FindGameObjectsWithTag("_Warehouse").ToList(); 

        // Sort by distance from unit
        Mines = Mines.OrderBy( (i)=>( i.transform.position - transform.position).sqrMagnitude ).ToList();
        Warehouses = Warehouses.OrderBy((i) => (i.transform.position - transform.position).sqrMagnitude).ToList();

        // retun closest mine and warehouse
        return (Mines[0], Warehouses[0]);
    }

    private bool _IsDestinationReached(bool ContinueWorking) 
    {
        // if destination reached 
        bool DestinationReached = (transform.position - _CurrentDestiantion.Position).sqrMagnitude < 10f;
        if (DestinationReached)
        {
            // if line stop
            if (_CurrentDestiantion.Type == DestinationType.Line)
            {
                _NavMeshAgent.isStopped = true;
            }

            // if mine or warehouse pickup/drop ore accordingly and continue working
            else
            {
                PickupItem( _CurrentDestiantion.Type == DestinationType.Mine? true : false );
                UpdateCurrentDestiantion(ContinueWorking);
            }
            return true;
        }
        // destination not reached
        return false;
    }



 ////////////// Start
    void Start()
    {        
        UpdateCurrentDestiantion(true);
    }



 ////////////// Update
    void Update()
    {
        _IsDestinationReached(true);
    }
}
