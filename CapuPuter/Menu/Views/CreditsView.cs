public class CreditsView : IMenuView
{
    void IMenuView.onOpen(){

    }

    string IMenuView.viewName(){
        return "Credits";
    }

    void IMenuView.buttonHandler(ButtonKeyType type){
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
        return "Credits to @RedBrumbler for the Watch inspiration\nShader help by @nox.games\nCoded by @realtaffycandy";
    }
}