using UnityEngine;
using UnityEngine.EventSystems;

public class UI_DiceClickHandler : MonoBehaviour, IPointerClickHandler
{

    public DiceItem connectedDiceLogic;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (connectedDiceLogic != null)
        {
          
            connectedDiceLogic.ToggleRiseFall();
        }
    }
}