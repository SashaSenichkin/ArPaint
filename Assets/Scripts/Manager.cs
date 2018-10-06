using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace PaintApp
{
    enum ScreenType
    {
        main = 1,
        detail = 2,
        ARscene = 3,
        contact = 4
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
        
        
        Config mainConf;
        int curPic;
        public void BackButton()
        {
            SetChoosedPic(curPic);
        }

     
        private void Awake()
        {
            Instanse = this;
            mainConf = new Config(JSON.Parse(ConfigJson.text));
            UIcontr.Initialize();
            UIcontr.MainSceneInvalidate();
            UIcontr.SwitchScreenTo(ScreenType.main);
            UIDetContr.OnDoubleClick += ()=> UIcontr.SwitchScreenTo(ScreenType.main);
            Debug.LogError("stop here");
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
        public void SetChoosedPic(int id)
        {
            curPic = id;
            var picture = mainConf.AllProducts[id];
            UIDetContr.DetailInvalidate(picture.MainPicture, picture.Description, GetPropForPic(id));
            UIcontr.SwitchScreenTo(ScreenType.detail);
            UIBot.SwitchScreenTo(ScreenType.detail);
            PicModel.sprite = picture.MainPicture;
        }
        public void OpenContact()
        {
            UIcontr.SwitchScreenTo(ScreenType.contact);
        }
        public void OpenAR()
        {
            UIBot.SwitchScreenTo(ScreenType.ARscene);
            UIcontr.SwitchScreenTo(ScreenType.ARscene);
        }
        public void SendContactEmail(string email, string phone)
        {
            Debug.Log("message send");
        }
    }
}
