﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HardcoreHistoryBlog.Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace HardcoreHistoryBlog.Models
{
    public class AccountVMs
    {

        public class RegisterViewModel
        {
            [Key]
            [BindProperty]
            public InputModel Input { get; set; }

            public string ReturnUrl { get; set; }

            public class InputModel
            {
                
                [Required]
                [DisplayName("First Name")]
                public string FirstName { get; set; }

                [Required]
                [DisplayName("Second Name")]
                public string LastName { get; set; }

                [Required]
                [EmailAddress]
                [Display(Name = "Email")]
                public string Email { get; set; }

                [Required]
                [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
                [DataType(DataType.Password)]
                [Display(Name = "Password")]
                public string Password { get; set; }

                [DataType(DataType.Password)]
                [Display(Name = "Confirm password")]
                [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
                public string ConfirmPassword { get; set; }
            }
        }
    }
}