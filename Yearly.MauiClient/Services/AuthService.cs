﻿using Yearly.Contracts.Authentication;
using Yearly.MauiClient.Services.SharpApiFacades;

namespace Yearly.MauiClient.Services;

public class AuthService
{ 
    private const string k_SessionCookieKey = "SessionCookie";

    public string? SessionCookie { get; private set; } = null;
    public UserDetailsResponse? UserDetails { get; private set; } = null;

    /// <summary>
    /// Returns true if a session cookie is set.
    /// </summary>
    public bool IsLoggedIn => SessionCookie is not null;

    public event Action OnLogin = null!; 
    

    private readonly AuthenticationFacade _authenticationFacade;
    private readonly SharpAPIClient _sharpAPIClient;

    public AuthService(AuthenticationFacade authenticationFacade, SharpAPIClient sharpAPIClient)
    {
        _authenticationFacade = authenticationFacade;
        _sharpAPIClient = sharpAPIClient;
    }

    /// <summary>
    /// Tries to load the session from client storage. Acts as <see cref="SetSessionAsync"/>.
    /// Does not attempt to login the user, only tries to load the session and checks if its valid.
    /// </summary>
    /// <returns>True if session is loaded and valid, False if session is invalid.</returns>
    public async Task<bool> TryLoadStoredSessionAsync()
    {
        var sessionCookie = await SecureStorage.GetAsync(k_SessionCookieKey);
        if (sessionCookie is null)
        {
            return false;
        }

        //Set the session cookie, so that we can get our details
        _sharpAPIClient.SetSessionCookie(sessionCookie);

        //Check that the session is valid
        var userDetails = await _authenticationFacade.GetMyDetailsAsync();
        if (userDetails is null)
        {
            //Session is not valid, our cookie is expired
            return false;
        }

        SessionCookie = sessionCookie;
        UserDetails = userDetails.Value;

        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        OnLogin?.Invoke();

        return true;
    }

    /// <summary>
    /// Sets the session cookie and user details.
    /// Also sets the session cookie in the <see cref="SharpAPIClient.SetSessionCookie"/>.
    /// Also stores the session cookie in the secure storage.
    /// Calls <see cref="OnLogin"/> after setting the session.
    /// </summary>
    public async Task SetSessionAsync(LoginResponse loginResponse)
    {
        SessionCookie = loginResponse.SessionCookie;
        UserDetails = loginResponse.UserDetails;

        _sharpAPIClient.SetSessionCookie(SessionCookie);

        await SecureStorage.SetAsync(k_SessionCookieKey, SessionCookie);

        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        OnLogin?.Invoke();
    }
}