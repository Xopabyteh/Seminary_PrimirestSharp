using Yearly.Contracts.Authentication;
using Yearly.MauiClient.Services.SharpApi.Facades;

namespace Yearly.MauiClient.Services;

public class AuthService
{ 
    /// <summary>
    /// Is null when the user is not logged in
    /// </summary>
    private UserDetailsResponse? userDetailsField = null;
    public UserDetailsResponse? UserDetails
    {
        get => userDetailsField;
        private set
        {
            userDetailsField = value;
            UserDetailsLazy = value ?? new(string.Empty, 0, new(0));
        }
    }

    /// <summary>
    /// Same as UserDetails, but instead of null, no value means default(UserDetailsResponse)
    /// Use this when you expect the field to be populated and don't want to go through null checks or the .Value disgust
    /// </summary>
    public UserDetailsResponse UserDetailsLazy { get; private set; } = default;

    /// <summary>
    /// Returns true if a session cookie is set.
    /// </summary>
    public bool IsLoggedIn => UserDetails is not null;

    public event Action OnLogin;


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
    /// Sets the session cookie and user details.
    /// Calls <see cref="OnLogin"/> after setting the session.
    /// </summary>
    public void SetSession(LoginResponse loginResponse)
    {
        UserDetails = loginResponse.UserDetails;

        OnLogin?.Invoke();
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

        UserDetails = null;

        //Todo: move to event based
        _myPhotosCacheService.InvalidateCache();
        _menuAndOrderCacheService.InvalidateCache();
    }
}