public class Pair<T, U> {
    public T first { get; set; }
    public U second { get; set; }

    public Pair() {}

    public Pair(T t, U u) {
        this.first = t;
        this.second = u;
    }
};