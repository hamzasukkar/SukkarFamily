using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SukkarFamily.Models
{
    public class Persone
    {
        [Key]
        public int Id { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public string  image { get; set; }
        public Persone Parent { get; set; }
        public int Generation { get; set; }
        public List<Persone> children { get; set; }


    }
}
