using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelInCloud.Models;
using TravelInCloud.Services;
using static TravelInCloud.Services.WeChatService;

namespace TravelInCloud.Controllers
{
    [RequireHttps]
    public class ApiController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;

        public ApiController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailSender emailSender,
        ISmsSender smsSender,
        ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<ManageController>();
        }

        public async Task<string> WeChatVerify(string signature, string timestamp, string nonce, string echostr)
        {
            if (await Verify(signature, timestamp, nonce))
            {
                return echostr;
            }
            return string.Empty;
        }
        [HttpPost]
        // POST: /api/WeChatVerify
        public async Task<string> WeChatVerify(string signature, string timestamp, string nonce, string echostr, object obj)
        {
            if (await Verify(signature, timestamp, nonce))
            {
                var s = Request.Form.ToString();
                var Result = await XMLDeserializeObjectAsync<xml>(s);
                var ReturnMessage = new xml
                {
                    Content = await Reply(Result.Content),
                    CreateTime = ConvertDateTimeInt(DateTime.Now),
                    ToUserName = Result.FromUserName,
                    FromUserName = Result.ToUserName,
                    MsgType = "text"
                };
                return await XMLSerializeObjectAsync(ReturnMessage);
            }
            return string.Empty;
        }

        // GET: /api/AccessTokenTest
        public async Task<string> AccessTokenTest()
        {
            return await AccessTokenAsync();
        }

        public async Task<string> ApplyMenu()
        {
            var Buttons = new Source
            {
                button = new List<Button>(1)
            };
            var NewButton = new Button
            {
                name = "云中旅行",
                sub_button = new List<SubButton>(1)
            };
            NewButton.sub_button.Add(new SubButton
            {
                name = "进入云中",
                type = "view",
                url = await GenerateAuthUrlAsync($"https://{Secrets.Host}/api/AuthRedirect")
            });
            Buttons.button.Add(NewButton);
            return JsonConvert.SerializeObject(Buttons);
        }

        // GET: /api/AuthRedirect
        public async Task<IActionResult> AuthRedirect(string code = "", string state = "")
        {
            //If user is using browser from wechat
            if (code != null)
            {
                //Get user information
                var AuthAccessToken = await AuthCodeToAccessTokenAsync(code);
                if (AuthAccessToken.openid == null)
                {
                    return NoContent();
                }
                var WCUser = await UserInfomationAsync(AuthAccessToken.openid, await AccessTokenAsync());
                //Find user in database
                var _wuser = await _userManager.Users.Where(t => t.openid == AuthAccessToken.openid).SingleOrDefaultAsync();
                //Already user
                if (_wuser != null)
                {
                    //Sign in
                    await _signInManager.SignInAsync(_wuser, false);
                    //Update his information from WeChat
                    _wuser.NickName = WCUser.nickname;
                    _wuser.IconAddress = WCUser.headimgurl;
                    await _userManager.UpdateAsync(_wuser);

                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }
                //Not Our User
                else
                {
                    //Create New Account
                    var UserName = StringOperation.RandomString(10) + Secrets.TempUserName;
                    var Password = Secrets.TempPassword;
                    //Register him
                    var NewUser = new ApplicationUser
                    {
                        UserName = UserName,
                        Email = UserName,
                        openid = AuthAccessToken.openid,
                        NickName = WCUser.nickname,
                        IconAddress = WCUser.headimgurl
                    };
                    await _userManager.CreateAsync(NewUser, Password);
                    //Sign in
                    await _signInManager.SignInAsync(NewUser, false);

                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }
            }
            //User is using typical browser
            else
            {

            }
            return RedirectToAction(nameof(HomeController.Index), "Home");
            //var LCuser = _userManager.Users.ToList().Find(t => t.openid == Result.openid);
            //switch (state.ToLower().Trim())
            //{
            //    case "checkmorning":
            //        return RedirectToAction("CheckMorning", "WeChat", new { openid = Result.openid });//await Check(WCUser);
            //    case "schedule":
            //        return RedirectToAction("Schedule", "WeChat", new { openid = Result.openid });//await Check(WCUser);
            //    case "grade":
            //        return RedirectToAction("Grade", "WeChat", new { openid = Result.openid });//await Check(WCUser);
            //}
            //return null;
        }
    }
}
