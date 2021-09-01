using System.ComponentModel.DataAnnotations;

namespace Shop.Models
{
    //[Table("Category")]
    public class Category
    {
        [Key]
        //[Column("Cat_ID")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        [MaxLength(60, ErrorMessage = "Este Campo deve conter entre 3 e 60 catacteres.")]
        [MinLength(3, ErrorMessage = "Este Campo deve conter entre 3 e 60 catacteres.")]
        public string Title { get; set; }

    }
}