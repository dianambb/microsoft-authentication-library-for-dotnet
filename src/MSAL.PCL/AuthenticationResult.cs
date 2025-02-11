﻿//----------------------------------------------------------------------
//
// Copyright (c) Microsoft Corporation.
// All rights reserved.
//
// This code is licensed under the MIT License.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Identity.Client.Internal;

namespace Microsoft.Identity.Client
{
    /// <summary>
    /// Contains the results of one token acquisition operation. 
    /// </summary>
    [DataContract]
    public sealed class AuthenticationResult
    {
        private const string Oauth2AuthorizationHeader = "Bearer ";

        /// <summary>
        /// Creates result returned from AcquireToken. Except in advanced scenarios related to token caching, you do not need to create any instance of AuthenticationResult.
        /// </summary>
        /// <param name="tokenType">Type of the Token returned</param>
        /// <param name="token">The Token requested</param>
        /// <param name="expiresOn">The point in time in which the Access Token returned in the Token property ceases to be valid</param>
        internal AuthenticationResult(string tokenType, string token, DateTimeOffset expiresOn)
        {
            this.TokenType = tokenType;
            this.Token = token;
            this.ExpiresOn = DateTime.SpecifyKind(expiresOn.DateTime, DateTimeKind.Utc);
        }

        /// <summary>
        /// Gets the type of the Token returned. 
        /// </summary>
        [DataMember]
        public string TokenType { get; private set; }

        /// <summary>
        /// Gets the Access Token requested.
        /// </summary>
        [DataMember]
        public string Token { get; internal set; }

        /// <summary>
        /// Gets the point in time in which the Access Token returned in the Token property ceases to be valid.
        /// This value is calculated based on current UTC time measured locally and the value expiresIn received from the service.
        /// </summary>
        [DataMember]
        public DateTimeOffset ExpiresOn { get; internal set; }

        /// <summary>
        /// Gets an identifier for the tenant the token was acquired from. This property will be null if tenant information is not returned by the service.
        /// </summary>
        [DataMember]
        public string TenantId { get; private set; }


        /// <summary>
        /// Gets an identifier for the family the token was acquired from. This property will be null if tenant information is not returned by the service.
        /// </summary>
        [DataMember]
        internal string FamilyId { get; set; }


        /// <summary>
        /// Gets otherUser information including otherUser Id. Some elements in User might be null if not returned by the service.
        /// </summary>
        [DataMember]
        public User User { get; internal set; }

        /// <summary>
        /// Gets the entire Id Token if returned by the service or null if no Id Token is returned.
        /// </summary>
        [DataMember]
        public string IdToken { get; internal set; }

        /// <summary>
        /// Gets the scope values returned from the service.
        /// </summary>
        public string[] Scope { get { return ScopeSet.AsArray(); } }


        /// <summary>
        /// Gets the scope values returned from the service.
        /// </summary>
        [DataMember]
        internal HashSet<string >ScopeSet { get; set; }

        /// <summary>
        /// Creates authorization header from authentication result.
        /// </summary>
        /// <returns>Created authorization header</returns>
        public string CreateAuthorizationHeader()
        {
            return Oauth2AuthorizationHeader + this.Token;
        }

        internal void UpdateTenantAndUser(string tenantId, string idToken, User otherUser)
        {
            this.TenantId = tenantId;
            this.IdToken = idToken;
            if (otherUser != null)
            {
                this.User = new User(otherUser);
            }
        }
    }
}
