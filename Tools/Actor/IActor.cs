using System;
using UnityEngine;

namespace LegendaryTools.Actor
{
    public interface IActor : IMonoBehaviour, IRectTransform, IGameObject, IDisposable
    {
        Transform Transform { get; }
        RectTransform RectTransform { get; }
        GameObject GameObject { get; }
    }
}