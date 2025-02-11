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
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Microsoft.Identity.Client.Internal
{
    [DataContract]
    internal class AuthenticationResultEx
    {
        [DataMember]
        public AuthenticationResult Result { get; set; }

        /// <summary>
        /// Gets the Refresh Token associated with the requested Access Token. Note: not all operations will return a Refresh Token.
        /// </summary>
        [DataMember]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Gets a value indicating whether the refresh token can be used for requesting access token for other resources.
        /// </summary>
        internal bool IsMultipleScopeRefreshToken
        {
            get
            {
                return (!string.IsNullOrWhiteSpace(this.RefreshToken));
                    // && this.ScopeInResponse!=null && this.ScopeInResponse.Length > 0);
            }            
        }

        [DataMember]
        public string UserAssertionHash { get; set; }

        /// <summary>
        /// Serializes the object to a JSON string
        /// </summary>
        /// <returns>Deserialized authentication result</returns>
        public static AuthenticationResultEx Deserialize(string serializedObject)
        {
            AuthenticationResultEx resultEx;
            var serializer = new DataContractJsonSerializer(typeof(AuthenticationResultEx));
            byte[] serializedObjectBytes = Encoding.UTF8.GetBytes(serializedObject);
            using (var stream = new MemoryStream(serializedObjectBytes))
            {
                resultEx = (AuthenticationResultEx)serializer.ReadObject(stream);
            }

            return resultEx;
        }

        /// <summary>
        /// Serializes the object to a JSON string
        /// </summary>
        /// <returns>Serialized authentication result</returns>
        public string Serialize()
        {
            string serializedObject;
            var serializer = new DataContractJsonSerializer(typeof(AuthenticationResultEx));
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.WriteObject(stream, this);
                serializedObject = Encoding.UTF8.GetString(stream.ToArray(), 0, (int)stream.Position);
            }

            return serializedObject;
        }

        internal Exception Exception { get; set; }
    }
}
