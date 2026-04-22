namespace EInsurance.Security;

public static class ValidationConstants
{
    public static class Email
    {
        public const int MaxLength = 100;
        public const string RegexPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
    }

    public static class Password
    {
        public const int MinLength = 8;
        public const int MaxLength = 255;
        public const string SpecialCharacters = @"!@#$%^&*()_+-=[]{}|;':'',./<>?";
    }

    public static class Username
    {
        public const int MaxLength = 50;
        public const int MinLength = 3;
    }

    public static class FullName
    {
        public const int MaxLength = 100;
        public const int MinLength = 2;
    }

    public static class Phone
    {
        public const int MinLength = 10;
        public const int MaxLength = 15;
        public const string RegexPattern = @"^\d{10,15}$";
    }

    public static class Currency
    {
        public const int Precision = 10;
        public const int Scale = 2;
    }

    public static class Age
    {
        public const int Minimum = 18;
    }

    public static class Role
    {
        public static readonly string[] ValidRoles = { RoleNames.Admin, RoleNames.Employee, RoleNames.Customer, RoleNames.InsuranceAgent };
    }
}