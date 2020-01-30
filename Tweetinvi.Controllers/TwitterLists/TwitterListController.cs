﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweetinvi.Core.Controllers;
using Tweetinvi.Core.Factories;
using Tweetinvi.Core.Iterators;
using Tweetinvi.Core.Parameters;
using Tweetinvi.Core.QueryGenerators;
using Tweetinvi.Core.Web;
using Tweetinvi.Logic.QueryParameters;
using Tweetinvi.Models;
using Tweetinvi.Models.DTO;
using Tweetinvi.Models.DTO.QueryDTO;
using Tweetinvi.Parameters;
using Tweetinvi.Parameters.ListsClient;

namespace Tweetinvi.Controllers.TwitterLists
{
    public class TwitterListController : ITwitterListController
    {
        private readonly ITweetFactory _tweetFactory;
        private readonly IUserFactory _userFactory;
        private readonly ITwitterListQueryExecutor _twitterListQueryExecutor;
        private readonly ITwitterListQueryParameterGenerator _twitterListQueryParameterGenerator;
        private readonly ITwitterListIdentifierFactory _twitterListIdentifierFactory;

        public TwitterListController(
            ITweetFactory tweetFactory,
            IUserFactory userFactory,
            ITwitterListQueryExecutor twitterListQueryExecutor,
            ITwitterListQueryParameterGenerator twitterListQueryParameterGenerator,
            ITwitterListIdentifierFactory twitterListIdentifierFactory)
        {
            _tweetFactory = tweetFactory;
            _userFactory = userFactory;
            _twitterListQueryExecutor = twitterListQueryExecutor;
            _twitterListQueryParameterGenerator = twitterListQueryParameterGenerator;
            _twitterListIdentifierFactory = twitterListIdentifierFactory;
        }

        public Task<ITwitterResult<ITwitterListDTO>> CreateList(ICreateListParameters parameters, ITwitterRequest request)
        {
            return _twitterListQueryExecutor.CreateList(parameters, request);
        }

        public Task<ITwitterResult<ITwitterListDTO>> GetList(IGetListParameters parameters, ITwitterRequest request)
        {
            return _twitterListQueryExecutor.GetList(parameters, request);
        }

        public Task<ITwitterResult<ITwitterListDTO[]>> GetUserLists(IGetUserListsParameters parameters, ITwitterRequest request)
        {
            return _twitterListQueryExecutor.GetUserLists(parameters, request);
        }

        public Task<ITwitterResult<ITwitterListDTO>> UpdateList(IUpdateListParameters parameters, ITwitterRequest request)
        {
            return _twitterListQueryExecutor.UpdateList(parameters, request);
        }

        Task<ITwitterResult<ITwitterListDTO>> ITwitterListController.DestroyList(IDestroyListParameters parameters, ITwitterRequest request)
        {
            return _twitterListQueryExecutor.DestroyList(parameters, request);
        }

        public Task<ITwitterResult<ITwitterListDTO>> AddMemberToList(IAddMemberToListParameters parameters, ITwitterRequest request)
        {
            return _twitterListQueryExecutor.AddMemberToList(parameters, request);
        }

        public ITwitterPageIterator<ITwitterResult<IUserCursorQueryResultDTO>> GetMembersOfListIterator(IGetMembersOfListParameters parameters, ITwitterRequest request)
        {
            var twitterCursorResult = new TwitterPageIterator<ITwitterResult<IUserCursorQueryResultDTO>>(
                parameters.Cursor,
                cursor =>
                {
                    var cursoredParameters = new GetMembersOfListParameters(parameters)
                    {
                        Cursor = cursor
                    };

                    return _twitterListQueryExecutor.GetMembersOfList(cursoredParameters, new TwitterRequest(request));
                },
                page => page.DataTransferObject.NextCursorStr,
                page => page.DataTransferObject.NextCursorStr == "0");

            return twitterCursorResult;
        }







        #region Get User Lists

        public async Task<IEnumerable<ITwitterList>> GetUserSubscribedLists(IUserIdentifier user, bool getOwnedListsFirst)
        {
            var listDTOs = await _twitterListQueryExecutor.GetUserSubscribedLists(user, getOwnedListsFirst);
            return null;
        }

        public async Task<IEnumerable<ITwitterList>> GetUserSubscribedLists(long userId, bool getOwnedListsFirst)
        {
            var listDTOs = await _twitterListQueryExecutor.GetUserSubscribedLists(new UserIdentifier(userId), getOwnedListsFirst);
            return null;
        }

