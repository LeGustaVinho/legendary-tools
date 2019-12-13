using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LegendaryTools.Actor
{
    public interface IGameObject : IUnityObject
    {
        string Tag { get; set; }
        
        bool activeInHierarchy { get; set; }
        
        bool activeSelf { get; set; }
        
        int layer { get; set; }
        
        Scene scene { get; set; }
        
        string tag { get; set; }

        void SetActive(bool value);

        Component AddComponent(Type componentType);
        
        T AddComponent<T>() where T : Component;
    }
}