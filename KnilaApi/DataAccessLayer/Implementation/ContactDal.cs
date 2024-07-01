using KnilaApi.BusinessLogicLayer.Interface;
using KnilaApi.DataAccessLayer.DataObject.DataContext.Interface;
using KnilaApi.DataAccessLayer.DataObject.Entity;
using KnilaApi.DataAccessLayer.DataObject.Helper;
using KnilaApi.DataAccessLayer.DataObject.ViewEntity;
using KnilaApi.DataAccessLayer.Interface;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace KnilaApi.DataAccessLayer.Implementation
{
    public class ContactDal : IContactDal
    {
        private ILogger<IContactDal> Logger { get; }
        private IDataContext _context { get; set; }
        private readonly IConfiguration _configuration;
        public ContactDal(IDataContext context, IConfiguration configuration, ILogger<IContactDal> logger)
        {
            _context = context;
            _configuration = configuration;
            Logger = logger;
        }
        public async Task<ResponseEntity<string>> PostData(ContactEntity modeldata)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    string responseMessage = "";
                    var EmployeeData = await _context.Contact.Where(k => k.ContactId == modeldata.ContactId).FirstOrDefaultAsync();
                    if (EmployeeData != null)
                    {
                        var patchDta = new JsonPatchDocument<ContactEntity>()
                            .Replace(p => p.FirstName, modeldata.FirstName)
                            .Replace(p => p.LastName, modeldata.LastName)
                            .Replace(p => p.Email, modeldata.Email)
                            .Replace(p => p.Password, modeldata.Password)
                            .Replace(p => p.Address, modeldata.Address)
                            .Replace(p => p.City, modeldata.City)
                            .Replace(p => p.State, modeldata.State)
                            .Replace(p => p.Country, modeldata.Country)
                            .Replace(p => p.UpdatedAt, modeldata.UpdatedAt)
                            .Replace(P => P.PhoneNumber, modeldata.PhoneNumber);
                        patchDta.ApplyTo(EmployeeData);
                        responseMessage = "Contact Updated Successfully!";
                    }
                    else
                    {
                        await _context.Contact.AddAsync(modeldata);
                        responseMessage = "Contact Added Successfully!";
                    }

                    await _context.SaveChangesAsync(default);
                    await transaction.CommitAsync();
                    return new ResponseEntity<string>()
                    {
                        Result = responseMessage,
                        IsSuccess = true,
                        ResponseMessage = "Successfully",
                        StatusCode = 200,
                        StatusMessage = "Success"
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Logger.LogError($"Exception Message - {ex.Message} ({ex.InnerException?.Message})", ex);
                    return new ResponseEntity<string>()
                    {
                        Result = null,
                        IsSuccess = false,
                        ResponseMessage = ex.Message,
                        StatusCode = 500,
                        StatusMessage = "Failed"
                    };
                }
            }
        }

        public async Task<ResponseEntity<ContactViewEntity>> GetAllContact()
        {
            try
            {
                var EmployeeData = await _context.Contact.Where(k => k.IsActive == true).Select(o => new ContactViewEntity()
                {
                    ContactId = o.ContactId,
                    FirstName = o.FirstName,
                    LastName = o.LastName,
                    Address = o.Address,
                    City = o.City,
                    Country = o.Country,
                    State = o.State,
                    PostalCode = o.PostalCode,
                }).ToListAsync();
                if (EmployeeData != null)
                {
                    return new ResponseEntity<ContactViewEntity>()
                    {
                        ListResult = EmployeeData.ToList(),
                        IsSuccess = true,
                        ResponseMessage = "Successfully",
                        StatusCode = 200,
                        StatusMessage = "Success"
                    };
                }
                return new ResponseEntity<ContactViewEntity>()
                {
                    Result = null,
                    IsSuccess = false,
                    ResponseMessage = "Data not found",
                    StatusCode = 400,
                    StatusMessage = "Error"
                };

            }
            catch (Exception ex)
            {
                return new ResponseEntity<ContactViewEntity>()
                {
                    Result = null,
                    IsSuccess = false,
                    ResponseMessage = ex.Message,
                    StatusCode = 500,
                    StatusMessage = "Failed"
                };
            }
        }

        public async Task<ResponseEntity<bool>> DeleteContact(int ContactID)
        {
            try
            {
                var employeeData = await _context.Contact.Where(l => l.ContactId == ContactID && l.IsActive == true).FirstOrDefaultAsync();
                if (employeeData != null)
                {
                    var patchDta = new JsonPatchDocument<ContactEntity>()
                       .Replace(p => p.IsActive, false)
                       .Replace(p => p.UpdatedAt, DateTime.Now);
                    patchDta.ApplyTo(employeeData);
                    await _context.SaveChangesAsync(default);
                    return new ResponseEntity<bool>()
                    {
                        Result = true,
                        IsSuccess = true,
                        ResponseMessage = "Successfully",
                        StatusCode = 200,
                        StatusMessage = "Success"
                    };
                }
                return new ResponseEntity<bool>()
                {
                    Result = false,
                    IsSuccess = false,
                    ResponseMessage = "Data not found !",
                    StatusCode = 400,
                    StatusMessage = "Error"
                };

            }
            catch (Exception ex)
            {

                return new ResponseEntity<bool>()
                {
                    Result = false,
                    IsSuccess = false,
                    ResponseMessage = ex.Message,
                    StatusCode = 500,
                    StatusMessage = "Failed"
                };
            }
        }

        public async Task<ResponseEntity<string>> Authentication(SigninRequestModel signinRequestModel)
        {
            try
            {
                var authenticationResponse = await _context.Contact.Where(x => x.Email == signinRequestModel.EmailId && x.Password == signinRequestModel.Password && x.IsActive == true).FirstOrDefaultAsync();
                if (authenticationResponse != null)
                {
                    var authClaims = new List<Claim>
                        {
                        new Claim("UserRefID", authenticationResponse.ContactId.ToString()),
                        new Claim("FirstName", authenticationResponse.FirstName),
                        new Claim("LastName", authenticationResponse.LastName),
                        new Claim("guid", Guid.NewGuid().ToString()),
                        new Claim("date", DateTime.UtcNow.ToString()),
                     };
                    var token = GenerateToken(authClaims);
                    var authToken = new JwtSecurityTokenHandler().WriteToken(token);
                    var refreshToken = GenerateRefreshToken();
                    return new ResponseEntity<string>
                    {
                        Result = "Successfully Logined",
                        Token = authToken.ToString(),
                        RefreshToken = refreshToken,
                        IsSuccess = true,
                        ResponseMessage = new HttpResponseMessage(HttpStatusCode.Accepted).RequestMessage?.ToString(),
                        StatusMessage = HttpStatusCode.OK.ToString()
                    };
                }
                else
                {
                    return new ResponseEntity<string>
                    {
                        IsSuccess = false,
                        ResponseMessage = new HttpResponseMessage(HttpStatusCode.Accepted).RequestMessage?.ToString(),
                        StatusMessage = HttpStatusCode.OK.ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseEntity<string>()
                {
                    IsSuccess = false,
                    ResponseMessage = ex.Message,
                    StatusMessage = HttpStatusCode.InternalServerError.ToString()
                };
            }
        }

        #region Helper
        public JwtSecurityToken GenerateToken(List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Jwt:Key")));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:AccessTokenExpiresMinutes")),
                    signingCredentials: signIn
                );
            return token;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        public async Task<ResponseEntity<ContactViewEntity>> GetAllContact(string? SortColumn = null, string? SortDirection = null, string? SearchKeyword = null, string? filters = null)
        {
            try
            {
                IQueryable<ContactEntity> query = _context.Contact.Where(k => k.IsActive == true);

                // Apply search filter
                if (!string.IsNullOrEmpty(SearchKeyword))
                {
                    query = query.Where(c => c.FirstName.Contains(SearchKeyword)
                                             || c.LastName.Contains(SearchKeyword)
                                             || c.Address.Contains(SearchKeyword)
                                             || c.Email.Contains(SearchKeyword)
                                             || c.City.Contains(SearchKeyword)
                                             || c.Country.Contains(SearchKeyword)
                                             || c.State.Contains(SearchKeyword)
                                             || c.PostalCode.Contains(SearchKeyword));
                }

                // Execute the query and project the results
                var contacts = await query
                      .OrderByDescending(o => o.ContactId)
                      .Select(o => new ContactViewEntity
                      {
                          ContactId = o.ContactId,
                          FirstName = o.FirstName,
                          LastName = o.LastName,
                          Address = o.Address,
                          Email = o.Email,
                          City = o.City,
                          Country = o.Country,
                          State = o.State,
                          //PostalCode = o.PostalCode,
                          PhoneNumber = o.PhoneNumber,
                      }).ToListAsync();

                // Create the response entity
                return new ResponseEntity<ContactViewEntity>
                {
                    ListResult = contacts,
                    IsSuccess = true,
                    ResponseMessage = "Successfully",
                    StatusCode = 200,
                    StatusMessage = "Success"
                };

            }
            catch (Exception ex)
            {
                return new ResponseEntity<ContactViewEntity>()
                {
                    ListResult = null,
                    IsSuccess = false,
                    ResponseMessage = ex.Message,
                    StatusMessage = HttpStatusCode.InternalServerError.ToString()
                };
            }
        }

        public async Task<ResponseEntity<ContactViewEntity>> GetByContactID(int ContactID)
        {
            try
            {
                var userByID = await _context.Contact.Where(p => p.ContactId == ContactID && p.IsActive == true).Select(o => new ContactViewEntity()
                {
                    ContactId = o.ContactId,
                    FirstName = o.FirstName,
                    LastName = o.LastName,
                    Address = o.Address,
                    Email = o.Email,
                    City = o.City,
                    Country = o.Country,
                    State = o.State,
                    PostalCode = o.PostalCode,
                    PhoneNumber = o.PhoneNumber,
                }).FirstOrDefaultAsync();
                if(userByID != null)
                {
                    return new ResponseEntity<ContactViewEntity>
                    {
                        Result = userByID,
                        IsSuccess = true,
                        ResponseMessage = "Successfully",
                        StatusCode = 200,
                        StatusMessage = "Success"
                    };
                }
                return new ResponseEntity<ContactViewEntity>
                {
                    Result = null,
                    IsSuccess = false,
                    ResponseMessage = "data not found",
                    StatusCode = 400,
                    StatusMessage = "error"
                };
            }
            catch (Exception ex)
            {
                return new ResponseEntity<ContactViewEntity>()
                {
                    Result = null,
                    IsSuccess = false,
                    ResponseMessage = ex.Message,
                    StatusMessage = HttpStatusCode.InternalServerError.ToString()
                };
            }
        }
        #endregion
    }
}
