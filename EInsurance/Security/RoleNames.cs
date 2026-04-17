namespace EInsurance.Security;

public static class RoleNames
{
    public const string Admin = "Admin";
    public const string Employee = "Employee";
    public const string InsuranceAgent = "InsuranceAgent";
    public const string Customer = "Customer";
    public const string AdminOrEmployee = Admin + "," + Employee;
}
