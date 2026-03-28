using Shadow_Protocol;
using Shadow_Protocol.Systems;

class Program
{
    static void Main()
    {
        MenuSystems menuSystems = new MenuSystems();

        menuSystems.MainMenu();
        string choice = menuSystems.MainMenu().ToString();
        switch (choice)
        {
            case "Nova hra":
                break;

            case "Nacist hru":
                break;

            case "Tutorial":
                break;
        }
    }
}