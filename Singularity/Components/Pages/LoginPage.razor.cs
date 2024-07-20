using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Singularity.Components.Layout;

namespace Singularity.Components.Pages;

public partial class LoginPage
{
    private string email = string.Empty;
    private string password = string.Empty;
    private string error = string.Empty;


    private void GoToSignup()
    {
        Nav.NavigateTo("/signup");
    }
    void OnEmailChanged(ChangeEventArgs e)
    {
        if (e.Value == null)
            return;
        email = e.Value.ToString()!;
        OnInputChanged(e);
    }
    void OnPasswordChanged(ChangeEventArgs e)
    {
        if (e.Value == null)
            return;
        password = e.Value.ToString()!;
        OnInputChanged(e);
    }

    void OnInputChanged(ChangeEventArgs e)
    {
        SetError(Validate());
    }

    bool SetError((bool IsValid, string? Error) result)
    {
        error = result.IsValid ? string.Empty : result.Error!;
        StateHasChanged();
        return result.IsValid;
    }

    private async void Login()
    {
        if (!SetError(Validate()))
            return;


        try
        {
            MainLayout.User = await AuthService.LoginUserAsync(email, password);
            Nav.NavigateTo("/");
        }
        catch(Exception e)
        {
            error = e.Message;
            StateHasChanged();
        }
    }

    private async void GenerateResetLink()
    {
        bool isEmailCorrect = true;
        if (email == null || email.Length < 2 || !IsValidEmail(email))
            isEmailCorrect = false;

        if (!isEmailCorrect)
        {
            error = "Please enter valid email to send password reset link.";
            StateHasChanged();
            return;
        }

        try
        {
            await AuthService.SendPasswordResetLinkAsync(email!);
            error = "Reset link is sent to given emaill";
        }
        catch (Exception e)
        {
            error = e.Message;
        }

        StateHasChanged();

    }
    private (bool IsValid, string? Error) Validate()
    {
        if (email == null || email.Length < 2 || !IsValidEmail(email))
            return (false, "Invalid email entered");

        if (password == null || password.Length < 6)
            return (false, "Invalid password entered please ensure minimum 6 characters are there ");

        return (true, null);
    }
    private bool IsValidEmail(string emailaddress)
    {
        return Regex.IsMatch(emailaddress, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");

    }
}
