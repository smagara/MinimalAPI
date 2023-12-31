using System.ComponentModel.DataAnnotations;

namespace DishesAPI.Models;

public class DishUpdateDto
{
        
    [Required]
    [StringLength(100, MinimumLength=3)]
    public required string Name { get; set; }
}