# ProjectTeamBlue
チームブルーのチーム制作プロジェクト

## 注意点
- productionブランチは各プロジェクトで作ること

## 開発環境

| エンジン | バージョン  |
| ---------- | ----------- |
| Unity      | [こちらを参照して下さい](ProjectSettings/ProjectVersion.txt#L1) |

## 導入済みアセット

## コード規則

変数名は[キャメルケース](https://e-words.jp/w/%E3%82%AD%E3%83%A3%E3%83%A1%E3%83%AB%E3%82%B1%E3%83%BC%E3%82%B9.html) (先頭小文字)

メンバー変数の接頭辞には「＿」(アンダースコア)を付けること

関数名　クラス名　プロパティの名前は[パスカルケース](https://wa3.i-3-i.info/word13955.html) (先頭大文字)  

### ブランチ名

ブランチの名前は[スネークケース](https://e-words.jp/w/%E3%82%B9%E3%83%8D%E3%83%BC%E3%82%AF%E3%82%B1%E3%83%BC%E3%82%B9.html#:~:text=%E3%82%B9%E3%83%8D%E3%83%BC%E3%82%AF%E3%82%B1%E3%83%BC%E3%82%B9%E3%81%A8%E3%81%AF%E3%80%81%E3%83%97%E3%83%AD%E3%82%B0%E3%83%A9%E3%83%9F%E3%83%B3%E3%82%B0,%E3%81%AA%E8%A1%A8%E8%A8%98%E3%81%8C%E3%81%93%E3%82%8C%E3%81%AB%E5%BD%93%E3%81%9F%E3%82%8B%E3%80%82)
(すべて小文字単語間は「＿」(アンダースコア))
- 機能を作成するブランチであれば接頭辞に「feature/」
- 機能の修正等は接頭辞に「fix/」
- 削除を行う際は接頭辞に「remove/」

### boolean メソッド命名規則

> https://qiita.com/GinGinDako/items/6e8b696c4734b8e92d2b
### region 規則

```shell
public class <ANY NAME>:...
{
    #region property
    // プロパティを入れる。
    #endregion

    #region serialize
    // unity inpectorに表示したいものを記述。
    #endregion

    #region private
    // プライベートなメンバー変数。
    #endregion

    #region Constant
    // 定数をいれる。
    #endregion

    #region Event
    //  System.Action, System.Func などのデリゲートやコールバック関数をいれるところ。
    #endregion

    #region unity methods
    //  Start, UpdateなどのUnityのイベント関数。
    private void Awake()
    {

    }

    private void Start()
    {

    }

    private void Update()
    {

    }
    #endregion

    #region public method
    //　自身で作成したPublicな関数を入れる。
    #endregion

    #region private method
    // 自身で作成したPrivateな関数を入れる。
    #endregion
}
```
# ゲームの仕様書

## タイトル
影の侵入者（仮）

## ジャンル
3Dステルスアクションゲーム

## プラットフォーム
PC

## カメラ
プレイヤーのキャラクターの真上からプレイヤーのキャラクターを見下ろし、追従する高い視点です。視界の後方調整が可能です。

## マップ
すべて平原で、マップチップでマップを構成しています。マップの境界は壁です。

## プレイヤーのキャラクター
### 操作
キーボードとマウスで操作します。

- 移動：前後左右(キーボードWASD)
- 攻撃：マウス左キー
- アクション（調べるなど）：Fキー

### 死亡
マップ上にはチェックポイントがあり、プレイヤーがチェックポイントの範囲内にいる場合は座標を記録しておき、死亡したら最後に記録された座標に戻ります。
プレイヤーは敵の攻撃を受けると残機を減らします。残機が全部なくなったらゲームオーバーになり、ミシュンが失敗となります。その場合は、当該ステージとプレイヤーの状態をリセットします。

### チェックポイント
チェックポイントの周りではプレイヤーは安全です。

## ミッション
### メインミッション
サーバールーム内のパソコンとアクションすることです。

### サブミッション
指定された複数のパソコンとアクションすることです。

## ゲームの流れ
タイトルから（ステージ選択ーゲームプレイーゲームクリア）までループし、すべてのステージをクリアすると、このゲームの完全クリアとなります。

## 敵
### 巡回、パトロールする敵
キャラクターより速く、キャラクターが敵の範囲内に入ると攻撃してきます。

### 停止する敵（監視カメラ）
キャラクターが敵の範囲内に入ると攻撃してきます。

## 素材アセットリスト
- キャラクター
  - スーツマンセット https://assetstore.unity.com/packages/3d/characters/humanoids/humans/bodyguards-31711
  - グルグルロボット https://assetstore.unity.com/packages/3d/characters/robots/robot-sphere-136226

- 敵キャラクター候補
  - スーツマンセット https://assetstore.unity.com/packages/3d/characters/humanoids/humans/bodyguards-31711

- ステージアセット
  - [優先]ピカピカ地下室 https://assetstore.unity.com/packages/3d/environments/3d-free-modular-kit-85732
  - 宇宙ステーション https://assetstore.unity.com/packages/3d/environments/3d-free-modular-kit-85732

- GUI
  - https://assetstore.unity.com/packages/2d/gui/sci-fi-gui-skin-15606

