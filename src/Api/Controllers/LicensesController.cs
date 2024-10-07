﻿using Bit.Core.AdminConsole.OrganizationFeatures.OrganizationConnections.Interfaces;
using Bit.Core.Billing.Licenses.OrganizationLicenses;
using Bit.Core.Billing.Licenses.UserLicenses;
using Bit.Core.Context;
using Bit.Core.Exceptions;
using Bit.Core.Models.Api.OrganizationLicenses;
using Bit.Core.Repositories;
using Bit.Core.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bit.Api.Controllers;

[Route("licenses")]
[Authorize("Licensing")]
[SelfHosted(NotSelfHostedOnly = true)]
public class LicensesController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly ICloudGetOrganizationLicenseQuery _cloudGetOrganizationLicenseQuery;
    private readonly IValidateBillingSyncKeyCommand _validateBillingSyncKeyCommand;
    private readonly ICurrentContext _currentContext;
    private readonly IGetUserLicenseQueryHandler _getUserLicenseQueryHandler;

    public LicensesController(
        IUserRepository userRepository,
        IOrganizationRepository organizationRepository,
        ICloudGetOrganizationLicenseQuery cloudGetOrganizationLicenseQuery,
        IValidateBillingSyncKeyCommand validateBillingSyncKeyCommand,
        ICurrentContext currentContext,
        IGetUserLicenseQueryHandler getUserLicenseQueryHandler)
    {
        _userRepository = userRepository;
        _organizationRepository = organizationRepository;
        _cloudGetOrganizationLicenseQuery = cloudGetOrganizationLicenseQuery;
        _validateBillingSyncKeyCommand = validateBillingSyncKeyCommand;
        _currentContext = currentContext;
        _getUserLicenseQueryHandler = getUserLicenseQueryHandler;
    }

    [HttpGet("user/{id}")]
    public async Task<UserLicense> GetUser(string id, [FromQuery] string key)
    {
        var user = await _userRepository.GetByIdAsync(new Guid(id));
        if (user == null)
        {
            return null;
        }
        else if (!user.LicenseKey.Equals(key))
        {
            await Task.Delay(2000);
            throw new BadRequestException("Invalid license key.");
        }

        return await _getUserLicenseQueryHandler.Handle(new GetUserLicenseQuery { User = user });
    }

    /// <summary>
    /// Used by self-hosted installations to get an updated license file
    /// </summary>
    [HttpGet("organization/{id}")]
    public async Task<OrganizationLicense> OrganizationSync(string id, [FromBody] SelfHostedOrganizationLicenseRequestModel model)
    {
        var organization = await _organizationRepository.GetByIdAsync(new Guid(id));
        if (organization == null)
        {
            throw new NotFoundException("Organization not found.");
        }

        if (!organization.LicenseKey.Equals(model.LicenseKey))
        {
            await Task.Delay(2000);
            throw new BadRequestException("Invalid license key.");
        }

        if (!await _validateBillingSyncKeyCommand.ValidateBillingSyncKeyAsync(organization, model.BillingSyncKey))
        {
            throw new BadRequestException("Invalid Billing Sync Key");
        }

        var license = await _cloudGetOrganizationLicenseQuery.GetLicenseAsync(organization, _currentContext.InstallationId.Value);
        return license;
    }
}
