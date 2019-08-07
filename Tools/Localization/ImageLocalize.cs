using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LegendaryTools.UI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Image))]
    [AddComponentMenu("UI/Localize Image")]
    public class ImageLocalize : BaseLocalize
    {
        public Image image;

        public override void OnLocalize()
        {
            if (image == null) image = GetComponent<Image>();
            string path = Localization.Get(key);

            //If we still don't have a key, leave the value as blank
            if (!string.IsNullOrEmpty(key) && image != null && !string.IsNullOrEmpty(path))
                StartCoroutine(LoadTexture(path));
        }

        IEnumerator LoadTexture(string path)
        {
            ResourceRequest request = Resources.LoadAsync<Sprite>(path);
            yield return request;
            Image lbl = GetComponent<Image>();
            lbl.sprite = request.asset as Sprite;
        }
    }
}