using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PrestamoDispositivos.Views.Shared
{
    public class RegisterModel : PageModel
    {

        [BindProperty]
        public string Nombre { get; set; }

        [BindProperty]
        public string Correo { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }
        public void OnGet()
        {
        }
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            // Aquí puedes procesar el registro (guardar, validar, etc.)
            return RedirectToPage("/Home/Index");
        }
    }
}
