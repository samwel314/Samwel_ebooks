using System.ComponentModel.DataAnnotations;

namespace Samwel.Models
{
    public class OrderDetails 
    {
        public int Id { get; set; }
        [Required]
        public int OrderHeaderId { get; set; }    

        public OrderHeader OrderHeader { get; set; }
        [Required]
        public int ProductId { get; set; }  

        public Product Product { get; set; }
        
        public int Count { get; set; }      

        public double Price { get; set; }       


    }

}
