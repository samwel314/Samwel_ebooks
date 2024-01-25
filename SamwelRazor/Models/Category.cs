using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SamwelRazor.Models
{
	public class Category
	{
		public int CategoryId { get; set; }
		[Required(ErrorMessage = "Please Enter Category Name")]
		[Display(Name = "Category Name ")]
		[MaxLength(50)]
		public string Name { get; set; } = string.Empty;
		[Display(Name = "Category Display Order")]
		[Range(1, 100)]
		[Required(ErrorMessage = "Please Enter Category Display Order")]

		public int DisplayOrder { get; set; }
	}
}
