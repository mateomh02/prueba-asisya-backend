using Asisya.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Asisya.Application.Routes.Interfaces
{
    public interface IRouteRepository
    {
        Task<List<AppRoute>> ListRoutesAsync(string roleName, CancellationToken ct = default);
    }
}
