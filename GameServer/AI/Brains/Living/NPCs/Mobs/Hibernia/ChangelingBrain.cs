using Core.GS.GameUtils;

namespace Core.GS.AI;

public class ChangelingBrain : StandardMobBrain
{
    public ChangelingBrain()
        : base()
    {
    }

    public void SetModel()
    {
        switch (Util.Random(5))
        {
            case 0:
                Body.Model = 69; // small goblin whelp
                break;
            case 1:
                Body.Model = 124; // mud creature
                break;
            case 2:
                Body.Model = 391; // vendo
                break;
            case 3:
                Body.Model = 47; // small grey wolf
                break;
            case 4:
                Body.Model = 123; // worm
                break;
            case 5:
                Body.Model = 112; // brownie
                break;
        }

        var size = (byte)Util.Random(25, 55);
        Body.Size = size;

    }

    public override void Think()
    {
        base.Think();
        if (Util.Chance(10))
        {
            SetModel();
        }
    }

}