using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace PaintApp
{
    class UIContact : MonoBehaviour
    {

        [SerializeField]
        private InputField Email;
        [SerializeField]
        private InputField Phone;

        public void SendEmail()
        {
            Manager.Instanse.SendContactEmail(Email.text, Phone.text);
        }
    }
}
