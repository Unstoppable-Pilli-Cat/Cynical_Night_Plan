public class GameState
{
    public int HotaruAffection { get; private set; }

    public void ChangeHotaruAffection(int amount)
    {
        HotaruAffection += amount;
    }
}
