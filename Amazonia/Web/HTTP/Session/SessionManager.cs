﻿using System;
using System.Web;
using System.Web.Security;
using Es.Udc.DotNet.Amazonia.Model;
using Es.Udc.DotNet.Amazonia.Model.ClientServiceImp;
using Es.Udc.DotNet.Amazonia.Web.HTTP.Util;
using Es.Udc.DotNet.Amazonia.Web.HTTP.View.ApplicationObjects;
using Es.Udc.DotNet.ModelUtil.IoC;

namespace Es.Udc.DotNet.Amazonia.Web.HTTP.Session
{
    /// <summary>
    /// Encapsules access to session data
    /// </summary>
    public class SessionManager
    {
        public const String LOCALE_SESSION_ATTRIBUTE = "locale";
        public static readonly String USER_SESSION_ATTRIBUTE = "userSession";

        private static IClientService clientService;

        public IClientService ClientService
        {
            set { clientService = value; }
        }

        static SessionManager()
        {
            IIoCManager iiocManager = (IIoCManager)HttpContext.Current.Application["managerIoC"];
            clientService = iiocManager.Resolve<IClientService>();
        }

        public static void SetLocale(HttpContext context, Locale locale)
        {
            context.Session[LOCALE_SESSION_ATTRIBUTE] = locale;
        }

        public static Locale GetLocale(HttpContext context)
        {
            Locale locale = (Locale)context.Session[LOCALE_SESSION_ATTRIBUTE];

            return (locale);
        }

        public static Boolean IsLocaleDefined(HttpContext context)
        {
            if (context.Session == null)
                return false;
            else
                return (context.Session[LOCALE_SESSION_ATTRIBUTE] != null);
        }

        /// <summary>
        /// Registers the user.
        /// </summary>
        /// <param name="context">Http Context includes request, response, etc.</param>
        /// <param name="loginName">Username</param>
        /// <param name="clearPassword">Password in clear text</param>
        /// <param name="userProfileDetails">The user profile details.</param>
        /// <exception cref="DuplicateInstanceException"/>
        public static void RegisterUser(HttpContext context, String loginName, String clearPassword, ClientDTO clientDTO)
        {
            /* Register user. */
            Client client = clientService.RegisterClient(loginName, clearPassword, clientDTO);
            long clientId = client.id;

            /* Insert necessary objects in the session. */
            UserSession userSession = new UserSession();
            userSession.UserProfileId = clientId;
            userSession.FirstName = clientDTO.FirstName;

            Locale locale = new Locale(clientDTO.Language);

            SessionManager.UpdateSessionForAuthenticatedUser(context, userSession, locale);

            FormsAuthentication.SetAuthCookie(loginName, false);
        }

        /// <summary>
        /// Login method. Authenticates an user in the current context.
        /// </summary>
        /// <param name="context">Http Context includes request, response, etc.</param>
        /// <param name="loginName">Username</param>
        /// <param name="clearPassword">Password in clear text</param>
        /// <param name="rememberMyPassword">Remember password to the next logins</param>
        /// <exception cref="IncorrectPasswordException"/>
        /// <exception cref="InstanceNotFoundException"/>
        public static void Login(HttpContext context, String loginName,
           String clearPassword, Boolean rememberMyPassword)
        {
            /* Try to login, and if successful, update session with the necessary
             * objects for an authenticated user. */
            LoginDTO loginResult = DoLogin(context, loginName,
                clearPassword, false, rememberMyPassword);

            /* Add cookies if requested. */
            if (rememberMyPassword)
            {
                CookiesManager.LeaveCookies(context, loginName,
                    loginResult.EncryptedPassword);
            }
        }

