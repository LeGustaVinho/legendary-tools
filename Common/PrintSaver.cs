using UnityEngine;
using System;
using System.Collections;

namespace LegendaryTools
{
    public class PrintSaver : MonoBehaviour
    {
#if UNITY_EDITOR
        // Use this for initialization
        void Start()
        {
            DontDestroyOnLoad(this);
        }

        // Update is called once per frame
        void Update()
        {

            if (UnityEngine.Input.GetKeyUp(KeyCode.F12))
            {
                ScreenCapture.CaptureScreenshot(GenerateUID() + ".png");
                Debug.Log("Print!");
            }
        }

        public static string GenerateUID()
        {
            return String.Format("{0: yyyyMMddHHmmssffff}", DateTime.Now);
        }
#endif
    }
}