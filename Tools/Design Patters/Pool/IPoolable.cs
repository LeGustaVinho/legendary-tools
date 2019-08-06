using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LegendaryTools.DesignPatters
{
    public interface IPoolable
    {
        void OnConstruct();

        void OnCreate();

        void OnRecycle();
    }
}