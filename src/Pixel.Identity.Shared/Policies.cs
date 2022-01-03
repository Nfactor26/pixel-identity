namespace Pixel.Identity.Shared
{
    /// <summary>
    /// Policies class holds the name of the different policies required by Pixel Identity
    /// for authorization of users for access control.
    /// </summary>
    public static class Policies
    {
        public const string CanManageApplications = nameof(CanManageApplications);
        public const string CanManageScopes = nameof(CanManageScopes);
        public const string CanManageUsers = nameof(CanManageUsers);
        public const string CanManageRoles = nameof(CanManageRoles);       
    }
}
