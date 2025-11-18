using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Application.DTOS.Auth
{
    public class UserWithRolesDto
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public List<RoleDto> Roles { get; set; } = new List<RoleDto>();
    }
}
