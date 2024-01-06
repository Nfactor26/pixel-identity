using Pixel.Identity.Shared.ViewModels;
using System.Collections.Generic;

namespace Pixel.Identity.Shared.Request
{
    public class AddUserRolesRequest
    {
        public string UserName { get; set; } = string.Empty;

        public List<UserRoleViewModel> RolesToAdd { get; set; } = [];

        public AddUserRolesRequest()
        {

        }

        public AddUserRolesRequest(string userName, IEnumerable<UserRoleViewModel> rolesToAdd)
        {
            this.UserName = userName;
            this.RolesToAdd.AddRange(rolesToAdd);
        }

    }
}
