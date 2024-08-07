# AdobeUserManagementApi

This project is a C#/.NET 8 wrapper for the Adobe User Management REST Api (UMAPI).


API Reference: https://adobe-apiplatform.github.io/umapi-documentation/en/RefOverview.html


Adobe developer console: https://console.adobe.io/home


Adobe admin console: https://adminconsole.adobe.com

## Installation

dotnet CLI:  ```dotnet add package AdobeUserManagementApi```

NuGet Gallery: https://www.nuget.org/packages/AdobeUserManagementApi

## Examples

### Authenticate to Adobe's API

```csharp
var adobe = new UserManagementApi(CLIENT_ID, CLIENT_SECRET, TECH_ACC_ID, ORG_ID);
adobe.Authenticate();
```

### List members of the organization

```csharp
List<AdobeUser> users = userManagementApi.GetOrgUsers();
```

### List members of a group

```csharp
List<AdobeUser> users = userManagementApi.GetGroupMembers("Your group name");
```

### Get all groups
```csharp
List<AdobeGroup> groups = userManagementApi.GetGroups();
```

### Create a group
```csharp
AdobeApiResponse response = userManagementApi.CreateUserGroup("Your group", "Your Group Description", "ignoreIfAlreadyExists");
```

### Update a group
```csharp
AdobeApiResponse response = userManagementApi.UpdateUserGroup(originalGroupName: "Your group", newGroupName: "Your New Group Name", description: "Your new description");
```

### Delete a group
```csharp
AdobeApiResponse response = userManagementApi.DeleteUserGroup("Your group");
```

### Add a User to a Group

```csharp
AdobeApiResponse response = userManagementApi.AddUserToGroup("Your group", "user@example.com");
```

### Add multiple Users to a Group

```csharp
string[] users = { "user1@example.com", "user2@example.com" };
AdobeApiResponse response = userManagementApi.AddUsersToGroup("Your group", users);
```

### Remove a User from a Group
```csharp
AdobeApiResponse response = userManagementApi.RemoveUserFromGroup("Your group", "user@example.com");
```

### Remove multiple Users from a Group
```csharp
string[] users = { "user1@example.com", "user2@example.com" };
AdobeApiResponse response = userManagementApi.RemoveUsersFromGroup("Your group", users);
```

### Get a specific user
```csharp
AdobeUser user = userManagementApi.GetUser("test@example.com");
```

### Create a User
```csharp
AdobeApiResponse response = userManagementApi.CreateUser("user@example.com", "John", "Doe");
```

### Update a user
```csharp
AdobeApiResponse response = userManagementApi.UpdateUser(originalUserName: "user@example.com", newUserName: "user1@example.com", firstName: "NewJohn", lastName: "NewDoe");
```

### Delete a User
```csharp
// hardDelete: false will only remove them from Users Menu
// hardDelete: true will remove them from Users Menu and Directory Users Menu
AdobeApiResponse response = userManagementApi.DeleteUser("user@example.com", hardDelete: false);
```


## Async/await support
Each synchronous method has a matching async method.
```csharp
// sync
List<AdobeUser> users = new();
users = userManagementApi.GetGroupMembers("Your group name");
// async
users = await userManagementApi.GetGroupMembersAsync("Your group name");
```

## Advanced Scenarios and bulk operations

The specific methods of the UserManagementApi class may not meet your needs. For advanced scenarios, you can use the following two methods.

```csharp
// An example of how to create a new user manually.
UserAction[] userActions = new UserAction[]
{
    new UserAction
    {
        User = userName,
        _do = new Do[]
        {
            new  Do
            {
                CreateEnterpriseID = new CreateEnterpriseID
                {
                    Country = "US",
                    Email = "test@example.com",
                    FirstName = "John",
                    LastName = "Doe",
                    Option = "ignoreIfAlreadyExists"
                }
            }
        }
    }
};
var response = userManagementApi.PerformUserActions(userActions);
```

```csharp
// An example of how to add users to a group manually.
string[] users = 
{ 
    "user1@example.com", 
    "user2@example.com" 
};
GroupAction[] groupActions = new GroupAction[]
{
    new GroupAction
    {
        UserGroup = group,
        _do = new Do[]
        {
            new Do
            {
                Add = new Add
                {
                    User = users
                }
            }
        }
    }
};
var response = userManagementApi.PerformGroupActions(actions);
```