using System.Collections.Generic;

namespace Models.Tables
{
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string SurName { get; set; } = string.Empty;
        public int Natoil { get; set; }
        public string UniqueParam {  get; set; } = string.Empty;

        public List<Socio> Soci { get; set; } = [];
        public Operatore? Operatore { get; set; }

    }
}
