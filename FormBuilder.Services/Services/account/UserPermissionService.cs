using FormBuilder.Core.Models;
using Microsoft.EntityFrameworkCore;

public class UserPermissionService : IUserPermissionService
{
    private readonly AkhmanageItContext _context;

    public UserPermissionService(AkhmanageItContext context)
    {
        _context = context;
    }


   async Task<IEnumerable<TblUserPermission>> IUserPermissionService.GetAllAsync()
    {

        return await _context.TblUserPermissions
                             .Where(p => p.IsActive) // اختياري: فقط النشطة
                             .ToListAsync();
    }
}
