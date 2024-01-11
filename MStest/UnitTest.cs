using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShopShoesAPI.auth;
using ShopShoesAPI.common;
using ShopShoesAPI.product;
using ShopShoesAPI.user;
using StackExchange.Redis;
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
        private IAuth auth;
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
            this.auth = new AuthService(this.userManager);
        }

        [TestMethod]
        public async Task IsValidQtyTest()
        {
            bool isValid = await this.product.IsValidBuyQty(1, 1);
            Assert.IsTrue(isValid);
        }

        [TestMethod]
        public async Task FinduserByEmailTest()
        {
            var user = await this.user.FindByEmail("user@example.com");
            Assert.IsNotNull(user);
        }

        [TestMethod]
        public  void VerifyAcessTokenTest()
        {
            var result = this.auth.VerifyAccessToken("Bearer eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJJZCI6IjNkOTdiNzYwLWU0ODUtNGE1Mi04MmMwLTg3ZDliODE3YjliYyIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6InVzZXJAZXhhbXBsZS5jb20iLCJqdGkiOiI5YmNlZGRiMi1lM2MzLTRjOGItYTMzYi1hNmM1MTM4NGI0N2QiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJVc2VyIiwiZXhwIjoxNzA1NTQyNzAyLCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo3Mjk2IiwiYXVkIjoiVXNlciJ9.53SAnRCrmvVOPtElhJGgFMQ3Xj5vkmYRCav1ljiP9EHGJ9nfTnN-aKvSjlv1nmj0jUNVmPJJGh2hbD7n-jeH-g");
            Assert.IsInstanceOfType(result, typeof(PayloadTokenDTO));
        }

        [TestMethod]
        public async Task LoginAsyncTest()
        {
            LoginDTO loginDTO = new LoginDTO()
            {
                Email = "user@example.com",
                Password = "string@123A"
            };
            var result = await this.auth.LoginAsync(loginDTO);
            Assert.IsTrue(result is TokenDTO);
        }
    }
}
