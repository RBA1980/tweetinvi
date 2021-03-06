﻿using System;
using System.Collections.Generic;
using Tweetinvi.Events;
using Tweetinvi.Models;

namespace Tweetinvi.Parameters
{
    public interface IUploadOptionalParameters
    {
        /// <summary>
        /// Type of element that you want to publish.
        /// This property will modify the QueryMediaType.
        /// </summary>
        MediaType? MediaType { get; set; }

        /// <summary>
        /// Type of element that you want to publish.
        /// This will be used as the ContentType in the HttpRequest.
        /// </summary>
        string QueryMediaType { get; set; }

        /// <summary>
        /// Type of upload. `tweet_video` allows to access the STATUS of the upload processing.
        /// This property will modify the QueryMediaCategory.
        /// </summary>
        MediaCategory? MediaCategory { get; set; }

        /// <summary>
        /// Type of upload. `tweet_video` allows to access the STATUS of the upload processing.
        /// </summary>
        string QueryMediaCategory { get; set; }

        /// <summary>
        /// Maximum size of a chunk size (in bytes) for a single upload.
        /// </summary>
        int MaxChunkSize { get; set; }

        /// <summary>
        /// Timeout after which a chunk request will fail.
        /// </summary>
        TimeSpan? Timeout { get; set; }

        /// <summary>
        /// User Ids who are allowed to use the uploaded media.
        /// </summary>
        List<long> AdditionalOwnerIds { get; set; }

        /// <summary>
        /// When an upload completes Twitter takes few seconds to process the media
        /// and confirm that it is a media that can be used on the platform.
        /// With WaitForTwitterProcessing enabled, Tweetinvi will wait for Twitter
        /// to confirm that the media has been successfully processed.
        /// </summary>
        bool WaitForTwitterProcessing { get; set; }

        /// <summary>
        /// Additional parameters to use during the upload INIT HttpRequest.
        /// </summary>
        ICustomRequestParameters InitCustomRequestParameters { get; set; }

        /// <summary>
        /// Additional parameters to use during the upload APPEND HttpRequest.
        /// </summary>
        ICustomRequestParameters AppendCustomRequestParameters { get; set; }

        /// <summary>
        /// Additional parameters to use during the upload FINALIZE HttpRequest.
        /// </summary>
        ICustomRequestParameters FinalizeCustomRequestParameters { get; set; }

        /// <summary>
        /// Event to notify that the upload state has changed
        /// </summary>
        Action<IMediaUploadProgressChangedEventArgs> UploadStateChanged { get; set; }
    }

    /// <inheritdoc/>
    public class UploadOptionalParameters : IUploadOptionalParameters
    {
        public UploadOptionalParameters()
        {
            QueryMediaType = "media";
            MaxChunkSize = TwitterLimits.DEFAULTS.UPLOAD_MAX_CHUNK_SIZE;
            AdditionalOwnerIds = new List<long>();
            WaitForTwitterProcessing = true;

            InitCustomRequestParameters = new CustomRequestParameters();
            AppendCustomRequestParameters = new CustomRequestParameters();
            FinalizeCustomRequestParameters = new CursorQueryParameters();
        }

        /// <inheritdoc/>
        public  MediaType? MediaType
        {
            get
            {
                switch (QueryMediaType)
                {
                    case "media":
                        return Models.MediaType.Media;
                    case "video/mp4":
                        return Models.MediaType.VideoMp4;
                    default:
                        return null;
                }
            }
            set
            {
                switch (value)
                {
                    case Models.MediaType.VideoMp4:
                        QueryMediaType = "video/mp4";
                        break;
                    default:
                        QueryMediaType = "media"; // By default we need to inform Twitter that the binary is a media
                        break;
                }
            }
        }

        /// <inheritdoc/>
        public string QueryMediaType { get; set; }
        /// <inheritdoc/>
        public MediaCategory? MediaCategory
        {
            get
            {
                switch (QueryMediaCategory)
                {
                    case "tweet_video":
                        return Models.MediaCategory.Video;
                    case "tweet_gif":
                        return Models.MediaCategory.Gif;
                    case "tweet_image":
                        return Models.MediaCategory.Image;
                    case "dm_image":
                        return Models.MediaCategory.DmImage;
                    case "dm_gif":
                        return Models.MediaCategory.DmGif;
                    case "dm_video":
                        return Models.MediaCategory.DmVideo;
                    default:
                        return null;
                }
            }
            set
            {
                switch (value)
                {
                    case Models.MediaCategory.Video:
                        QueryMediaCategory = "tweet_video";
                        break;
                    case Models.MediaCategory.Gif:
                        QueryMediaCategory = "tweet_gif";
                        break;
                    case Models.MediaCategory.Image:
                        QueryMediaCategory = "tweet_image";
                        break;
                    case Models.MediaCategory.DmImage:
                        QueryMediaCategory = "dm_image";
                        break;
                    case Models.MediaCategory.DmGif:
                        QueryMediaCategory = "dm_gif";
                        break;
                    case Models.MediaCategory.DmVideo:
                        QueryMediaCategory = "dm_video";
                        break;
                    default:
                        QueryMediaCategory = null;
                        break;
                }
            }
        }
        /// <inheritdoc/>
        public string QueryMediaCategory { get; set; }
        /// <inheritdoc/>
        public int MaxChunkSize { get; set; }
        /// <inheritdoc/>
        public TimeSpan? Timeout { get; set; }
        /// <inheritdoc/>
        public List<long> AdditionalOwnerIds { get; set; }
        /// <inheritdoc/>
        public bool WaitForTwitterProcessing { get; set; }
        /// <inheritdoc/>
        public ICustomRequestParameters InitCustomRequestParameters { get; set; }
        /// <inheritdoc/>
        public ICustomRequestParameters AppendCustomRequestParameters { get; set; }
        /// <inheritdoc/>
        public ICustomRequestParameters FinalizeCustomRequestParameters { get; set; }
        /// <inheritdoc/>
        public Action<IMediaUploadProgressChangedEventArgs> UploadStateChanged { get; set; }
    }
}
