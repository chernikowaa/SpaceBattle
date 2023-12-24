namespace SpaceBattle.Lib;
public interface IQueue<T>
{
    public void Put(T obj);
    public T Take();
}
