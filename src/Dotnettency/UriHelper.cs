// Copyright (c) .NET Foundation. All rights reserved.

//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at

//    http://www.apache.org/licenses/LICENSE-2.0

//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

// THIS FILE ORIGINATED FROM: https://github.com/aspnet/HttpAbstractions/blob/ab0185a0b8d0b7a80a6169fd78a45f00a28e057d/src/Microsoft.AspNetCore.Http.Extensions/UriHelper.cs
// I (Darrell William Tunnell) have copied and made amendments to this file (as per below), and am including this as a "prominent notice" in conjunction with the licence requirements of the Apache licence version 2.0.
using Microsoft.AspNetCore.Http;
using System;
using System.Text;

namespace Dotnettency
{
    /// <summary>
    /// A helper class for constructing Uris from a Request.
    /// </summary>
    public static class UriHelper
    {
        private const string SchemeDelimiter = "://";

        /// <summary>
        /// Returns the combined components of the request URL as a URI.
        /// </summary>
        /// <param name="request">The request to assemble the uri pieces from.</param>
        /// <returns></returns>
        public static Uri GetUri(this HttpRequest request)
        {
            var host = request.Host.Value;
            var pathBase = request.PathBase.Value;
            var path = request.Path.Value;
            var queryString = request.QueryString.Value;

            // PERF: Calculate string length to allocate correct buffer size for StringBuilder.
            var length = request.Scheme.Length + SchemeDelimiter.Length + host.Length
                + pathBase.Length + path.Length + queryString.Length;

            return new Uri(new StringBuilder(length)
                .Append(request.Scheme)
                .Append(SchemeDelimiter)
                .Append(host)
                .Append(pathBase)
                .Append(path)
                .Append(queryString)
                .ToString());
        }

        /// <summary>
        /// Returns the combined components of the request URL authority.
        /// </summary>
        /// <param name="request">The request to assemble the uri pieces from.</param>
        /// <returns></returns>
        public static Uri GetAuthorityUri(this HttpRequest request)
        {
            var host = request.Host.Value;
            var pathBase = request.PathBase.Value;
            var queryString = request.QueryString.Value;

            // PERF: Calculate string length to allocate correct buffer size for StringBuilder.
            var length = request.Scheme.Length + SchemeDelimiter.Length + host.Length
                + pathBase.Length;

            return new Uri(new StringBuilder(length)
                .Append(request.Scheme)
                .Append(SchemeDelimiter)
                .Append(host)
                .Append(pathBase)
                .Append(queryString)
                .ToString());
        }
    }
}
