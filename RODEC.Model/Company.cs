using System;

namespace RODEC.Model
{
    public class Company
    {
        public string Code { get; set; }
        public string State { get; set; }

        public Company Clone()
        {
            return new Company() { Code =  this.Code, State =  this.State };
        }
    }
}
