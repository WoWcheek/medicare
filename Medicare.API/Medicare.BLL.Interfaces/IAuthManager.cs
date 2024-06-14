using Medicare.BLL.DTO;

namespace Medicare.BLL.Interfaces;

public interface IAuthManager
{
    Task<bool> Register(RegisterDTO registerDTO);
    Task<LoggedInUserWithTokenDTO?> Login(LoginDTO loginDTO);
}
