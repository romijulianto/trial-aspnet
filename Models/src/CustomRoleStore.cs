namespace ASPNETMaker2023.Models;

/// <summary>
/// Custom role store class (Use User Level as role)
/// </summary>
public class CustomRoleStore : IRoleStore<ApplicationRole>
{
    // Constructor
    public CustomRoleStore()
    {
    }

    // CreateAsync
    public Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    // DeleteAsync
    public Task<IdentityResult> DeleteAsync(ApplicationRole role, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    // Dispose
    public void Dispose() {}

    // FindByIdAsync
    public Task<ApplicationRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    // FindByNameAsync
    public Task<ApplicationRole> FindByNameAsync(string roleName, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    // GetNormalizedRoleNameAsync
    public Task<string> GetNormalizedRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    // GetRoleIdAsync
    public Task<string> GetRoleIdAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (role == null)
            throw new ArgumentNullException(nameof(role));
        return Task.FromResult(role.Id);
    }

    // GetRoleNameAsync
    public Task<string> GetRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (role == null)
            throw new ArgumentNullException(nameof(role));
        return Task.FromResult<string>(role.Name);
    }

    // SetNormalizedRoleNameAsync
    public Task SetNormalizedRoleNameAsync(ApplicationRole role, string normalizedName, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    // SetRoleNameAsync
    public Task SetRoleNameAsync(ApplicationRole role, string roleName, CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    // UpdateAsync
    public Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken) =>
        throw new NotImplementedException();
}
