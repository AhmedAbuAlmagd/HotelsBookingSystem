using HotelsBookingSystem.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class HotelImage
{
    [Key]
    public int ImageId { get; set; }

    [Required]
    [RegularExpression(@"^.*\.(jpg|jpeg|png)$", ErrorMessage = "Only .jpg, .jpeg, .png files are allowed.")]
    public string ImageUrl { get; set; }

    public string? Caption { get; set; }

    public bool IsPrimary { get; set; }

    [ForeignKey("Hotel")]
    public int HotelId { get; set; }
    public virtual Hotel Hotel { get; set; }

}
