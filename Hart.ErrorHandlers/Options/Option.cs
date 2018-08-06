
namespace Hart.ErrorHandlers.Options
{

    public static class Option {

        public static Some<T> Some<T>(T value) => new Some<T>(value);

        public static None<T> None<T>() => new None<T>();

    }

}