        /// <summary>
        /// Tries to log in with the corresponding method of
        /// <c>UserService</c>, and if successful, inserts in the
        /// session the necessary objects for an authenticated user.
        /// </summary>
        /// <param name="context">Http Context includes request, response, etc.</param>
        /// <param name="loginName">Username</param>
        /// <param name="password">User Password</param>
        /// <param name="passwordIsEncrypted">Password is either encrypted or
        /// in clear text</param>
        /// <param name="rememberMyPassword">Remember password to the next
        /// logins</param>
        /// <exception cref="IncorrectPasswordException"/>
        /// <exception cref="InstanceNotFoundException"/>
        private static LoginDTO DoLogin(HttpContext context,
             String loginName, String password, Boolean passwordIsEncrypted,
             Boolean rememberMyPassword)
        {
            LoginDTO loginDTO = clientService.Login(loginName, password,
                    passwordIsEncrypted);

            /* Insert necessary objects in the session. */

            UserSession userSession = new UserSession();
            userSession.UserProfileId = loginDTO.UserProfileId;
            userSession.FirstName = loginDTO.FirstName;

            Locale locale =
                new Locale(loginDTO.Language);

            UpdateSessionForAuthenticatedUser(context, userSession, locale);

            return loginDTO;
        }


        /// <summary>
        /// Destroys the session, and removes the cookies if the user had
        /// selected "remember my password".
        /// </summary>
        /// <param name="context">Http Context includes request, response, etc.</param>
        public static void Logout(HttpContext context)
        {
            /* Remove cookies. */
            CookiesManager.RemoveCookies(context);

            /* Invalidate session. */
            context.Session.Abandon();

            /* Invalidate Authentication Ticket */
            FormsAuthentication.SignOut();
        }

        /// <summary>
        /// Updates the session values for an previously authenticated user
        /// </summary>
        /// <param name="context">Http Context includes request, response, etc.</param>
        /// <param name="userSession">The user data stored in session.</param>
        /// <param name="locale">The locale info.</param>
        public static void UpdateSessionForAuthenticatedUser(
            HttpContext context, UserSession userSession, Locale locale)
        {
            /* Insert objects in session. */
            context.Session.Add(USER_SESSION_ATTRIBUTE, userSession);
            context.Session.Add(LOCALE_SESSION_ATTRIBUTE, locale);
        }

        /// <sumary>
        /// Guarantees that the session will have the necessary objects if the
        /// user has been authenticated or had selected "remember my password"
        /// in the past.
        /// </sumary>
        /// <param name="context">Http Context includes request, response, etc.</param>
        public static void TouchSession(HttpContext context)
        {
            /* Check if "UserSession" object is in the session. */
            UserSession userSession = null;

            if (context.Session != null)
            {
                userSession =
                    (UserSession)context.Session[USER_SESSION_ATTRIBUTE];

                // If userSession object is in the session, nothing should be doing.
                if (userSession != null)
                {
                    return;
                }
            }

            /*
             * The user had not been authenticated or his/her session has
             * expired. We need to check if the user has selected "remember my
             * password" in the last login (login name and password will come
             * as cookies). If so, we reconstruct user's session objects.
             */
            UpdateSessionFromCookies(context);
        }

        /// <summary>
        /// Tries to login (inserting necessary objects in the session) by using
        /// cookies (if present).
        /// </summary>
        /// <param name="context">Http Context includes request, response, etc.</param>
        private static void UpdateSessionFromCookies(HttpContext context)
        {
            HttpRequest request = context.Request;
            if (request.Cookies == null)
            {
                return;
            }

            /*
             * Check if the login name and the encrypted password come as
             * cookies.
             */
            String loginName = CookiesManager.GetLoginName(context);
            String encryptedPassword = CookiesManager.GetEncryptedPassword(context);

            if ((loginName == null) || (encryptedPassword == null))
            {
                return;
            }

            /* If loginName and encryptedPassword have valid values (the user selected "remember
             * my password" option) try to login, and if successful, update session with the
             * necessary objects for an authenticated user.
             */
            try
            {
                DoLogin(context, loginName, encryptedPassword, true, true);

                /* Authentication Ticket. */
                FormsAuthentication.SetAuthCookie(loginName, true);
            }
            catch (Exception)
            { // Incorrect loginName or encryptedPassword
                return;
            }
        }
    }
}