using IT.WebHost.Admin.Models.Users;
using IT.WebServices.Authentication;
using IT.WebServices.Fragments.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace IT.WebHost.Admin.Controllers
{
    public class UsersController : Controller
    {
        private UserInterface.UserInterfaceClient users {  get; set; }
        private readonly ONUserHelper userHelper;

        public UsersController(UserInterface.UserInterfaceClient users, ONUserHelper userHelper)
        {
            this.users = users;
            this.userHelper = userHelper;
        }

        [HttpGet("/users")]
        public async Task<IActionResult> ListUsersView(
            string? pageSize,
            string? pageOffset
         )
        {
            var vm = new ListUsersViewModel
            {
                PageSize = pageSize,
                PageOffset = pageOffset,
                Users = new List<UserRecord>(),
                PageTotalItems = "0"
            };
            return View(vm);
        }
    }
}
