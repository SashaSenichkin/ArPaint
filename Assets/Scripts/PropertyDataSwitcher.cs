using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace PaintApp
{
    class PropertyDataSwitcher: MonoBehaviour
    {
        public Text PropertyName;
        public Text PropertyValue;
        public void SetValues(string name, string value)
        {
            PropertyName.text = name;
            PropertyValue.text = value;
        }
    }
}
