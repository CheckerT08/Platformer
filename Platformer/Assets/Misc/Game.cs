public static class Game
{
    public static class Tags
    {
        public const string Player = "Player";
        public const string Enemy = "Enemy";
        public const string Collectible = "Collectible";
    }

    public static class Layers
    {
        public static readonly int Ground = LayerMask.NameToLayer("Ground");
        public static readonly int Enemy = LayerMask.NameToLayer("Enemy");
        public static readonly LayerMask GroundMask = 1 << Ground;
    }

    public static class Anim
    {
        public const string IsRunning = "isRunning";
        public const string IsJumping = "isJumping";
    }

    public static class Scenes
    {
        public const string MainMenu = "MainMenu";
        public const string Level1 = "Level1";
    }

    public static class Colors
    {
        public static readonly Color DamageFlash = Color.white;
        public static readonly Color HealFlash = Color.green;
    }
}
