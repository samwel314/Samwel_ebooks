using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Samwel.Models
{
    public class ShoppingCart
    {
        [Key]
        public int Id { get; set; } 
        public int ProductId { get; set; }
        [ValidateNever]
        public Product Product { get; set; }
        [Range(1 , 1000 , ErrorMessage ="Please Enter Value Between 1 : 100")]
        public int Count { get; set; }  

        public string AppUserId { get; set; }   
        public AppUser AppUser { get; set; }
        [NotMapped]
        public double Price { get; set; }
    }
}
