using System;
[Serializable]
public class ItemDetail
{
    // Use public fields for easy JSON deserialization and Inspector visibility
    public string Manufacturer;
    public string LogoURL;

    // Parameterless ctor required by some serializers
    public ItemDetail() { }

    public ItemDetail(string manufacturer, string logoURL)
    {
        Manufacturer = manufacturer;
        LogoURL = logoURL;
    }

    public override string ToString() => $"{Manufacturer} ({LogoURL})";
}
