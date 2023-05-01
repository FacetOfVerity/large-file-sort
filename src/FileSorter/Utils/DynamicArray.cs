namespace FileSorter.Utils;

public class DynamicArray<T>
{
    public T[] Buffer { get; private set; }
    public int Length { get; private set; }

    public DynamicArray(int initialCapacity = 4)
    {
        Buffer = new T[initialCapacity];
        Length = 0;
    }

    public void Add(T item)
    {
        ExtendBuffer();
        Length++;
        Buffer[Length - 1] = item;
    }

    public bool Any() => Length > 0;

    public void Reset() => Length = 0;

    private void ExtendBuffer()
    {
        if (Buffer.Length <= Length)
        {
            var newBuffer = new T[Buffer.Length * 2];
            Array.Copy(Buffer, newBuffer, Buffer.Length);
            Buffer = newBuffer;
        }
    }
}