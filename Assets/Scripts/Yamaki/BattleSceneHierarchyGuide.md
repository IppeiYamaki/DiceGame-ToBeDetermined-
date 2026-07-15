# BattleScene Hierarchy 構築ガイド

このドキュメントは、新ルール（予約レーン方式・複数敵対応）のBattleSceneをUnity上で構築するための、**GameObjectの種類・親子関係・アタッチするComponent・Componentへの設定内容**をまとめたガイドです。

- Object形式の凡例
  - `Empty` = 空のGameObject（Create Empty）
  - `Panel` = UI > Panel（Image付きRectTransform）
  - `Button` = UI > Button - TextMeshPro
  - `Text` = UI > Text - TextMeshPro
  - `Image` = UI > Image
  - `Slider` = UI > Slider

---

## 1. シーン全体のHierarchy

```text
BattleScene
├── Main Camera
├── EventSystem                     ← UIクリックに必須
├── ActionPointBattleSceneController (Empty)
└── BattleCanvas (Canvas)
	├── LeftSide
	│   └── ReservationLane (Panel)             ← 行動予約レーン
	│       └── Content (Empty + VerticalLayoutGroup)
	├── TopSide
	│   ├── TurnText (Text)
	│   ├── PhaseText (Text)
	│   └── RollResultText (Text)
	├── EnemyArea (Empty + HorizontalLayoutGroup)  ← 敵が動的生成される
	├── PlayerStatus
	│   ├── PlayerHpView (Empty)
	│   │   ├── HpText (Text)
	│   │   └── HpSlider (Slider)
	│   ├── PlayerBlockView (Empty)
	│   │   └── BlockText (Text)
	│   └── ActionPointView (Empty)
	│       ├── ApText (Text)
	│       └── ApSlider (Slider) ※任意
	├── CommandArea
	│   ├── AttackCommand (Panel)
	│   ├── DefenseCommand (Panel)
	│   ├── SkillList (Panel)
	│   │   └── Content (Empty + HorizontalLayoutGroup)
	│   └── ItemList (Panel)
	│       └── Content (Empty + HorizontalLayoutGroup)
	├── DiceRoleList (Panel)
	│   └── Content (Empty + VerticalLayoutGroup)
	├── ConfirmButton (Button)                  ← 行動決定ボタン
	└── BattleLogText (Text)
```

※ `RunSystemManager` / `RunPlayerManager` はタイトル/ラン開始シーンで生成される常駐Objectのため、BattleSceneには置きません（テスト用に置いてもDontDestroyOnLoadの重複破棄で問題ありません）。

---

## 2. 各Objectの作成方法とComponent設定

### 2.1 ActionPointBattleSceneController

| 項目 | 内容 |
|---|---|
| Object形式 | Empty |
| 親 | シーン直下 |
| Component | `ActionPointBattleSceneController` |

Inspector設定:

| フィールド | 設定内容 |
|---|---|
| System Manager | 未設定で可（`RunSystemManager.Instance` を自動参照） |
| Player Manager | 未設定で可（`RunPlayerManager.Instance` を自動参照） |
| 敵データ（複数対応） | 出現させる `EnemyDefinition` を必要な数だけ登録（1〜複数） |
| 敵データ | 旧式の単一敵フォールバック用。複数対応を使う場合は未設定で可 |
| EnemyAreaView（動的生成用） | `EnemyArea` の `EnemyAreaView` |
| 行動予約レーンView | `ReservationLane` の `ActionReservationLaneView` |
| アイテム一覧View | `ItemList` の `ItemListView` |
| ダイス役一覧View | `DiceRoleList` の `DiceRoleListView` |
| 敵行動の間隔（秒） | 0.6 など |
| 外部ロール演出を待つ | 外部演出と連携する場合のみON |
| Player HP表示 | `PlayerHpView` の `HpView` |
| Player Block表示 | `PlayerBlockView` の `BlockView` |
| Enemy表示 | 未設定で可（複数敵対応時はEnemyAreaViewを使用） |
| ActionPoint表示 | `ActionPointView` |
| 攻撃コマンド | `AttackCommand` の `AdjustableCostCommandView` |
| 防御コマンド | `DefenseCommand` の `AdjustableCostCommandView` |
| スキル一覧 | `SkillList` の `SkillListView` |
| ターン終了ボタン | `ConfirmButton`（行動決定ボタンとして予約を順次実行） |
| ターン表示 | `TurnText` |
| フェーズ表示 | `PhaseText` |
| ロール結果表示 | `RollResultText` |
| 戦闘ログ | `BattleLogText` |
| SE再生用 AudioSource(任意) | SEを鳴らす場合に `AudioSource` を設定（未設定なら `PlayClipAtPoint` で再生） |

