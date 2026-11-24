using System;
using System.Collections.Generic;
using System.Text;

namespace Asisya.Application.Auth.Dto
{
    public record LoginResponse(string Token, string Email, int RoleId, string Role);
}
