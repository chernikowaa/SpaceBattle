using Hwdtech;

namespace SpaceBattle.Lib;

public class CreateShipsCMD : ICommand
{
    public void Execute()
    {
        var allShips = IoC.Resolve<int>("Game.NumOfAllShips");
        var ships = IoC.Resolve<Dictionary<string, UObject>>("Game.Get.UObjects");
        for (var i = 0; i < allShips; i++)
        {
            var id = Guid.NewGuid().ToString();
            ships[id] = IoC.Resolve<UObject>("Game.Ship.Create");
        }
    }
}