using StoreApp.Domain.BaseEntities;

namespace StoreApp.Domain.Entities;

public class TrainTicket: BaseTicket
{
    public string TrainCompany { get; set; }
    public string TrainNumber { get; set; }
    public int VagonNumber { get; set; }


}
