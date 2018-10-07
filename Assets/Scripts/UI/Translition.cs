using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace PaintApp
{
    class Translition : MonoBehaviour
    {

        static float AnimDurationMs = 3000f;
        static DateTime TransStart;
        static RectTransform OldScreen;
        static RectTransform NewScreen;
        public static Translition Instance;
        private void Awake()
        {
            Instance = this;
        }
        public static void StartReplaceScreens(RectTransform oldScreen, RectTransform newScreen)
        {
            if (oldScreen == null || newScreen == null)
            {
                print("StartReplaceScreens null value " + oldScreen + " new " + newScreen);
            }
            TransStart = DateTime.Now;
            OldScreen = oldScreen;
            NewScreen = newScreen;
        }
        public static void StartReplaceScreens(GameObject oldScreen, GameObject newScreen)
        {
            StartReplaceScreens(oldScreen.GetComponent<RectTransform>(), newScreen.GetComponent<RectTransform>());
        }
        private void Update()
        {
            float timePassed = (float)(DateTime.Now - TransStart).TotalMilliseconds;
            if (OldScreen && NewScreen && timePassed < AnimDurationMs)
            {
                float increaser = timePassed / AnimDurationMs;
                OldScreen.anchorMax = new Vector2(OldScreen.anchorMax.x - increaser, OldScreen.anchorMax.y);
                NewScreen.anchorMax = new Vector2(OldScreen.anchorMax.x - increaser, OldScreen.anchorMax.y);
                OldScreen.anchorMin = new Vector2(OldScreen.anchorMin.x - increaser, OldScreen.anchorMin.y);
                NewScreen.anchorMin = new Vector2(OldScreen.anchorMin.x - increaser, OldScreen.anchorMin.y);
            }
        }
    }
}
