using UnityEngine;

public class DisconnectView : IMenuView
{
    string IMenuView.viewName()
    {
        return "Disconnect";
    }

    void IMenuView.buttonHandler(ButtonKeyType type)
    {
        switch (type)
        {
            case ButtonKeyType.Enter:
                Debug.Log("Disconnecting");
                break;
            case ButtonKeyType.Back:
                MenuHandler.ReturnToSelect();
                break;
            default:
                break;
        }
    }

    void IMenuView.onOpen()
    {

    }
    
    string IMenuView.getText() {
        return "Press the \"Enter\" button to Disconnect from the current lobby.\nPress \"Back\" to return to menu.\nStatus: <color=red>Offline</color>";
    }
}