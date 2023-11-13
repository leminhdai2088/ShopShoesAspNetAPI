namespace ShopShoesAPI.user
{
    public interface IUser
    {
        public Task<UserDTO> FindById(string id);
        public Task<UserDTO> FindByEmail(string email);
        public Task<string> Update(string userId, UpdateUserDTO updateUser);
        public Task<string> ChangePassword(string userId,ChangePasswordDTO changePassword);
    }
}
