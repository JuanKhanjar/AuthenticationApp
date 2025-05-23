using AuthenticationApp.Domain.Entities;

namespace AuthenticationApp.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<ApplicationUser?> GetByIdAsync(string id);
        Task<bool> ExistsByEmailAsync(string email);
    }
}
