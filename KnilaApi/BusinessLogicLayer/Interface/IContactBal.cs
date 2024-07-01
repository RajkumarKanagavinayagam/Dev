using KnilaApi.DataAccessLayer.DataObject.ViewEntity;

namespace KnilaApi.BusinessLogicLayer.Interface
{
    public interface IContactBal
    {
        Task<ResponseEntity<string>> PostEmployee(ContactViewEntity modeldata);
        Task<ResponseEntity<ContactViewEntity>> GetAllContact();
        Task<ResponseEntity<bool>> DeleteContact(int ContactID);
        Task<ResponseEntity<string>> Authentication(SigninRequestModel signinModel);
        Task<ResponseEntity<ContactViewEntity>> GetAllContact(string? SortColumn = null, string? SortDirection = null, string? SearchKeyword = null, string? filters = null);
        Task<ResponseEntity<ContactViewEntity>> GetByContactID(int ContactID);
    }
}
