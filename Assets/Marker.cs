using UnityEngine;


public class Marker : MonoBehaviour
{
 ////////////// Structs,Enums
    public enum MarkerTypes
    {
        None, Attack , Defend
    }



 ////////////// Variables
    ///<summary> Don't set to None, that is used only by playerController </summary>
    public MarkerTypes MarkerType;

    public OrderManager.Line ParentLine; 



 ////////////// Functions
    /// <summary> Destroy line this marker belongs to </summary>
    public void DestroyLine() 
    {   
        // notify units in line it is gone
        foreach (GameObject Unit in ParentLine.AssignedUnits)
        {
            UnitOrders UnitOrders = Unit.GetComponent<UnitOrders>();
            UnitOrders.AssignedLine = null ;
            UnitOrders.UpdateCurrentDestiantion(true);
        }

        // destroy markers in line
        foreach (Marker item in ParentLine.Markers)
        {
            Destroy(item.gameObject);
        }

        // remove line from Orders list
        var OrdersListRef = OrderManager.OrderManagerRef.OrdersList;

            /// <summary> Selected Orders.Attack or Orders.Defense lines </summary>
            var SelectedOrdersList = (ParentLine.LineType == MarkerTypes.Attack)? OrdersListRef.AttackLines : OrdersListRef.DefenseLines ;

            SelectedOrdersList.Remove(ParentLine);
    }




 ////////////// Start
    void Start()
    {
        GetComponent<MeshRenderer>().material.color = (MarkerType == MarkerTypes.Attack ? Color.red : Color.green);
    }



 ////////////// OnMouseDown
    private void OnMouseDown()
    {
        // called with delay so we don't instantly spawn new marker 
        Invoke("DestroyLine", 0.2f);
    }
}
