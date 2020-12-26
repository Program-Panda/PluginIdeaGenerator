using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuControls : MonoBehaviour
{
    public PluginObject pluginObject;

    public void OpenPlugin() {
        Application.OpenURL(pluginObject.pluginURL);
    }

    public void Completed() {
        RetireveIdeas.retireveIdeas.completedPlugins.Add(pluginObject.pluginIdea);
        RetireveIdeas.retireveIdeas.GenerateGameList();
    }
}
