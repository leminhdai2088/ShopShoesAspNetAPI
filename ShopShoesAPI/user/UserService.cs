using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using ShopShoesAPI.common;
using ShopShoesAPI.Data;
using ShopShoesAPI.email;

namespace ShopShoesAPI.user
{
    public class UserService : IUser
    {
        private readonly MyDbContext _context;
        private readonly AppSettings _appSettings;
        private readonly UserManager<UserEnityIndetity> userManager;

        public UserService(MyDbContext context, IOptionsMonitor<AppSettings> optionsMonitor,
            UserManager<UserEnityIndetity> userManager)
        {
            this._context = context;
            this._appSettings = optionsMonitor.CurrentValue;
            this.userManager = userManager;
        }

        public UserService(MyDbContext context, UserManager<UserEnityIndetity> userManager)
        {
            this._context = context;
            this.userManager = userManager;
        }

        public async Task<UserDTO> FindById(string id)
        {
            try
            {
                var user = await this.userManager.FindByIdAsync(id);
                if(user == null)
                {
                    throw new ArgumentException("User is not found");
                }
                return new UserDTO
                {
                    FullName = user.FullName,
                    Email = user.Email,
                    Address = user.Address,
                    Phone = user.PhoneNumber
                };
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserDTO> FindByEmail(string email)
        {
            try
            {
                var user = await this.userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    throw new ArgumentException("User is not found");
                }
                return new UserDTO
                {
                    FullName = user.FullName,
                    Email = user.Email,
                    Address = user.Address,
                    Phone = user.PhoneNumber
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<string> Update(string userId, UpdateUserDTO updateUser)
        {
            try
            {
                var user = await this.userManager.FindByIdAsync(userId);
                if(user == null)
                {
                    throw new Exception("User is not found");
                }
                if(updateUser.FullName != null)
                {
                    user.FullName = updateUser.FullName;

                }
                if (updateUser.Address != null)
                {
                    user.Address = updateUser.Address;

                }

                var result = await this.userManager.UpdateAsync(user);
                if(!result.Succeeded) 
                { 
                    throw new Exception($"Failed to update user: {userId}");
                }
                this._context.Update(user);
                await this._context.SaveChangesAsync();
                return "Update user successfully";
                
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> ChangePassword(string userId, ChangePasswordDTO changePassword)
        {
            try
            {
                var user = await this.userManager.FindByIdAsync(userId);
                if(user == null)
                {
                    throw new Exception("User is not found");
                }

                var isValidCurrentPassword = await this.userManager.CheckPasswordAsync(user, changePassword.OldPassword);
                if(isValidCurrentPassword == false)
                {
                    throw new Exception("Invalid current password");
                }
                var result = await this.userManager
                    .ChangePasswordAsync(user, changePassword.OldPassword, 
                                        changePassword.NewPassword);
                if(!result.Succeeded)
                {
                    throw new Exception("Failed to change password");
                }
                return "Change password successfully!";
            }catch(Exception ex )
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
