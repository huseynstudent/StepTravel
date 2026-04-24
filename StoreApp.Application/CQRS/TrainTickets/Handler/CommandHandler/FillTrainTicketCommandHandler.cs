using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StoreApp.Application.CQRS.TrainTickets.Command.Request;
using StoreApp.Application.CQRS.TrainTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.DAL.Context;
using StoreApp.Domain.Enums;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.TrainTickets.Handler.CommandHandler;

public class FillTrainTicketCommandHandler : IRequestHandler<FillTrainTicketCommandRequest, ResponseModel<FillTrainTicketCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly StoreAppDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FillTrainTicketCommandHandler> _logger;

    public FillTrainTicketCommandHandler(
        IUnitOfWork unitOfWork,
        StoreAppDbContext db,
        IConfiguration configuration,
        ILogger<FillTrainTicketCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _db = db;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ResponseModel<FillTrainTicketCommandResponse>> Handle(
        FillTrainTicketCommandRequest request,
        CancellationToken cancellationToken)
    {
        // 1. Axtar?? n?tic?sind?n g?l?n bilet (qrupun n?may?nd?si)
        var trainTicket = await _unitOfWork.TrainTicketRepository.GetByIdAsync(request.Id);
        if (trainTicket == null)
            return new ResponseModel<FillTrainTicketCommandResponse>(null);

        // 2. ?stifad?ci
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);
        if (user == null)
            return new ResponseModel<FillTrainTicketCommandResponse>(null);

        // 3. Se?ilmi? oturacaq
        var seat = await _unitOfWork.SeatRepository.GetByIdAsync(request.ChosenSeatId);
        if (seat == null || seat.IsOccupied)
            return new ResponseModel<FillTrainTicketCommandResponse>(null);

        // 4. Oturacaqin ?sl sahibi olan bileti tap
        if (seat.TrainTicketId == null)
            return new ResponseModel<FillTrainTicketCommandResponse>(null);

        var targetTicket = await _db.TrainTickets.FirstOrDefaultAsync(
            t => t.Id == seat.TrainTicketId && !t.IsDeleted, cancellationToken);

        // 5. Oturacaqin eyni qatar qrupuna aid oldugunu yoxla
        if (targetTicket == null
            || targetTicket.TrainNumber != trainTicket.TrainNumber
            || targetTicket.VagonNumber != trainTicket.VagonNumber
            || targetTicket.DueDate != trainTicket.DueDate
            || targetTicket.FromId != trainTicket.FromId
            || targetTicket.ToId != trainTicket.ToId)
            return new ResponseModel<FillTrainTicketCommandResponse>(null);

        // 6. H?d?f bilet art?q al?nibsa r?dd et
        if (targetTicket.CustomerId != null)
            return new ResponseModel<FillTrainTicketCommandResponse>(null);

        // 7. Variant, from, to
        var variant = await _unitOfWork.VariantRepository.GetByIdAsync(seat.VariantId);
        var from = await _unitOfWork.LocationRepository.GetByIdAsync(targetTicket.FromId);
        var to = await _unitOfWork.LocationRepository.GetByIdAsync(targetTicket.ToId);
        if (from == null || to == null)
            return new ResponseModel<FillTrainTicketCommandResponse>(null);

        // 8. Bileti doldur
        targetTicket.CustomerId = user.Id;
        targetTicket.Customer = user;
        targetTicket.State = State.Booked;
        targetTicket.BroughtDate = DateTime.UtcNow;
        targetTicket.ChosenSeatId = seat.Id;
        targetTicket.VariantId = seat.VariantId;
        targetTicket.Variant = variant;
        targetTicket.HasPet = request.HasPet;
        targetTicket.HasChild = request.HasChild;
        targetTicket.LuggageCount = request.LuggageCount;
        targetTicket.TotalLuggageKg = request.TotalLuggageKg;
        targetTicket.Note = request.Note;

        seat.IsOccupied = true;

        // 9. Qiymeti hesabla
        var variantAddition = variant?.Price ?? 0.0;
        double basePrice = from.CountryId == to.CountryId
            ? 70 + variantAddition
            : Math.Abs(from.DistanceToken - to.DistanceToken) * 40 + variantAddition;

        double includedLuggage = variant?.AllowedLuggageKg ?? 15;
        double luggageFee = request.TotalLuggageKg > includedLuggage
            ? (request.TotalLuggageKg - includedLuggage) * 2
            : 0;

        bool isBirthday = user.Birthday.Month == DateTime.UtcNow.Month
                       && user.Birthday.Day == DateTime.UtcNow.Day;
        double manualDiscount = targetTicket.Discount is > 0 and <= 1 ? targetTicket.Discount : 1.0;
        double finalPrice = (basePrice + luggageFee) * (isBirthday ? 0.5 : 1.0) * manualDiscount;
        targetTicket.Price = finalPrice;

        await _unitOfWork.SaveChangesAsync();

        // 10. Tesдiq emaili arxa fonda gonder
        var now = targetTicket.BroughtDate ?? DateTime.UtcNow;
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
                if (request.HasPet) extras.Append("<tr><td style='padding:8px 14px;color:#64748b;'>Pet</td><td style='padding:8px 14px;font-weight:600;'>Yes</td></tr>");
                if (request.HasChild) extras.Append("<tr style='background:#f1f5f9;'><td style='padding:8px 14px;color:#64748b;border-radius:6px 0 0 6px;'>Child</td><td style='padding:8px 14px;font-weight:600;border-radius:0 6px 6px 0;'>Yes</td></tr>");
                if (request.TotalLuggageKg > 0)
                    extras.Append($"<tr><td style='padding:8px 14px;color:#64748b;'>Luggage</td><td style='padding:8px 14px;font-weight:600;'>{request.TotalLuggageKg} kg</td></tr>");
                if (!string.IsNullOrEmpty(request.Note))
                    extras.Append($"<tr style='background:#f1f5f9;'><td style='padding:8px 14px;color:#64748b;border-radius:6px 0 0 6px;'>Note</td><td style='padding:8px 14px;border-radius:0 6px 6px 0;'><em>{request.Note}</em></td></tr>");

                string discountRow = isBirthday
                    ? "<tr><td style='padding:8px 14px;color:#27ae60;'>🎂 Birthday Discount</td><td style='padding:8px 14px;font-weight:600;color:#27ae60;'>–50%</td></tr>"
                    : "";

                var emailService = new Service.Email();
                emailService.Send(
                    sender,
                    appPass,
                    user.Email,
                    "🚂 Train Ticket Booking Confirmation – StepTravel",
                    $@"
                    <div style='font-family:sans-serif;max-width:520px;'>
                      <div style='background:#0a0e14;padding:24px 28px;border-radius:12px 12px 0 0;'>
                        <h2 style='color:#fff;margin:0;font-size:20px;letter-spacing:-0.5px;'>
                          🚂 Booking Confirmed
                        </h2>
                        <p style='color:rgba(255,255,255,0.45);margin:6px 0 0;font-size:13px;'>
                          Your train ticket has been successfully booked.
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
                              {from.Name} → {to.Name}
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
                            <td style='padding:8px 14px;color:#64748b;'>Train</td>
                            <td style='padding:8px 14px;font-weight:600;'>{targetTicket.TrainCompany} · {targetTicket.TrainNumber}</td>
                          </tr>
                          <tr style='background:#f1f5f9;'>
                            <td style='padding:8px 14px;color:#64748b;border-radius:6px 0 0 6px;'>Vagon</td>
                            <td style='padding:8px 14px;font-weight:600;border-radius:0 6px 6px 0;'>
                              {targetTicket.VagonNumber}
                            </td>
                          </tr>
                          <tr>
                            <td style='padding:8px 14px;color:#64748b;'>Departure</td>
                            <td style='padding:8px 14px;font-weight:700;color:#dc2626;'>
                              {targetTicket.DueDate:dd MMM yyyy, HH:mm} UTC
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

                _logger.LogInformation("Train booking confirmation email sent to {Email}", user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send train booking confirmation email to {Email}", user.Email);
            }
        });

        return new ResponseModel<FillTrainTicketCommandResponse>(new FillTrainTicketCommandResponse
        {
            Id = targetTicket.Id,
            State = targetTicket.State,
            BroughtDate = (DateTime)targetTicket.BroughtDate,
            ChosenSeatId = (int)targetTicket.ChosenSeatId,
            VariantId = (int)targetTicket.VariantId,
            HasPet = targetTicket.HasPet,
            HasChild = targetTicket.HasChild,
            LuggageCount = targetTicket.LuggageCount,
            TotalLuggageKg = targetTicket.TotalLuggageKg,
            Discount = targetTicket.Discount,
            Price = targetTicket.Price,
            Note = targetTicket.Note
        });
    }
}