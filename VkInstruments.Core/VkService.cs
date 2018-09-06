﻿using System;
using System.Collections.Generic;
using System.Linq;
using VkInstruments.Core.VkSystem;
using VkNet.Model;
using VkNet.Model.RequestParams;
using VkNet.Model.RequestParams.Database;

namespace VkInstruments.Core
{
    public class VkService : IVkService
    {
        private IVkSystem _vk;

        public VkService(IVkSystem vk)
        {
            _vk = vk;
        }

        public IEnumerable<long> ParseLikesFromPost(string postLink)
        {
            return Parser.GetLikes(_vk.Vk, postLink);
        }

        public IEnumerable<long> ParseLikesFromPosts(ICollection<string> postLinkColletion)
        {
            var postLinks = postLinkColletion.ToList();
            var likeIds = Parser.GetLikes(_vk.Vk, postLinks[0]);
            for (var i = 1; i < postLinks.Count; i++)
            {
                likeIds.AddRange(Parser.GetLikes(_vk.Vk, postLinks[i]));
            }

            return likeIds;
        }

        public IEnumerable<User> FilterIds(string userIds, UserSearchParams @params)
        {
            var userNames = userIds.Replace("vk.com/id", "")
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            var users = _vk.Vk.Users.Get(userNames, UserFilter.GetProfileFields(@params));
            var filteredUsers = UserFilter.ApplyFilter(users, @params);
            return filteredUsers;
        }

        public Dictionary<long?, string> GetCountries(bool? needAll = null)
        {
            return _vk.Vk.Database
                .GetCountries(needAll)
                .ToDictionary(x => x.Id, x => x.Title);
        }

        public Dictionary<long?, string> GetCities(int countryId)
        {
            return _vk.Vk.Database
                .GetCities(new GetCitiesParams { CountryId = countryId })
                .ToDictionary(x => x.Id, x => x.Title);
        }
    }
}