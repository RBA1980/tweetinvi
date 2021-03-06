# Followers, Friends and Relationships

From a user account perspective another user can either be a follower or a person that you you follow also described as friend.

## Add Friend

``` c#
// follow a user
await client.Users.FollowUserAsync("tweetinviapi");

// stop following a user
await client.Users.UnfollowUserAsync(42);
```

## List Friends

> List the users followed by a specific user.

<div class="iterator-available">

``` c#
var friendIds = await client.Users.GetFriendIdsAsync("tweetinviapi");
// or
var friendIdsIterator = client.Users.GetFriendIdsIterator(new GetFriendIdsParameters("tweetinviapi"));
```
</div>

Tweetinvi can fetch populated user objects by performing a set of additional requests. This iterator can retrieve up to 100 users per page.

<div class="iterator-available">

``` c#
var friends = await client.Users.GetFriendsAsync("tweetinviapi");
// or
var friendsIterator = client.Users.GetFriendsIterator(new GetFriendsParameters("tweetinviapi"));
```
</div>

## List Followers

<div class="iterator-available">

``` c#
var followerIds = await client.Users.GetFollowerIdsAsync("tweetinviapi");
// or
var followerIdsIterator = client.Users.GetFollowerIdsIterator(new GetFollowerIdsParameters("tweetinviapi"));
```
</div>

Tweetinvi can fetch populated user objects by performing a set of additional requests. This iterator can retrieve up to 100 users per page.

<div class="iterator-available">

``` c#
var followers = await client.Users.GetFollowersAsync("tweetinviapi");
// or
var followersIterator = client.Users.GetFollowersIterator(new GetFollowersParameters("tweetinviapi"));
```
</div>

## Outgoing Follower Requests

> As an authenticated user following a private user, an outgoing follower request is sent to this private user. 

<div class="iterator-available">

``` c#
var userIdsYouRequestedToFollow = await client.Users.GetUserIdsYouRequestedToFollowAsync();
// or
var userIdsYouRequestedToFollowIterator = client.Users.GetUserIdsYouRequestedToFollowIterator();
```
</div>

Tweetinvi can fetch populated user objects by performing a set of additional requests. This iterator can retrieve up to 100 users per page.

<div class="iterator-available">

``` c#
var usersYouRequestedToFollow = await client.Users.GetUsersYouRequestedToFollowAsync();
// or
var usersYouRequestedToFollowIterator = client.Users.GetUsersYouRequestedToFollowIterator();
```
</div>

## Incoming Follower Requests

> As a private user, any other user attempting to follow you will create an incoming request.

<div class="iterator-available">

``` c#
var userIdsRequestingFriendship = await client.Users.GetUserIdsRequestingFriendshipAsync();
// or
var userIdsRequestingFriendshipIterator = client.Users.GetUserIdsRequestingFriendshipIterator();
```
</div>

Tweetinvi can fetch populated user objects by performing a set of additional requests. This iterator can retrieve up to 100 users per page.

<div class="iterator-available">

``` c#
var userRequestingFriendship = await client.Users.GetUsersRequestingFriendshipAsync();
// or
var usersRequestingFriendshipIterator = client.Users.GetUsersRequestingFriendshipIterator();
```
</div>

## Relationships

> Details of relationships between 2 users.

``` c#
var relationship = await client.Users.GetRelationshipBetweenAsync("source_user", "target_user");
// You can for example check if you
var canSourceSendDirectMessagesToTarget = relationship.CanSendDirectMessage;
```

List the relationships between the client's authenticated user and a list of users.

``` c#
var relationships = await client.Users.GetRelationshipsWithAsync(new [] { "target_user" });
var userSentAFollowingRequest = relationships[0].FollowingRequested;
```

Updated relationship state between the authenticated user and a target user.

``` c#
await client.Users.UpdateRelationshipAsync(new UpdateRelationshipParameters("target_user")
{
    EnableRetweets = true,
    EnableDeviceNotifications = true
});
```