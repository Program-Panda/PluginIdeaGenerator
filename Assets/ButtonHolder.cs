using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHolder : MonoBehaviour{
    public PluginObject pluginObject;
    public GameObject buttons;

    public void clickButton() {
        var found = GameObject.FindWithTag("Button");
        if (found != null) {
            Destroy(found);
        }
        var go = Instantiate(buttons, transform.parent);
        var controls = go.GetComponent<MenuControls>();
        controls.pluginObject = pluginObject;
        go.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
    }
}
