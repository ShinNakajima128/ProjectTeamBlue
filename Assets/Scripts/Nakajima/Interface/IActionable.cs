/// <summary>
/// アクション可能のオブジェクトで使用するインターフェース
/// </summary>
public interface IActionable 
{
    TargetType Type { get; }
    bool IsCompleted { get; }
    void OnAction();
}
