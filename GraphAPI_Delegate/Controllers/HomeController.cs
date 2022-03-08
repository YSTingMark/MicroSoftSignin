using GraphAPI_Delegate.Class;
using GraphAPI_Delegate.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;

using Newtonsoft.Json.Linq;

using RestSharp;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GraphAPI_Delegate.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ITokenAcquisition tokenAcquisition)
        {
            _logger = logger;
            _tokenAcquisition = tokenAcquisition;
        }
        private readonly ITokenAcquisition _tokenAcquisition;
        public IActionResult Index()
        {
            if (CallBack() == "成功登入，請回IMO")
            {
                //return Redirect("home/SuccessSignin");
                //System.Threading.Thread.Sleep(60000*60);//測試azure收費
                //return Redirect("MicrosoftIdentity/Account/SignIn");
            }
            return View();
        }
        public IActionResult SuccessSignin()
        {
            return View();
        }
        public string CallBack()
        {
            string Newtoken = "";
            try
            {
                Newtoken = _tokenAcquisition.GetAccessTokenForUserAsync(new string[] { "User.ReadBasic.All", "User.Read" }).Result;
                ////signin 後導回更新VDM
                //先透過抓取team個人ID
                var client = new RestClient("https://graph.microsoft.com/v1.0/me/");
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("Authorization", Newtoken);
                IRestResponse response = client.Execute(request);

                JObject json = JObject.Parse(response.Content);
                foreach (var temp in json)
                {
                    if (temp.Key.ToString() == "id")
                    {
                        //寫入
                        if (VDM.Sgt.TeamsToken.ContainsKey(temp.Value.ToString()))
                        {
                            VDM.Sgt.TeamsToken[temp.Value.ToString()] = Newtoken;
                        }
                        else
                        {
                            VDM.Sgt.TeamsToken.Add(temp.Value.ToString(), Newtoken);
                        }
                    }
                }
                
                return "成功登入，請回IMO";
            }
            catch (Exception ex)
            {
                //What's wrong wait for add
                return "成功失敗";
            }
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
