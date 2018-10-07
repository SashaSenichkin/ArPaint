using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace PaintApp
{
    class Mover : MonoBehaviour
    {
        public RectTransform MyRect;
        private float StartRectXMax;
        private float StartRectXMin;
        public ScreenType MyType;

        private void Awake()
        {
            SetStartParams(MyRect.anchorMax.x, MyRect.anchorMin.x);
        }
        public void SetStartParams(float newRectMax, float newRectMin)
        {
            StartRectXMax = newRectMax;
            StartRectXMin = newRectMin;
            SetRect(StartRectXMax, StartRectXMin);
        }
        public void SetRectShift(float shift)
        {
            SetRect(StartRectXMax - shift, StartRectXMin - shift);
        }
        public void SetRect(float newRectMax, float newRectMin)
        {
            MyRect.anchorMax = new Vector2(newRectMax, MyRect.anchorMax.y);
            MyRect.anchorMin = new Vector2(newRectMin, MyRect.anchorMin.y);
        }
    }
}
