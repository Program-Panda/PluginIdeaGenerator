using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RetireveIdeas : MonoBehaviour{
    public static RetireveIdeas retireveIdeas;
    
    public string BaseURL;
    private List<string> urls = new List<string>();
    private List<string> individualURls = new List<string>();
    [HideInInspector] public List<string> completedPlugins = new List<string>();
    public List<PluginObject> pluginObjects = new List<PluginObject>();

    [Header("LoadingScreen")] 
    public GameObject loadingScreen;
    public GameObject inputcompoents;
    public Scrollbar scrollBar;
    public Text displayText;
    public InputField inputField;

    [Header("Current Stats")] 
    private PluginObject currentPlugin;
    private int numberOfPages;

    private void Awake() {
        retireveIdeas = this;
    }

    private void Start(){
        loadingScreen.SetActive(true);
    }

    public void StartLoading(){
        if (!String.IsNullOrEmpty(inputField.text)){
            inputcompoents.SetActive(false);
            numberOfPages = int.Parse(inputField.text) + 1;
            for (var i = 1; i < numberOfPages; i++){
                urls.Add(BaseURL + i);
            }
            StartCoroutine(RetiriveIdeasOneAtATime());
        }  
    }

    private IEnumerator RetiriveIdeasOneAtATime(){
        float number = 0;
        float expectedURls = 0;
        var looped = false;
        
        foreach (var url in urls){
            using (var www = UnityWebRequest.Get(url)){
                yield return www.Send();
                if (!www.isNetworkError && !www.isHttpError){
                    var currentTextDocument = www.downloadHandler.text;
                    
                    var values = currentTextDocument.Split(" "[0]).ToList();
                    foreach (var val in values) {
                        if (val.Contains("threads") && val.Contains("title")) {
                            var plugin = new PluginObject();
                            var ext = val.Split("\""[0]).ToList()[1];
                            plugin.pluginURL = "https://bukkit.org/" + ext;
                            plugin.pluginIdea = ext.Split('/').ToList()[1].Split('.')[0].Replace("-"," ").ToUpper();
                            if (!completedPlugins.Contains(plugin.pluginIdea) && !plugin.pluginIdea.Contains("READ ME FIRST")) {
                                pluginObjects.Add(plugin);
                            }
                        }
                        
                        if (pluginObjects.Count >= number && !looped)
                        {
                            number = pluginObjects.Count;
                        }
                    }
                }
            }

            looped = true;
            var maxSize = number * numberOfPages;
            var size = pluginObjects.Count / maxSize;
            scrollBar.size = size;
            displayText.text = pluginObjects.Count + "/" + maxSize;
            yield return new WaitForSeconds(0.1f);
        }
        
        yield return new WaitForSeconds(1f);
        loadingScreen.SetActive(false);
        GenerateGameList();
    }

    [Header("Prefab")] 
    public GameObject pluginPrefab;
    public Transform spawnArea;

    public void GenerateGameList()
    {
        for (var i = 0; i < spawnArea.childCount; i++) {
            Destroy(spawnArea.GetChild(i).gameObject);
        }
        
        foreach (var idea in pluginObjects) {
            var go = Instantiate(pluginPrefab, spawnArea);
            go.GetComponentInChildren<Text>().text = idea.pluginIdea;
            go.GetComponent<ButtonHolder>().pluginObject = idea;
        } 
    }

    public void RandomPlugin() {
        Application.OpenURL(pluginObjects[Random.Range(0, pluginObjects.Count)].pluginURL);
    }

    public void Close()
    {
        
        for (var i = 0; i < spawnArea.childCount; i++) {
            Destroy(spawnArea.GetChild(i).gameObject);
        }

        inputcompoents.SetActive(true);
        urls.Clear();
        individualURls.Clear();
        displayText.text = "";
        scrollBar.size = 0;
        loadingScreen.SetActive(true);
        inputField.text = "";
    }
}
