using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace PaintApp
{
    [Serializable]
    enum ScreenType
    {
        main = 1,
        detail = 2,
        ARScene = 3,
        contact = 4,
        header,
        bottom,
        paramDialog,
        ARHelpDialog,
        background
    }
    class Manager : MonoBehaviour
    {
        ScreenType curOpenScreen = ScreenType.main;
        public static Manager Instanse { get; private set; }
        public UICanvas UIcontr;
        public UIDetail UIDetContr;
        public UIBottom UIBot;
        public TextAsset ConfigJson;
        public SpriteRenderer PicModel;

        private List<ScreenType> History;
        Config mainConf;
        int curPic;
     
        private void Awake()
        {
            History = new List<ScreenType>();
            Instanse = this;
            mainConf = new Config(JSON.Parse(ConfigJson.text));
            UIcontr.Initialize();
            UIcontr.MainSceneInvalidate();
            History.Add(ScreenType.main);
            UIDetContr.OnDoubleClick += ()=> SwitchScreenWrap(ScreenType.ARScene);
            UIDetContr.OnSwipeLeft += () => SetNextPic(true);
            UIDetContr.OnSwipeRight += () => SetNextPic(false);
        }

        public Dictionary<string, string> GetPropForPic(int id)
        {
            var result = new Dictionary<string, string>();
            var properties = mainConf.AllProducts[id].UserParams;
            foreach (var item in properties)
            {
                result.Add(item.Key, item.Value);
            }
            return result;
        }

        public Dictionary<int, Sprite> GetAllPreviews()
        {
            var result = new Dictionary<int, Sprite>();

            foreach (var item in mainConf.AllProducts)
            {
                result.Add(item.Key, item.Value.PreviewPicture);
            }
            return result;
        }
        private void SetNextPic(bool Isleft)
        {
            int currentIndex = mainConf.AllProducts[curPic].Index;//bullshit....
            int newIndex = Isleft ? ++currentIndex : --currentIndex;
            if (newIndex < 0)
            {
                newIndex = mainConf.AllProducts.Count -1;
            }
            else if (newIndex == mainConf.AllProducts.Count)
            {
                newIndex = 0;
            }
            print("SetNextPic " + newIndex);
            SetChoosedPic(mainConf.AllProducts.ElementAt(newIndex).Key);
        }
        public void SetChoosedPic(int id)
        {
            curPic = id;
            var picture = mainConf.AllProducts[id];
            UIDetContr.DetailInvalidate(picture.MainPicture, picture.Description, GetPropForPic(id));
            SwitchScreenWrap(ScreenType.detail);
            if (PicModel != null)
            {
                PicModel.sprite = picture.MainPicture;
                Debug.Log("SetChoosedPic " + picture.SizeX+ " " + picture.SizeY);
                PicModel.gameObject.transform.localScale = new Vector3(picture.SizeX, picture.SizeY,0);
            }       
        }

        public void SwitchScreenWrap(ScreenType screen)
        {
            if (screen == History.Last())
            {
                return;
            }
            UIBot.SwitchScreenTo(screen);
            UIcontr.SwitchScreenTo(History.Last(), screen);
            History.Add(screen);
        }
        #region On scene reference
        public void BackButton()
        {
            int lastIndex = History.Count - 1;
            if (lastIndex > 0)
            {
                UIcontr.SwitchScreenTo(History[lastIndex], History[lastIndex - 1]);
                print("BackButton SwitchScreenTo location " + History[lastIndex] + " to " + History[lastIndex - 1]);
                UIBot.SwitchScreenTo(History[lastIndex - 1]);
                History.RemoveAt(lastIndex);
            }
            else
            {
                print("last history location");
            }
        }
        public void OpenContact()
        {
            SwitchScreenWrap(ScreenType.contact);
        }
        public void OpenAR()
        {
            SwitchScreenWrap(ScreenType.ARScene);
        }
        public void OpenMain()
        {
            SwitchScreenWrap(ScreenType.main);
        }
        public void InfoDialogShowHide()
        {
            UIcontr.SwitchDialog (ScreenType.paramDialog);
        }
        public void ArHelperDialogHide()
        {
            UIcontr.SetDialogTo(ScreenType.ARHelpDialog, false);
        }
        public void ArHelperDialogShow()
        {
            UIcontr.SetDialogTo(ScreenType.ARHelpDialog, true);
        }
        #endregion
        public void SendContactEmail(string email, string phone)
        {
            BackButton();
            try
            {
                Debug.Log("mail start");
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.mail.ru", 2525);

                mail.From = new MailAddress("shild_and_sword@mail.ru");
                //mail.To.Add("korusant.jedi@gmail.com");
                mail.To.Add("shild_and_sword@mail.ru");
                mail.Subject = "Dude! some man form " + email +" and phone " + phone + " want to buy " + mainConf.AllProducts[curPic].Description;
                mail.Body = "man "; 
                SmtpServer.Credentials = (ICredentialsByHost)new NetworkCredential("shild_and_sword@mail.ru", "redgard1");

                SmtpServer.EnableSsl = true;
                ServicePointManager.ServerCertificateValidationCallback =
                    delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                    { return true; };


                SmtpServer.Send(mail);

                Debug.Log("mail Sent");
            }
            catch (Exception ex)
            {
                Debug.Log("SendEmail" + ex.ToString());
            }
        }
    }
}
