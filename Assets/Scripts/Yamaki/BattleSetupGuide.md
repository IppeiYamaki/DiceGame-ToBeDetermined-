# Yamaki ActionPoint戦闘セットアップガイド

このガイドは、新ルール用に追加した `RunSystemManager`、`RunPlayerManager`、`ActionPointBattleSceneController`、ActionPoint UIをUnity上で接続する手順です。

旧仕様の「6個から3個を手動選択して即攻撃するUI」ではなく、**所持ダイス6個からターン開始時にランダムで3個をロールし、ActionPointを消費して行動を予約し、行動決定ボタンで順次実行する戦闘**を前提にします。敵は複数体を可変で配置できます。

シーンのHierarchy構成（どのGameObjectにどのComponentを付けるか）の詳細は `BattleSceneHierarchyGuide.md` を参照してください。

---

## 1. マスターデータを作成する

### DiceDefinition

1. Projectビューで右クリックします。
2. `Create > DiceGame > DiceDefinition` を選択します。
3. 通常ダイスの場合は `標準出目を使用` をONにします。
4. 特殊ダイスの場合は `標準出目を使用` をOFFにし、面リストの数値を設定します。

例:

```text
通常6面: 1,2,3,4,5,6
特殊6面: 2,2,4,4,6,8
8面予定: 1,2,3,4,5,6,7,8
```

### DiceRoleDefinition

1. Projectビューで右クリックします。
2. `Create > DiceGame > DiceRoleDefinition` を選択します。
3. 役名、倍率、条件タイプ、判定用数字リストを設定します。

### SkillDefinition

1. Projectビューで右クリックします。
2. `Create > DiceGame > SkillDefinition` を選択します。
3. 以下を設定します。
   - `スキル名`
   - `説明`
   - `コスト`
   - `効果種別`（None / Heal / Damage / Block / GainActionPoint / ApplyStatusEffect）
   - `効果量`
   - `付与する状態異常`（効果種別が ApplyStatusEffect の場合に `StatusEffectDefinition` を設定）
   - `1戦镘1回のみ使用可能`（OncePerBattle）
   - `使用時SE`（任意の `AudioClip`）
   - `アイコン`
   - `対象タイプ`（Self / SingleEnemy / AllEnemies / RandomEnemy）

例:

```text
スキル名: 応急処置
コスト: 10
効果種別: Heal
効果量: 5
```

### ItemData（Yasuda版）と ItemEffectTable

旧 `ItemDefinition` は廃止しました。アイテムは Yasuda版 `ItemData` を使用し、効果は Yamaki側の `ItemEffectTable` で定義します（Yasudaフォルダは変更しません）。

1. Projectビューで `Create > Yasuda > ItemData`（既存の作成メニュー）で `ItemData` を作成し、アイテム名・説明・アイコン（Texture）を設定します。
2. `Create > DiceGame > ItemEffectTable` で `ItemEffectTable` を作成します。
3. エントリに以下を登録します。
   - `対象アイテム`（`ItemData`）
   - `効果種別`（Heal / Damage / Block / GainActionPoint など）
   - `効果量`
   - `対象タイプ`（Self / SingleEnemy / AllEnemies / RandomEnemy）
   - `アイコン上書き`（任意。未設定なら `ItemData.icon` から自動生成）
4. 作成した `ItemEffectTable` を `RunPlayerManager` の `ItemEffectTable` フィールドに設定します。

同一 `ItemData` を重複登録すると警告が出ます。テーブルに未登録のアイテムは効果なしとして扱われます。

### EnemyDefinition

1. Projectビューで右クリックします。
2. `Create > DiceGame > EnemyDefinition` を選択します。
3. 敵名、最大HP、基礎攻撃力、表示Sprite/Texture、行動パターンを設定します。

---

## 2. DiceMasterRegistryを設定する

1. Projectビューで `Create > DiceGame > DiceMasterRegistry` を選択します。
2. `ダイス定義（マスタ）` に使用する `DiceDefinition` を登録します。
3. `役定義（マスタ）` に `DiceRoleDefinition` を登録します。

`ActionPointBattleSceneController` は、ここに登録された役一覧を使ってActionPoint計算を行います。

---

## 3. 常駐Systemを作成する

1. ラン開始シーン、または最初に必ず読み込まれるシーンに空のGameObjectを作成します。
2. 名前を `RunSystemManager` にします。
3. `RunSystemManager` コンポーネントを追加します。
4. Inspectorで以下を設定します。
   - `ダイスマスタ`: 作成した `DiceMasterRegistry`
   - `バフ/デバフマスタ`: 任意
   - `戦闘アイコン設定`: 任意
   - `マスター音量`
   - `BGM音量`
   - `SE音量`

