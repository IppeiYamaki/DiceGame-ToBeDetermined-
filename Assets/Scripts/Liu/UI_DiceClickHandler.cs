using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 


public class UI_DiceClickHandler : MonoBehaviour, IPointerClickHandler
{

    public DiceLogic connectedDiceLogic;

   
    public void OnPointerClick(PointerEventData eventData)
    {
   
        if (connectedDiceLogic == null) return;

        connectedDiceLogic.ToggleRiseFall();
    }
}