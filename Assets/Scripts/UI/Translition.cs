using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace PaintApp
{
    class Translition : MonoBehaviour
    { 
        static float AnimDurationMs = 600f;
        static DateTime TransStart;
        public static Translition Instance;
        private void Awake()
        {
           Instance = this;
        }
        public void StartReplaceScreens(Mover oldScreen, Mover newScreen)
        {
            if (oldScreen == null || newScreen == null)
            {
                print("StartReplaceScreens null value " + oldScreen + " new " + newScreen);
                return;
            }
            else if (oldScreen.MyType == newScreen.MyType)
            {
                print("StartReplaceScreens wtf?? sceens are simular " + oldScreen);
                return;
            }
            StartCoroutine(ReplaceCorout(oldScreen, newScreen));
        }

        public void StartReplaceScreens(GameObject oldScreen, GameObject newScreen)
        {
            StartReplaceScreens(oldScreen.GetComponent<Mover>(), newScreen.GetComponent<Mover>());
        }
        public void StartSwitchDialog(Mover screen, bool show)
        {
            if (show)
            {
                StartCoroutine(ShowCorout(screen));
            }
            else
            {
                StartCoroutine(HideCorout(screen));
            }
        }
        private IEnumerator ShowCorout(Mover screen)
        {
            TransStart = DateTime.Now;
            screen.SetStartParams(2, 1);
            screen.gameObject.SetActive(true);
            float timePassed = float.MinValue;
            while (timePassed < AnimDurationMs)
            {
                timePassed = (float)(DateTime.Now - TransStart).TotalMilliseconds;
                float increaser = timePassed / AnimDurationMs;
                screen.SetRectShift(increaser);
                yield return new WaitForEndOfFrame();
            }
            print("ReplaceCorout set default ");
            screen.SetStartParams(1, 0);
        }
        private IEnumerator HideCorout(Mover screen)
        {
            TransStart = DateTime.Now;
            float timePassed = float.MinValue;
            while (timePassed < AnimDurationMs)
            {
                timePassed = (float)(DateTime.Now - TransStart).TotalMilliseconds;
                float increaser = timePassed / AnimDurationMs;
                screen.SetRectShift(increaser);
                yield return new WaitForEndOfFrame();
            }
            print("ReplaceCorout set default ");
            screen.gameObject.SetActive(false);
            screen.SetStartParams(1, 0);
        }
        private IEnumerator ReplaceCorout(Mover oldScreen, Mover newScreen)
        {
            TransStart = DateTime.Now;
            newScreen.SetStartParams(2, 1);
            newScreen.gameObject.SetActive(true);
            float timePassed = float.MinValue;
            while (timePassed < AnimDurationMs)
            {
                timePassed = (float)(DateTime.Now - TransStart).TotalMilliseconds;
                float increaser = timePassed / AnimDurationMs;
                oldScreen.SetRectShift(increaser);
                newScreen.SetRectShift(increaser);
                yield return new WaitForEndOfFrame();
            }
            print("ReplaceCorout set default ");
            oldScreen.gameObject.SetActive(false);
            newScreen.SetStartParams(1, 0);
            oldScreen.SetStartParams(1, 0);
        }
    }
}
