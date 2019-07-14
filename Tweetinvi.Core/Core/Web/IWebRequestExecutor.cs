﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Tweetinvi.Models;
using Tweetinvi.Models.Interfaces;

namespace Tweetinvi.Core.Web
{
    /// <summary>
    /// Generate a Token that can be used to perform OAuth queries
    /// </summary>
    public interface IWebRequestExecutor
    {
        /// <summary>
        /// Execute a TwitterQuery and return the resulting json data.
        /// </summary>
        Task<ITwitterResponse> ExecuteQuery(ITwitterRequest request, ITwitterClientHandler handler = null);

        /// <summary>
        /// Execute a multipart TwitterQuery and return the resulting json data.
        /// </summary>
        Task<ITwitterResponse> ExecuteMultipartQuery(ITwitterRequest request, string contentId,
            IEnumerable<byte[]> binaries);
    }
}