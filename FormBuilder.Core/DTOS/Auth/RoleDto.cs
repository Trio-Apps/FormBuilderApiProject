using FormBuilder.core.DTOS.Auth;
using System;
using System.Collections.Generic;

namespace FormBuilder.Application.DTOS.Auth
{
    public class RoleDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }




    public class RoleDetailDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
        public List<UserDto> Users { get; set; } = new List<UserDto>();
    }

  

   
}