`RunSystemManager` は `DontDestroyOnLoad` によりシーン遷移後も残ります。

Systemが持つもの:

- オプション設定
- 共有アイコン/Texture設定
- DiceMasterRegistry参照
- StatusEffectRegistry参照
- ラン経過時間

---

## 4. 常駐Playerを作成する

1. ラン開始シーン、または最初に必ず読み込まれるシーンに空のGameObjectを作成します。
2. 名前を `RunPlayerManager` にします。
3. `RunPlayerManager` コンポーネントを追加します。
4. Inspectorで以下を設定します。
   - `プレイヤー名`
   - `最大HP`
   - `現在HP`
   - `所持ダイス（6個）`
   - `最大スキル所持数`
   - `所持スキル`
   - `最大アイテム所持数`
   - `所持アイテム`（`ItemData` と個数のスタック形式。同一アイテムは1枠にまとめて `Count` で管理）
   - `ItemEffectTable`（作成した `ItemEffectTable` を設定）

`RunPlayerManager` も `DontDestroyOnLoad` によりシーン遷移後も残ります。

Playerが持つもの:

- 現在HP / 最大HP
- 所持ダイス6枠
- 所持スキル
- 所持アイテム（`OwnedItemStack`: `ItemData` + 個数）
- ItemEffectTable参照
- 最大所持数

バトルシーン側は、プレイヤーHPや所持ダイスを直接持たず、`RunPlayerManager.Instance` を参照します。

---

## 5. Battle Canvasを作成する

Canvas配下に以下を用意します。

- Player HP表示用 `HpView`
- Player Block表示用 `BlockView`
- 複数敵表示用 `EnemyAreaView`（EnemyView Prefabを動的生成）
- ActionPoint表示用 `ActionPointView`
- 行動予約レーン用 `ActionReservationLaneView`
- 攻撃コマンドUI
- 防御コマンドUI
- スキル一覧UI `SkillListView`
- アイテム一覧UI `ItemListView`
- ダイス役一覧UI `DiceRoleListView`
- 行動決定Button（旧: ターン終了Button）
- ターン表示用 `TextMeshPro - Text`
- フェーズ表示用 `TextMeshPro - Text`
- ロール結果表示用 `TextMeshPro - Text`
- 戦闘ログ表示用 `TextMeshPro - Text`

---

## 6. ActionPointViewを設定する

1. AP表示用GameObjectを作成します。
2. `ActionPointView` を追加します。
3. Inspectorで以下を設定します。
   - `AP表示テキスト`
   - `APスライダー（任意）`
   - `AP表示フォーマット`

例:

```text
AP: {0} / {1}
```

`{0}` は現在AP、`{1}` はターン開始時に獲得したAPです。

---

## 7. 攻撃/防御コマンドUIを設定する

攻撃用と防御用に、それぞれ `AdjustableCostCommandView` を用意します。

1. 攻撃コマンド用GameObjectを作成します。
2. `AdjustableCostCommandView` を追加します。
3. 以下を設定します。
   - `コマンド名`
   - `コマンド名テキスト`
   - `コストテキスト`
   - `増加ボタン`
   - `減少ボタン`
   - `実行ボタン`
   - `最小コスト`
   - `初期コスト`
4. 防御用にも同じ手順で作成します。

攻撃は以下の2方式で予約できます。

- 実行ボタンを押す（選択中のターゲット、未選択なら生存敵を自動選択）
- 攻撃枠をドラッグして敵へドロップする（ドロップ先の敵を対象に予約）

防御は実行ボタンで防御予約値を増やします。予約された防御値は行動決定の予約実行後にBlockへ変換されます。

いずれの行動も即時実行されず、行動予約レーンに積まれます。予約時にAPを確保し、キャンセルするとAPが返却されます。

---

## 8. 攻撃ドラッグ&ドロップを設定する

### 攻撃側

1. 攻撃コマンドUIに `AttackDragSource` を追加します。
2. `攻撃コマンドView` に攻撃用 `AdjustableCostCommandView` を設定します。

### 敵側

1. EnemyView Prefabの敵画像、または敵のDrop判定用GameObjectに `EnemyDropTarget` を追加します。
2. `ActionPoint戦闘Controller` にシーン上の `ActionPointBattleSceneController` を設定します（未設定なら自動検索）。
3. 敵インデックスは `EnemyAreaView` が生成時に自動で設定するため、手動設定は不要です。

これで攻撃UIを敵へドラッグ&ドロップすると、設定中のPtでその敵への攻撃が予約されます。

---

## 9. スキル一覧UIを設定する

### SkillButtonView Prefab

