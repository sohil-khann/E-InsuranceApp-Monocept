using EInsurance.Data;
using EInsurance.Domain.Entities;
using EInsurance.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Security.Claims;

namespace EInsurance.Controllers;

[Authorize]
public class PaymentController(ApplicationDbContext context) : Controller
{
    [Authorize(Roles = RoleNames.Customer)]
    [HttpGet]
    public async Task<IActionResult> PaymentHistory(CancellationToken cancellationToken)
    {
        var customerIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(customerIdValue, out var customerId))
            return Forbid();

        var payments = await context.Payments
            .Include(p => p.Policy)
                .ThenInclude(pol => pol.Scheme)
                    .ThenInclude(s => s.Plan)
            .Include(p => p.Policy.Customer)
            .Where(p => p.CustomerId == customerId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);

        return View(payments);
    }

    [Authorize(Roles = RoleNames.Customer)]
    [HttpGet("DownloadReceipt/{paymentId}")]
    public async Task<IActionResult> DownloadReceipt(int paymentId, CancellationToken cancellationToken)
    {
        var customerIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(customerIdValue, out var customerId))
            return Forbid();

        var payment = await context.Payments
            .Include(p => p.Policy)
                .ThenInclude(pol => pol.Scheme)
                    .ThenInclude(s => s.Plan)
            .Include(p => p.Policy.Customer)
                .ThenInclude(c => c.Agent)
            .FirstOrDefaultAsync(p => p.PaymentId == paymentId, cancellationToken);

        if (payment == null)
            return NotFound("Receipt not found");

        if (payment.CustomerId != customerId)
            return Forbid();

        var basePremium = payment.Amount / 1.18m;
        var tax = payment.Amount - basePremium;

        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(50);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Element(c => ComposeHeader(c, payment));
                page.Content().Element(c => ComposeContent(c, payment, basePremium, tax));
                page.Footer().Element(ComposeFooter);
            });
        });

        var pdfBytes = document.GeneratePdf();
        return File(pdfBytes, "application/pdf", $"Receipt_{payment.PaymentId}.pdf");
    }

    private void ComposeHeader(IContainer container, Payment payment)
    {
        container.Column(column =>
        {
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("EInsurance")
                        .FontSize(24).Bold().FontColor(Colors.Green.Darken1);
                    col.Item().Text("Insurance Payment Receipt")
                        .FontSize(14).FontColor(Colors.Grey.Darken1);
                });

                row.ConstantItem(150).Column(col =>
                {
                    col.Item().AlignRight().Text($"Receipt No: {payment.PaymentId}")
                        .Bold();
                    col.Item().AlignRight().Text($"Date: {payment.PaymentDate:dd MMM yyyy}")
                        .FontColor(Colors.Grey.Darken1);
                });
            });

            column.Item().PaddingVertical(20);
        });
    }

    private void ComposeContent(IContainer container, Payment payment, decimal basePremium, decimal tax)
    {
        container.Column(column =>
        {
            column.Spacing(15);

            column.Item().Element(c => ComposeSection(c, "Customer Details", new[]
            {
                $"Name: {payment.Policy.Customer.FullName}",
                $"Email: {payment.Policy.Customer.Email}"
            }));

            column.Item().Element(c => ComposeSection(c, "Policy Details", new[]
            {
                $"Policy ID: POL-{payment.Policy.PolicyId:D6}",
                $"Plan: {payment.Policy.Scheme.Plan.PlanName}",
                $"Scheme: {payment.Policy.Scheme.SchemeName}",
                $"Coverage Period: {payment.Policy.DateIssued:dd MMM yyyy} to {payment.Policy.PolicyLapseDate:dd MMM yyyy}"
            }));

            if (payment.Policy.Customer.Agent != null)
            {
                column.Item().Element(c => ComposeSection(c, "Agent Details", new[]
                {
                    $"Serviced by: {payment.Policy.Customer.Agent.FullName}",
                    $"Agent Email: {payment.Policy.Customer.Agent.Email}"
                }));
            }

            column.Item().PaddingTop(10).Element(c =>
            {
                c.Background(Colors.Grey.Lighten4).Padding(15).Column(col =>
                {
                    col.Item().Text("Payment Summary").Bold().FontSize(13);
                    col.Item().PaddingTop(10);
                    
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text("Base Premium");
                        row.ConstantItem(100).AlignRight().Text($"₹{basePremium:N2}");
                    });
                    
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text("GST (18%)");
                        row.ConstantItem(100).AlignRight().Text($"₹{tax:N2}");
                    });
                    
                    col.Item().PaddingTop(5).BorderTop(1).BorderColor(Colors.Grey.Medium).Row(row =>
                    {
                        row.RelativeItem().Text("Total Amount Paid").Bold();
                        row.ConstantItem(100).AlignRight().Text($"₹{payment.Amount:N2}").Bold().FontColor(Colors.Green.Darken1);
                    });
                });
            });

            column.Item().PaddingTop(30).AlignCenter().Text("Thank you for your payment!")
                .FontColor(Colors.Green.Darken1).Bold();
        });
    }

    private void ComposeSection(IContainer container, string title, string[] items)
    {
        container.Background(Colors.White).Padding(15).Border(1).BorderColor(Colors.Grey.Lighten2).Column(column =>
        {
            column.Item().Text(title).Bold().FontSize(12).FontColor(Colors.Grey.Darken2);
            column.Item().PaddingTop(8);
            foreach (var item in items)
            {
                column.Item().Text(item).FontSize(11);
            }
        });
    }

    private void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(text =>
        {
            text.Span("Page ");
            text.CurrentPageNumber();
            text.Span(" of ");
            text.TotalPages();
        });
    }
}