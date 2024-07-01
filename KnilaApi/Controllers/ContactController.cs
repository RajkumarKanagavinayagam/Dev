using KnilaApi.BusinessLogicLayer.Interface;
using KnilaApi.DataAccessLayer.DataObject.ViewEntity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KnilaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContactController : ControllerBase
    {
        private IContactBal _contactBal { get; set; }
        public ContactController(IContactBal contactBal)
        {
            _contactBal = contactBal;
        }
        [HttpGet("GetAllContact")]
        [NonAction]
        public async Task<IActionResult> GetAllContact()
        {
            return new JsonResult(await _contactBal.GetAllContact());
        }
        [HttpGet("GetAllContact")]
        public async Task<IActionResult> GetAllContact( string? SortColumn = null, string? SortDirection = null, string? SearchKeyword = null, string? filters = null)
        {
            return new JsonResult(await _contactBal.GetAllContact(SortColumn, SortDirection, SearchKeyword, filters));
        }
        [HttpPost("AddOrEditContact")]
        public async Task<IActionResult> RegisterEmployee(ContactViewEntity ModelEntity)
        {
            if (ModelState.IsValid)
            {
                return new JsonResult(await _contactBal.PostEmployee(ModelEntity));
            }
            return BadRequest(ModelState);
        }
        [HttpGet("DeleteContactByContactID")]
        public async Task<IActionResult> DeleteContact(int ContactID)
        {
            return new JsonResult(await _contactBal.DeleteContact(ContactID));
        }
        [AllowAnonymous]
        [HttpPost("Auth")]
        public async Task<IActionResult> Authentication(SigninRequestModel model)
        {
            return new JsonResult(await _contactBal.Authentication(model));
        }
        [HttpGet("GetByContactID")]
        public async Task<IActionResult> GetByContactID(int ContactID)
        {
            return new JsonResult(await _contactBal.GetByContactID(ContactID));
        }
    }
}
