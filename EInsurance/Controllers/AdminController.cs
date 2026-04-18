using EInsurance.Data;
using EInsurance.Domain.Entities;
using EInsurance.Models.Admin;
using EInsurance.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EInsurance.Controllers;

[Authorize(Roles = RoleNames.Admin)]
public class AdminController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> ManageUsers()
    {
        var admins = await context.Admins.AsNoTracking().ToListAsync();
        var employees = await context.Employees.AsNoTracking().ToListAsync();
        var agents = await context.InsuranceAgents.AsNoTracking().ToListAsync();
        var customers = await context.Customers.AsNoTracking().ToListAsync();

        var users = new List<UserListItemViewModel>();

        users.AddRange(admins.Select(a => new UserListItemViewModel
        {
            Id = $"admin_{a.AdminId}",
            FullName = a.FullName,
            Email = a.Email,
            Username = a.Username,
            Role = "SYSTEM ADMIN",
            Status = "Active",
            CreatedAt = a.CreatedAt
        }));

        users.AddRange(employees.Select(e => new UserListItemViewModel
        {
            Id = $"employee_{e.EmployeeId}",
            FullName = e.FullName,
            Email = e.Email,
            Username = e.Username,
            Role = e.Role.ToUpper(),
            Status = "Active",
            CreatedAt = e.CreatedAt
        }));

        users.AddRange(agents.Select(a => new UserListItemViewModel
        {
            Id = $"agent_{a.AgentId}",
            FullName = a.FullName,
            Email = a.Email,
            Username = a.Username,
            Role = "AGENT",
            Status = "Active",
            CreatedAt = a.CreatedAt
        }));

        users.AddRange(customers.Select(c => new UserListItemViewModel
        {
            Id = $"customer_{c.CustomerId}",
            FullName = c.FullName,
            Email = c.Email,
            Username = c.Email, // Customer doesn't have Username, using Email
            Role = "CUSTOMER",
            Status = "Active",
            CreatedAt = c.CreatedAt
        }));

        var viewModel = new ManageUsersViewModel
        {
            Users = users.OrderByDescending(u => u.CreatedAt).ToList()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(string id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var parts = id.Split('_');
        if (parts.Length != 2) return BadRequest();

        var type = parts[0];
        var userId = int.Parse(parts[1]);

        switch (type)
        {
            case "admin":
                var admin = await context.Admins.FindAsync(userId);
                if (admin != null) context.Admins.Remove(admin);
                break;
            case "employee":
                var employee = await context.Employees.FindAsync(userId);
                if (employee != null) context.Employees.Remove(employee);
                break;
            case "agent":
                var agent = await context.InsuranceAgents.FindAsync(userId);
                if (agent != null) context.InsuranceAgents.Remove(agent);
                break;
            case "customer":
                var customer = await context.Customers.FindAsync(userId);
                if (customer != null) context.Customers.Remove(customer);
                break;
            default:
                return BadRequest();
        }

        await context.SaveChangesAsync();
        return RedirectToAction(nameof(ManageUsers));
    }

    [HttpGet]
    public async Task<IActionResult> EditUser(string id)
    {
        if (string.IsNullOrEmpty(id)) return NotFound();

        var parts = id.Split('_');
        if (parts.Length != 2) return BadRequest();

        var type = parts[0];
        var userId = int.Parse(parts[1]);

        EditUserViewModel model;

        switch (type)
        {
            case "admin":
                var admin = await context.Admins.FindAsync(userId);
                if (admin == null) return NotFound();
                model = new EditUserViewModel { Id = id, FullName = admin.FullName, Email = admin.Email, Role = "SYSTEM ADMIN", Status = "Active" };
                break;
            case "employee":
                var employee = await context.Employees.FindAsync(userId);
                if (employee == null) return NotFound();
                model = new EditUserViewModel { Id = id, FullName = employee.FullName, Email = employee.Email, Role = employee.Role, Status = "Active" };
                break;
            case "agent":
                var agent = await context.InsuranceAgents.FindAsync(userId);
                if (agent == null) return NotFound();
                model = new EditUserViewModel { Id = id, FullName = agent.FullName, Email = agent.Email, Role = "AGENT", Status = "Active" };
                break;
            case "customer":
                var customer = await context.Customers.FindAsync(userId);
                if (customer == null) return NotFound();
                model = new EditUserViewModel { Id = id, FullName = customer.FullName, Email = customer.Email, Role = "CUSTOMER", Status = "Active" };
                break;
            default:
                return BadRequest();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(EditUserViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var parts = model.Id.Split('_');
        var type = parts[0];
        var userId = int.Parse(parts[1]);

        switch (type)
        {
            case "admin":
                var admin = await context.Admins.FindAsync(userId);
                if (admin != null) { admin.FullName = model.FullName; admin.Email = model.Email; }
                break;
            case "employee":
                var employee = await context.Employees.FindAsync(userId);
                if (employee != null) { employee.FullName = model.FullName; employee.Email = model.Email; employee.Role = model.Role; }
                break;
            case "agent":
                var agent = await context.InsuranceAgents.FindAsync(userId);
                if (agent != null) { agent.FullName = model.FullName; agent.Email = model.Email; }
                break;
            case "customer":
                var customer = await context.Customers.FindAsync(userId);
                if (customer != null) { customer.FullName = model.FullName; customer.Email = model.Email; }
                break;
        }

        await context.SaveChangesAsync();
        return RedirectToAction(nameof(ManageUsers));
    }
}
