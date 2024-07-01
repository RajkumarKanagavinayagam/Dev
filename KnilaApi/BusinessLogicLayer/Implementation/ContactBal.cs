using KnilaApi.BusinessLogicLayer.Interface;
using KnilaApi.DataAccessLayer.DataObject.Entity;
using KnilaApi.DataAccessLayer.DataObject.ViewEntity;
using KnilaApi.DataAccessLayer.Interface;

namespace KnilaApi.BusinessLogicLayer.Implementation
{
    public class ContactBal : IContactBal
    {
        private IContactDal _contactDal;
        public ContactBal(IContactDal contactDal)
        {
            _contactDal = contactDal;
        }

        public async Task<ResponseEntity<ContactViewEntity>> GetAllContact()
        {
            return await _contactDal.GetAllContact();
        }

        public async Task<ResponseEntity<string>> PostEmployee(ContactViewEntity modeldata)
        {
            var postmodel = new ContactEntity()
            {
                ContactId = modeldata.ContactId,
                FirstName = modeldata.FirstName,
                LastName = modeldata.LastName,
                Password = modeldata.Password,
                Email = modeldata.Email,
                PhoneNumber = modeldata.PhoneNumber,
                Address = modeldata.Address,
                City = modeldata.City,
                State = modeldata.State,
                Country = modeldata.Country,
                PostalCode = modeldata.PostalCode,
                IsActive = true,
            };
            return await _contactDal.PostData(postmodel);
        }
        public async Task<ResponseEntity<bool>> DeleteContact(int ContactID)
        {
            return await _contactDal.DeleteContact(ContactID);
        }

        public async Task<ResponseEntity<string>> Authentication(SigninRequestModel signinModel)
        {
            return await _contactDal.Authentication(signinModel);
        }

        public async Task<ResponseEntity<ContactViewEntity>> GetAllContact(string? SortColumn = null, string? SortDirection = null, string? SearchKeyword = null, string? filters = null)
        {
            return await _contactDal.GetAllContact( SortColumn, SortDirection, SearchKeyword, filters);
        }

        public async Task<ResponseEntity<ContactViewEntity>> GetByContactID(int ContactID)
        {
            return await _contactDal.GetByContactID(ContactID);
        }
    }
}