### 2.2 ReservationLane（行動予約レーン）

| 項目 | 内容 |
|---|---|
| Object形式 | Panel |
| 親 | BattleCanvas > LeftSide |
| Component | `ActionReservationLaneView` |

子Objectとして `Content`（Empty）を作成し、`VerticalLayoutGroup` と `ContentSizeFitter`（Vertical Fit: Preferred Size）を追加します。

Inspector設定:

| フィールド | 設定内容 |
|---|---|
| ReservationEntryView Prefab | 後述の `ReservationEntry` Prefab |
| 配置先 Transform | `Content` |

### 2.3 ReservationEntry Prefab（予約1件分）

| 項目 | 内容 |
|---|---|
| Object形式 | Panel（Prefab化する） |
| Component | `ReservationEntryView` |

子構成:

```text
ReservationEntry (Panel) ← ReservationEntryView
├── TargetIcon (Image)    ※任意
├── ActionIcon (Image)    ※任意
├── LabelText (Text)      ← 「攻撃 5 → スライム」等の表示
└── CancelButton (Button) ← ×ボタン
```

Inspector設定:

| フィールド | 設定内容 |
|---|---|
| 対象アイコン | `TargetIcon`（任意） |
| 行動アイコン | `ActionIcon`（任意） |
| ラベルテキスト | `LabelText` |
| キャンセルボタン | `CancelButton` |

### 2.4 EnemyArea（複数敵の表示エリア）

| 項目 | 内容 |
|---|---|
| Object形式 | Empty（RectTransform） |
| 親 | BattleCanvas |
| Component | `EnemyAreaView`、`HorizontalLayoutGroup` |

`HorizontalLayoutGroup` の設定例: Child Alignment = Middle Center / Spacing = 40 / Child Force Expand OFF。
敵の数が増減すると自動で中央寄せに整列し直されます。

Inspector設定:

| フィールド | 設定内容 |
|---|---|
| EnemyView Prefab | 後述の `Enemy` Prefab |
| 生成コンテナ | 自分自身（EnemyArea）のTransform |

### 2.5 Enemy Prefab（敵1体分）

| 項目 | 内容 |
|---|---|
| Object形式 | Empty（RectTransform、Prefab化する） |
| Component（ルート） | `EnemyView` |

子構成:

```text
Enemy (Empty) ← EnemyView
├── EnemyImage (Image)        ← EnemySelectable / EnemyDropTarget を追加
│   └── TargetMarker (Image)  ← 選択中マーカー（矢印や枠）。初期は非表示
├── NameText (Text)
├── HpText (Text)
├── HpSlider (Slider)         ※任意
├── BlockText (Text)
└── IntentIcons (Empty + HorizontalLayoutGroup) ※任意
```

Component設定:

| Object | Component | 設定 |
|---|---|---|
| Enemy | `EnemyView` | 敵名テキスト=NameText / HPテキスト=HpText / HPスライダー=HpSlider / Blockテキスト=BlockText / 敵画像=EnemyImage |
| EnemyImage | `EnemySelectable` | ターゲットマーカー=TargetMarker |
| EnemyImage | `EnemyDropTarget` | ActionPoint戦闘Controller=未設定で可（自動検索）。敵インデックスは生成時に自動設定 |

※ `EnemyImage` は Raycast Target をONにしてください（クリック/ドロップ判定に必要）。

### 2.6 PlayerHpView / PlayerBlockView / ActionPointView

| Object | Object形式 | Component | 設定 |
|---|---|---|---|
| PlayerHpView | Empty | `HpView` | HPテキスト=HpText / HPスライダー=HpSlider |
| PlayerBlockView | Empty | `BlockView` | Blockテキスト=BlockText |
| ActionPointView | Empty | `ActionPointView` | AP表示テキスト=ApText / APスライダー=ApSlider（任意）/ AP表示フォーマット=`AP: {0} / {1}` |

### 2.7 AttackCommand / DefenseCommand

| 項目 | 内容 |
|---|---|
| Object形式 | Panel |
| 親 | BattleCanvas > CommandArea |
| Component | `AdjustableCostCommandView`（攻撃側は追加で `AttackDragSource`） |

子構成（攻撃・防御共通）:

