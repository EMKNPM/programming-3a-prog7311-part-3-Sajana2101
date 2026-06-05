using GLMS2.ViewModels;

namespace GLMS2.Interfaces
{
    public interface IAuthApiService
    {
        Task<string?> LoginAsync(LoginViewModel model);
    }
}