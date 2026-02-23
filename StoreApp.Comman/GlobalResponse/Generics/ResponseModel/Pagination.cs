namespace StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

public class Pagination<T>
{
    public List<T> Data { get; set; }
    public int TotalDataCount { get; set; }
    public int Page { get; set; }
    public int Size { get; set; }

    public Pagination(List<T> datas, int totalDataCount, int page, int size)
    {
        Data = datas;
        TotalDataCount = totalDataCount;
        Page = page;
        Size = size;
    }
    public Pagination()
    {
        Data = new List<T>();
        TotalDataCount = 0;
    }
}
