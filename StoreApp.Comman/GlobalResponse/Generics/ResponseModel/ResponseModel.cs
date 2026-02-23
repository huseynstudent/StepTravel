namespace StoreApp.Comman.GlobalResponse.Generics.ResponseModel;

public class ResponseModel<T>
{
    public T Data { get; set; }

    public ResponseModel(T data)
    {
        Data = data;
    }
}
