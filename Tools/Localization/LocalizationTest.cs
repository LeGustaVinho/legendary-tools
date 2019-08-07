using System.Collections;
using System.Collections.Generic;
using LegendaryTools.UI;
using UnityEngine;

public class LocalizationTest : MonoBehaviour
{
    public LocalizationConfig Config;
    
    [ContextMenu("Start")]
    void Start()
    {
        Localization.Init(Config);
    }
}
