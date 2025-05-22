Vector2 CalculateBallisticVelocity(Vector2 toHit, float arcHeight, float time)
{
    // Horizontale Geschwindigkeit = konstanter Anteil
    float vx = toHit.x / time;

    // Vertikale Bewegung unter Gravitation
    // y(t) = v0y * t + 0.5 * g * t^2
    // Du willst:
    // - am Ende bei toHit.y ankommen
    // - die Maximalhöhe bei time/2 erreichen

    float g = -Physics2D.gravity.y; // meistens 9.81, positiv verwenden

    // Maximalhöhe relativ zur Startposition
    float peakHeight = Mathf.Max(arcHeight, toHit.y + arcHeight);

    // berechne vertikale Geschwindigkeit so, dass zur Halbzeit der höchste Punkt erreicht wird
    float tUp = time / 2f;
    float vy = (4 * peakHeight - 2 * toHit.y) / time;

    return new Vector2(vx, vy);
}
