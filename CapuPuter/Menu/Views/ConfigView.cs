public class ConfigView : IMenuView
{
    void IMenuView.onOpen() {

    }

    string IMenuView.viewName() {
        return "Config";
    }

    void IMenuView.buttonHandler(ButtonKeyType type) {
        switch (type) {
            case ButtonKeyType.Enter:
                break;
            case ButtonKeyType.Back:
                MenuHandler.ReturnToSelect();
                break;
            default:
                break;
        }
    }

    string IMenuView.getText() {
        return ">No Config Yet";
    }
}