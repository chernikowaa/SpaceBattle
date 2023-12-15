namespace SpaceBattle.Lib;
public class MyVector
{

    private readonly int[] _array;

    public MyVector(params int[] array)
    {
        _array = (int[])array.Clone();
    }

    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }

        return obj.GetType() == typeof(MyVector) && Enumerable.SequenceEqual(((MyVector)obj)._array, _array);
    }

    public int this[int index] => _array[index];

    public int Size()
    {
        return _array.Length;
    }

    public static MyVector operator +(MyVector a, MyVector b)
    {
        if (a.Size() != b.Size())
        {
            throw new ArgumentException();
        }
        else
        {
            var new_array = a._array.Select((value,index) => value + b._array[index]).ToArray();
            return new MyVector(new_array);
        }
    }


    public override int GetHashCode()
    {
        var reduceValues = _array.Aggregate((sum, next) => HashCode.Combine(sum, next));
        return reduceValues;
    }

}
