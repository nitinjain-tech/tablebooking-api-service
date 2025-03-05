﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantTableBookingApp.Core;

public partial class RestaurantBranch
{
    [Key]
    public int Id { get; set; }

    public int RestaurantId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(200)]
    public string Address { get; set; } = null!;

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    [InverseProperty("RestaurantBranch")]
    public virtual ICollection<DiningTable> DiningTables { get; set; } = new List<DiningTable>();

    [ForeignKey("RestaurantId")]
    [InverseProperty("RestaurantBranches")]
    public virtual Restaurant Restaurant { get; set; } = null!;
}
