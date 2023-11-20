using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
 ////////////// Structs,Enums
    public struct Line
    {
        public List<Marker> Markers;
        public List<GameObject> AssignedUnits;
        public Marker.MarkerTypes LineType;

        // Constructor, used later when creating new lines
        public Line(bool Initialize)
        {
            Markers = new List<Marker>();
            AssignedUnits = new List<GameObject>();
            LineType = Marker.MarkerTypes.Attack;
        }
    }
    public struct Orders
    {
        public List<Line> AttackLines;
        public List<Line> DefenseLines;
        public Marker.MarkerTypes LineType;

        // Constructor, used later when creating new order
        public Orders(bool Initialize)
        {
            AttackLines = new List<Line>(); 
            DefenseLines = new List<Line>();
            LineType = Marker.MarkerTypes.Defend;
        }
    }



 ////////////// Variables
    /// <summary> List of attack and defense lines  </summary>
    public Orders OrdersList = new Orders(true);

    /// <summary> Ratio of workers / combat units </summary>
    [HideInInspector] public float CombatUnitsPercent;

    /// <summary> Ratio of attackers / defenders for combat units </summary>
    [HideInInspector] public float AttackUnitsPercent;

    /// <summary>  Singleton reference  </summary>
    [HideInInspector] public static OrderManager OrderManagerRef;

    [HideInInspector] public List<GameObject> UnitsList;

    // used by CalculateRequiredUnits
    private int UnitsCount;
    private int CombatUnitsCount;
    private int AttackUnitsCount;
    private int DefenseUnitsCount; 
    private int AttackUnitsPerLine;
    private int DefenseUnitsPerLine;



    ////////////// Functions
    private void _UpdateUnitsList()
    {
        UnitsList = GameObject.FindGameObjectsWithTag("_Unit").ToList();
    }

    private void _CalculateRequiredUnits()
    {
        UnitsCount = UnitsList.Count;
        CombatUnitsCount = (int)(UnitsCount * CombatUnitsPercent);

        // Attack/Defense units count based on UI sliders
        AttackUnitsCount = (int)(CombatUnitsCount * AttackUnitsPercent);
        DefenseUnitsCount = CombatUnitsCount - AttackUnitsCount;

        // divide attack units over all attack lines, same for defense
        if (OrdersList.AttackLines.Count > 0)                                                       // prevent 0 division
        {
            AttackUnitsPerLine = AttackUnitsCount / OrdersList.AttackLines.Count;
        }
        if (OrdersList.DefenseLines.Count > 0)                                                      // prevent 0 division
        {
            DefenseUnitsPerLine = DefenseUnitsCount / OrdersList.DefenseLines.Count;
        }
    }

    /// <summary> Switches between AttackUnitsPerLine and DefenseUnitsPerLine </summary>
    private int _TargetUnitsCount(Line line)
    {
        return (line.LineType == Marker.MarkerTypes.Attack)? AttackUnitsPerLine : DefenseUnitsPerLine ;
    }

    /// <summary> add or remove units to lines as necessary  </summary>
    private void _AddRemoveUnits()
    {
        // combine atttack and defense lists and do them in a single loop 
        List<Line> CombinedLines = OrdersList.AttackLines.Concat(OrdersList.DefenseLines).ToList();

        foreach (Line Line in CombinedLines)
        {
            // Add units
            if (Line.AssignedUnits.Count < _TargetUnitsCount(Line))
            {
                foreach (GameObject Unit in UnitsList)
                {
                    UnitOrders UnitOrders = Unit.GetComponent<UnitOrders>();

                    //add unit if available
                    if (UnitOrders.AssignedLine == null)
                    {
                        UnitOrders.AssignedLine = Line;
                        Line.AssignedUnits.Add(Unit);

                        UnitOrders.UpdateCurrentDestiantion(true);
                        break;
                    }
                }
            }

            // Remove units
            if (Line.AssignedUnits.Count > _TargetUnitsCount(Line))
            {
                //get any unit on this line
                GameObject Unit = Line.AssignedUnits[0];
                UnitOrders UnitOrders = Unit.GetComponent<UnitOrders>();

                //remove unit 
                UnitOrders.AssignedLine = null;
                Line.AssignedUnits.Remove(Unit);

                UnitOrders.UpdateCurrentDestiantion(true);
            }
        }
    }


 ////////////// Awake
    private void Awake()
    {
        OrderManagerRef = this;

        // Set default values
        CombatUnitsPercent = 0f;
        AttackUnitsPercent = 0.5f;
    }



 ////////////// Update
    void Update()
    {
        _UpdateUnitsList();

        // calculate available and required combat units and store them
        _CalculateRequiredUnits();
        
        _AddRemoveUnits();
    }
}
