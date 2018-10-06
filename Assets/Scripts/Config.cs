using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaintApp
{
    public class Config : JSON.ISeralizable
    {
        const string STR_Pictures = "pictures";
        const string STR_User_params = "user_params";
        public Dictionary<int, Picture> AllProducts = new Dictionary<int, Picture>();
        public static Dictionary<string, string> Localization = new Dictionary<string, string>();
        public Config(JSON.ANode node)
        {
            JsonReadFrom(node);
        }
        public void JsonReadFrom(JSON.ANode node, string outerName = "")
        {
            JSON.Class userParArr = node[STR_User_params].AsClass;
            var paramsNodes = userParArr.GetNamesOfAllChildren();
            foreach (string item in paramsNodes)
            {
                Localization.Add(item, userParArr[item]);
            }
            JSON.Array picArr = node[STR_Pictures].AsArray;
            foreach (JSON.ANode item in picArr)
            {
                Picture newPic = new Picture(item);
                AllProducts.Add(newPic.Id, newPic);
            }
        }

        public void JsonWriteTo(JSON.ANode parent, string forceName = "")
        {
            throw new NotImplementedException();
        }
    }
}