using MudBlazor;

namespace GoodHamburger.Web.Themes;

public static class CustomTheme
{
    public static MudTheme CustomMudTheme()
    {
        return new MudTheme()
        {
            PaletteLight = new PaletteLight()
            {
                Primary = Colors.Blue.Darken3,
                Secondary = Colors.Green.Accent4,
                AppbarBackground = Colors.Blue.Darken3,
            },
            PaletteDark = new PaletteDark()
            {
                Primary = Colors.Blue.Lighten1
            },

        };
    }
}