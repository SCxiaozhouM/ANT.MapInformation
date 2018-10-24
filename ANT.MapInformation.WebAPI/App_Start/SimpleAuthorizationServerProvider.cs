using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using ANT.MapInformation.Dapper;
using ANT.MapInformation.Entity;
using Microsoft.Owin.Security.OAuth;

namespace ANT.MapInformation.WebAPI
{
    /// <summary>
    /// OAuth身份认证
    /// </summary>
    public class SimpleAuthorizationServerProvider: OAuthAuthorizationServerProvider
    {



        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return base.ValidateClientAuthentication(context);//验证客户端身份验证
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new []{"*"});
            /*
             * 身份验证
             */
            var cooike = new HttpCookie("name", context.UserName);
            HttpContext.Current.Response.AppendCookie(cooike);
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("sub", context.UserName));
            identity.AddClaim(new Claim("role", "user"));
            context.Validated(identity);

        }
    }
}