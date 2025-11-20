using System;
using System.Collections.Generic;

namespace FormBuilder.Application.DTOS.Auth
{
    public class UsersDto // ✅ تغيير من UsersDto إلى UserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}