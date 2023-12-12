namespace SpaceBattle.Lib;
public class MyVector {
    private int[] array;
    private int size;

    public MyVector(params int[] array) 
    {
        size = array.Length;
        this.array = new int[size];
        for (int i = 0; i < size; i++) 
        {
            this.array[i] = array[i];
        }
    }
    
     public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            return obj.GetType() == typeof(MyVector) && Enumerable.SequenceEqual(((Vector)obj)._array, _array);
        }

    public int this[int index] 
    {
        get 
        {
            return array[index];
        }
    }

    public int Size() 
    {
        return size;
    }

    public static MyVector operator+(MyVector a, MyVector b) 
    {
        if (a.Size() != b.Size()) 
        {
            throw new ArgumentException();
        } 
        else 
        {
            var new_array = new int[a.Size()];
            for (int i = 0; i < a.Size(); i++) {
                new_array[i] = a[i] + b[i];
            }
            return new MyVector(new_array);
        }
    }

}
