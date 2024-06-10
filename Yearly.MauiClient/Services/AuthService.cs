using Microsoft.AspNetCore.Mvc;
using Yearly.Contracts.Authentication;
using Yearly.MauiClient.Services.SharpApi.Facades;

namespace Yearly.MauiClient.Services;

public class AuthService
{ 
    /// <summary>
    /// Is null when the user is not logged in
    /// </summary>
    private UserDetailsResponse? activeActiveUserDetailsField = null;
    public UserDetailsResponse? ActiveUserDetails
    {
        get => activeActiveUserDetailsField;
        private set
        {
            activeActiveUserDetailsField = value;
            ActiveUserDetailsLazy = value ?? new(string.Empty, 0, new(0));
        }
    }

    /// <summary>
    /// Same as UserDetails, but instead of null, no value means default(UserDetailsResponse)
    /// Use this when you expect the field to be populated and don't want to go through null checks or the .Value disgust
    /// </summary>
    public UserDetailsResponse ActiveUserDetailsLazy { get; private set; } = default;

    /// <summary>
    /// Less common case of when a user can switch context between users
    /// within his "user tenant".
    /// Is null when <see cref="LoginAsync"/> hasn't been called yet
    /// </summary>
    public IReadOnlyList<UserDetailsResponse>? AvailableUsersWithinTenant { get; private set; }

    /// <summary>
    /// Returns true if a session cookie is set.
    /// </summary>
    public bool IsLoggedIn => ActiveUserDetails is not null;

    public event Action? OnLogin;

    /// <summary>
    /// Loads when <see cref="EnsureAutoLoginStateLoadedAsync"/> is called.
    /// If you want to use this, make sure to call the method.
    /// </summary>
    public LoginRequest? AutoLoginStoredCredentials { get; private set; }
    /// <summary>
    /// Loads when <see cref="EnsureAutoLoginStateLoadedAsync"/> is called.
    /// If you want to use this, make sure to call the method.
    /// </summary>
    public bool IsAutoLoginSetup => AutoLoginStoredCredentials is not null;

    private readonly AuthenticationFacade _authenticationFacade;
    private readonly MenuAndOrderCacheService _menuAndOrderCacheService; //Todo: remove dependency
    private readonly MyPhotosCacheService _myPhotosCacheService; //Todo: remove dependency
    

    public AuthService(
        AuthenticationFacade authenticationFacade,
        MenuAndOrderCacheService menuAndOrderCacheService,
        MyPhotosCacheService myPhotosCacheService)
    {
        _authenticationFacade = authenticationFacade;
        _menuAndOrderCacheService = menuAndOrderCacheService;
        _myPhotosCacheService = myPhotosCacheService;
    }

    /// <summary>
    /// Logs in using <see cref="LoginAsync"/> but tries to use stored credentials.
    /// </summary>
    /// <returns>A problem or null if success</returns>
    public Task<ProblemDetails?> AttemptAutoLoginAsync()
    {
        if(AutoLoginStoredCredentials is null)
            throw new NullReferenceException("AutoLoginStoredCredentials is null");

        var request = new LoginRequest(AutoLoginStoredCredentials.Username, AutoLoginStoredCredentials.Password);
        return LoginAsync(request);
    }

    /// <summary>
    /// Logs in using <see cref="AuthenticationFacade"/> and establishes the session.
    /// Calls <see cref="OnLogin"/> after setting the session.
    /// </summary>
    /// <returns>A problem or null if success</returns>
    public async Task<ProblemDetails?> LoginAsync(LoginRequest loginRequest)
    {
        var loginResult = await _authenticationFacade.LoginAsync(loginRequest);
        if (loginResult.IsT1)
        {
            //Problem
            return loginResult.AsT1;
        }

        // Establish session
        var login = loginResult.AsT0;

        AvailableUsersWithinTenant = login.AvailableUserDetails;
        ActiveUserDetails =  login.AvailableUserDetails
            .Single(u => u.UserId == login.InitialActiveUserId);

        OnLogin?.Invoke();

        return null; // Success, no problem
    }

    private const string k_AutoLoginUsernameKey = "autologinusername";
    private const string k_AutoLoginPasswordKey = "autologinpassword";
    public async Task SetupAutoLoginAsync(LoginRequest storedCredentials)
    {
        await SecureStorage.Default.SetAsync(k_AutoLoginUsernameKey, storedCredentials.Username);
        await SecureStorage.Default.SetAsync(k_AutoLoginPasswordKey, storedCredentials.Password);
        AutoLoginStoredCredentials = storedCredentials;
    }

    public async Task EnsureAutoLoginStateLoadedAsync()
    {
        if (AutoLoginStoredCredentials is not null)
            return;

        var username = await SecureStorage.Default.GetAsync(k_AutoLoginUsernameKey);
        var password = await SecureStorage.Default.GetAsync(k_AutoLoginPasswordKey);

        if (username is null || password is null)
            return;

        AutoLoginStoredCredentials = new LoginRequest(username, password);
    }

    public void RemoveAutoLogin()
    {
        SecureStorage.Default.Remove(k_AutoLoginUsernameKey);
        SecureStorage.Default.Remove(k_AutoLoginPasswordKey);
        AutoLoginStoredCredentials = null;
    }

    /// <summary>
    /// Removes the session using <see cref="AuthenticationFacade"/>, but also removes auto login.
    /// Is used when the user want's to login to another account.
    /// If you only wish to remove the session, use <see cref="RemoveSessionAsync"/>.
    /// </summary>
    /// <returns></returns>
    public async Task LogoutAsync()
    {
        RemoveAutoLogin(); //When logging out, it makes no sense to keep login details about the user that just logged out
        await RemoveSessionAsync();
    }

    /// <summary>
    /// Removes session using <see cref="AuthenticationFacade"/> and removes stored details about this session from service cache.
    /// </summary>
    /// <returns></returns>
    public async Task RemoveSessionAsync()
    {
        await _authenticationFacade.LogoutAsync();

        ActiveUserDetails = null;

        //Todo: move to event based
        _myPhotosCacheService.InvalidateCache();
        _menuAndOrderCacheService.InvalidateCache();
    }

    public async Task SwitchContextAsync(UserDetailsResponse newUser)
    {
        // Switch context in api
        await _authenticationFacade.SwitchContextAsync(newUser.UserId);

        // Update local state
        ActiveUserDetails = newUser;

        // Clear all caches
        _myPhotosCacheService.InvalidateCache();
        _menuAndOrderCacheService.InvalidateCache();
    }
}