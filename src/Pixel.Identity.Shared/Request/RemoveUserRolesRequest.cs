using Pixel.Identity.Shared.ViewModels;
using System.Collections.Generic;

namespace Pixel.Identity.Shared.Request
{
    public class RemoveUserRolesRequest
    {
        public string UserName { get; set; } = string.Empty;

        public List<UserRoleViewModel> RolesToRemove { get; set; } = [];

        public RemoveUserRolesRequest()
        {

        }
        public RemoveUserRolesRequest(string userName, IEnumerable<UserRoleViewModel> rolesToRemove)
        {
            this.UserName = userName;
            this.RolesToRemove.AddRange(rolesToRemove);
        }

    }
}
