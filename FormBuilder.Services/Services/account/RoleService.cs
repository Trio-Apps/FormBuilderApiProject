using FormBuilder.Core.Models;
using Microsoft.EntityFrameworkCore;
namespace FormBuilder.Core.Models;

public class RoleService : IRoleService
{
    private readonly AkhmanageItContext _context;

    public RoleService(AkhmanageItContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TblUserGroup>> GetAllRolesAsync()
    {
        return await _context.TblUserGroups
                             .Where(r => r.IsActive == true)
                             .ToListAsync();
    }
}
