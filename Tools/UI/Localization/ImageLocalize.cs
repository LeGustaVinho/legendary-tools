using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LegendaryTools.UI
{
    [ExecuteInEditMode, RequireComponent(typeof(Image)), AddComponentMenu("UI/Localize Image")]
    public class ImageLocalize : BaseLocalize
    {
        public Image Image;
        private Coroutine loadTextureRoutine;

        protected override void Start()
        {
            base.Start();

            Localize(Localization.Get(key));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (loadTextureRoutine != null)
            {
                StopCoroutine(loadTextureRoutine);
                loadTextureRoutine = null;
            }
        }

        public override void Localize(string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            if (Image == null)
            {
                Image = GetComponent<Image>();
            }

            if (Image == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(value))
            {
                loadTextureRoutine = StartCoroutine(LoadTexture(value));
            }
        }

        private IEnumerator LoadTexture(string path)
        {
            ResourceRequest request = Resources.LoadAsync<Sprite>(path);
            yield return request;

            if (request.asset == null)
            {
                yield break;
            }

            Image.sprite = request.asset as Sprite;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                EditorUtility.SetDirty(this);
            }
#endif
        }
    }
}