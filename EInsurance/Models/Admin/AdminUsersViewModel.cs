using EInsurance.Models.Common;

namespace EInsurance.Models.Admin;

public class AdminUsersViewModel
{
    public PagedResult<UserListItemViewModel> Users { get; set; } = new();
    public string SearchTerm { get; set; } = string.Empty;
}