using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StoreApp.Application.CQRS.PlaneTickets.Command.Request;
using StoreApp.Application.CQRS.PlaneTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.DAL.Context;
using StoreApp.Domain.Enums;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.PlaneTickets.Handler.CommandHandler;

public class FillPlaneTicketCommandHandler : IRequestHandler<FillPlaneTicketCommandRequest, ResponseModel<FillPlaneTicketCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly StoreAppDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FillPlaneTicketCommandHandler> _logger;

    public FillPlaneTicketCommandHandler(
        IUnitOfWork unitOfWork,
        StoreAppDbContext db,
        IConfiguration configuration,
        ILogger<FillPlaneTicketCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _db = db;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ResponseModel<FillPlaneTicketCommandResponse>> Handle(
        FillPlaneTicketCommandRequest request,
        CancellationToken cancellationToken)
    {
        // 1. Bileti tap
        var ticket = await _unitOfWork.PlaneTicketRepository.GetByIdAsync(request.Id);
        if (ticket == null || ticket.IsDeleted)
            return new ResponseModel<FillPlaneTicketCommandResponse>(null);

        // 2. Bilet artiq alinibsa redd et
        if (ticket.CustomerId != null || ticket.State == State.Used || ticket.State == State.Expired)
            return new ResponseModel<FillPlaneTicketCommandResponse>(null);

        // 3. Oturacagi tap ve yoxla
        var seat = await _unitOfWork.SeatRepository.GetByIdAsync(request.ChosenSeatId);
        if (seat == null || seat.IsOccupied)
            return new ResponseModel<FillPlaneTicketCommandResponse>(null);

        // 4. Istifadecini tap
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);
        if (user == null)
            return new ResponseModel<FillPlaneTicketCommandResponse>(null);

        // 5. Variant, from, to - email ve qiymeti hazirlamaq ucun
        var variant = await _unitOfWork.VariantRepository.GetByIdAsync(seat.VariantId);
        var fromLoc = await _unitOfWork.LocationRepository.GetByIdAsync(ticket.FromId);
        var toLoc = await _unitOfWork.LocationRepository.GetByIdAsync(ticket.ToId);

        var now = DateTime.UtcNow;

        // 6. Qiymeti hesabla
        double basePrice = Number(ticket.Price);   // admin terefinden qoyulmus qiymet
        double variantExtra = variant?.Price ?? 0;
        double includedLuggage = variant?.AllowedLuggageKg ?? 20;
        double luggageExtra = request.TotalLuggageKg > includedLuggage
                                    ? (request.TotalLuggageKg - includedLuggage) * 2
                                    : 0;
        bool isBirthday = user.Birthday.Month == now.Month && user.Birthday.Day == now.Day;
        double manualDiscount = ticket.Discount is > 0 and <= 1 ? ticket.Discount : 1.0;
        double finalPrice = (basePrice + variantExtra + luggageExtra)
                            * (isBirthday ? 0.5 : 1.0)
                            * manualDiscount;

        // 7. Oturacagi isgal et
        seat.IsOccupied = true;
        _unitOfWork.SeatRepository.Update(seat);

        // 8. Bileti doldur
        ticket.CustomerId = request.UserId;
        ticket.ChosenSeatId = request.ChosenSeatId;
        ticket.VariantId = seat.VariantId;          // seçilmiş oturacaqdan variant
        ticket.HasPet = request.HasPet;
        ticket.HasChild = request.HasChild;
        ticket.LuggageCount = request.LuggageCount;
        ticket.TotalLuggageKg = request.TotalLuggageKg;
        ticket.Note = request.Note;
        ticket.State = State.Booked;            // Booked — my-tickets-de gorunsun
        ticket.Price = finalPrice;
        ticket.BroughtDate = now;
        ticket.UpdatedDate = now;

        _unitOfWork.PlaneTicketRepository.Update(ticket);
        await _unitOfWork.SaveChangesAsync();

        // 9. Response
        var response = new FillPlaneTicketCommandResponse
        {
            Id = ticket.Id,
            CustomerFullName = $"{user.Name} {user.Surname}".Trim(),
            CustomerEmail = user.Email,
            State = ticket.State,
            DueDate = ticket.DueDate,
            BroughtDate = ticket.BroughtDate,
            ChosenSeatId = ticket.ChosenSeatId,
            VariantId = ticket.VariantId,
            HasPet = ticket.HasPet,
            HasChild = ticket.HasChild,
            LuggageCount = ticket.LuggageCount,
            TotalLuggageKg = ticket.TotalLuggageKg,
            Discount = ticket.Discount,
            Price = ticket.Price,
            Note = ticket.Note,
        };

        // 10. Tesдiq emaili arxa fonda gonder
        _ = Task.Run(() =>
        {
            try
            {
                string sender = _configuration["EmailSettings:SenderEmail"];
                string appPass = _configuration["EmailSettings:AppPassword"];

                if (string.IsNullOrEmpty(sender) || string.IsNullOrEmpty(appPass))
                {
                    _logger.LogWarning("EmailSettings is missing in appsettings.json");
                    return;
                }

                var extras = new System.Text.StringBuilder();
                if (request.HasPet) extras.Append("<tr><td style='padding:4px 12px 4px 0;color:#555;'>Pet</td><td><strong>Yes</strong></td></tr>");
                if (request.HasChild) extras.Append("<tr><td style='padding:4px 12px 4px 0;color:#555;'>Child</td><td><strong>Yes</strong></td></tr>");
                if (request.TotalLuggageKg > 0)
                    extras.Append($"<tr><td style='padding:4px 12px 4px 0;color:#555;'>Luggage</td><td><strong>{request.TotalLuggageKg} kg</strong></td></tr>");
                if (!string.IsNullOrEmpty(request.Note))
                    extras.Append($"<tr><td style='padding:4px 12px 4px 0;color:#555;'>Note</td><td><em>{request.Note}</em></td></tr>");

                string discountRow = isBirthday
                    ? "<tr><td style='padding:4px 12px 4px 0;color:#27ae60;'>🎂 Birthday Discount</td><td><strong style='color:#27ae60;'>–50%</strong></td></tr>"
                    : "";

                var emailService = new Service.Email();
                emailService.Send(
                    sender,
                    appPass,
                    user.Email,
                    "✈ Flight Booking Confirmation – StepTravel",
                    $@"
                    <div style='font-family:sans-serif;max-width:520px;'>
                      <div style='background:#0a0e14;padding:24px 28px;border-radius:12px 12px 0 0;'>
                        <h2 style='color:#fff;margin:0;font-size:20px;letter-spacing:-0.5px;'>
                          ✈ Booking Confirmed
                        </h2>
                        <p style='color:rgba(255,255,255,0.45);margin:6px 0 0;font-size:13px;'>
                          Your flight has been successfully booked.
                        </p>
                      </div>
                      <div style='background:#f8fafc;padding:24px 28px;border-radius:0 0 12px 12px;border:1px solid #e2e8f0;'>
                        <p style='margin:0 0 18px;color:#1e293b;'>
                          Dear <strong>{user.Name} {user.Surname}</strong>,
                        </p>
                        <table style='border-collapse:collapse;width:100%;font-size:14px;'>
                          <tr style='background:#f1f5f9;'>
                            <td style='padding:8px 14px;color:#64748b;border-radius:6px 0 0 6px;'>Route</td>
                            <td style='padding:8px 14px;font-weight:700;color:#0f172a;border-radius:0 6px 6px 0;'>
                              {fromLoc?.Name ?? "N/A"} → {toLoc?.Name ?? "N/A"}
                            </td>
                          </tr>
                          <tr>
                            <td style='padding:8px 14px;color:#64748b;'>Seat</td>
                            <td style='padding:8px 14px;font-weight:600;'>{seat.Name}</td>
                          </tr>
                          <tr style='background:#f1f5f9;'>
                            <td style='padding:8px 14px;color:#64748b;border-radius:6px 0 0 6px;'>Class</td>
                            <td style='padding:8px 14px;font-weight:600;border-radius:0 6px 6px 0;'>
                              {variant?.Name ?? "Standard"}
                            </td>
                          </tr>
                          <tr>
                            <td style='padding:8px 14px;color:#64748b;'>Airline</td>
                            <td style='padding:8px 14px;font-weight:600;'>{ticket.Airline ?? "—"}</td>
                          </tr>
                          <tr style='background:#f1f5f9;'>
                            <td style='padding:8px 14px;color:#64748b;border-radius:6px 0 0 6px;'>Flight</td>
                            <td style='padding:8px 14px;font-weight:600;border-radius:0 6px 6px 0;'>
                              {ticket.Plane ?? "—"}
                            </td>
                          </tr>
                          <tr>
                            <td style='padding:8px 14px;color:#64748b;'>Gate</td>
                            <td style='padding:8px 14px;font-weight:600;'>{ticket.Gate ?? "—"}</td>
                          </tr>
                          <tr style='background:#f1f5f9;'>
                            <td style='padding:8px 14px;color:#64748b;border-radius:6px 0 0 6px;'>Departure</td>
                            <td style='padding:8px 14px;font-weight:700;color:#dc2626;border-radius:0 6px 6px 0;'>
                              {ticket.DueDate:dd MMM yyyy, HH:mm} UTC
                            </td>
                          </tr>
                          {extras}
                          {discountRow}
                          <tr style='background:#0f172a;'>
                            <td style='padding:10px 14px;color:#94a3b8;border-radius:6px 0 0 6px;font-size:13px;'>
                              Total Paid
                            </td>
                            <td style='padding:10px 14px;font-weight:800;color:#fff;font-size:17px;border-radius:0 6px 6px 0;'>
                              {finalPrice:F2} ₼
                            </td>
                          </tr>
                        </table>
                        <p style='margin:20px 0 0;font-size:12px;color:#94a3b8;'>
                          Booked on {now:dd MMM yyyy, HH:mm} UTC · StepTravel
                        </p>
                      </div>
                    </div>"
                );

                _logger.LogInformation("Booking confirmation email sent to {Email}", user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send booking confirmation email to {Email}", user.Email);
            }
        });

        return new ResponseModel<FillPlaneTicketCommandResponse>(response);
    }

    private static double Number(double v) => double.IsNaN(v) || double.IsInfinity(v) ? 0 : v;
}