```text
AttackCommand (Panel) ← AdjustableCostCommandView, AttackDragSource(攻撃のみ)
├── NameText (Text)      ← 「攻撃」「防御」
├── CostText (Text)      ← 現在コスト表示
├── UpButton (Button)    ← コスト増加
├── DownButton (Button)  ← コスト減少
└── ExecuteButton (Button) ← 実行（予約）ボタン
```

`AdjustableCostCommandView` の設定:

| フィールド | 設定内容 |
|---|---|
| コマンド名 | 「攻撃」または「防御」 |
| コマンド名テキスト | NameText |
| コストテキスト | CostText |
| 増加ボタン | UpButton |
| 減少ボタン | DownButton |
| 実行ボタン | ExecuteButton |
| 最小コスト / 初期コスト | 1 / 1 など |

`AttackDragSource`（攻撃のみ）の設定:

| フィールド | 設定内容 |
|---|---|
| 攻撃コマンドView | 同Objectの `AdjustableCostCommandView` |

これでドラッグ&ドロップ時に現在コストが `EnemyDropTarget` へ渡り、ドロップした敵への攻撃予約になります。

### 2.8 SkillList / ItemList

| Object | Component | 設定 |
|---|---|---|
| SkillList | `SkillListView` | スキルボタンPrefab=`SkillButton` Prefab / 配置先=Content |
| ItemList | `ItemListView` | ItemButtonView Prefab=`ItemButton` Prefab / 配置先 Transform=Content |

SkillButton Prefab構成:

```text
SkillButton (Button) ← SkillButtonView
├── NameText (Text)
├── CostText (Text)
├── DescriptionText (Text)
└── Icon (Image)
```

ItemButton Prefab構成:

```text
ItemButton (Button) ← ItemButtonView
├── NameText (Text)
├── CountText (Text)       ※任意（所持数「×N」表示用。未設定の場合はアイテム名に連結表示）
├── DescriptionText (Text)
└── Icon (Image)
```

`ItemButtonView` の設定: アイテム名テキスト=NameText / 所持数テキスト=CountText（任意）/ 説明テキスト=DescriptionText / アイコン=Icon / 使用ボタン=自身のButton。

※ アイテムは Yasuda版 `ItemData` を使用します。同一アイテムは1ボタンにスタックされ、表示数は「所持数 − 予約済み数」です。アイコンや効果は `RunPlayerManager` に登録した `ItemEffectTable` から解決されます。

### 2.9 DiceRoleList（役一覧）

| 項目 | 内容 |
|---|---|
| Object形式 | Panel |
| Component | `DiceRoleListView` |

行Prefab構成（`RoleRow`）:

```text
RoleRow (Text) ← TextMeshPro - Text 1個のみ
```

Inspector設定:

| フィールド | 設定内容 |
|---|---|
| 行Prefab（TextMeshPro - Text） | `RoleRow` Prefab |
| 配置先Transform | `Content`（VerticalLayoutGroup付き） |

### 2.10 ConfirmButton（行動決定ボタン）

| 項目 | 内容 |
|---|---|
| Object形式 | Button |
| 表示テキスト | 「行動決定」 |
| 接続先 | Controllerの `ターン終了ボタン` フィールドに設定（onClickへの手動登録は不要。Controllerが自動登録） |

押すと予約レーンの行動が上から順次実行され、防御予約がBlock化された後、敵行動へ移行します。

### 2.11 テキスト類

| Object | Object形式 | 用途 |
|---|---|---|
| TurnText | Text | 「Turn 1」表示 |
| PhaseText | Text | 「Phase: PlayerAction」表示 |
| RollResultText | Text | ロール結果（ダイス名/出目/合計/役/AP）表示 |
| BattleLogText | Text | 戦闘ログ。ScrollView内に置くことを推奨 |

---

## 3. 動作フロー確認チェックリスト

1. Playすると敵リストの数だけ敵が `EnemyArea` に整列して表示される。
2. ターン開始時に3個のダイスが自動ロールされ、APが表示される。
3. 敵をクリックすると `TargetMarker` が表示され、ターゲット選択される。
4. 攻撃の実行ボタン、またはドラッグ&ドロップで攻撃が予約レーンに追加される。
5. スキル/アイテムのボタンで予約レーンに追加される（APはスキル予約時に減少）。
6. 予約エントリのキャンセルボタンでAPが返却される。
7. 行動決定ボタンで予約が順次実行され、敵が倒れると自動で詰め直される。
8. 同一アイテムは1ボタンにスタックされ、使用すると個数が減り、0で枠が消える。
9. `1戦镘1回のみ使用可能` なスキルは同一戦闘で再使用・再予約できない。
10. 全敵撃破で勝利、プレイヤーHP0でゲームオーバーになる。
