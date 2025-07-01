using System;
using UnityEngine;

public interface IMenuView
{
    string viewName();
    void onOpen();
    void buttonHandler(ButtonKeyType type);
    string getText();
}