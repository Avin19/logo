using System.Collections.Generic;
using System.Linq;

public static class CategoryToItemMapper
{
    public static List<ItemDetail> Map(CategorySO category)
    {
        var list = new List<ItemDetail>();
        if (category == null || category.logos == null) return list;

        foreach (var le in category.logos)
        {
            var it = new ItemDetail
            {
                Manufacturer = le.name ?? string.Empty,
                LogoURL = le.image
            };
            list.Add(it);
        }

        return list;
    }

    public static List<ItemDetail> Map(IEnumerable<CategorySO> categories)
    {
        return categories.SelectMany(c => Map(c)).ToList();
    }
}
