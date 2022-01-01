using Pixel.Identity.Shared.ViewModels;
using System.Collections.Generic;

namespace Pixel.Identity.Shared.Request
{
    public class AddUserRolesRequest
    {
        public string UserName { get; set; }

        public List<UserRoleViewModel> RolesToAdd { get; set; } = new List<UserRoleViewModel>();

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
