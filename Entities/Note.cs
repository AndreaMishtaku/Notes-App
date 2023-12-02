using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities;

public class Note
{
    [Key]
    public int Id { get; set; }

    public string Title { get; set; }


    public string Content { get; set; }


    public DateTime? CreatedAt { get; set; }


    public DateTime? UpdatedAt { get; set; }


    [ForeignKey("User")]
    public int UserId { get; set; }
    public User User { get; set; }

}
