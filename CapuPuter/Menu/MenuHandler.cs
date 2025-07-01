using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Il2CppInterop.Runtime;
using Il2CppSystem.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using MelonLoader;

public class MenuHandler
{
    public MenuHandler() => instance = this;

    public static MenuHandler instance;
    public List<IMenuView> menuItems;
    public Text menuText;
    public IMenuView currentView;
    public float buttonCooldown;
    public int cursorIndex;
    public int pageIndex;
    public int maxItems;
    public bool isHome;

    Transform watchParent;
    Transform screenTransform;
    Transform watchTransform;

    Component AddKeyButton(GameObject key, ButtonKeyType type)
    {
        MenuButtonKey comp = key.AddComponent<MenuButtonKey>();
        key.layer = LayerMask.NameToLayer("Trigger");
        comp.buttonType = (int)type;

        return comp;
    }

    private string GetPackageName() => Application.identifier;

    void FixShaders(GameObject obj)
    {
        // scuffed shader fix because assetbundles r weird?? credit to @nox.games for help
        foreach (Renderer rend in obj.GetComponentsInChildren<Renderer>())
        {
            foreach (Material mat in rend.GetMaterialArray())
            {
                mat.shader = Shader.Find(mat.shader.name);
            }
        }
    }

    public void RegisterView(IMenuView view)
    {
        if (!menuItems.Contains(view))
            menuItems.Add(view);
    }

    public byte[] GetBundleStream()
    {
        Assembly assembly = this.GetType().Assembly;
        string fileName = assembly.GetManifestResourceNames()[0];
        byte[] bytes = null;
        using (Stream stream = assembly.GetManifestResourceStream(fileName))
        {
            MelonLogger.Msg("Loading Bundle at " + fileName);
            using (MemoryStream memStream = new MemoryStream())
            {
                stream.CopyTo(memStream);
                bytes = memStream.ToArray();
            }
        }
        return bytes;
    }

    public void Initialize()
    {
        menuItems = new();
        currentView = null;
        buttonCooldown = 0.0f;
        cursorIndex = 0;
        pageIndex = 0;
        maxItems = 8;
        isHome = true;

        GameObject menuParent = new GameObject();
        watchParent = menuParent.transform;
        watchParent.localScale = new Vector3(110, 110, 110);

        #region asset bundle
        AssetBundle bundle = AssetBundle.LoadFromMemory(GetBundleStream());
        GameObject menuAsset = GameObject.Instantiate(bundle.LoadAsset<GameObject>("Menu")) as GameObject;
        GameObject menuWatch = GameObject.Instantiate(bundle.LoadAsset<GameObject>("Menu Watch")) as GameObject;


        screenTransform = menuAsset.transform;
        watchTransform = menuWatch.transform;

        screenTransform.parent = watchParent;
        screenTransform.transform.localPosition = new Vector3(-0.0026f, 0, 0);
        screenTransform.transform.localEulerAngles = new Vector3(0, 0, 90);
        screenTransform.transform.localScale = Vector3.one * .004f;


        watchTransform.parent = watchParent;
        watchTransform.transform.localPosition = new Vector3(0.000496f, 0, 0);
        watchTransform.transform.localEulerAngles = new Vector3(0, -90, 0);
        watchTransform.transform.localScale = new Vector3(-0.01034016f, -0.01034016f, -0.001921267f);


        menuText = GameObject.Find(menuAsset.name + "/Canvas/Text").GetComponent<Text>() as Text;
        #endregion

        #region buttons
        GameObject upKey = GameObject.Find(menuAsset.name + "/Up");
        GameObject downKey = GameObject.Find(menuAsset.name + "/Down");
        GameObject leftKey = GameObject.Find(menuAsset.name + "/Left");
        GameObject rightKey = GameObject.Find(menuAsset.name + "/Right");
        GameObject enterKey = GameObject.Find(menuAsset.name + "/Enter");
        GameObject backKey = GameObject.Find(menuAsset.name + "/BackBtn");
        GameObject openKey = GameObject.Find(menuWatch.name + "/WatchCollider");

        Component upComponent = AddKeyButton(upKey, ButtonKeyType.Up);
        Component downComponent = AddKeyButton(downKey, ButtonKeyType.Down);
        Component leftComponent = AddKeyButton(leftKey, ButtonKeyType.Left);
        Component rightComponent = AddKeyButton(rightKey, ButtonKeyType.Right);
        Component enterComponent = AddKeyButton(enterKey, ButtonKeyType.Enter);
        Component backComponent = AddKeyButton(backKey, ButtonKeyType.Back);
        Component openComponent = AddKeyButton(openKey, ButtonKeyType.Open);
        #endregion

        FixShaders(menuAsset);
        FixShaders(menuWatch);

        menuAsset.SetActive(false);
    }

    public static void ReturnToSelect()
    {
        MenuHandler.instance.currentView = null;
        MenuHandler.instance.isHome = true;
    }

    public void Run(Transform wrist)
    {
        watchParent.SetPositionAndRotation(wrist.position, wrist.rotation * Quaternion.Euler(new Vector3(180, 180, 0)));
        //TODO: Make screenTransform look at camera
    }

    public void UpdateMenu()
    {
        if (isHome) {
            string menuString = "";
            int start = maxItems * pageIndex;

            for (int i = start; i < Math.Min(start + maxItems, (int)menuItems.Count); ++i) {
                string prefix = cursorIndex == (i - start) ? ">" : "";
                menuString += prefix + menuItems[i].viewName() + "\n";
            }

            menuText.text = menuString;
        }
        else {
            if (currentView != null) {
                menuText.text = "Press \"back\" to select\n\n" + currentView.getText();
            }
        }
    }

    public void HandleButton(ButtonKeyType type)
    {
        if (isHome) {
            switch (type) {
                case ButtonKeyType.Open:
                    screenTransform.gameObject.SetActive(true);
                    break;
                case ButtonKeyType.Up:
                    cursorIndex = cursorIndex - 1;
                    break;
                case ButtonKeyType.Down:
                    cursorIndex = cursorIndex + 1;
                    break;
                case ButtonKeyType.Left:
                    pageIndex--;
                    break;
                case ButtonKeyType.Right:
                    pageIndex++;
                    break;
                case ButtonKeyType.Enter:
                {
                    int actualIndex = pageIndex * maxItems + cursorIndex;
                    if (actualIndex < menuItems.Count) {
                        IMenuView selectedOption = menuItems[actualIndex];
                        selectedOption.onOpen();
                        currentView = selectedOption;
                        isHome = false;
                    }
                }
                    break;
                case ButtonKeyType.Back:
                    screenTransform.gameObject.SetActive(false);
                    break;
                default:
                    break;
            }

            int categorySize = (int)menuItems.Count;
            int maxPages = Math.Max(0, (int)Math.Ceiling((float)categorySize / (float)maxItems) - 1);

            pageIndex = Math.Clamp(pageIndex, 0, maxPages);

            int itemsOnCurrentPage = Math.Min(maxItems, categorySize - (maxItems * pageIndex));

            cursorIndex = Math.Clamp(cursorIndex, 0, Math.Max(0, itemsOnCurrentPage - 1));
        } else {
            if (currentView != null) {
                currentView.buttonHandler(type);
            }
        }

        UpdateMenu();
    }
}