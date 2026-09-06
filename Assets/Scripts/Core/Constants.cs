/// <summary>
/// Global constants for reel timing, bounce, and reel count.
/// Centralized here so tuning changes don't require editing multiple scripts.
/// </summary>
public static class GameConstants
{
    public const int REEL_COUNT = 3;

    // Staggered stop delays create the cascading left-to-right stop effect.
    public const float REEL_STOP_DELAY_REEL_0 = 1.0f;
    public const float REEL_STOP_DELAY_REEL_1 = 1.5f;
    public const float REEL_STOP_DELAY_REEL_2 = 2.0f;

    // Bounce: the reel overshoots the target by this fraction of symbolHeight,
    // then settles back via a half-sine curve.
    public const float REEL_BOUNCE_OVERSHOOT = 0.05f;
    public const float REEL_BOUNCE_SETTLE_DURATION = 0.15f;
}
