namespace SillyGames.TreasureHunt
{
    internal class EditHuntUI : UIScreen
    {
        public void ReadFrom(HuntData a_data)
        {

        }

        public void Save()
        {

        }

        public void Run()
        {
            THGame.Instance.RunTheHunt();
        }
    }
}