        public async Task<IEnumerable<ITwitterList>> GetUserSubscribedLists(string userScreenName, bool getOwnedListsFirst)
        {
            var listDTOs = await _twitterListQueryExecutor.GetUserSubscribedLists(new UserIdentifier(userScreenName), getOwnedListsFirst);
            return null;
        }

        #endregion

        #region Owned Lists
        public Task<IEnumerable<ITwitterList>> GetUserOwnedLists(long userId, int maximumNumberOfListsToRetrieve)
        {
            return GetUserOwnedLists(new UserIdentifier(userId), maximumNumberOfListsToRetrieve);
        }

        public Task<IEnumerable<ITwitterList>> GetUserOwnedLists(string userScreenName, int maximumNumberOfListsToRetrieve)
        {
            return GetUserOwnedLists(new UserIdentifier(userScreenName), maximumNumberOfListsToRetrieve);
        }

        public async Task<IEnumerable<ITwitterList>> GetUserOwnedLists(IUserIdentifier user, int maximumNumberOfListsToRetrieve)
        {
            var listDTOs = await _twitterListQueryExecutor.GetUserOwnedLists(user, maximumNumberOfListsToRetrieve);
            return null;
        }
        #endregion

        #region Get Tweets from List
        public Task<IEnumerable<ITweet>> GetTweetsFromList(long listId)
        {
            var identifier = _twitterListIdentifierFactory.Create(listId);
            return GetTweetsFromList(identifier);
        }

        public Task<IEnumerable<ITweet>> GetTweetsFromList(string slug, IUserIdentifier owner)
        {
            var identifier = _twitterListIdentifierFactory.Create(slug, owner);
            return GetTweetsFromList(identifier);
        }

        public Task<IEnumerable<ITweet>> GetTweetsFromList(string slug, string ownerScreenName)
        {
            var identifier = _twitterListIdentifierFactory.Create(slug, ownerScreenName);
            return GetTweetsFromList(identifier);
        }

        public Task<IEnumerable<ITweet>> GetTweetsFromList(string slug, long ownerId)
        {
            var identifier = _twitterListIdentifierFactory.Create(slug, ownerId);
            return GetTweetsFromList(identifier);
        }

        public Task<IEnumerable<ITweet>> GetTweetsFromList(ITwitterListIdentifier list, IGetTweetsFromListParameters parameters = null)
        {
            var queryParameters = _twitterListQueryParameterGenerator.CreateTweetsFromListQueryParameters(list, parameters);
            return GetTweetsFromList(queryParameters);
        }

        private async Task<IEnumerable<ITweet>> GetTweetsFromList(IGetTweetsFromListQueryParameters queryParameters)
        {
            var tweetsDTO = await _twitterListQueryExecutor.GetTweetsFromList(queryParameters);
            return _tweetFactory.GenerateTweetsFromDTO(tweetsDTO, null, null);
        }
        #endregion

        #region Add Multiple Members to List

        public Task<MultiRequestsResult> AddMultipleMembersToList(long listId, IEnumerable<long> userIds)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(listId);
            return AddMultipleMembersToList(listIdentifier, userIds);
        }

