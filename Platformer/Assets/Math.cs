public static class Math
{
    public static void CalculateBallisticArc(
        Vector2 start,
        Vector2 end,
        float arcHeight,
        float time,
        out Vector2 velocity,
        out float gravity
    )
    {
        float xDist = end.x - start.x;
        float vx = xDist / time;

        float y0 = start.y;
        float yPeak = y0 + arcHeight;
        float yEnd = end.y;

        gravity = -4f * (y0 - 2f * yPeak + yEnd) / (time * time);
        float vy = (-3f * y0 + 4f * yPeak - yEnd) / time;

        velocity = new Vector2(vx, vy);
    }
}
