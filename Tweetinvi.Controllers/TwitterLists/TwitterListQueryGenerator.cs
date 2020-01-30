﻿using System.Collections.Generic;
using System.Text;
using Tweetinvi.Controllers.Properties;
using Tweetinvi.Controllers.Shared;
using Tweetinvi.Core;
using Tweetinvi.Core.Extensions;
using Tweetinvi.Core.Parameters;
using Tweetinvi.Core.QueryGenerators;
using Tweetinvi.Core.QueryValidators;
using Tweetinvi.Models;
using Tweetinvi.Parameters.ListsClient;

namespace Tweetinvi.Controllers.TwitterLists
{
    public interface ITwitterListQueryGenerator
    {
        // list
        string GetCreateListQuery(ICreateListParameters parameters);
        string GetListQuery(IGetListParameters parameters);
        string GetUserListsQuery(IGetUserListsParameters parameters);
        string GetUpdateListQuery(IUpdateListParameters parameters);
        string GetDestroyListQuery(IDestroyListParameters parameters);

        // members
        string GetAddMemberToListQuery(IAddMemberToListParameters parameters);
        string GetMembersOfListQuery(IGetMembersOfListParameters parameters);





        // old
        string GetUserSubscribedListsQuery(IUserIdentifier user, bool getOwnedListsFirst);

        string GetUsersOwnedListQuery(IUserIdentifier user, int maximumNumberOfListsToRetrieve);

        string GetTweetsFromListQuery(IGetTweetsFromListQueryParameters queryParameters);

        string GetAddMultipleMembersToListQuery(ITwitterListIdentifier listIdentifier, IEnumerable<IUserIdentifier> users);
        string GetRemoveMemberFromListQuery(ITwitterListIdentifier listIdentifier, IUserIdentifier user);
        string GetRemoveMultipleMembersFromListQuery(ITwitterListIdentifier listIdentifier, IEnumerable<IUserIdentifier> users);
        string GetCheckIfUserIsAListMemberQuery(ITwitterListIdentifier listIdentifier, IUserIdentifier user);

        string GetUserSubscribedListsQuery(IUserIdentifier user, int maximumNumberOfListsToRetrieve);

        string GetListSubscribersQuery(ITwitterListIdentifier listIdentifier, int maximumNumberOfSubscribersToRetrieve);
        string GetSubscribeUserToListQuery(ITwitterListIdentifier listIdentifier);
        string GetUnSubscribeUserFromListQuery(ITwitterListIdentifier listIdentifier);
        string GetCheckIfUserIsAListSubscriberQuery(ITwitterListIdentifier listIdentifier, IUserIdentifier user);
        string GetUserListMembershipsQuery(IGetUserListMembershipsQueryParameters parameters);
    }

    public class TwitterListQueryGenerator : ITwitterListQueryGenerator
    {
        private readonly ITwitterListQueryValidator _listsQueryValidator;
        private readonly IUserQueryParameterGenerator _userQueryParameterGenerator;
        private readonly IUserQueryValidator _userQueryValidator;
        private readonly IQueryParameterGenerator _queryParameterGenerator;
        private readonly ITweetinviSettingsAccessor _tweetinviSettingsAccessor;
        private readonly ITwitterListQueryParameterGenerator _twitterListQueryParameterGenerator;

        public TwitterListQueryGenerator(
            ITwitterListQueryValidator listsQueryValidator,
            IUserQueryParameterGenerator userQueryParameterGenerator,
            IUserQueryValidator userQueryValidator,
            IQueryParameterGenerator queryParameterGenerator,
            ITweetinviSettingsAccessor tweetinviSettingsAccessor,
            ITwitterListQueryParameterGenerator twitterListQueryParameterGenerator)
        {
            _listsQueryValidator = listsQueryValidator;
            _userQueryParameterGenerator = userQueryParameterGenerator;
            _userQueryValidator = userQueryValidator;
            _queryParameterGenerator = queryParameterGenerator;
            _tweetinviSettingsAccessor = tweetinviSettingsAccessor;
            _twitterListQueryParameterGenerator = twitterListQueryParameterGenerator;
        }

