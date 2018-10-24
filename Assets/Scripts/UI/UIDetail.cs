using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace PaintApp
{
    class UIDetail:MonoBehaviour
    {
        //public event Action OnDoubleClick;
        public event Action OnSwipeLeft;
        public event Action OnSwipeRight;
        //private DateTime LastClick;
        private float? LastClickPointX = null;
        //private float DoubleClickMs = 500f;
        [SerializeField]
        private Image DetailImage;
        [SerializeField]
        private Text DetailDescription;
        [SerializeField]
        private GameObject PropertyProxy;
        [SerializeField]
        private GameObject OnePropertyPrefab;
        private bool IsParamsVisible = false;

        public void DetailInvalidate(Sprite picImage, string description, Dictionary<string, string> properties)
        {
            for (int i = 0; i < PropertyProxy.transform.childCount; i++)
            {
                Destroy(PropertyProxy.transform.GetChild(i).gameObject);
            }
            foreach (var item in properties)
            {
                var newProp = Instantiate(OnePropertyPrefab, PropertyProxy.transform);
                newProp.GetComponent<PropertyDataSwitcher>().SetValues(item.Key, item.Value);
            }
            DetailImage.sprite = picImage;
            DetailDescription.text = UICanvas.GetColoredText(description);
            IsParamsVisible = false;
            PropertyProxy.SetActive(false);
        }
        public void OnMouseDown()
        {
            //if (LastClick == null)
            //{
            //    LastClick = DateTime.Now;
            //}
            //else if ((DateTime.Now - LastClick).TotalMilliseconds <= DoubleClickMs)
            //{
            //    if (OnDoubleClick != null)
            //    {
            //        OnDoubleClick();
            //    }
            //}
            //else
            //{
                LastClickPointX = Application.isEditor ? Input.mousePosition.x : Input.touches.First().position.x;
            //}
            //LastClick = DateTime.Now;
        }
        public void OnMouseUp()
        {
            float clickX = Application.isEditor ? Input.mousePosition.x : Input.touches.First().position.x;
            if (LastClickPointX == null)
            {
                return;
            }
            else if (clickX > LastClickPointX && OnSwipeRight != null)
            {
                OnSwipeRight();
                LastClickPointX = null;
            }
            else if (clickX < LastClickPointX && OnSwipeLeft != null)
            {
                OnSwipeLeft();
                LastClickPointX = null;
            }
        }
        public void SwitchParamWindow()
        {
            IsParamsVisible = !IsParamsVisible;
            PropertyProxy.SetActive(IsParamsVisible);
        }
    }
}