1. スキル1個分のUIを作成します。
2. `SkillButtonView` を追加します。
3. 以下を設定します。
   - `スキル名テキスト`
   - `コストテキスト`
   - `説明テキスト`
   - `アイコン`
   - `発動ボタン`
4. Prefab化します。

### SkillListView

1. スキル一覧用Panelを作成します。
2. `SkillListView` を追加します。
3. 以下を設定します。
   - `スキルボタンPrefab`
   - `配置先`

`ActionPointBattleSceneController` が `RunPlayerManager` の所持スキルを参照し、APが足りる場合のみ発動（予約）可能にします。`1戦镘1回のみ使用可能` なスキルは、使用・予約済みの場合は同一戦闘中再度予約できません。`使用時SE` が設定されていると実行時に再生されます。

---

## 10. アイテム一覧UIを設定する

### ItemButtonView Prefab

1. アイテム1種類分のUIを作成します。
2. `ItemButtonView` を追加します。
3. 以下を設定します。
   - `アイテム名テキスト`
   - `所持数テキスト`（任意。未設定なら名前に「×N」を連結表示）
   - `説明テキスト`
   - `アイコン`
   - `使用ボタン`
4. Prefab化します。

### ItemListView

1. アイテム一覧用Panelを作成します。
2. `ItemListView` を追加します。
3. 以下を設定します。
   - `ItemButtonView Prefab`
   - `配置先 Transform`

`ActionPointBattleSceneController` が `RunPlayerManager` の所持アイテム（スタック）を参照して一覧を更新します。ボタンには「所持数 − 予約済み数」が表示され、残数0のアイテムは予約できません。使用ボタンで予約され、実行時に個数が減少します。効果は `ItemEffectTable` から解決されます。

---

## 11. 行動予約レーンUIを設定する

### ReservationEntryView Prefab

1. 予約1件分のUIを作成します。
2. `ReservationEntryView` を追加します。
3. 以下を設定します。
   - `ラベルテキスト`
   - `キャンセルボタン`
4. Prefab化します。

### ActionReservationLaneView

1. 画面左側などに予約レーン用Panel（VerticalLayoutGroup推奨）を作成します。
2. `ActionReservationLaneView` を追加します。
3. 以下を設定します。
   - `ReservationEntryView Prefab`
   - `配置先 Transform`

予約の追加・キャンセルのたびにControllerが自動で再描画します。

---

## 12. 複数敵表示（EnemyAreaView）を設定する

### EnemyView Prefab

1. 敵1体分のUI（敵名/HP/Block/画像など）を作成し、`EnemyView` を追加します。
2. クリック選択用に `EnemySelectable` を追加します。
3. ドラッグ&ドロップ受け用に `EnemyDropTarget` を追加します。
4. Prefab化します。

### EnemyAreaView

1. 敵表示エリア用のGameObject（HorizontalLayoutGroup推奨）を作成します。
2. `EnemyAreaView` を追加します。
3. 以下を設定します。
   - `EnemyView Prefab`
   - `生成コンテナ（HorizontalLayoutGroupを想定）`

戦闘開始時に敵リストの数だけEnemyViewが生成され、撃破されると自動で詰め直されます。敵をクリックするとターゲット選択できます。

---

## 13. ダイス役一覧UIを設定する

1. 役一覧用Panelを作成します。
2. `DiceRoleListView` を追加します。
3. 以下を設定します。
   - `行Prefab（TextMeshPro - Text）`
   - `配置先Transform`

`DiceMasterRegistry` に登録された役一覧が「役名 x倍率」形式で表示されます。

---

## 14. ActionPointBattleSceneControllerを設定する

1. シーン上に空のGameObjectを作成します。
2. 名前を `ActionPointBattleSceneController` にします。
3. `ActionPointBattleSceneController` コンポーネントを追加します。
4. Inspectorで以下を設定します。
   - `System Manager`: 通常は未設定でも `RunSystemManager.Instance` を参照します
   - `Player Manager`: 通常は未設定でも `RunPlayerManager.Instance` を参照します
   - `敵データ（複数対応）`: 出現させる `EnemyDefinition` を必要な数だけ登録
   - `敵データ`: 旧式の単一敵フォールバック用（複数対応を使う場合は未設定で可）
   - `EnemyAreaView（動的生成用）`: `EnemyAreaView`
   - `行動予約レーンView`: `ActionReservationLaneView`
   - `アイテム一覧View`: `ItemListView`
   - `ダイス役一覧View`: `DiceRoleListView`
   - `敵行動の間隔（秒）`
   - `外部ロール演出を待つ`: 外部演出で出目を決める場合ON
   - `Player HP表示`: `HpView`
   - `Player Block表示`: `BlockView`
   - `Enemy表示`: 旧式の単一 `EnemyView`（複数対応を使う場合は未設定で可）
   - `ActionPoint表示`: `ActionPointView`
   - `攻撃コマンド`: 攻撃用 `AdjustableCostCommandView`
   - `防御コマンド`: 防御用 `AdjustableCostCommandView`
   - `スキル一覧`: `SkillListView`
   - `ターン終了ボタン`: 行動決定に使うButton（予約を順次実行）
   - `ターン表示`: TMP_Text
   - `フェーズ表示`: TMP_Text
   - `ロール結果表示`: TMP_Text
   - `戦闘ログ`: TMP_Text

