using UnityEngine;

namespace LegendaryTools.UI
{
    [System.Serializable]
    public class DraggingObject
    {
        public int ID;
        public UIDrag Object;
        public RectTransform Plane;

        public bool OnEndDrag = false;
        public bool OnDrop = false;

        public DraggingObject(int id, UIDrag theObject, RectTransform plane = null)
        {
            ID = id;
            Object = theObject;
            Plane = plane;
        }
    }
}