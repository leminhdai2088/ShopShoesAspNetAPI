using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopShoesAPI.common;
using ShopShoesAPI.user;
using ShopShoesAPI.product;

namespace ShopShoesAPI.admin
{
    public class AdminService : IAdmin
    {
        private readonly UserManager<UserEnityIndetity> userManager;
        public AdminService(UserManager<UserEnityIndetity> userManager) {
            this.userManager = userManager;
        }

        public Task<string> DeleteUser(string id)
        {
            throw new NotImplementedException();
        }
        public Task<Summary> SummaryStats()
        {
            try
            {

            }
            catch (Exception ex)           
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<UserDTO>> FindAllUser(QueryAndPaginateDTO queryAndPaginate)
        {
            try
            {
               
                var query = this.userManager.Users.AsQueryable();
                if(!string.IsNullOrEmpty(queryAndPaginate.searchString) )
                {
                    query = query.Where(x => (
                        x.FullName.Contains(queryAndPaginate.searchString) ||
                        x.Email.Contains(queryAndPaginate.searchString) ||
                        x.UserName.Contains(queryAndPaginate.searchString)
                    ));
                }
                var users = await query
                    .OrderBy(u => u.Email)
                    .Skip((queryAndPaginate.pageIndex - 1) * queryAndPaginate.pageSize)
                    .Take(queryAndPaginate.pageSize)
                    .ToListAsync();

                List<UserDTO> userDTOs= new List<UserDTO>();
                foreach (var user in users)
                {
                    var userDTO = new UserDTO
                    {
                        FullName = user.FullName,
                        Email = user.Email,
                        Phone = user.PhoneNumber,
                        Address = user.Address,
                    };
                    userDTOs.Add(userDTO);
                }
                return userDTOs;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

             async Task<string> DeleteUser(string id)
            {
                try
                {
                    var user = await this.userManager.FindByIdAsync(id);
                    if (user == null)
                        throw new BadHttpRequestException("User is not found");

                    var result = await this.userManager.DeleteAsync(user);
                    if (result.Succeeded)
                    {
                        return "Delete user successfully";
                    }
                    return "Delete user failure";

                }
                catch (Exception ex )
                {
                    throw new Exception(ex.Message);
                }
            }

        }
    }
}
