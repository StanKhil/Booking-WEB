using Booking_WEB.Data;
using Booking_WEB.Data.DataAccessors;
using Booking_WEB.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Booking_WEB.Controllers
{
    public class AdministratorController( 
        RealtyAccessor realtyAccessor,
        UserAccessAccessor userAccessAccessor) : Controller
    {
        private readonly RealtyAccessor _realtyAccessor = realtyAccessor;
        private readonly UserAccessAccessor _userAccessAccessor = userAccessAccessor;

        public async Task<IActionResult> GetRealtiesTable()
        {
            var realties = await _realtyAccessor.GetAllAsync(isEditable: false);
            string? tableBodyContent = "";
            foreach(var realty in realties)
            {
                tableBodyContent += $"<tr><td>{realty.Name}</td> <td>{realty.Description}</td> " +
                    $"<td>{realty.Slug}</td> <td>{realty.Price}</td> <td>{realty.Country.Name}</td> <td>{realty.City.Name}</td> <td>{realty.RealtyGroup.Name}</td> </tr>";
            }
            return Content(tableBodyContent, "text/html");
        }

        public async Task<IActionResult> GetUsersTable()
        {
            //var userAccesses = (from userAccess in _dataContext.UserAccesses 
            //            join user in _dataContext.Users on userAccess.UserId equals user.Id 
            //            join role in _dataContext.UserRoles on userAccess.RoleId equals role.Id 
            //            select userAccess).AsNoTracking();
            var userAccesses = await _userAccessAccessor.GetAllAccesses(isEditable: false);

            string? tableBodyContent = "";
            foreach(var userAccess in userAccesses)
            {
                tableBodyContent += $"<tr><td>{userAccess.UserData.FirstName}</td> <td>{userAccess.UserData.LastName}</td> " +
                    $"<td>{userAccess.UserData.Email}</td> <td>{userAccess.Login}</td> <td>{userAccess.UserData.BirthDate}</td> <td>{userAccess.UserRole.Id}</td></tr>";
            }

            return Content(tableBodyContent, "text/html");
        }
    }
}
