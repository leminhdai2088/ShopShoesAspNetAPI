using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShopShoesAPI.product;
using ShopShoesAPI.user;
using System;
using System.Threading.Tasks;

namespace MStest
{
    [TestClass]
    public class UnitTest
    {
        private MyDbContext context;
        private IProduct product;
        private IUser user;
        private UserManager<UserEnityIndetity> userManager;

        [TestInitialize]
        public void Initialize()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = configuration.GetConnectionString("MyDb");
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseSqlServer(connectionString).Options;

            this.context = new MyDbContext(options);
            this.context.Database.EnsureCreated();

            // Initialize UserManager
            var userStore = new UserStore<UserEnityIndetity>(this.context);
            var identityOptions = new IdentityOptions();
            var passwordHasher = new PasswordHasher<UserEnityIndetity>();

            this.userManager = new UserManager<UserEnityIndetity>(
                userStore,
                null,
                passwordHasher,
                null,
                null,
                null,
                null,
                null,
                null
            );

            this.product = new ProductService(this.context);
            this.user = new UserService(this.context, this.userManager);
        }

        [TestMethod]
        public async Task IsValidQtyTest()
        {
            bool isValid = await this.product.IsValidBuyQty(1, 200);
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public async Task FinduserByEmailTest()
        {
            var user = await this.user.FindByEmail("123@gmail.com");
            Assert.IsNotNull(user);
        }
    }
}
