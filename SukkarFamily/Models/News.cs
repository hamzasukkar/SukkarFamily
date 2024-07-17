using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SukkarFamily.Models
{
    public class News
    {
        public int ID { get; set; }
        public string Title { get; set; }
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }
        public string ImgUrl { get; set; }
    }
}
