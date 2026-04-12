using MediatR;
using StoreApp.Application.CQRS.PlaneTickets.Command.Request;
using StoreApp.Application.CQRS.PlaneTickets.Command.Response;
using StoreApp.Comman.GlobalResponse.Generics.ResponseModel;
using StoreApp.Domain.Entities;
using StoreApp.Domain.Enums;
using StoreApp.Repository.Comman;

namespace StoreApp.Application.CQRS.PlaneTickets.Handler.CommandHandler;

public class CreatePlaneTicketCommandHandler : IRequestHandler<CreatePlaneTicketCommandRequest, ResponseModel<CreatePlaneTicketCommandResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreatePlaneTicketCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseModel<CreatePlaneTicketCommandResponse>> Handle(
        CreatePlaneTicketCommandRequest request, CancellationToken cancellationToken)
    {
        // 1. Lokasiyaların mövcudluğunu yoxla (Navigation property set etmirik, sadəcə yoxlayırıq)
        var from = await _unitOfWork.LocationRepository.GetByIdAsync(request.FromId);
        if (from == null) return new ResponseModel<CreatePlaneTicketCommandResponse>(null);

        var to = await _unitOfWork.LocationRepository.GetByIdAsync(request.ToId);
        if (to == null) return new ResponseModel<CreatePlaneTicketCommandResponse>(null);

        var columns = "ABCDEFGHIJK";
        var createdTickets = new List<PlaneTicket>();

        // --- MƏRHƏLƏ 1: Biletlərin (PlaneTicket) yaradılması ---
        foreach (var group in request.SeatGroups)
        {
            var variant = await _unitOfWork.VariantRepository.GetByIdAsync(group.VariantId);
            var variantPrice = variant?.Price ?? 0;

            for (int row = 1; row <= group.RowCount; row++)
            {
                for (int col = 0; col < group.SeatsPerRow; col++)
                {
                    var ticket = new PlaneTicket
                    {
                        Airline = request.Airline,
                        Gate = request.Gate,
                        Plane = request.Plane,
                        Meal = request.Meal,
                        LuggageKg = request.LuggageKg,
                        DueDate = request.DueDate,
                        FromId = request.FromId, // Yalnız ID kifayətdir
                        ToId = request.ToId,     // Yalnız ID kifayətdir
                        // From = from,  <-- Xətaya səbəb olan sətir budur, sildik
                        // To = to,      <-- Xətaya səbəb olan sətir budur, sildik
                        State = State.Pending,
                        Price = variantPrice
                    };

                    await _unitOfWork.PlaneTicketRepository.AddAsync(ticket);
                    createdTickets.Add(ticket);
                }
            }
        }

        // Biletləri yadda saxlayırıq ki, ID-ləri yaransın (Seats üçün lazımdır)
        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Xətanı daxildən görmək üçün (InnerException vacibdir)
            throw new Exception($"Biletləri yadda saxlayarkən xəta: {ex.InnerException?.Message ?? ex.Message}");
        }

        // --- MƏRHƏLƏ 2: Oturacaqların (Seat) yaradılması ---
        int ticketIndex = 0;
        int currentTotalRowOffset = 0;

        foreach (var group in request.SeatGroups)
        {
            for (int row = 1; row <= group.RowCount; row++)
            {
                for (int col = 0; col < group.SeatsPerRow; col++)
                {
                    // Oturacaq nömrəsini hesabla (məsələn: 1A, 2B və s.)
                    var seatName = $"{row + currentTotalRowOffset}{columns[col]}";

                    if (ticketIndex < createdTickets.Count)
                    {
                        var linkedTicket = createdTickets[ticketIndex++];

                        await _unitOfWork.SeatRepository.AddAsync(new Seat
                        {
                            Name = seatName,
                            IsOccupied = false,
                            VariantId = group.VariantId,
                            PlaneTicketId = linkedTicket.Id
                        });
                    }
                }
            }
            // Hər qrup bitdikdən sonra row sayını offsetə əlavə et ki, növbəti qrup fərqli sıralardan başlasın
            currentTotalRowOffset += group.RowCount;
        }

        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Oturacaqları yadda saxlayarkən xəta: {ex.InnerException?.Message ?? ex.Message}");
        }

        var first = createdTickets.First();
        return new ResponseModel<CreatePlaneTicketCommandResponse>(
            new CreatePlaneTicketCommandResponse
            {
                Id = first.Id,
                Airline = first.Airline,
                Gate = first.Gate,
                Plane = first.Plane,
                Meal = first.Meal,
                LuggageKg = first.LuggageKg,
                DueDate = first.DueDate,
                FromId = first.FromId,
                ToId = first.ToId,
                State = first.State.ToString(),
                TotalTicketsCreated = createdTickets.Count
            });
    }
}