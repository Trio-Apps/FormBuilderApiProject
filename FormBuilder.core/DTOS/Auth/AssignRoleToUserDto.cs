using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Application.DTOS.Auth
{
    public class AssignRoleToUserDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public int RoleId { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
