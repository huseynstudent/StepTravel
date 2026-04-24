using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StoreApp.Application.CQRS.TrainTickets.Command.Request;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.DAL.Context;
using StoreApp.Domain.Enums;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.TrainTickets.Handler.CommandHandler;

public class ReturnTrainTicketCommandHandler
    : IRequestHandler<ReturnTrainTicketCommandRequest, ResponseModel<ReturnTrainTicketCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly StoreAppDbContext _db;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ReturnTrainTicketCommandHandler> _logger;

    public ReturnTrainTicketCommandHandler(
        IUnitOfWork unitOfWork,
        StoreAppDbContext db,
        IConfiguration configuration,
        ILogger<ReturnTrainTicketCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _db = db;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ResponseModel<ReturnTrainTicketCommandResponse>> Handle(
        ReturnTrainTicketCommandRequest request,
        CancellationToken cancellationToken)
    {
        var ticket = await _unitOfWork.TrainTicketRepository.GetByIdAsync(request.Id);
        if (ticket == null || ticket.IsDeleted)
            return new ResponseModel<ReturnTrainTicketCommandResponse>(null);

        if (ticket.State != State.Booked || ticket.CustomerId == null)
            return new ResponseModel<ReturnTrainTicketCommandResponse>(null);

        if (ticket.DueDate < DateTime.UtcNow)
            return new ResponseModel<ReturnTrainTicketCommandResponse>(null);

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == ticket.CustomerId && !u.IsDeleted, cancellationToken);

        if (ticket.ChosenSeatId.HasValue)
        {
            var seat = await _unitOfWork.SeatRepository.GetByIdAsync(ticket.ChosenSeatId.Value);
            if (seat != null)
            {
                seat.IsOccupied = false;
                _unitOfWork.SeatRepository.Update(seat);
            }
        }
        double paidPrice = ticket.Price;
        double hoursLeft = (ticket.DueDate - DateTime.UtcNow).TotalHours;
        double refundRate = hoursLeft >= 24 ? 1.0 : 0.5;
        double refundAmt = Math.Round(paidPrice * refundRate, 2);

        ticket.CustomerId = null;
        ticket.ChosenSeatId = null;
        ticket.VariantId = null;
        ticket.State = State.Available;
        ticket.HasPet = false;
        ticket.HasChild = false;
        ticket.LuggageCount = 0;
        ticket.TotalLuggageKg = 0;
        ticket.Note = null;
        ticket.BroughtDate = null;
        ticket.Price = 0;
        ticket.UpdatedDate = DateTime.UtcNow;

        _unitOfWork.TrainTicketRepository.Update(ticket);
        await _unitOfWork.SaveChangesAsync();

        if (user != null && !string.IsNullOrEmpty(user.Email))
        {
            var fromLoc = await _unitOfWork.LocationRepository.GetByIdAsync(ticket.FromId);
            var toLoc = await _unitOfWork.LocationRepository.GetByIdAsync(ticket.ToId);
            var now = DateTime.UtcNow;

            _ = Task.Run(() =>
            {
                try
                {
                    string sender = _configuration["EmailSettings:SenderEmail"];
                    string appPass = _configuration["EmailSettings:AppPassword"];

                    if (string.IsNullOrEmpty(sender) || string.IsNullOrEmpty(appPass))
                    {
                        _logger.LogWarning("EmailSettings missing — refund email not sent.");
                        return;
                    }

                    string refundNote = refundRate < 1.0
                        ? "<p style='color:#f59e0b;font-size:13px;'>⚠️ Since your departure was less than 24 hours away, a 50% cancellation fee was applied.</p>"
                        : "<p style='color:#22c55e;font-size:13px;'>✓ Full refund applied — cancelled more than 24 hours before departure.</p>";

                    var emailService = new Service.Email();
                    emailService.Send(
                        sender,
                        appPass,
                        user.Email,
                        "🚂 Train Ticket Return Confirmation – StepTravel",
                        $@"
                        <div style='font-family:sans-serif;max-width:520px;'>
                          <div style='background:#0a0e14;padding:24px 28px;border-radius:12px 12px 0 0;'>
                            <h2 style='color:#fff;margin:0;font-size:20px;letter-spacing:-0.5px;'>
                              ↩ Ticket Returned
                            </h2>
                            <p style='color:rgba(255,255,255,0.45);margin:6px 0 0;font-size:13px;'>
                              Your train ticket has been successfully cancelled.
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
                                <td style='padding:8px 14px;color:#64748b;'>Train</td>
                                <td style='padding:8px 14px;font-weight:600;'>
                                  {ticket.TrainCompany} · #{ticket.TrainNumber}
                                </td>
                              </tr>
                              <tr style='background:#f1f5f9;'>
                                <td style='padding:8px 14px;color:#64748b;border-radius:6px 0 0 6px;'>Coach</td>
                                <td style='padding:8px 14px;font-weight:600;border-radius:0 6px 6px 0;'>
                                  {ticket.VagonNumber}
                                </td>
                              </tr>
                              <tr>
                                <td style='padding:8px 14px;color:#64748b;'>Departure</td>
                                <td style='padding:8px 14px;font-weight:600;'>
                                  {ticket.DueDate:dd MMM yyyy, HH:mm} UTC
                                </td>
                              </tr>
                              <tr style='background:#f1f5f9;'>
                                <td style='padding:8px 14px;color:#64748b;border-radius:6px 0 0 6px;'>Amount Paid</td>
                                <td style='padding:8px 14px;font-weight:600;border-radius:0 6px 6px 0;'>
                                  {paidPrice:F2} ₼
                                </td>
                              </tr>
                              <tr style='background:#0f172a;'>
                                <td style='padding:10px 14px;color:#94a3b8;border-radius:6px 0 0 6px;font-size:13px;'>
                                  Refund Amount
                                </td>
                                <td style='padding:10px 14px;font-weight:800;color:#4ade80;font-size:17px;border-radius:0 6px 6px 0;'>
                                  {refundAmt:F2} ₼
                                </td>
                              </tr>
                            </table>
                            {refundNote}
                            <p style='margin:16px 0 0;font-size:12px;color:#94a3b8;'>
                              Cancelled on {now:dd MMM yyyy, HH:mm} UTC · StepTravel
                            </p>
                          </div>
                        </div>"
                    );
                    _logger.LogInformation("Train refund email sent to {Email}", user.Email);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send train refund email to {Email}", user.Email);
                }
            });
        }

        return new ResponseModel<ReturnTrainTicketCommandResponse>(new ReturnTrainTicketCommandResponse
        {
            Id = ticket.Id,
            State = ticket.State.ToString(),
            Refund = refundAmt,
            Message = refundRate < 1.0
                ? $"Ticket returned. 50% cancellation fee applied. Refund: {refundAmt:F2} ₼"
                : $"Ticket returned successfully. Full refund: {refundAmt:F2} ₼"
        });
    }
}