﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.IdentityModel.Clients.ActiveDirectory
{
    /// <summary>
    /// Native applications (desktop/phone/iOS/Android).
    /// </summary>
    public sealed class PublicClientApplication : AbstractClientApplication
    {
        private const string DEFAULT_CLIENT_ID = "default-client-id";
        private const string DEFAULT_REDIRECT_URI = "default-redirect-uri";

        /// <summary>
        /// Default consutructor of the application. It is here to emphasise the lack of parameters.
        /// </summary>
        public PublicClientApplication():this(DEFAULT_AUTHORTIY)
        {
        }

        public PublicClientApplication(string authority):base(authority, DEFAULT_CLIENT_ID, DEFAULT_REDIRECT_URI)
        {
        }
        
        /// <summary>
        /// .NET specific property that allows configuration of platform specific properties. For example, in iOS/Android it would include the flag to enable/disable broker.
        /// </summary>
        public IPlatformParameters PlatformParameters { get; set; }

        /// <summary>
        /// Returns a User centric view over the cache that provides a list of all the signed in users.
        /// </summary>
        public IEnumerable<User> GetUsers(string identifier)
        {
            return null;
        }

        //TODO look into adding user identifier when domain cannot be queried or privacy settings are against you
        /// <summary>
        /// .NET specific method for intergrated auth. To support Xamarin, we would need to move these to platform specific libraries.
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public async Task<AuthenticationResult> AcquireTokenWithIntegratedAuthAsync(string[] scope)
        {
            return null;
        }

        /// <summary>
        /// .NET specific method for intergrated auth. To support Xamarin, we would need to move these to platform specific libraries.
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="authority"></param>
        /// <returns></returns>
        public async Task<AuthenticationResult> AcquireTokenWithIntegratedAuthAsync(string[] scope, string authority)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public async Task<AuthenticationResult> AcquireTokenSilentAsync(string[] scope)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<AuthenticationResult> AcquireTokenSilentAsync(string[] scope, User userId)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<AuthenticationResult> AcquireTokenSilentAsync(string[] scope, string userId)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="userId"></param>
        /// <param name="authority"></param>
        /// <returns></returns>
        public async Task<AuthenticationResult> AcquireTokenSilentAsync(string[] scope, string userId,
            string authority, string policy)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="userId"></param>
        /// <param name="authority"></param>
        /// <returns></returns>
        public async Task<AuthenticationResult> AcquireTokenSilentAsync(string[] scope, User userId,
            string authority, string policy)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public async Task<AuthenticationResult> AcquireTokenAsync(string[] scope)
        {
            return null;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<AuthenticationResult> AcquireTokenAsync(string[] scope, string identifier)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="userId"></param>
        /// <param name="extraQueryParameters"></param>
        /// <returns></returns>
        public async Task<AuthenticationResult> AcquireTokenAsync(string[] scope, string identifier,
            UiOptions options, string extraQueryParameters)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="userId"></param>
        /// <param name="extraQueryParameters"></param>
        /// <param name="additionalScope"></param>
        /// <param name="authority"></param>
        /// <returns></returns>
        public async Task<AuthenticationResult> AcquireTokenAsync(string[] scope, string identifier,
            UiOptions options, string extraQueryParameters, string[] additionalScope, string authority)
        {
            return null;
        }

        //what about device code methods?
        //TODO we should look at them later.



        internal IWebUI CreateWebAuthenticationDialog(IPlatformParameters parameters)
        {
            return PlatformPlugin.WebUIFactory.CreateAuthenticationDialog(parameters);
        }

        private async Task<AuthenticationResult> AcquireTokenUsingIntegratedAuthCommonAsync(string[] scope, string clientId, UserCredential userCredential, string policy)
        {
            var handler = new AcquireTokenNonInteractiveHandler(this.Authenticator, this.TokenCache, scope, clientId, userCredential, policy);
            return await handler.RunAsync().ConfigureAwait(false);
        }

        private async Task<AuthenticationResult> AcquireTokenCommonAsync(string[] scope, string clientId, UserAssertion userAssertion, string policy)
        {
            var handler = new AcquireTokenNonInteractiveHandler(this.Authenticator, this.TokenCache, scope, clientId, userAssertion, policy);
            return await handler.RunAsync().ConfigureAwait(false);
        }

        private async Task<AuthenticationResult> AcquireTokenCommonAsync(string[] scope, string[] additionalScope, string clientId, Uri redirectUri, string loginHint, string extraQueryParameters, string policy)
        {
            var handler = new AcquireTokenInteractiveHandler(this.Authenticator, this.TokenCache, scope, additionalScope, clientId, redirectUri, this.PlatformParameters, loginHint, extraQueryParameters, policy, this.CreateWebAuthenticationDialog(this.PlatformParameters));
            return await handler.RunAsync().ConfigureAwait(false);
        }


    }
}
