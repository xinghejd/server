using Bit.Core.AdminConsole.Entities;
using Bit.Core.AdminConsole.Repositories;
using Bit.Core.Repositories;
using Xunit;

namespace Bit.Infrastructure.IntegrationTest.Repositories;

public class GroupRepositoryTests
{
    [DatabaseTheory, DatabaseData]
    public async Task CreateAsync_WithoutAccessAll_Works(IGroupRepository groupRepository,
        IOrganizationRepository organizationRepository)
    {
        var organization = await organizationRepository.CreateAsync(new Organization()
        {
            Name = "Test Org",
            BillingEmail = "email", // TODO: EF does not enfore this being NOT NULL
            Plan = "Test", // TODO: EF does not enforce this being NOT NULl
        });

        // Test: write group
        var group = await groupRepository.CreateAsync(new Group
        {
            Id = new Guid(), Name = "my group", ExternalId = "externalId", OrganizationId = organization.Id
        });

        // Test: read group
        var readGroup = await groupRepository.GetByIdAsync(group.Id);
        Assert.Equal(readGroup?.Id, group.Id);
    }
}
