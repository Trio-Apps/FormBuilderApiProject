using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Application.DTOS.Auth
{
    public class CreateRoleDto
    {
        [Required]
        [MaxLength(50)]
        public string RoleName { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        public List<int> PermissionIds { get; set; } = new List<int>();
    }
}
