using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopShoesAPI.common;
using ShopShoesAPI.Data;
using ShopShoesAPI.enums;
using ShopShoesAPI.Enums;
using ShopShoesAPI.user;
using ShopShoesAPI.product;

namespace ShopShoesAPI.admin
{
    public class AdminService : IAdmin
    {
        private readonly UserManager<UserEnityIndetity> userManager;
        private readonly MyDbContext context;
        public AdminService(UserManager<UserEnityIndetity> userManager, MyDbContext context) {
            this.userManager = userManager;
            this.context = context;
        }

        public async Task<bool> DeleteUserById(string id)
        {
            try
            {
                var user = await this.userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (user == null)
                {
                    throw new Exception("User is not found");
                }

                var isAdmin = await this.userManager.IsInRoleAsync(user, Roles.Admin);
                if (isAdmin)
                {
                    throw new Exception("Cannot delete this user");
                }
                user.Deleted = true;
                await this.context.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<UserDTO>> FindAllUser(QueryAndPaginateDTO queryAndPaginate, StatusUserEnum? statusUserEnum)
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

                if(statusUserEnum == StatusUserEnum.Active)
                {
                    query = query.Where(u => u.Deleted == false);
                }else if(statusUserEnum == StatusUserEnum.Deleted)
                {
                    query = query.Where(u => u.Deleted == true);
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

        public async Task<IEnumerable<object>> FindAllOrder(QueryAndPaginateDTO queryAndPaginate, OrderStatusEnum? status)
        {
            try
            {
                var query = this.context.OrderEntities.AsQueryable();
                if (!string.IsNullOrEmpty(queryAndPaginate.searchString))
                {
                    query = query.Where(x => 
                        (x.User.FullName == queryAndPaginate.searchString ||
                        x.User.Email == queryAndPaginate.searchString ||
                        x.User.PhoneNumber == queryAndPaginate.searchString));
                }
                if(status != null)
                {
                    query = query.Where(x => x.Status == status);
                }
                var orders = await query
                    .Include(x => x.User)
                        .ThenInclude(u => u.Id)
                    .Include(x => x.User)
                        .ThenInclude(u => u.FullName)
                    .Include(x => x.User)
                        .ThenInclude(u => u.Email)
                    .Include(x => x.User)
                        .ThenInclude(u => u.PhoneNumber )
                    .OrderBy(x => x.Id)
                    .Skip((queryAndPaginate.pageIndex - 1) * queryAndPaginate.pageSize)
                    .Take(queryAndPaginate.pageSize)
                    .ToListAsync();
                return orders;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<decimal> CalculateTotalSale()
        {
            try
            {
                decimal total = 0;
                var orders = await this.context.OrderEntities.ToListAsync();
                foreach( var order in orders)
                {
                    total += order.Total;
                }
                return total;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> CalculateTotalOrder()
        {
            try
            {
                decimal total = 0;
                var orders = await this.context.OrderEntities.CountAsync();
                return orders;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> CalculateTotalProduct()
        {
            try
            {
                decimal total = 0;
                var products = await this.context.ProductEntities.CountAsync();
                return products;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<object[]> CalculateTotalNewOrder()
        {
            try
            {
                var orders = await this.context.OrderEntities
                    .OrderByDescending(e => e.createdAt)
                    .Take(5)
                    .ToArrayAsync();
                return orders;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
