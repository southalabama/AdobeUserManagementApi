# AdobeUserManagementApi

This project is a C#/.NET 5.0 wrapper for the Adobe User Management REST Api (UMAPI).

API Reference: https://adobe-apiplatform.github.io/umapi-documentation/en/RefOverview.html

Adobe developer console: https://console.adobe.io/home


Adobe admin console: https://adminconsole.adobe.com

## Examples

### Authenticate to Adobe's API

```
// There are optional parameters with sensible default values - metascopes and several URIs. You can change these if necessary for advanced scenarios.
var userManagementApi = new UserManagementApi("yourClientId", "yourClientSecret", "yourTechAcctId", "yourOrgId");
// You will need to supply the X509 cert in a way that makes sense for your use case. 
X509Cert cert = null; 
Dictionary<object, object> claims = userManagementApi.GetAuthClaims();
userManagementApi.Authenticate(cert, claims);
```

### List members of the organization

```
List<AdobeUser> users = userManagementApi.GetOrgUsers();
```

### List members of a group

```
List<AdobeUser> users = userManagementApi.GetGroupMembers("Your group name");
```

### Create a group
```
ApiResponse response = userManagementApi.CreateUserGroup("Your group", "Your Group Description", "ignoreIfAlreadyExists");
```

### Update a group
```
ApiResponse response = userManagementApi.UpdateUserGroup(originalGroupName: "Your group", newGroupName: "Your New Group Name", description: "Your new description");
```

### Delete a group
```
ApiResponse response = userManagementApi.DeleteUserGroup("Your group");
```

### Add a User to a Group

```
ApiResponse response = userManagementApi.AddUserToGroup("Your group", "user@example.com");
```

### Add multiple Users to a Group

```
string[] users = { "user1@example.com", "user2@example.com" };
ApiResponse response = userManagementApi.AddUsersToGroup("Your group", users);
```

### Remove a User from a Group
```
ApiResponse response = userManagementApi.RemoveUserFromGroup("Your group", "user@example.com");
```

### Remove multiple Users from a Group
```
string[] users = { "user1@example.com", "user2@example.com" };
ApiResponse response = userManagementApi.RemoveUsersFromGroup("Your group", users);
```

### Get a specific user
```
AdobeUser user = userManagementApi.GetUser("test@example.com");
```

### Create a User
```
ApiResponse response = userManagementApi.CreateUser("user@example.com", "John", "Doe");
```

### Update a user
```
ApiResponse response = userManagementApi.UpdateUser(originalUserName: "user@example.com", newUserName: "user1@example.com", firstName: "NewJohn", lastName: "NewDoe");
```

### Delete a User
```
// hardDelete: false will only remove them from Users Menu
// hardDelete: true will remove them from Users Menu and Directory Users Menu
ApiResponse response = userManagementApi.DeleteUser("user@example.com", hardDelete: false);
```


## Async/await support
Each synchronous method has a matching async method.
```
// sync
List<AdobeUser> users = new();
users = userManagementApi.GetGroupMembers("Your group name");
// async
users = await userManagementApi.GetGroupMembersAsync("Your group name");
```

## Common issues

An X509 certificate is required for authentication. Your certificate info will come from Adobe when you set up your API access within it. X509 certificate storage flags are tricky to work with and will change depending on your target environment.

For Console applications, you will want to use ```X509KeyStorageFlags.Exportable``` and in Web Applications ```X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable```

## Advanced Scenarios and bulk operations

The specific methods of the UserManagementApi class may not meet your needs. For advanced scenarios, you can use the following two methods.

```
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

```
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