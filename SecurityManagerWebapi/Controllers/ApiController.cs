using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace SecurityManagerWebapi.Controllers
{
    [Route("[controller]/[action]")]
    public class ApiController : Controller
    {

        Global global { get { return Global.Instance; } }
        Config config { get { return global.Config; } }

        #region constructor
        public ApiController()
        {
        }
        #endregion

        #region helpers
        CommonResponse InvalidAuthResponse()
        {
            return new CommonResponse() { ExitCode = CommonResponseExitCodes.InvalidAuth };
        }

        CommonResponse SuccessfulResponse()
        {
            return new CommonResponse() { ExitCode = CommonResponseExitCodes.Successful };
        }

        CommonResponse ErrorResponse(string errMsg)
        {
            return new CommonResponse()
            {
                ExitCode = CommonResponseExitCodes.Error,
                ErrorMsg = errMsg
            };
        }

        bool CheckAuth(string password, int pin)
        {
            var is_valid = !string.IsNullOrEmpty(config?.AdminPassword) &&
                            config.Pin != 0 &&
                            config?.AdminPassword == password &&
                            config.Pin == pin;

            if (!is_valid)
            {
                var q = HttpContext.Request.Headers["X-Real-IP"];
                var url = "";
                if (q.Count > 0) url = q.First();
                global.LogWarning($"invalid login attempt from [{url}]");
                // todo : autoban
            }

            return is_valid;
        }
        #endregion

        [HttpPost]
        public CommonResponse SaveCred(string password, int pin, CredInfo cred)
        {
            try
            {
                if (!CheckAuth(password, pin)) return InvalidAuthResponse();                

                config.SaveCred(cred);

                return SuccessfulResponse();
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex.Message);
            }
        }

        [HttpPost]
        public CommonResponse RandomPassword(string password, int pin)
        {
            try
            {
                if (!CheckAuth(password, pin)) return InvalidAuthResponse();

                var res = new RandomPasswordResponse();

                res.Password = SearchAThing.NETCoreUtil.Util.RandomPassword();

                return res;
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex.Message);
            }
        }


        [HttpPost]
        public CommonResponse LoadCred(string password, int pin, string guid)
        {
            try
            {
                if (!CheckAuth(password, pin)) return InvalidAuthResponse();

                var response = new CredInfoResponse();

                response.Cred = config.LoadCred(guid);

                return response;
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex.Message);
            }
        }

        [HttpPost]
        public CommonResponse DeleteCred(string password, int pin, string guid)
        {
            try
            {
                if (!CheckAuth(password, pin)) return InvalidAuthResponse();                

                config.DeleteCred(guid);

                return SuccessfulResponse();
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex.Message);
            }
        }

        [HttpPost]
        public CommonResponse CredShortList(string password, int pin, string filter)
        {
            try
            {
                if (!CheckAuth(password, pin)) return InvalidAuthResponse();

                var response = new CredShortListResponse();

                response.CredShortList = config.GetCredShortList(filter);

                return response;
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex.Message);
            }
        }

        [HttpPost]
        public CommonResponse Aliases(string password, int pin)
        {
            try
            {
                if (!CheckAuth(password, pin)) return InvalidAuthResponse();

                var response = new AliasResponse();

                response.Aliases = config.GetAliases();

                return response;
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex.Message);
            }
        }

        [HttpPost]
        public CommonResponse IsAuthValid(string password, int pin)
        {
            try
            {
                if (!CheckAuth(password, pin)) return InvalidAuthResponse();
                return SuccessfulResponse();
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex.Message);
            }
        }        

    }
}