        public Task<MultiRequestsResult> AddMultipleMembersToList(long listId, IEnumerable<string> userScreenNames)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(listId);
            return AddMultipleMembersToList(listIdentifier, userScreenNames);
        }

        public Task<MultiRequestsResult> AddMultipleMembersToList(long listId, IEnumerable<IUserIdentifier> users)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(listId);
            return AddMultipleMembersToList(listIdentifier, users);
        }

        public Task<MultiRequestsResult> AddMultipleMembersToList(string slug, long ownerId, IEnumerable<long> userIds)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, ownerId);
            return AddMultipleMembersToList(listIdentifier, userIds);
        }

        public Task<MultiRequestsResult> AddMultipleMembersToList(string slug, long ownerId, IEnumerable<string> userScreenNames)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, ownerId);
            return AddMultipleMembersToList(listIdentifier, userScreenNames);
        }

        public Task<MultiRequestsResult> AddMultipleMembersToList(string slug, long ownerId, IEnumerable<IUserIdentifier> users)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, ownerId);
            return AddMultipleMembersToList(listIdentifier, users);
        }

        public Task<MultiRequestsResult> AddMultipleMembersToList(string slug, string ownerScreenName, IEnumerable<long> userIds)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, ownerScreenName);
            return AddMultipleMembersToList(listIdentifier, userIds);
        }

        public Task<MultiRequestsResult> AddMultipleMembersToList(string slug, string ownerScreenName, IEnumerable<string> userScreenNames)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, ownerScreenName);
            return AddMultipleMembersToList(listIdentifier, userScreenNames);
        }

        public Task<MultiRequestsResult> AddMultipleMembersToList(string slug, string ownerScreenName, IEnumerable<IUserIdentifier> users)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, ownerScreenName);
            return AddMultipleMembersToList(listIdentifier, users);
        }

        public Task<MultiRequestsResult> AddMultipleMembersToList(string slug, IUserIdentifier owner, IEnumerable<long> userIds)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, owner);
            return AddMultipleMembersToList(listIdentifier, userIds);
        }

        public Task<MultiRequestsResult> AddMultipleMembersToList(string slug, IUserIdentifier owner, IEnumerable<string> userScreenNames)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, owner);
            return AddMultipleMembersToList(listIdentifier, userScreenNames);
        }

        public Task<MultiRequestsResult> AddMultipleMembersToList(string slug, IUserIdentifier owner, IEnumerable<IUserIdentifier> users)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, owner);
            return AddMultipleMembersToList(listIdentifier, users);
        }

        public Task<MultiRequestsResult> AddMultipleMembersToList(ITwitterListIdentifier list, IEnumerable<long> newUserIds)
        {
            var users = newUserIds.Select(userId => new UserIdentifier(userId));
            return AddMultipleMembersToList(list, users);
        }

        public Task<MultiRequestsResult> AddMultipleMembersToList(ITwitterListIdentifier list, IEnumerable<string> newUserScreenNames)
        {
            var users = newUserScreenNames.Select(screenName => new UserIdentifier(screenName));
            return AddMultipleMembersToList(list, users);
        }

        public Task<MultiRequestsResult> AddMultipleMembersToList(ITwitterListIdentifier list, IEnumerable<IUserIdentifier> newUserIdentifiers)
        {
            return _twitterListQueryExecutor.AddMultipleMembersToList(list, newUserIdentifiers);
        }
        #endregion

        #region Remove Member From List

        public Task<bool> RemoveMemberFromList(long listId, long newUserId)
        {
            var identifier = _twitterListIdentifierFactory.Create(listId);
            return RemoveMemberFromList(identifier, newUserId);
        }

        public Task<bool> RemoveMemberFromList(long listId, string newUserName)
        {
            var identifier = _twitterListIdentifierFactory.Create(listId);
            return RemoveMemberFromList(identifier, newUserName);
        }

        public Task<bool> RemoveMemberFromList(long listId, IUserIdentifier newUser)
        {
            var identifier = _twitterListIdentifierFactory.Create(listId);
            return RemoveMemberFromList(identifier, newUser);
        }

        public Task<bool> RemoveMemberFromList(string slug, long ownerId, long newUserId)
        {
            var identifier = _twitterListIdentifierFactory.Create(slug, ownerId);
            return RemoveMemberFromList(identifier, newUserId);
        }

        public Task<bool> RemoveMemberFromList(string slug, long ownerId, string newUserName)
        {
            var identifier = _twitterListIdentifierFactory.Create(slug, ownerId);
            return RemoveMemberFromList(identifier, newUserName);
        }

        public Task<bool> RemoveMemberFromList(string slug, long ownerId, IUserIdentifier newUser)
        {
            var identifier = _twitterListIdentifierFactory.Create(slug, ownerId);
            return RemoveMemberFromList(identifier, newUser);
        }

        public Task<bool> RemoveMemberFromList(string slug, string ownerScreenName, long newUserId)
        {
            var identifier = _twitterListIdentifierFactory.Create(slug, ownerScreenName);
            return RemoveMemberFromList(identifier, newUserId);
        }

        public Task<bool> RemoveMemberFromList(string slug, string ownerScreenName, string newUserName)
        {
            var identifier = _twitterListIdentifierFactory.Create(slug, ownerScreenName);
            return RemoveMemberFromList(identifier, newUserName);
        }

        public Task<bool> RemoveMemberFromList(string slug, string ownerScreenName, IUserIdentifier newUser)
        {
            var identifier = _twitterListIdentifierFactory.Create(slug, ownerScreenName);
            return RemoveMemberFromList(identifier, newUser);
        }

        public Task<bool> RemoveMemberFromList(string slug, IUserIdentifier owner, long newUserId)
        {
            var identifier = _twitterListIdentifierFactory.Create(slug, owner);
            return RemoveMemberFromList(identifier, newUserId);
        }

        public Task<bool> RemoveMemberFromList(string slug, IUserIdentifier owner, string newUserName)
        {
            var identifier = _twitterListIdentifierFactory.Create(slug, owner);
            return RemoveMemberFromList(identifier, newUserName);
        }

        public Task<bool> RemoveMemberFromList(string slug, IUserIdentifier owner, IUserIdentifier newUser)
        {
            var identifier = _twitterListIdentifierFactory.Create(slug, owner);
            return RemoveMemberFromList(identifier, newUser);
        }

        public Task<bool> RemoveMemberFromList(ITwitterListIdentifier list, long newUserId)
        {
            return RemoveMemberFromList(list, new UserIdentifier(newUserId));
        }

        public Task<bool> RemoveMemberFromList(ITwitterListIdentifier list, string newUserName)
        {
            return RemoveMemberFromList(list, new UserIdentifier(newUserName));
        }

        public Task<bool> RemoveMemberFromList(ITwitterListIdentifier list, IUserIdentifier newUser)
        {
            return _twitterListQueryExecutor.RemoveMemberFromList(list, newUser);
        }

        // Multiple

        public Task<MultiRequestsResult> RemoveMultipleMembersFromList(long listId, IEnumerable<long> userIds)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(listId);
            return RemoveMultipleMembersFromList(listIdentifier, userIds);
        }

        public Task<MultiRequestsResult> RemoveMultipleMembersFromList(long listId, IEnumerable<string> userScreenNames)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(listId);
            return RemoveMultipleMembersFromList(listIdentifier, userScreenNames);
        }

        public Task<MultiRequestsResult> RemoveMultipleMembersFromList(long listId, IEnumerable<IUserIdentifier> users)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(listId);
            return RemoveMultipleMembersFromList(listIdentifier, users);
        }

        public Task<MultiRequestsResult> RemoveMultipleMembersFromList(string slug, long ownerId, IEnumerable<long> userIds)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, ownerId);
            return RemoveMultipleMembersFromList(listIdentifier, userIds);
        }

        public Task<MultiRequestsResult> RemoveMultipleMembersFromList(string slug, long ownerId, IEnumerable<string> userScreenNames)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, ownerId);
            return RemoveMultipleMembersFromList(listIdentifier, userScreenNames);
        }

        public Task<MultiRequestsResult> RemoveMultipleMembersFromList(string slug, long ownerId, IEnumerable<IUserIdentifier> users)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, ownerId);
            return RemoveMultipleMembersFromList(listIdentifier, users);
        }

        public Task<MultiRequestsResult> RemoveMultipleMembersFromList(string slug, string ownerScreenName, IEnumerable<long> userIds)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, ownerScreenName);
            return RemoveMultipleMembersFromList(listIdentifier, userIds);
        }

        public Task<MultiRequestsResult> RemoveMultipleMembersFromList(string slug, string ownerScreenName, IEnumerable<string> userScreenNames)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, ownerScreenName);
            return RemoveMultipleMembersFromList(listIdentifier, userScreenNames);
        }

        public Task<MultiRequestsResult> RemoveMultipleMembersFromList(string slug, string ownerScreenName, IEnumerable<IUserIdentifier> users)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, ownerScreenName);
            return RemoveMultipleMembersFromList(listIdentifier, users);
        }

        public Task<MultiRequestsResult> RemoveMultipleMembersFromList(string slug, IUserIdentifier owner, IEnumerable<long> userIds)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, owner);
            return RemoveMultipleMembersFromList(listIdentifier, userIds);
        }

        public Task<MultiRequestsResult> RemoveMultipleMembersFromList(string slug, IUserIdentifier owner, IEnumerable<string> userScreenNames)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, owner);
            return RemoveMultipleMembersFromList(listIdentifier, userScreenNames);
        }

        public Task<MultiRequestsResult> RemoveMultipleMembersFromList(string slug, IUserIdentifier owner, IEnumerable<IUserIdentifier> users)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, owner);
            return RemoveMultipleMembersFromList(listIdentifier, users);
        }

        public Task<MultiRequestsResult> RemoveMultipleMembersFromList(ITwitterListIdentifier list, IEnumerable<long> userIds)
        {
            var users = userIds.Select(userId => new UserIdentifier(userId));
            return RemoveMultipleMembersFromList(list, users);
        }

        public Task<MultiRequestsResult> RemoveMultipleMembersFromList(ITwitterListIdentifier list, IEnumerable<string> userScreenNames)
        {
            var users = userScreenNames.Select(screenName => new UserIdentifier(screenName));
            return RemoveMultipleMembersFromList(list, users);
        }

        public Task<MultiRequestsResult> RemoveMultipleMembersFromList(ITwitterListIdentifier list, IEnumerable<IUserIdentifier> users)
        {
            return _twitterListQueryExecutor.RemoveMultipleMembersFromList(list, users);
        }

        #endregion

        #region GetUserSubscribedLists
        public Task<IEnumerable<ITwitterList>> GetUserSubscribedLists(long userId, int maxNumberOfListsToRetrieve)
        {
            return GetUserSubscribedLists(new UserIdentifier(userId), maxNumberOfListsToRetrieve);
        }

        public Task<IEnumerable<ITwitterList>> GetUserSubscribedLists(string userName, int maxNumberOfListsToRetrieve)
        {
            return GetUserSubscribedLists(new UserIdentifier(userName), maxNumberOfListsToRetrieve);
        }

        public async Task<IEnumerable<ITwitterList>> GetUserSubscribedLists(IUserIdentifier user, int maxNumberOfListsToRetrieve)
        {
            var listDTOs = await _twitterListQueryExecutor.GetUserSubscribedLists(user, maxNumberOfListsToRetrieve);
            return null;
        }
        #endregion

        #region Check Membership
        public Task<bool> CheckIfUserIsAListMember(long listId, long userId)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(listId);
            return CheckIfUserIsAListMember(listIdentifier, userId);
        }

        public Task<bool> CheckIfUserIsAListMember(long listId, string userScreenName)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(listId);
            return CheckIfUserIsAListMember(listIdentifier, userScreenName);
        }

        public Task<bool> CheckIfUserIsAListMember(long listId, IUserIdentifier user)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(listId);
            return CheckIfUserIsAListMember(listIdentifier, user);
        }

        public Task<bool> CheckIfUserIsAListMember(string slug, long ownerId, long userId)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, ownerId);
            return CheckIfUserIsAListMember(listIdentifier, userId);
        }

        public Task<bool> CheckIfUserIsAListMember(string slug, long ownerId, string userScreenName)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, ownerId);
            return CheckIfUserIsAListMember(listIdentifier, userScreenName);
        }

        public Task<bool> CheckIfUserIsAListMember(string slug, long ownerId, IUserIdentifier user)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, ownerId);
            return CheckIfUserIsAListMember(listIdentifier, user);
        }

        public Task<bool> CheckIfUserIsAListMember(string slug, string ownerScreenName, long userId)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, ownerScreenName);
            return CheckIfUserIsAListMember(listIdentifier, userId);
        }

        public Task<bool> CheckIfUserIsAListMember(string slug, string ownerScreenName, string userScreenName)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, ownerScreenName);
            return CheckIfUserIsAListMember(listIdentifier, userScreenName);
        }

        public Task<bool> CheckIfUserIsAListMember(string slug, string ownerScreenName, IUserIdentifier user)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, ownerScreenName);
            return CheckIfUserIsAListMember(listIdentifier, user);
        }

        public Task<bool> CheckIfUserIsAListMember(string slug, IUserIdentifier owner, long userId)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, owner);
            return CheckIfUserIsAListMember(listIdentifier, userId);
        }

        public Task<bool> CheckIfUserIsAListMember(string slug, IUserIdentifier owner, string userScreenName)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, owner);
            return CheckIfUserIsAListMember(listIdentifier, userScreenName);
        }

        public Task<bool> CheckIfUserIsAListMember(string slug, IUserIdentifier owner, IUserIdentifier user)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, owner);
            return CheckIfUserIsAListMember(listIdentifier, user);
        }

        public Task<bool> CheckIfUserIsAListMember(ITwitterListIdentifier listIdentifier, long userId)
        {
            return CheckIfUserIsAListMember(listIdentifier, new UserIdentifier(userId));
        }

        public Task<bool> CheckIfUserIsAListMember(ITwitterListIdentifier listIdentifier, string userScreenName)
        {
            return CheckIfUserIsAListMember(listIdentifier, new UserIdentifier(userScreenName));
        }

        public Task<bool> CheckIfUserIsAListMember(ITwitterListIdentifier listIdentifier, IUserIdentifier user)
        {
            return _twitterListQueryExecutor.CheckIfUserIsAListMember(listIdentifier, user);
        }
        #endregion

        #region Get list subscribers
        public Task<IEnumerable<IUser>> GetListSubscribers(long listId, int maximumNumberOfUsersToRetrieve = 100)
        {
            var identifier = _twitterListIdentifierFactory.Create(listId);
            return GetListSubscribers(identifier, maximumNumberOfUsersToRetrieve);
        }

        public Task<IEnumerable<IUser>> GetListSubscribers(string slug, IUserIdentifier owner, int maximumNumberOfUsersToRetrieve = 100)
        {
            var identifier = _twitterListIdentifierFactory.Create(slug, owner);
            return GetListSubscribers(identifier, maximumNumberOfUsersToRetrieve);
        }

        public Task<IEnumerable<IUser>> GetListSubscribers(string slug, string ownerScreenName, int maximumNumberOfUsersToRetrieve = 100)
        {
            var identifier = _twitterListIdentifierFactory.Create(slug, ownerScreenName);
            return GetListSubscribers(identifier, maximumNumberOfUsersToRetrieve);
        }

        public Task<IEnumerable<IUser>> GetListSubscribers(string slug, long ownerId, int maximumNumberOfUsersToRetrieve = 100)
        {
            var identifier = _twitterListIdentifierFactory.Create(slug, ownerId);
            return GetListSubscribers(identifier, maximumNumberOfUsersToRetrieve);
        }

        public async Task<IEnumerable<IUser>> GetListSubscribers(ITwitterListIdentifier list, int maximumNumberOfUsersToRetrieve = 100)
        {
            var usersDTO = await _twitterListQueryExecutor.GetListSubscribers(list, maximumNumberOfUsersToRetrieve);
            return _userFactory.GenerateUsersFromDTO(usersDTO, null);
        }
        #endregion

        #region Add subscriber to List

        public Task<bool> SubscribeAuthenticatedUserToList(long listId)
        {
            var identifier = _twitterListIdentifierFactory.Create(listId);
            return SubscribeAuthenticatedUserToList(identifier);
        }

        public Task<bool> SubscribeAuthenticatedUserToList(string slug, long ownerId)
        {
            var identifier = _twitterListIdentifierFactory.Create(slug, ownerId);
            return SubscribeAuthenticatedUserToList(identifier);
        }

        public Task<bool> SubscribeAuthenticatedUserToList(string slug, string ownerScreenName)
        {
            var identifier = _twitterListIdentifierFactory.Create(slug, ownerScreenName);
            return SubscribeAuthenticatedUserToList(identifier);
        }

        public Task<bool> SubscribeAuthenticatedUserToList(string slug, IUserIdentifier owner)
        {
            var identifier = _twitterListIdentifierFactory.Create(slug, owner);
            return SubscribeAuthenticatedUserToList(identifier);
        }

        public Task<bool> SubscribeAuthenticatedUserToList(ITwitterListIdentifier list)
        {
            return _twitterListQueryExecutor.SubscribeAuthenticatedUserToList(list);
        }

        #endregion

        #region UnSubscribeAuthenticatedUserFromList
        public Task<bool> UnSubscribeAuthenticatedUserFromList(long listId)
        {
            var identifier = _twitterListIdentifierFactory.Create(listId);
            return UnSubscribeAuthenticatedUserFromList(identifier);
        }

        public Task<bool> UnSubscribeAuthenticatedUserFromList(string slug, long ownerId)
        {
            var identifier = _twitterListIdentifierFactory.Create(slug, ownerId);
            return UnSubscribeAuthenticatedUserFromList(identifier);
        }

        public Task<bool> UnSubscribeAuthenticatedUserFromList(string slug, string ownerScreenName)
        {
            var identifier = _twitterListIdentifierFactory.Create(slug, ownerScreenName);
            return UnSubscribeAuthenticatedUserFromList(identifier);
        }

        public Task<bool> UnSubscribeAuthenticatedUserFromList(string slug, IUserIdentifier owner)
        {
            var identifier = _twitterListIdentifierFactory.Create(slug, owner);
            return UnSubscribeAuthenticatedUserFromList(identifier);
        }

        public Task<bool> UnSubscribeAuthenticatedUserFromList(ITwitterListIdentifier list)
        {
            return _twitterListQueryExecutor.UnSubscribeAuthenticatedUserFromList(list);
        }
        #endregion

        #region CheckIfUserIsAListSubscriber
        public Task<bool> CheckIfUserIsAListSubscriber(long listId, long userId)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(listId);
            return CheckIfUserIsAListSubscriber(listIdentifier, userId);
        }

        public Task<bool> CheckIfUserIsAListSubscriber(long listId, string userScreenName)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(listId);
            return CheckIfUserIsAListSubscriber(listIdentifier, userScreenName);
        }

        public Task<bool> CheckIfUserIsAListSubscriber(long listId, IUserIdentifier user)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(listId);
            return CheckIfUserIsAListSubscriber(listIdentifier, user);
        }

        public Task<bool> CheckIfUserIsAListSubscriber(string slug, long ownerId, long userId)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, ownerId);
            return CheckIfUserIsAListSubscriber(listIdentifier, userId);
        }

        public Task<bool> CheckIfUserIsAListSubscriber(string slug, long ownerId, string userScreenName)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, ownerId);
            return CheckIfUserIsAListSubscriber(listIdentifier, userScreenName);
        }

        public Task<bool> CheckIfUserIsAListSubscriber(string slug, long ownerId, IUserIdentifier user)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, ownerId);
            return CheckIfUserIsAListSubscriber(listIdentifier, user);
        }

        public Task<bool> CheckIfUserIsAListSubscriber(string slug, string ownerScreenName, long userId)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, ownerScreenName);
            return CheckIfUserIsAListSubscriber(listIdentifier, userId);
        }

        public Task<bool> CheckIfUserIsAListSubscriber(string slug, string ownerScreenName, string userScreenName)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, ownerScreenName);
            return CheckIfUserIsAListSubscriber(listIdentifier, userScreenName);
        }

        public Task<bool> CheckIfUserIsAListSubscriber(string slug, string ownerScreenName, IUserIdentifier user)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, ownerScreenName);
            return CheckIfUserIsAListSubscriber(listIdentifier, user);
        }

        public Task<bool> CheckIfUserIsAListSubscriber(string slug, IUserIdentifier owner, long userId)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, owner);
            return CheckIfUserIsAListSubscriber(listIdentifier, userId);
        }

        public Task<bool> CheckIfUserIsAListSubscriber(string slug, IUserIdentifier owner, string userScreenName)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, owner);
            return CheckIfUserIsAListSubscriber(listIdentifier, userScreenName);
        }

        public Task<bool> CheckIfUserIsAListSubscriber(string slug, IUserIdentifier owner, IUserIdentifier user)
        {
            var listIdentifier = _twitterListIdentifierFactory.Create(slug, owner);
            return CheckIfUserIsAListSubscriber(listIdentifier, user);
        }

        public Task<bool> CheckIfUserIsAListSubscriber(ITwitterListIdentifier listIdentifier, long userId)
        {
            return CheckIfUserIsAListSubscriber(listIdentifier, new UserIdentifier(userId));
        }

        public Task<bool> CheckIfUserIsAListSubscriber(ITwitterListIdentifier listIdentifier, string userScreenName)
        {
            return CheckIfUserIsAListSubscriber(listIdentifier, new UserIdentifier(userScreenName));
        }

        public Task<bool> CheckIfUserIsAListSubscriber(ITwitterListIdentifier listIdentifier, IUserIdentifier user)
        {
            return _twitterListQueryExecutor.CheckIfUserIsAListSubscriber(listIdentifier, user);
        }



        #endregion

        public Task<IEnumerable<ITwitterList>> GetUserListsMemberships(IUserIdentifier userIdentifier, IGetUserListMembershipsParameters parameters)
        {
            var queryParameters = new GetUserListMembershipsQueryParameters(userIdentifier);

            if (parameters != null)
            {
                queryParameters.Parameters = parameters;
            }

            return GetUserListsMemberships(queryParameters);
        }

        public async Task<IEnumerable<ITwitterList>> GetUserListsMemberships(IGetUserListMembershipsQueryParameters parameters)
        {
            var twitterListDtos = await _twitterListQueryExecutor.GetUserListMemberships(parameters);
            return null;
        }
    }
}