        // User Lists
        public string GetCreateListQuery(ICreateListParameters parameters)
        {
            var query = new StringBuilder(Resources.List_Create);

            AppendListMetadataToQuery(parameters, query);

            query.AddFormattedParameterToQuery(parameters.FormattedCustomQueryParameters);

            return query.ToString();
        }

        private static void AppendListMetadataToQuery(IListMetadataParameters parameters, StringBuilder query)
        {
            query.AddParameterToQuery("name", parameters.Name);
            query.AddParameterToQuery("mode", parameters.PrivacyMode?.ToString()?.ToLowerInvariant());
            query.AddParameterToQuery("description", parameters.Description);
        }

        public string GetListQuery(IGetListParameters parameters)
        {
            var query = new StringBuilder(Resources.List_Get);

            _twitterListQueryParameterGenerator.AppendListIdentifierParameter(query, parameters.List);
            query.AddFormattedParameterToQuery(parameters.FormattedCustomQueryParameters);

            return query.ToString();
        }

        public string GetUserListsQuery(IGetUserListsParameters parameters)
        {
            var query = new StringBuilder(Resources.List_GetUserLists);

            query.AddFormattedParameterToQuery(_userQueryParameterGenerator.GenerateIdOrScreenNameParameter(parameters.User));
            query.AddParameterToQuery("reverse", parameters.Reverse);

            query.AddFormattedParameterToQuery(parameters.FormattedCustomQueryParameters);

            return query.ToString();
        }

        public string GetUpdateListQuery(IUpdateListParameters parameters)
        {
            var query = new StringBuilder(Resources.List_Update);

            _twitterListQueryParameterGenerator.AppendListIdentifierParameter(query, parameters.List);

            AppendListMetadataToQuery(parameters, query);
            query.AddFormattedParameterToQuery(parameters.FormattedCustomQueryParameters);

            return query.ToString();
        }

        public string GetDestroyListQuery(IDestroyListParameters parameters)
        {
            var query = new StringBuilder(Resources.List_Destroy);

            _twitterListQueryParameterGenerator.AppendListIdentifierParameter(query, parameters.List);
            query.AddFormattedParameterToQuery(parameters.FormattedCustomQueryParameters);

            return query.ToString();
        }

        public string GetAddMemberToListQuery(IAddMemberToListParameters parameters)
        {
            var query = new StringBuilder(Resources.List_Members_Create);

            _twitterListQueryParameterGenerator.AppendListIdentifierParameter(query, parameters.List);
            query.AddFormattedParameterToQuery(_userQueryParameterGenerator.GenerateIdOrScreenNameParameter(parameters.User));

            query.AddFormattedParameterToQuery(parameters.FormattedCustomQueryParameters);

            return query.ToString();
        }

        public string GetMembersOfListQuery(IGetMembersOfListParameters parameters)
        {
            var query = new StringBuilder(Resources.List_Members_List);

            _twitterListQueryParameterGenerator.AppendListIdentifierParameter(query, parameters.List);

            query.AddParameterToQuery("cursor", parameters.Cursor);
            query.AddParameterToQuery("count", parameters.PageSize);

            query.AddFormattedParameterToQuery(parameters.FormattedCustomQueryParameters);

            return query.ToString();
        }

        public string GetUserSubscribedListsQuery(IUserIdentifier user, bool getOwnedListsFirst)
        {
            _userQueryValidator.ThrowIfUserCannotBeIdentified(user);

            var identifierParameter = _userQueryParameterGenerator.GenerateIdOrScreenNameParameter(user);
            return string.Format(Resources.List_GetUserLists, identifierParameter, getOwnedListsFirst);
        }

