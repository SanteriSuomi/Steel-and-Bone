public interface IHasHealthbar
{
    float Health { get; }
    void GiveHealthBar(EnemyHealthBar healthbar);
}