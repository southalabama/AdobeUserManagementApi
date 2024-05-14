using System;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Jose;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace AdobeUserManagementApi
{
    // https://www.adobe.io/authentication/auth-methods.html#!AdobeDocs/adobeio-auth/master/JWT/JWT.md
    // https://github.com/AdobeDocs/adobeio-auth/tree/stage/JWT/samples/adobe-jwt-dotnet
    public class UserManagementApi(
        string clientId,
        string clientSecret,
        string techAcctId,
        string orgId,
        string metascopes = "https://ims-na1.adobelogin.com/s/ent_user_sdk",
        string apiBaseUri = "https://usermanagement.adobe.io/v2/usermanagement",
        string apiAuthUri = "https://ims-na1.adobelogin.com/ims/exchange/jwt/",
        string apiAudienceUri = "https://ims-na1.adobelogin.com/c/")
    {
        // These fields can be found here https://console.adobe.io/projects/
        private readonly string _clientId = clientId, 
                                _clientSecret = clientSecret, 
                                _techAcctId = techAcctId, 
                                _orgId = orgId, 
                                _metascopes = metascopes, 
                                _apiBaseUri = apiBaseUri, 
                                _apiAuthUri = apiAuthUri, 
                                _apiAudienceUri = apiAudienceUri;

        private string API_ACTION_URI => $"action/{_orgId}";
        private string API_USER_URI => $"users/{_orgId}";
        private string API_GROUP_URI => $"groups/{_orgId}";
        private string API_AUD_URI => $"{_apiAudienceUri}{_clientId}";
        private static JsonSerializerSettings JsonSettings => new() { NullValueHandling = NullValueHandling.Ignore };
        private string _authToken;
        private readonly RestClient _restClient = new RestClient(apiBaseUri, configureSerialization: s => s.UseNewtonsoftJson(JsonSettings));

        #region Synchronous Methods
        // https://restsharp.dev/usage/
        public T Execute<T>(RestRequest request) where T : new()
        {
            // These headers are used for every request
            request.AddHeader("Authorization", $"Bearer {_authToken}");
            request.AddHeader("X-Api-Key", _clientId);
            request.AddHeader("content-type", "application/json");
            var response = _restClient.Execute<T>(request);
            if (response.ErrorException != null)
            {
                throw response.ErrorException;
            }
            return response.Data;
        }

        /// <summary>
        /// The response contains an access token that is valid for 24 hours after it is issued.
        /// Pass this token in the Authorization header in all subsequent requests to the User Management API.
        /// </summary>
        /// <returns></returns>
        public string Authenticate(X509Certificate2 certificate, Dictionary<object, object> claims)
        {
            string token = Jose.JWT.Encode(claims, certificate.GetRSAPrivateKey(), JwsAlgorithm.RS256);

            RestClient client = new();

            RestRequest request = new(_apiAuthUri, Method.Post);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "multipart/form-data; boundary=----boundary");
            request.AddParameter("multipart/form-data",
                    "------boundary\r\nContent-Disposition: form-data; name=\"client_id\"\r\n\r\n" + _clientId +
                "\r\n------boundary\r\nContent-Disposition: form-data; name=\"client_secret\"\r\n\r\n" + _clientSecret +
                "\r\n------boundary\r\nContent-Disposition: form-data; name=\"jwt_token\"\r\n\r\n" + token +
                "\r\n------boundary--", ParameterType.RequestBody);
            RestResponse response = client.Execute(request);
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);
            _authToken = dict["access_token"];
            return _authToken;
        }

        /// <summary>
        /// General method for more advanced scenarios.
        /// </summary>
        /// <param name="actions"></param>
        /// <returns></returns>
        public AdobeApiResponse PerformUserActions(UserAction[] actions)
        {
            RestRequest request = new(API_ACTION_URI, Method.Post);
            request.AddJsonBody(actions);
            return Execute<AdobeApiResponse>(request);
        }

        /// <summary>
        /// General method for more advanced scenarios.
        /// </summary>
        /// <param name="actions"></param>
        /// <returns></returns>
        public AdobeApiResponse PerformGroupActions(GroupAction[] actions)
        {
            RestRequest request = new(API_ACTION_URI, Method.Post);
            request.AddJsonBody(actions);
            return Execute<AdobeApiResponse>(request);
        }

        /// <summary>
        /// Adds multiple users to the group. User value should be the email address of the user.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public AdobeApiResponse AddUsersToGroup(string group, string[] users)
        {
            if (string.IsNullOrWhiteSpace(group))
            {
                throw new ArgumentException("group is required.", nameof(group));
            }
            if (users == null || users != null && users.Length == 0)
            {
                throw new ArgumentException("One or more users are required.", nameof(users));
            }
            RestRequest request = new(API_ACTION_URI, Method.Post);
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
            request.AddJsonBody(groupActions);
            return Execute<AdobeApiResponse>(request);
        }

        /// <summary>
        /// Adds multiple users to the group. User value should be the email address of the user.
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="group"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public AdobeApiResponse AddUserToGroup(string group, string user)
        {
            if (string.IsNullOrWhiteSpace(group))
            {
                throw new ArgumentException("group is required.", nameof(group));
            }
            if (string.IsNullOrWhiteSpace(user))
            {
                throw new ArgumentException("user is required.", nameof(user));
            }
            RestRequest request = new(API_ACTION_URI, Method.Post);
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
                                User = new string[] { user }
                            }
                        }
                    }
                }
            };
            request.AddJsonBody(groupActions);
            return Execute<AdobeApiResponse>(request);
        }

        /// <summary>
        /// Removes user from the group. User value should be the email address of the user.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public AdobeApiResponse RemoveUserFromGroup(string group, string userName)
        {
            if (string.IsNullOrWhiteSpace(group))
            {
                throw new ArgumentException("group is required.", nameof(group));
            }
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("userName is required.", nameof(userName));
            }
            RestRequest request = new(API_ACTION_URI, Method.Post);
            GroupAction[] groupActions = new GroupAction[]
            {
                new GroupAction
                {
                    UserGroup = group,
                    _do = new Do[]
                    {
                        new Do
                        {
                            Remove = new Remove
                            {
                                User = new string[] { userName }
                            }
                        }
                    }
                }
            };
            request.AddJsonBody(groupActions);
            return Execute<AdobeApiResponse>(request);
        }

        /// <summary>
        /// Removes multiple users from the group. User value should be the email address of the user.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public AdobeApiResponse RemoveUsersFromGroup(string group, string[] users)
        {
            if (string.IsNullOrWhiteSpace(group))
            {
                throw new ArgumentException("group is required.", nameof(group));
            }
            RestRequest request = new(API_ACTION_URI, Method.Post);
            GroupAction[] groupActions = new GroupAction[]
            {
                new GroupAction
                {
                    UserGroup = group,
                    _do = new Do[]
                    {
                        new Do
                        {
                            Remove = new Remove
                            {
                                User = users
                            }
                        }

                    }
                }
            };
            request.AddJsonBody(groupActions);
            return Execute<AdobeApiResponse>(request);
        }

        /// <summary>
        /// Creates a new user. By default, does nothing if the user already exists.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="country"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public AdobeApiResponse CreateUser(string userName, string firstName, string lastName, string country = "US", string option = "ignoreIfAlreadyExists")
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("userName is required.", nameof(userName));
            }
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException("firstName is required.", nameof(firstName));
            }
            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException("lastName is required.", nameof(lastName));
            }
            if (string.IsNullOrWhiteSpace(country))
            {
                throw new ArgumentException("country is required.", nameof(country));
            }
            if (string.IsNullOrWhiteSpace(option))
            {
                throw new ArgumentException("option is required.", nameof(option));
            }
            RestRequest request = new(API_ACTION_URI, Method.Post);
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
                                Country = country,
                                Email = userName,
                                FirstName = firstName,
                                LastName = lastName,
                                Option = option
                            }
                        }
                    }
                }
            };
            request.AddJsonBody(userActions);
            return Execute<AdobeApiResponse>(request);
        }

        /// <summary>
        /// Update an Enterprise or Federated ID user.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns></returns>
        public AdobeApiResponse UpdateUser(string originalUserName, string newUserName, string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(originalUserName))
            {
                throw new ArgumentException("originalUserName is required.", nameof(originalUserName));
            }
            if (string.IsNullOrWhiteSpace(newUserName))
            {
                throw new ArgumentException("newUserName is required.", nameof(newUserName));
            }
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException("firstName is required.", nameof(firstName));
            }
            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException("lastName is required.", nameof(lastName));
            }
            RestRequest request = new(API_ACTION_URI, Method.Post);
            UserAction[] userActions = new UserAction[]
            {
                new UserAction
                {
                    User = originalUserName,
                    _do = new Do[]
                    {
                        new Do
                        {
                            Update = new Update
                            {
                                Email = newUserName,
                                FirstName = firstName,
                                LastName = lastName,
                                UserName = newUserName
                            }
                        }
                    }
                }
            };
            request.AddJsonBody(userActions);
            return Execute<AdobeApiResponse>(request);
        }

        /// <summary>
        /// Gets user by email or username + domain.
        /// </summary>
        /// <param name="userString"></param>
        /// <returns></returns>
        public UserResponse GetUser(string userString)
        {
            RestRequest request = new($"organizations/{_orgId}/users/{userString}", Method.Get);
            return Execute<UserResponse>(request);
        }

        /// <summary>
        /// Gets users in the organization
        /// </summary>
        /// <returns></returns>
        public List<AdobeUser> GetOrgUsers()
        {
            // adobe api paging is zero-indexed
            List<AdobeUser> adobeUsers = new();
            GroupUsersResponse GroupUsersResponse = new();
            RestRequest request = new($"{API_USER_URI}/0", Method.Get);
            GroupUsersResponse = Execute<GroupUsersResponse>(request);
            adobeUsers.AddRange(GroupUsersResponse.Users);
            return adobeUsers;
        }

        /// <summary>
        /// Get members of a group
        /// https://adobe-apiplatform.github.io/umapi-documentation/en/api/getUsersByGroup.html
        /// </summary>
        /// <returns></returns>
        public List<AdobeUser> GetGroupMembers(string group)
        {
            if (string.IsNullOrWhiteSpace(group))
            {
                throw new ArgumentException("group is required.", nameof(group));
            }
            List<AdobeUser> adobeUsers = new();
            GroupUsersResponse GroupUsersResponse = new();
            RestRequest request = new($"{API_USER_URI}/0/{group}", Method.Get);
            GroupUsersResponse = Execute<GroupUsersResponse>(request);
            adobeUsers.AddRange(GroupUsersResponse.Users);
            return adobeUsers;
        }

        /// <summary>
        /// List all groups owned by the org.
        /// </summary>
        /// <returns></returns>
        public List<AdobeGroup> GetGroups()
        {
            List<AdobeGroup> adobeGroups = new();
            GroupResponse groupResponse = new();
            RestRequest request = new($"{API_GROUP_URI}/0", Method.Get);
            groupResponse = Execute<GroupResponse>(request);
            adobeGroups.AddRange(groupResponse.Groups);
            return adobeGroups;
        }

        /// <summary>
        /// Creates a user group
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="description"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public AdobeApiResponse CreateUserGroup(string groupName, string description, string option)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                throw new ArgumentException("groupName is required.", nameof(groupName));
            }
            RestRequest request = new(API_ACTION_URI, Method.Post);
            GroupAction[] groupActions = new GroupAction[]
            {
                new GroupAction
                {
                    UserGroup = groupName,
                    _do = new Do[]
                    {
                        new Do
                        {
                            CreateUserGroup = new CreateUserGroup
                            {
                                Name = groupName,
                                Description = description,
                                Option = option
                            }
                        }
                    }
                }
            };
            request.AddJsonBody(groupActions);
            return Execute<AdobeApiResponse>(request);
        }

        /// <summary>
        /// Updates a user group
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="description"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public AdobeApiResponse UpdateUserGroup(string originalGroupName, string newGroupName, string description)
        {
            if (string.IsNullOrWhiteSpace(originalGroupName))
            {
                throw new ArgumentException("originalGroupName is required.", nameof(originalGroupName));
            }
            if (string.IsNullOrWhiteSpace(newGroupName))
            {
                throw new ArgumentException("newGroupName is required.", nameof(newGroupName));
            }
            RestRequest request = new(API_ACTION_URI, Method.Post);
            GroupAction[] groupActions = new GroupAction[]
            {
                new GroupAction
                {
                    UserGroup = originalGroupName,
                    _do = new Do[]
                    {
                        new Do
                        {
                            UpdateUserGroup = new UpdateUserGroup
                            {
                                Name = newGroupName,
                                Description = description,
                            }
                        }
                    }
                }
            };
            request.AddJsonBody(groupActions);
            return Execute<AdobeApiResponse>(request);
        }

        /// <summary>
        /// Delete a user group
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="description"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public AdobeApiResponse DeleteUserGroup(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                throw new ArgumentException("groupName is required.", nameof(groupName));
            }
            RestRequest request = new(API_ACTION_URI, Method.Post);
            GroupAction[] groupActions = new GroupAction[]
            {
                new GroupAction
                {
                    UserGroup = groupName,
                    _do = new Do[]
                    {
                        new Do
                        {
                            DeleteUserGroup = new()
                        }
                    }
                }
            };
            request.AddJsonBody(groupActions);
            return Execute<AdobeApiResponse>(request);
        }

        /// <summary>
        /// Deletes a user. If hardDelete is false, it removes them from Users Menu. If hardDelete is true it removes them from Directory Users Menu.
        /// hardDelete = true implies loss of account metadata and associated cloud assets.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="hardDelete"></param>
        /// <returns></returns>
        public AdobeApiResponse DeleteUser(string userName, bool hardDelete)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("userName is required.", nameof(userName));
            }
            RestRequest request = new(API_ACTION_URI, Method.Post);
            UserAction[] userActions = new UserAction[]
            {
                new UserAction
                {
                    User = userName,
                    _do = new Do[]
                    {
                        new Do
                        {
                            RemoveFromOrg = new RemoveFromOrg
                            {
                                DeleteAccount = hardDelete
                            }
                        }
                    }
                }
            };
            request.AddJsonBody(userActions);
            return Execute<AdobeApiResponse>(request);
        }


        #endregion

        #region Async Methods

        public async Task<T> ExecuteAsync<T>(RestRequest request) where T : new()
        {
            // These headers are used for every request
            request.AddHeader("Authorization", $"Bearer {_authToken}");
            request.AddHeader("X-Api-Key", _clientId);
            request.AddHeader("content-type", "application/json");
            var response = await _restClient.ExecuteAsync<T>(request);

            if (response.ErrorException != null)
            {
                throw response.ErrorException;
            }
            return response.Data;
        }

        /// <summary>
        /// The response contains an access token that is valid for 24 hours after it is issued.
        /// Pass this token in the Authorization header in all subsequent requests to the User Management API.
        /// </summary>
        /// <returns></returns>
        public async Task<string> AuthenticateAsync(X509Certificate2 certificate, Dictionary<object, object> claims)
        {
            string token = Jose.JWT.Encode(claims, certificate.GetRSAPrivateKey(), JwsAlgorithm.RS256);

            RestClient client = new();

            RestRequest request = new(_apiAuthUri, Method.Post);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "multipart/form-data; boundary=----boundary");
            request.AddParameter("multipart/form-data",
                    "------boundary\r\nContent-Disposition: form-data; name=\"client_id\"\r\n\r\n" + _clientId +
                "\r\n------boundary\r\nContent-Disposition: form-data; name=\"client_secret\"\r\n\r\n" + _clientSecret +
                "\r\n------boundary\r\nContent-Disposition: form-data; name=\"jwt_token\"\r\n\r\n" + token +
                "\r\n------boundary--", ParameterType.RequestBody);
            RestResponse response = await client.ExecuteAsync(request);
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);
            _authToken = dict["access_token"];
            return _authToken;
        }

        /// <summary>
        /// General method for more advanced scenarios.
        /// </summary>
        /// <param name="actions"></param>
        /// <returns></returns>
        public async Task<AdobeApiResponse> PerformUserActionsAsync(UserAction[] actions)
        {
            RestRequest request = new(API_ACTION_URI, Method.Post);
            request.AddJsonBody(actions);
            return await ExecuteAsync<AdobeApiResponse>(request);
        }

        /// <summary>
        /// General method for more advanced scenarios.
        /// </summary>
        /// <param name="actions"></param>
        /// <returns></returns>
        public async Task<AdobeApiResponse> PerformGroupActionsAsync(GroupAction[] actions)
        {
            RestRequest request = new(API_ACTION_URI, Method.Post);
            request.AddJsonBody(actions);
            return await ExecuteAsync<AdobeApiResponse>(request);
        }


        /// <summary>
        /// Adds multiple users to the group. User value should be the email address of the user.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<AdobeApiResponse> AddUsersToGroupAsync(string group, string[] users)
        {
            if (string.IsNullOrWhiteSpace(group))
            {
                throw new ArgumentException("group is required.", nameof(group));
            }
            if (users == null || users != null && users.Length == 0)
            {
                throw new ArgumentException("One or more users are required.", nameof(users));
            }
            RestRequest request = new(API_ACTION_URI, Method.Post);
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
            request.AddJsonBody(groupActions);
            return await ExecuteAsync<AdobeApiResponse>(request);
        }

        /// <summary>
        /// Adds multiple users to the group. User value should be the email address of the user.
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="group"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<AdobeApiResponse> AddUserToGroupAsync(string group, string user)
        {
            if (string.IsNullOrWhiteSpace(group))
            {
                throw new ArgumentException("group is required.", nameof(group));
            }
            if (string.IsNullOrWhiteSpace(user))
            {
                throw new ArgumentException("user is required.", nameof(user));
            }
            RestRequest request = new(API_ACTION_URI, Method.Post);
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
                                User = new string[] { user }
                            }
                        }
                    }
                }
            };
            request.AddJsonBody(groupActions);
            return await ExecuteAsync<AdobeApiResponse>(request);
        }

        /// <summary>
        /// Removes user from the group. User value should be the email address of the user.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<AdobeApiResponse> RemoveUserFromGroupAsync(string group, string userName)
        {
            if (string.IsNullOrWhiteSpace(group))
            {
                throw new ArgumentException("group is required.", nameof(group));
            }
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("userName is required.", nameof(userName));
            }
            RestRequest request = new(API_ACTION_URI, Method.Post);
            GroupAction[] groupActions = new GroupAction[]
            {
                new GroupAction
                {
                    UserGroup = group,
                    _do = new Do[]
                    {
                        new Do
                        {
                            Remove = new Remove
                            {
                                User = new string[] { userName }
                            }
                        }
                    }
                }
            };

            request.AddJsonBody(groupActions);
            return await ExecuteAsync<AdobeApiResponse>(request);
        }

        /// <summary>
        /// Removes multiple users from the group. User value should be the email address of the user.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<AdobeApiResponse> RemoveUsersFromGroupAsync(string group, string[] users)
        {
            if (string.IsNullOrWhiteSpace(group))
            {
                throw new ArgumentException("group is required.", nameof(group));
            }
            RestRequest request = new(API_ACTION_URI, Method.Post);
            GroupAction[] groupActions = new GroupAction[]
            {
                new GroupAction
                {
                    UserGroup = group,
                    _do = new Do[]
                    {
                        new Do
                        {
                            Remove = new Remove
                            {
                                User = users
                            }
                        }
                    }
                }
            };
            request.AddJsonBody(groupActions);
            return await ExecuteAsync<AdobeApiResponse>(request);
        }

        /// <summary>
        /// Creates a new user. By default, does nothing if the user already exists.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="country"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public async Task<AdobeApiResponse> CreateUserAsync(string userName, string firstName, string lastName, string country = "US", string option = "ignoreIfAlreadyExists")
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("userName is required.", nameof(userName));
            }
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException("firstName is required.", nameof(firstName));
            }
            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException("lastName is required.", nameof(lastName));
            }
            if (string.IsNullOrWhiteSpace(country))
            {
                throw new ArgumentException("country is required.", nameof(country));
            }
            if (string.IsNullOrWhiteSpace(option))
            {
                throw new ArgumentException("option is required.", nameof(option));
            }
            RestRequest request = new(API_ACTION_URI, Method.Post);
            UserAction[] userActions = new UserAction[]
            {
                new UserAction
                {
                    User = userName,
                    _do = new Do[]
                    {
                        new Do
                        {
                            CreateEnterpriseID = new CreateEnterpriseID
                            {
                                Country = country,
                                Email = userName,
                                FirstName = firstName,
                                LastName = lastName,
                                Option = option
                            }
                        }
                    }
                }
            };
            request.AddJsonBody(userActions);
            return await ExecuteAsync<AdobeApiResponse>(request);
        }

        /// <summary>
        /// Update an Enterprise or Federated ID user.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns></returns>
        public async Task<AdobeApiResponse> UpdateUserAsync(string originalUserName, string newUserName, string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(originalUserName))
            {
                throw new ArgumentException("originalUserName is required.", nameof(originalUserName));
            }
            if (string.IsNullOrWhiteSpace(newUserName))
            {
                throw new ArgumentException("newUserName is required.", nameof(newUserName));
            }
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException("firstName is required.", nameof(firstName));
            }
            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException("lastName is required.", nameof(lastName));
            }
            RestRequest request = new(API_ACTION_URI, Method.Post);
            UserAction[] userActions = new UserAction[]
            {
                new UserAction
                {
                    User = originalUserName,
                    _do = new Do[]
                    {
                        new Do
                        {
                            Update = new Update
                            {
                                Email = newUserName,
                                FirstName = firstName,
                                LastName = lastName,
                                UserName = newUserName
                            }
                        }
                    }
                }
            };
            request.AddJsonBody(userActions);
            return await ExecuteAsync<AdobeApiResponse>(request);
        }

        /// <summary>
        /// Gets user by email or username + domain.
        /// </summary>
        /// <param name="userString"></param>
        /// <returns></returns>
        public async Task<UserResponse> GetUserAsync(string userString)
        {
            RestRequest request = new($"organizations/{_orgId}/users/{userString}", Method.Get);
            return await ExecuteAsync<UserResponse>(request);
        }

        /// <summary>
        /// Gets users in the organization
        /// </summary>
        /// <returns></returns>
        public async Task<List<AdobeUser>> GetOrgUsersAsync()
        {
            List<AdobeUser> adobeUsers = new();
            GroupUsersResponse GroupUsersResponse = new();
                RestRequest request = new($"{API_USER_URI}/0", Method.Get);
                GroupUsersResponse = await ExecuteAsync<GroupUsersResponse>(request);
                adobeUsers.AddRange(GroupUsersResponse.Users);
            return adobeUsers;
        }

        /// <summary>
        /// Get members of a group
        /// https://adobe-apiplatform.github.io/umapi-documentation/en/api/getUsersByGroup.html
        /// </summary>
        /// <returns></returns>
        public async Task<List<AdobeUser>> GetGroupMembersAsync(string group)
        {
            if (string.IsNullOrWhiteSpace(group))
            {
                throw new ArgumentException("group is required.", nameof(group));
            }
            List<AdobeUser> adobeUsers = new List<AdobeUser>();
            GroupUsersResponse GroupUsersResponse = new GroupUsersResponse();
            var request = new RestRequest($"{API_USER_URI}/0/{group}", Method.Get);
            GroupUsersResponse = await ExecuteAsync<GroupUsersResponse>(request);
            adobeUsers.AddRange(GroupUsersResponse.Users);
            return adobeUsers;
        }

        /// <summary>
        /// List all groups owned by the org.
        /// </summary>
        /// <returns></returns>
        public async Task<List<AdobeGroup>> GetGroupsAsync()
        {
            // adobe api paging is zero-indexed
            List<AdobeGroup> adobeGroups = new();
            GroupResponse groupResponse = new();
            RestRequest request = new($"{API_GROUP_URI}/0", Method.Get);
            groupResponse = await ExecuteAsync<GroupResponse>(request);
            adobeGroups.AddRange(groupResponse.Groups);
            return adobeGroups;
        }

        /// <summary>
        /// Creates a user group
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="description"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public async Task<AdobeApiResponse> CreateUserGroupAsync(string groupName, string description, string option = "ignoreIfAlreadyExists")
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                throw new ArgumentException("groupName is required.", nameof(groupName));
            }
            RestRequest request = new(API_ACTION_URI, Method.Post);
            GroupAction[] groupActions = new GroupAction[]
            {
                new GroupAction
                {
                    UserGroup = groupName,
                    _do = new Do[]
                    {
                        new Do
                        {
                            CreateUserGroup = new CreateUserGroup
                            {
                                Name = groupName,
                                Description = description,
                                Option = option
                            }
                        }
                    }
                }
            };
            request.AddJsonBody(groupActions);
            return await ExecuteAsync<AdobeApiResponse>(request);
        }

        /// <summary>
        /// Updates a user group
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="description"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public async Task<AdobeApiResponse> UpdateUserGroupAsync(string originalGroupName, string newGroupName, string description)
        {
            if (string.IsNullOrWhiteSpace(originalGroupName))
            {
                throw new ArgumentException("originalGroupName is required.", nameof(originalGroupName));
            }
            if (string.IsNullOrWhiteSpace(newGroupName))
            {
                throw new ArgumentException("newGroupName is required.", nameof(newGroupName));
            }
            RestRequest request = new(API_ACTION_URI, Method.Post);
            GroupAction[] groupActions = new GroupAction[]
            {
                new GroupAction
                {
                    UserGroup = originalGroupName,
                    _do = new Do[]
                    {
                        new Do
                        {
                            UpdateUserGroup = new UpdateUserGroup
                            {
                                Name = newGroupName,
                                Description = description,
                            }
                        }
                    }
                }
            };
            request.AddJsonBody(groupActions);
            return await ExecuteAsync<AdobeApiResponse>(request);
        }

        /// <summary>
        /// Delete a user group
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="description"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public async Task<AdobeApiResponse> DeleteUserGroupAsync(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                throw new ArgumentException("groupName is required.", nameof(groupName));
            }
            RestRequest request = new(API_ACTION_URI, Method.Post);
            GroupAction[] groupActions = new GroupAction[]
            {
                new GroupAction
                {
                    UserGroup = groupName,
                    _do = new Do[]
                    {
                        new Do
                        {
                            DeleteUserGroup = new()
                        }
                    }
                }
            };
            request.AddJsonBody(groupActions);
            return await ExecuteAsync<AdobeApiResponse>(request);
        }


        /// <summary>
        /// Deletes a user. If hardDelete is false, it removes them from Users Menu. If hardDelete is true it removes them from Directory Users Menu.
        /// hardDelete = true implies loss of account metadata and associated cloud assets.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="hardDelete"></param>
        /// <returns></returns>
        public async Task<AdobeApiResponse> DeleteUserAsync(string userName, bool hardDelete)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException("userName is required.", nameof(userName));
            }
            RestRequest request = new(API_ACTION_URI, Method.Post);
            UserAction[] userActions = new UserAction[]
            {
                new UserAction
                {
                    User = userName,
                    _do = new Do[]
                    {
                        new Do
                        {
                            RemoveFromOrg = new RemoveFromOrg
                            {
                                DeleteAccount = hardDelete
                            }
                        }
                    }
                }
            };
            request.AddJsonBody(userActions);
            return await ExecuteAsync<AdobeApiResponse>(request);
        }
        #endregion

        #region utils
        public Dictionary<object, object> GetAuthClaims()
        {
            Dictionary<object, object> claims = new Dictionary<object, object>
            {
                { "exp", DateTimeOffset.Now.ToUnixTimeSeconds() + 600 },
                { "iss", _orgId },
                { "sub", _techAcctId },
                { "aud", API_AUD_URI }
            };
            string[] scopes = _metascopes.Split(',');
            foreach (string scope in scopes)
            {
                claims.Add(scope, true);
            }
            return claims;
        }
        #endregion
    }
}
