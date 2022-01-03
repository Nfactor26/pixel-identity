namespace Pixel.Identity.Shared.ViewModels
{
    public class ClaimViewModel
    {        
        public string Type { get; set; }
       
        public string Value { get; set; }     
       
        public ClaimViewModel()
        {

        }

        public ClaimViewModel(string type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}
