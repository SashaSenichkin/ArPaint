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
        public event Action OnDoubleClick;
        private DateTime LastClick;
        private float DoubleClickMs;
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
            DetailDescription.text = description;
            IsParamsVisible = false;
            PropertyProxy.SetActive(false);
        }
        public void OnMouseDown()
        {
            print("OnMouseDown");
            if (LastClick == null)
            {
                LastClick = DateTime.Now;
            }
            else if ((DateTime.Now - LastClick).TotalMilliseconds <= DoubleClickMs)
            {
                if (OnDoubleClick != null)
                {
                    OnDoubleClick();
                }
            }
            LastClick = DateTime.Now;
        }
        public void SwitchParamWindow()
        {
            IsParamsVisible = !IsParamsVisible;
            PropertyProxy.SetActive(IsParamsVisible);
        }
    }
}
