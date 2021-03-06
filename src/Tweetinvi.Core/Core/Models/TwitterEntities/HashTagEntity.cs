﻿using Newtonsoft.Json;
using Tweetinvi.Core.Extensions;
using Tweetinvi.Models.Entities;

namespace Tweetinvi.Core.Models.TwitterEntities
{
    /// <summary>
    /// A hashtag is a keyword prefixed by # and representing a category of tweet
    /// This class stores information related with an user mention
    /// </summary>
    public class HashtagEntity : IHashtagEntity
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("indices")]
        public int[] Indices { get; set; }

        public bool Equals(IHashtagEntity other)
        {
            if (other == null || Text != other.Text)
            {
                return false;
            }

            if (Indices == null || other.Indices == null)
            {
                return Indices == other.Indices;
            }

            return Indices.ContainsSameObjectsAs(other.Indices, true);
        }

        public override string ToString()
        {
            return $"#{Text}";
        }
    }
}