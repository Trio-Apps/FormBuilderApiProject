using FormBuilder.Core.Models;

public interface IUserPermissionService
{
    Task<IEnumerable<TblUserPermission>> GetAllAsync();
}
