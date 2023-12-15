namespace SpaceBattle.Lib;
public class MyVector
{

    private readonly int[] _array;

    public MyVector(params int[] array)
    {
        _array.CopyTo(array, array.Length);
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
            var new_array = new int[a.Size()];
            for (var i = 0; i < a.Size(); i++)
            {
                new_array[i] = a[i] + b[i];
            }

            return new MyVector(new_array);
        }
    }

    public override int GetHashCode()
    {
        var reduceValues = _array.Aggregate((sum, next) => HashCode.Combine(sum, next));
        return reduceValues;
    }
}
