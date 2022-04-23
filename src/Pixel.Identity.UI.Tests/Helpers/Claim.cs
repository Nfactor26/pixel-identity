namespace Pixel.Identity.UI.Tests.Helpers;

internal class Claim
{     
    public string Type { get; set; }
 
    public string Value { get; set; }
   
    public bool IncludeInAccessToken { get; set; } = true;
 
    public bool IncludeInIdentityToken { get; set; } = false;

    public Claim(string type, string value)
    {
        this.Type = type;
        this.Value = value;
    }
}
