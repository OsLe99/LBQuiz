using MudBlazor;

namespace LBQuiz.Themes;

public static class LBQuizCustomTheme
{
    public static class LBQuizTheme
    {
        public static MudTheme Instance { get;  } = new MudTheme
        {
            PaletteLight = new PaletteLight()
            {
                Primary = "#FF8C00",
                Secondary = "#1A237E",
                Background = "#F5F5F5",
                Surface = "#FFFFFF",
                AppbarBackground = "#FF8C00",
                AppbarText = "#FFFFFF",
                DrawerBackground = "#FFFFFF",
                DrawerText = "#000000",
                Success = "#4CAF50",
                Info = "#2196F3",
                Warning = "#FFC107",
                Error = "#F44336"
            }
        };
    }
}