        public string GetUserListMembershipsQuery(IGetUserListMembershipsQueryParameters parameters)
        {
            _userQueryValidator.ThrowIfUserCannotBeIdentified(parameters.UserIdentifier);

            var userIdentifierParameter = _userQueryParameterGenerator.GenerateIdOrScreenNameParameter(parameters.UserIdentifier);
            var additionalParameters = parameters.Parameters;

            var baseQuery = string.Format(Resources.List_GetUserMemberships, userIdentifierParameter);
            var queryBuilder = new StringBuilder(baseQuery);

            queryBuilder.AddParameterToQuery("count", additionalParameters.PageSize);
            queryBuilder.AddParameterToQuery("filter_to_owned_lists", additionalParameters.FilterToOwnLists);

            return queryBuilder.ToString();
        }

        // Owned Lists
        public string GetUsersOwnedListQuery(IUserIdentifier user, int maximumNumberOfListsToRetrieve)
        {
            _userQueryValidator.ThrowIfUserCannotBeIdentified(user);

            var identifierParameter = _userQueryParameterGenerator.GenerateIdOrScreenNameParameter(user);
            return string.Format(Resources.List_Ownership, identifierParameter, maximumNumberOfListsToRetrieve);
        }

        public string GetTweetsFromListQuery(IGetTweetsFromListQueryParameters getTweetsFromListQueryParameters)
        {
            _listsQueryValidator.ThrowIfGetTweetsFromListQueryParametersIsNotValid(getTweetsFromListQueryParameters);

            var identifier = getTweetsFromListQueryParameters.TwitterListIdentifier;
            var parameters = getTweetsFromListQueryParameters.Parameters;

            StringBuilder queryParameters = new StringBuilder();

            queryParameters.Append(_twitterListQueryParameterGenerator.GenerateIdentifierParameter(identifier));

            if (parameters != null)
            {
                queryParameters.Append(_queryParameterGenerator.GenerateSinceIdParameter(parameters.SinceId));
                queryParameters.Append(_queryParameterGenerator.GenerateMaxIdParameter(parameters.MaxId));
                queryParameters.Append(_queryParameterGenerator.GenerateCountParameter(parameters.MaximumNumberOfTweetsToRetrieve));
                queryParameters.Append(_queryParameterGenerator.GenerateIncludeEntitiesParameter(parameters.IncludeEntities));
                queryParameters.Append(_queryParameterGenerator.GenerateIncludeRetweetsParameter(parameters.IncludeRetweets));
            }
            else
            {
                queryParameters.Append(_queryParameterGenerator.GenerateCountParameter(TweetinviConsts.LIST_GET_TWEETS_COUNT));
            }

            queryParameters.AddFormattedParameterToParametersList(_queryParameterGenerator.GenerateTweetModeParameter(_tweetinviSettingsAccessor.CurrentThreadSettings.TweetMode));

            return string.Format(Resources.List_GetTweetsFromList, queryParameters);
        }

        public string GetAddMultipleMembersToListQuery(ITwitterListIdentifier listIdentifier, IEnumerable<IUserIdentifier> users)
        {
            _listsQueryValidator.ThrowIfListIdentifierIsNotValid(listIdentifier);

            var userIdsAndScreenNameParameter = _userQueryParameterGenerator.GenerateListOfUserIdentifiersParameter(users);
            var query = new StringBuilder(Resources.List_CreateMembers);

            query.AddFormattedParameterToQuery(_twitterListQueryParameterGenerator.GenerateIdentifierParameter(listIdentifier));
            query.AddFormattedParameterToQuery(userIdsAndScreenNameParameter);

            return query.ToString();
        }

        public string GetRemoveMemberFromListQuery(ITwitterListIdentifier listIdentifier, IUserIdentifier user)
        {
            _listsQueryValidator.ThrowIfListIdentifierIsNotValid(listIdentifier);
            _userQueryValidator.ThrowIfUserCannotBeIdentified(user);

            var listIdentifierParameter = _twitterListQueryParameterGenerator.GenerateIdentifierParameter(listIdentifier);
            var userParameter = _userQueryParameterGenerator.GenerateIdOrScreenNameParameter(user);

            return string.Format(Resources.List_DestroyMember, listIdentifierParameter, userParameter);
        }

