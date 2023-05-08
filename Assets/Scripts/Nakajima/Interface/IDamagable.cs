/// <summary>
/// ダメージ処理を行う際に使用するインターフェース
/// </summary>
public interface IDamagable
{
    #region property
    int CurrentHP { get; }
    #endregion

    #region public method
    void Damage(int attackValue);
    #endregion
}
