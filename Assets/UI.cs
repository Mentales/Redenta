using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
 ////////////// Variables
    /// <summary> Stored reference to OrderManager instance </summary> 
    private OrderManager _OrderManagerReff;

    public Slider CombatUnits_Slider;
    public Slider AttakcUnits_Slider;

    public Text UnitsNumber_Text;



 ////////////// Functions
    void SetCombatUnitsPercent(float value)
    {
        _OrderManagerReff.CombatUnitsPercent = value ;
    }
    void SetAttackUnitsPercent(float value)
    {
        _OrderManagerReff.AttackUnitsPercent = value;
    }



 ////////////// Start
    void Start()
    {
        // store reference
        _OrderManagerReff = OrderManager.OrderManagerRef;

        // set up listeners for UI sliders
            // Listen to Economy slider changes and update OrderManager
            CombatUnits_Slider.onValueChanged.AddListener
            (
                (value) => { SetCombatUnitsPercent(value); }
            );

            // Listen to Attack/Defense slider changes and update OrderManager
            AttakcUnits_Slider.onValueChanged.AddListener 
            (
                (value) => { SetAttackUnitsPercent(value); }  
            );
    }



 ////////////// Update
    void Update()
    {
        // update units number UI text
        UnitsNumber_Text.text = "Total units " + _OrderManagerReff.UnitsList.Count.ToString();
    }
}
