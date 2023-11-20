using UnityEngine;
using UnityEngine.EventSystems;

using MarkerTypes = Marker.MarkerTypes;                                                                                                                // shortened for ease of use


public class playerController : MonoBehaviour
{
 ////////////// Variables
    ///<summary> Temporary line of Markers </summary>
    OrderManager.Line TempLine = new OrderManager.Line(true);

    OrderManager.Orders OrdersListRef;                                                                                                                 // shortened for ease of use
    public Marker MarkerToSpawn;
    public int MarkerMinSpacing = 2;

    ///<summary> Attack or Defense marker </summary>
    private MarkerTypes _MarkerTypeToSpawn = MarkerTypes.None;

    Vector3 LastMarkerLoc = default;

    ///<summary>  Singleton reference  </summary>
    [HideInInspector] static public playerController playerControllerRef;



 ////////////// Functions
    private void _CommitLineAndResetTempMarkers()
    {
        // Commit spawned Markers, if any
        if (TempLine.Markers.Count > 0) 
        {
            // Select Orders.Attack or Orders.Defense lines List and add in new line
            switch (_MarkerTypeToSpawn)
            {
                case MarkerTypes.Attack:
                    TempLine.LineType = MarkerTypes.Attack;
                    OrdersListRef.AttackLines.Add(TempLine);
                    break;

                case MarkerTypes.Defend:
                    TempLine.LineType = MarkerTypes.Defend;
                    OrdersListRef.DefenseLines.Add(TempLine);
                    break;

                default:
                    Debug.Log("Error marker type, line not added");
                    break;
            }
        }
        
        // reset temp markers 
        _MarkerTypeToSpawn = MarkerTypes.None;
        TempLine = new OrderManager.Line(true);
    }

    (RaycastHit HitResult, bool IsHit) RaycastTerrain()
    {
        Ray Ray = GetComponentInParent<Camera>(true).ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(Ray, out var HitResult);
        bool IsHit = (HitResult.transform.gameObject.layer == LayerMask.NameToLayer("Terrain"));
        return (HitResult, IsHit);
    }

    private void _DrawLineOnMouseDown()
    {
        bool PointerOverUI = EventSystem.current.IsPointerOverGameObject();                                                                         // avoid spawn when interracting with UI

        // set Marker type to spawn based on Mouse button
        if (_MarkerTypeToSpawn == MarkerTypes.None && RaycastTerrain().IsHit && !PointerOverUI)
        {
            if (Input.GetMouseButtonDown(0)) { _MarkerTypeToSpawn = MarkerTypes.Attack; }
            if (Input.GetMouseButtonDown(1)) { _MarkerTypeToSpawn = MarkerTypes.Defend; }
        }

        // try to spawn marker
        if (_MarkerTypeToSpawn != MarkerTypes.None)
        {
            // check raycast for terrain
            (RaycastHit HitResult, bool IsHit) = RaycastTerrain();                                                                                    // store value to avoid multiple function calls

            // if first Marker or far enough from last marker  
            if (IsHit && (TempLine.Markers.Count == 0 || (LastMarkerLoc - HitResult.point).magnitude > MarkerMinSpacing))
            {

                //Spawn marker
                Marker SpawnedMarker = Instantiate(MarkerToSpawn, HitResult.point, Quaternion.identity).GetComponent<Marker>();
                TempLine.Markers.Add(SpawnedMarker);
                LastMarkerLoc = HitResult.point;

                SpawnedMarker.MarkerType = _MarkerTypeToSpawn;
                SpawnedMarker.ParentLine = TempLine;
                SpawnedMarker.ParentLine.LineType = _MarkerTypeToSpawn;
            }
        }

        // commit line of markers on mouse release 
        if (Input.GetMouseButtonUp(0) && _MarkerTypeToSpawn == MarkerTypes.Attack) { _CommitLineAndResetTempMarkers(); }
        if (Input.GetMouseButtonUp(1) && _MarkerTypeToSpawn == MarkerTypes.Defend) { _CommitLineAndResetTempMarkers(); }
    }


    ////////////// Start
    void Start()
    {
       // get and set instance references 
       OrdersListRef = OrderManager.OrderManagerRef.OrdersList;
       playerControllerRef = this; 
    }



 ////////////// Update
    void Update()
    {
        _DrawLineOnMouseDown();
    }

    
}
