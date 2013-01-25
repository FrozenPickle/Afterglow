using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;

namespace Afterglow.Web.Host
{
    public class AfterglowCredentialsAuthProvider: CredentialsAuthProvider
    {
        public override bool TryAuthenticate(ServiceStack.ServiceInterface.IServiceBase authService, string userName, string password)
        {
            return (Program.Runtime.Setup.UserName ?? "").ToLower() == (userName ?? "").ToLower()
                && (Program.Runtime.Setup.Password ?? "") == (password ?? "");
        }

        public override void OnAuthenticated(ServiceStack.ServiceInterface.IServiceBase authService, IAuthSession session, IOAuthTokens tokens, Dictionary<string, string> authInfo)
        {
            session.DisplayName = session.UserAuthName;
            session.IsAuthenticated = true;
            authService.SaveSession(session, SessionExpiry);
        }
    }
}
