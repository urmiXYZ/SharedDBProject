using MDUA.Entities.Bases;
using MDUA.Entities.List;
using MDUA.Entities;
using MDUA.Facade.Interface;

public interface IUserLoginFacade : ICommonFacade<UserLogin, UserLoginList, UserLoginBase>
{
    UserLoginResult GetUserLoginBy(string email, string password);

    // New method to fetch user by Id
    UserLoginResult GetUserLoginById(int userId);

    // Optional: fetch permissions names
    List<string> GetUserPermissionNamesByUserId(int userId);
    bool IsUserAuthorized(int userId, string actionName);
    List<string> GetAllUserPermissionNames(int userId);

}
