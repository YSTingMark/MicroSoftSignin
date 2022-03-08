using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;

using RestSharp;

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using GraphAPI_Delegate.Class;

namespace GraphAPI_Delegate.Controllers
{
    public class MicroSoftController : Controller
    {
        private readonly ITokenAcquisition _tokenAcquisition;

        public MicroSoftController(ITokenAcquisition tokenAcquisition)
        {
            _tokenAcquisition = tokenAcquisition;
        }
        public List<int> AllIndexesOf(string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }
        public string OwnTokenAPI(string IdentityID)
        {
            string token = "";
            if (VDM.Sgt.TeamsToken.ContainsKey(IdentityID))
            {
                token = VDM.Sgt.TeamsToken[IdentityID];
            }
            else
            {
                token = "Something Wrong";
            }
            
            return token;
        }
        public IActionResult Index()
        {
            string token = "";
            try
            {
                token = _tokenAcquisition.GetAccessTokenForUserAsync(new string[] { "User.ReadBasic.All", "User.Read" }).Result;
            }
            catch (Exception ex)
            {
                //do login 
                return Redirect("MicrosoftIdentity/Account/SignIn");
            }
            //var token = _tokenAcquisition.GetAccessTokenForUserAsync(new string[] { "User.ReadBasic.All", "User.Read" }).Result;
            var client = new RestClient("https://graph.microsoft.com/v1.0/me/");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", token);
            IRestResponse response = client.Execute(request);

            List<int> indexes = AllIndexesOf(response.Content, "\\u");
            string Retrun_View = response.Content;
            foreach (var temp in indexes)
            {
                string ThisTempValue = response.Content[temp + 2].ToString() + response.Content[temp + 3].ToString() + response.Content[temp + 4].ToString() + response.Content[temp + 5].ToString();
                int code = int.Parse(ThisTempValue, System.Globalization.NumberStyles.HexNumber);
                string unicodeString = char.ConvertFromUtf32(code);
                Retrun_View = Retrun_View.Replace("\\u" + ThisTempValue, unicodeString);
            }

            ViewBag.Show = Retrun_View;
            return View();
        }

    }

}