---

## 15. 外部ロール演出と接続する

他担当のダイスロール演出で出目を決める場合は、`ActionPointBattleSceneController` の `外部ロール演出を待つ` をONにします。

流れ:

1. ターン開始時、Controllerが所持ダイス6個からランダムで3個を選びます。
2. `ExternalRollRequested` イベントで、その3個のダイスが外部演出側へ渡されます。
3. 外部演出側で3個の出目を決定します。
4. 演出完了時に以下を呼びます。

```text
SubmitExternalRollNumbers(int[] numbers)
```

例:

```text
numbers = [3, 5, 6]
```

Controllerは受け取った出目から役判定とActionPoint計算を行います。

外部演出を使わない場合は `外部ロール演出を待つ` をOFFにします。この場合、Controller内部でランダムロールします。

---

## 16. ダイス/スキル/アイテム入手時の交換

### ダイス

新しいダイスを入手しても、所持数は増やしません。

```text
RunPlayerManager.ReplaceDice(slotIndex, newDice)
```

6枠のどれかを選ばせ、既存ダイスと交換します。

### スキル

空きがある場合:

```text
RunPlayerManager.TryAddSkill(newSkill)
```

満杯の場合:

```text
RunPlayerManager.ReplaceSkill(slotIndex, newSkill)
```

### アイテム

アイテムはスタック管理です。同一アイテムは個数が増え、新規アイテムは空き枠があれば追加されます。

```text
RunPlayerManager.TryAddItem(itemData)
```

削除（使用時の消費）:

```text
RunPlayerManager.TryRemoveItem(itemData)
```

所持数の取得:

```text
RunPlayerManager.GetItemCount(itemData)
```

交換UI自体は今後作成予定です。

---

## 17. 再生確認

1. Playボタンを押します。
2. `RunSystemManager` と `RunPlayerManager` が存在することを確認します。
3. 戦闘シーンに `ActionPointBattleSceneController` が存在することを確認します。
4. 敵リストに登録した数だけ敵が自動整列で表示されます。
5. ターン開始時に3個のダイスがランダムでロールされます。
6. ロール結果に以下が表示されます。
   - 選ばれたダイス
   - 3個の出目
   - 合計
   - 役
   - 獲得AP
7. AP表示が更新されます。
8. 敵をクリックするとターゲット選択できます。
9. 攻撃/防御の上下ボタンで使用Ptを調整できます。
10. 攻撃ボタン、またはドラッグ&ドロップで攻撃を予約でき、予約レーンに表示されます。
11. 防御ボタンで防御予約を追加できます。
12. スキルボタンでAPを確保してスキルを予約できます。
13. アイテムボタンでアイテムを予約できます（同一アイテムは所持数まで。表示は残数で更新）。
14. 予約エントリのキャンセルボタンで予約を取り消すとAPが返却されます。
15. 行動決定ボタンを押すと、予約が順次実行され、防御予約がBlockに変換され、敵が行動します。
16. 全敵撃破で勝利、勝敗未決定なら次ターンへ進みます。

---

## 注意

- `RunPlayerManager` の所持ダイスには、最低3個以上の `DiceDefinition` を設定してください。
- 新ルールでは所持ダイスは基本6個です。
- `DiceDefinition` は `DiceMasterRegistry` にも登録してください。
- `DiceRoleDefinition` を登録しない場合、役は成立せず倍率は1.0として扱われます。
- 旧 `BattleSceneController` は旧仕様用です。新仕様では `ActionPointBattleSceneController` を使用してください。
- 旧 `ItemDefinition` は廃止しました。アイテムは Yasuda版 `ItemData` + `ItemEffectTable` で運用します（Yasudaフォルダは変更しない）。
- `RunPlayerManager` に `ItemEffectTable` を設定し忘れると、アイテムの効果・説明・アイコンが解決できません。
- 行動はすべて予約レーン経由で実行されます。即時実行はAllEnemies/RandomEnemy対象の効果処理と同様、今後の拡張対象です。
- 交換UI、勝利/敗北画面遷移、セーブ/ロードは今後の拡張対象です。
