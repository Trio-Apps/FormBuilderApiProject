using FormBuilder.API.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.core.DTOS.Auth
{

    public class AuthResult
    {
        public bool Success { get; set; }
        public UserDto User { get; set; }
        public string ErrorMessage { get; set; }
        public int StatusCode { get; set; }
    }
}