        public string GetRemoveMultipleMembersFromListQuery(ITwitterListIdentifier listIdentifier, IEnumerable<IUserIdentifier> users)
        {
            _listsQueryValidator.ThrowIfListIdentifierIsNotValid(listIdentifier);

            var listIdentifierParameter = _twitterListQueryParameterGenerator.GenerateIdentifierParameter(listIdentifier);
            var userIdsAndScreenNameParameter = _userQueryParameterGenerator.GenerateListOfUserIdentifiersParameter(users);

            var query = new StringBuilder(Resources.List_DestroyMembers);

            query.AddFormattedParameterToQuery(listIdentifierParameter);
            query.AddFormattedParameterToQuery(userIdsAndScreenNameParameter);

            return query.ToString();
        }

        public string GetCheckIfUserIsAListMemberQuery(ITwitterListIdentifier listIdentifier, IUserIdentifier user)
        {
            _listsQueryValidator.ThrowIfListIdentifierIsNotValid(listIdentifier);
            _userQueryValidator.ThrowIfUserCannotBeIdentified(user);

            var listIdentifierParameter = _twitterListQueryParameterGenerator.GenerateIdentifierParameter(listIdentifier);
            var userParameter = _userQueryParameterGenerator.GenerateIdOrScreenNameParameter(user);

            return string.Format(Resources.List_CheckMembership, listIdentifierParameter, userParameter);
        }

        // Subscriptions
        public string GetUserSubscribedListsQuery(IUserIdentifier user, int maximumNumberOfListsToRetrieve)
        {
            _userQueryValidator.ThrowIfUserCannotBeIdentified(user);

            var userIdParameter = _userQueryParameterGenerator.GenerateIdOrScreenNameParameter(user);
            return string.Format(Resources.List_UserSubscriptions, userIdParameter, maximumNumberOfListsToRetrieve);
        }

        public string GetListSubscribersQuery(ITwitterListIdentifier listIdentifier, int maximumNumberOfSubscribersToRetrieve)
        {
            _listsQueryValidator.ThrowIfListIdentifierIsNotValid(listIdentifier);

            var identifierParameter = _twitterListQueryParameterGenerator.GenerateIdentifierParameter(listIdentifier);
            return string.Format(Resources.List_GetSubscribers, identifierParameter, maximumNumberOfSubscribersToRetrieve);
        }

        public string GetSubscribeUserToListQuery(ITwitterListIdentifier listIdentifier)
        {
            _listsQueryValidator.ThrowIfListIdentifierIsNotValid(listIdentifier);

            var listIdentifierParameter = _twitterListQueryParameterGenerator.GenerateIdentifierParameter(listIdentifier);
            return string.Format(Resources.List_Subscribe, listIdentifierParameter);
        }

        public string GetUnSubscribeUserFromListQuery(ITwitterListIdentifier listIdentifier)
        {
            _listsQueryValidator.ThrowIfListIdentifierIsNotValid(listIdentifier);

            var listIdentifierParameter = _twitterListQueryParameterGenerator.GenerateIdentifierParameter(listIdentifier);
            return string.Format(Resources.List_UnSubscribe, listIdentifierParameter);
        }

        public string GetCheckIfUserIsAListSubscriberQuery(ITwitterListIdentifier listIdentifier, IUserIdentifier user)
        {
            _listsQueryValidator.ThrowIfListIdentifierIsNotValid(listIdentifier);
            _userQueryValidator.ThrowIfUserCannotBeIdentified(user);

            var listIdentifierParameter = _twitterListQueryParameterGenerator.GenerateIdentifierParameter(listIdentifier);
            var userParameter = _userQueryParameterGenerator.GenerateIdOrScreenNameParameter(user);

            var query = new StringBuilder(Resources.List_CheckSubscriber);

            query.AddFormattedParameterToQuery(listIdentifierParameter);
            query.AddFormattedParameterToQuery(userParameter);
            query.AddParameterToQuery("skip_status", "true");

            return query.ToString();
        }
    }
}