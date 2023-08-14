﻿using AdminConsole.Identity;
using AdminConsole.Services;
using AdminConsole.Services.Mail;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminConsole.Pages.Organization;

public class SettingsModel : PageModel
{
    private readonly DataService _dataService;
    private readonly SignInManager<ConsoleAdmin> _signInManager;
    private readonly IMailService _mailService;
    private readonly ISystemClock _systemClock;
    private readonly ILogger<SettingsModel> _logger;

    [BindProperty]
    public Models.Organization Organization { get; set; }

    public SettingsModel(
        DataService dataService,
        SignInManager<ConsoleAdmin> signInManager,
        IMailService mailService,
        ISystemClock systemClock,
        ILogger<SettingsModel> logger)
    {
        _dataService = dataService;
        _signInManager = signInManager;
        _mailService = mailService;
        _systemClock = systemClock;
        _logger = logger;
    }

    public async Task OnGet()
    {
        Organization = await _dataService.GetOrganizationWithData();
    }

    public async Task<IActionResult> OnPostDeleteAsync()
    {
        var username = User.Identity?.Name ?? throw new InvalidOperationException();
        var organization = await _dataService.GetOrganizationWithData();
        var emails = organization.Admins.Select(x => x.Email).ToList();
        await _mailService.SendOrganizationDeletedAsync(organization.Name, emails, username, _systemClock.UtcNow.UtcDateTime);

        var isDeleted = await _dataService.DeleteOrganizationAsync(Organization.Id);
        if (isDeleted)
        {
            await _signInManager.SignOutAsync();
        }

        return RedirectToPage();
    }

    public bool CanDelete => !Organization.Applications.Any();
}