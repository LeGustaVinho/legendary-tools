using System;
using System.Collections.Generic;
using UnityEngine;

namespace LegendaryTools
{
    public class GameObjectListing<TGameObject, TData>
        where TGameObject : UnityEngine.Component, GameObjectListing<TGameObject, TData>.IListingItem
    {
        public interface IListingItem
        {
            void Init(TData item);
        }

		public bool AutoDestroyAllBeforeAdd = true;
        public TGameObject Prefab;
        public Transform Parent;
        public List<TGameObject> Listing = new List<TGameObject>();

        public readonly Func<List<TData>> DataProvider = null;

        public GameObjectListing()
        {

        }

        public GameObjectListing(Func<List<TData>> dataProvider)
        {
            DataProvider = dataProvider;
        }

        public virtual List<TGameObject> Generate()
        {
            return DataProvider != null ? GenerateList(DataProvider.Invoke()) : null;
        }

        public virtual List<TGameObject> GenerateList(List<TData> itens)
        {
			if (AutoDestroyAllBeforeAdd)
                DestroyAll();
			
            for (int i = 0; i < itens.Count; i++)
                CreateGameObject(itens[i]);

            return Listing;
        }

        public virtual void DestroyAll()
        {
            for (int i = 0; i < Listing.Count; i++)
            {
                if (Listing[i] != null)
                    GameObject.Destroy(Listing[i].gameObject);
            }

            Listing.Clear();
        }

        public virtual TGameObject CreateGameObject(TData item)
        {
            Listing.Add(UnityEngine.Object.Instantiate<TGameObject>(Prefab));

            Listing.Last().transform.SetParent(Parent);
            Listing.Last().transform.localScale = Prefab.transform.localScale;
            Listing.Last().Init(item);

            return Listing.Last();
        }
    }
}