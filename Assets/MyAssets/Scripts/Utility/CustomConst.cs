/// <summary>
/// C++ const 키워드를 흉내낸 구조체.
/// </summary>
/// <typeparam name="T"></typeparam>
public struct Const<T>
{
    public T Value { get; private set; }

    public Const(T value) : this()
    {
        this.Value = value;
    }
}