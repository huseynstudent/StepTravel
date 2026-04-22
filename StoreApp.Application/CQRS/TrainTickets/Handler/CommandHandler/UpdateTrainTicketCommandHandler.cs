using MediatR;
using Microsoft.EntityFrameworkCore;
using StoreApp.Application.CQRS.TrainTickets.Command.Request;
using StoreApp.Application.CQRS.TrainTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.DAL.Context;
using StoreApp.Domain.Enums;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.TrainTickets.Handler.CommandHandler;

// Tək bilet yeniləmə
public class UpdateTrainTicketCommandHandler : IRequestHandler<UpdateTrainTicketCommandRequest, ResponseModel<UpdateTrainTicketCommandResponse>>
{
    private readonly StoreAppDbContext _db;

    public UpdateTrainTicketCommandHandler(StoreAppDbContext db)
    {
        _db = db;
    }

    public async Task<ResponseModel<UpdateTrainTicketCommandResponse>> Handle(UpdateTrainTicketCommandRequest request, CancellationToken cancellationToken)
    {
        // Hədəf bileti tap
        var trainTicket = await _db.TrainTickets
            .FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, cancellationToken);

        if (trainTicket == null)
            return new ResponseModel<UpdateTrainTicketCommandResponse>(null);

        if (trainTicket.State == State.Used || trainTicket.State == State.Expired || trainTicket.State == State.Missed)
            return new ResponseModel<UpdateTrainTicketCommandResponse>(null);

        // ✅ FIX: Eyni qrupa aid BÜTÜN biletləri tap
        // (eyni TrainCompany + TrainNumber + VagonNumber + DueDate.Date + From + To)
        var groupTickets = await _db.TrainTickets
            .Where(t =>
                t.TrainCompany == trainTicket.TrainCompany &&
                t.TrainNumber == trainTicket.TrainNumber &&
                t.VagonNumber == trainTicket.VagonNumber &&
                t.DueDate.Date == trainTicket.DueDate.Date &&
                t.FromId == trainTicket.FromId &&
                t.ToId == trainTicket.ToId &&
                !t.IsDeleted)
            .ToListAsync(cancellationToken);

        var invalidStates = new[] { State.Used, State.Expired, State.Missed };

        foreach (var ticket in groupTickets)
        {
            // Used/Expired/Missed biletlərə toxunma
            if (invalidStates.Contains(ticket.State))
                continue;

            // Cancel əməliyyatı — oturacağı azad et
            if (request.State == State.Canceled && ticket.State != State.Canceled)
            {
                if (ticket.ChosenSeatId.HasValue)
                {
                    var seat = await _db.Seats.FirstOrDefaultAsync(s => s.Id == ticket.ChosenSeatId.Value, cancellationToken);
                    if (seat != null)
                        seat.IsOccupied = false;
                }
                ticket.CustomerId = null;
            }

            // Booked biletdə state dəyişmə (yalnız tarix/şirkət/nömrə update et)
            if (ticket.State == State.Booked && request.State != State.Canceled)
            {
                ticket.TrainCompany = request.TrainCompany;
                ticket.TrainNumber = request.TrainNumber;
                ticket.VagonNumber = request.VagonNumber;
                if (request.DueDate.HasValue)
                    ticket.DueDate = request.DueDate.Value;
                ticket.UpdatedDate = DateTime.UtcNow;
                continue;
            }

            // Normal update
            ticket.TrainCompany = request.TrainCompany;
            ticket.TrainNumber = request.TrainNumber;
            ticket.VagonNumber = request.VagonNumber;
            ticket.State = request.State;
            if (request.DueDate.HasValue)
                ticket.DueDate = request.DueDate.Value;
            ticket.UpdatedDate = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(cancellationToken);

        return new ResponseModel<UpdateTrainTicketCommandResponse>(new UpdateTrainTicketCommandResponse
        {
            Id = trainTicket.Id,
            TrainCompany = trainTicket.TrainCompany,
            TrainNumber = trainTicket.TrainNumber,
            VagonNumber = trainTicket.VagonNumber,
            State = trainTicket.State,
            DueDate = trainTicket.DueDate
        });
    }
}

// Qrup yeniləmə (dəyişmədi)
public class UpdateTrainTicketGroupCommandHandler
    : IRequestHandler<UpdateTrainTicketGroupCommandRequest, ResponseModel<List<UpdateTrainTicketCommandResponse>>>
{
    private readonly StoreAppDbContext _db;

    public UpdateTrainTicketGroupCommandHandler(StoreAppDbContext db)
    {
        _db = db;
    }

    public async Task<ResponseModel<List<UpdateTrainTicketCommandResponse>>> Handle(
        UpdateTrainTicketGroupCommandRequest request, CancellationToken cancellationToken)
    {
        var groupTickets = await _db.TrainTickets
            .Where(t =>
                t.TrainCompany == request.TrainCompany &&
                t.TrainNumber == request.TrainNumber &&
                t.VagonNumber == request.VagonNumber &&
                t.DueDate.Date == request.DueDate.Date &&
                t.FromId == request.FromId &&
                t.ToId == request.ToId &&
                !t.IsDeleted)
            .ToListAsync(cancellationToken);

        if (!groupTickets.Any())
            return new ResponseModel<List<UpdateTrainTicketCommandResponse>>(null);

        var invalidStates = new[] { State.Used, State.Expired, State.Missed };

        foreach (var ticket in groupTickets)
        {
            if (invalidStates.Contains(ticket.State))
                continue;

            if (ticket.State == State.Booked && request.NewState != State.Canceled)
            {
                ticket.TrainCompany = request.NewTrainCompany;
                ticket.TrainNumber = request.NewTrainNumber;
                ticket.VagonNumber = request.NewVagonNumber;
                if (request.NewDueDate.HasValue)
                    ticket.DueDate = request.NewDueDate.Value;
                ticket.UpdatedDate = DateTime.UtcNow;
                continue;
            }

            if (request.NewState == State.Canceled && ticket.State != State.Canceled)
            {
                if (ticket.ChosenSeatId.HasValue)
                {
                    var seat = await _db.Seats.FirstOrDefaultAsync(s => s.Id == ticket.ChosenSeatId.Value, cancellationToken);
                    if (seat != null)
                        seat.IsOccupied = false;
                }
                ticket.CustomerId = null;
            }

            ticket.TrainCompany = request.NewTrainCompany;
            ticket.TrainNumber = request.NewTrainNumber;
            ticket.VagonNumber = request.NewVagonNumber;
            ticket.State = request.NewState;
            if (request.NewDueDate.HasValue)
                ticket.DueDate = request.NewDueDate.Value;
            ticket.UpdatedDate = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(cancellationToken);

        return new ResponseModel<List<UpdateTrainTicketCommandResponse>>(
            groupTickets.Select(t => new UpdateTrainTicketCommandResponse
            {
                Id = t.Id,
                TrainCompany = t.TrainCompany,
                TrainNumber = t.TrainNumber,
                VagonNumber = t.VagonNumber,
                State = t.State,
                DueDate = t.DueDate
            }).ToList());
    }
}