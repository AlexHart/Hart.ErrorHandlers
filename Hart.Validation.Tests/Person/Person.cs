using System;

namespace Hart.Validation.Tests
{
    public class Person
    {
        public Person(int id, string name, DateTime birthday)
        {
            Id = id;
            Name = name;
            Birthday = birthday;
        }

        public int Id { get; }
        public string Name { get; }
        public DateTime Birthday { get; }
    }

}
