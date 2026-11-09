
using System;

namespace SzallodaAlkalmazas
{
    public class Guest
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Phone { get; private set; }
        public string Email { get; private set; }

        public Guest(int id, string name, string phone, string email)
        {
            if (id <= 0)
                throw new ArgumentException("Hibás vendégazonosító.");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("A név kötelező.");

            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("A telefonszám kötelező.");

            if (string.IsNullOrWhiteSpace(email) ||
                !email.Contains("@") || !email.Contains("."))
                throw new ArgumentException("Hibás e-mail-cím.");

            Id = id;
            Name = name;
            Phone = phone;
            Email = email;
        }

        public override string ToString()
        {
            return $"{Id} - {Name} | {Phone} | {Email}";
        }
    }
}
