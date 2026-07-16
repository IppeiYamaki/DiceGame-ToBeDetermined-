# Test_Battle1 シーン設定手順書

このドキュメントは、敵行動システム刷新・ブロック表示・ダイスロール演出を `Test_Battle1` シーンへ組み込むための Unity エディタ作業手順です。

> コード側の実装は完了しています。以下はすべて **Unity エディタ上での配置・配線作業** です。

---

## 1. 旧アセットの削除（最初に行う）

`EnemyActionDefinition` クラスは廃止されました。古いアセットが残っているとコンソールに
"Missing Script" 系の警告が出るため、先に削除します。

1. Project ビューで `EnemyActionDefinition_` で検索する
2. ヒットした `.asset` ファイルをすべて削除する
   （例: `Assets/Data/EnemyActionDefinition_Slash.asset` など）

> **補足**: 旧戦闘スタック（`DiceBattleController` / `BattleUIManager` / `EnemyIntentPanel` / `EnemyActionEvaluator`）は
> 廃止された `EnemyActionDefinition` に依存していたため削除済みです。
> 旧シーン `Test_Battle.unity` を開くと "Missing Script" 警告が出ますが、現行の `Test_Battle 1.unity` には影響ありません。

---

## 2. EnemyDefinition アセットの HP帯行動を設定

`EnemyDefinition` アセット（例: `Assets/Data/EnemyDefinition_Slime.asset`）を選択すると、
新しい Inspector（HP帯別行動セット）が表示されます。

### 2-1. HP帯の編集

- 初期状態では `100% ~ 0%` の1帯が自動生成されます
- **「帯を追加（末尾を分割）」** ボタンで帯を分割できます
- 各帯の境界％を変更すると、隣接する帯も自動的に補正され、行動が存在しないHP帯はできません

### 2-2. 行動グループの設定

各HP帯の中に **行動グループ**（ウェイト付き）を複数追加できます。

- `Weight`: 抽選の重み。大きいほど選ばれやすい（例: Weight 3 と Weight 1 なら 3:1 の確率）
- `Actions`: そのグループが選ばれた時に**すべて実行される**行動リスト
  - 例:「攻撃10＋防御5」を1グループに入れると、同一ターンに両方実行されます

### 2-3. 各行動（EnemyActionEntry）の設定

| 行動区分 | 設定項目 | 説明 |
|---|---|---|
| Attack | Power | プレイヤーへのダメージ |
| Defense | Power | 敵が得るブロック値 |
| Buff | StatusEffect | 自身に付与する `StatusEffectDefinition` |
| Debuff | StatusEffect | プレイヤーに付与する `StatusEffectDefinition` |

任意でエフェクト（Prefab / 表示位置 / 表示時間）とサウンド（AudioClip / タイミング / 音量）を設定できます。

### 設定例

```
EnemyDefinition_Slime
├ HP帯 100% ~ 50%
│  ├ グループ1 (Weight 2): [攻撃 Power=8]
│  └ グループ2 (Weight 1): [攻撃 Power=5, 防御 Power=5]
├ HP帯 49% ~ 20%
│  ├ グループ1 (Weight 1): [攻撃 Power=12]
│  └ グループ2 (Weight 1): [バフ(攻撃力強化), 防御 Power=8]
└ HP帯 19% ~ 0%
   └ グループ1 (Weight 1): [デバフ(防御低下), 攻撃 Power=15]
```

---

## 3. Block 表示の配置（Player / Enemy）

### 3-1. Player 側

1. Hierarchy で Player の HP 表示付近に新しい UI を作る
   - 右クリック → `UI > Text - TextMeshPro` を作成し、名前を `PlayerBlockText` にする
   - 任意で盾アイコン用の `UI > Image` を横に配置（名前: `PlayerBlockIcon`）
2. 親オブジェクト（例: `PlayerStatusPanel`）に **`BlockView`** コンポーネントを追加
3. `BlockView` の Inspector で以下を割り当てる
   - `Block表示テキスト` ← `PlayerBlockText`
   - `Blockアイコン（任意）` ← `PlayerBlockIcon`
   - `Block表示フォーマット` ← `Block: {0}`（好みで変更可）
   - `Block値が0の時に非表示にする` ← お好み（推奨: ON）
4. `BattleSceneController` の Inspector で
   - **`Player Block表示`** ← 上で作った `BlockView` を割り当てる

### 3-2. Enemy 側

1. Enemy の UI（`EnemyView` が付いているオブジェクト配下）に同様に
   `EnemyBlockText`（TextMeshPro）と任意の `EnemyBlockIcon`（Image）を作成
2. `BlockView` コンポーネントを追加して同様に割り当てる
3. **`EnemyView`** の Inspector で
   - **`敵Block表示`** ← 上で作った `BlockView` を割り当てる

### 3-3. Enemy のバフ/デバフアイコン表示（任意）

1. Enemy UI 配下に `IconListView` 用の空オブジェクト（例: `EnemyStatusIcons`）を作成
   - `Horizontal Layout Group` を付けると自動で横に並びます
2. `IconListView` コンポーネントを追加し、アイコン親 Transform などを設定
3. `EnemyView` の **`敵ステータス効果アイコン`** に割り当てる

---

## 4. サウンド再生（BattleAudioPlayer）

1. Hierarchy で `BattleSceneController` が付いている GameObject を選択
2. **`BattleAudioPlayer`** コンポーネントを追加（`AudioSource` が自動で付きます）
3. `BattleSceneController` の Inspector で
   - **`サウンド再生`** ← この `BattleAudioPlayer` を割り当てる

---

## 5. エフェクト再生（BattleEffectPlayer）

1. 同じく Controller の GameObject に **`BattleEffectPlayer`** コンポーネントを追加
2. アンカー用の空オブジェクトを3つ用意して割り当てる
   - `プレイヤーアンカー` ← プレイヤー画像付近の Transform
   - `敵アンカー` ← 敵画像付近の Transform
   - `中央アンカー` ← 画面中央の Transform
3. `BattleSceneController` の Inspector で
   - **`エフェクト再生`** ← この `BattleEffectPlayer` を割り当てる

> エフェクトの表示位置は `EnemyActionEntry` の「エフェクト表示位置」で指定します。
> Self=敵自身 / Opponent=プレイヤー / Center=画面中央 に対応します。

---

## 6. ダイスロール演出（Miyahara 物理ダイスを使用する場合）

現在は Miyahara 側の物理ダイス(RandomDice ×3) を直接使用する演出に移行しています。
旧来の `BattleDiceRollAnimator` は不要になったため削除しました。

Miyahara の物理ダイスを表示する手順（簡易）:

1. ダイス用レイヤーを作成（例: `DiceRollFX`）。
2. DiceRig（RandomDice ×3）を作成し、子オブジェクトをすべて `DiceRollFX` に設定。
3. 演出用カメラ (`DiceRollCamera`) を作成し、`Culling Mask` を `DiceRollFX` のみ有効にする。
4. RenderTexture を作成し、`DiceRollCamera.Target Texture` に割り当てる。
5. 最前面 Canvas に RawImage を作成し、`Texture` に RenderTexture を割り当てる。
6. 新規コンポーネント `MiyaharaDiceRollBridge` を作成して Controller に割り当てる。
   - `MiyaharaDiceRollBridge` の `RandomDice` に場面内の RandomDice（×3）を割り当てる
   - `m_waitExternalRollResult` を `true` にしておくと、Battle システムがフェーズ開始時に ExternalRollRequested を発火し、Bridge が自動でダイスを転がします
   - RawImage は自動的に raycastTarget=false にされ、UI の操作を阻害しません

備考: シーンに残る `BattleDiceRollAnimator` の参照は Unity Editor 上で削除してください。

### 6-1. 演出用レイヤーの作成

1. メニュー `Edit > Project Settings > Tags and Layers`
2. 空いている User Layer に **`DiceRollFX`** を追加

### 6-2. RenderTexture の作成

1. Project ビュー右クリック → `Create > Render Texture`
2. 名前: `RT_DiceRoll`、サイズ: `1024 x 1024` 程度（必要に応じて調整）

### 6-3. ダイスリグの配置

1. Hierarchy に空オブジェクト `DiceRollRig` を作成（位置はメインカメラに映らない遠い場所、例: `(1000, 0, 0)`）
2. その子に Miyahara のダイス Prefab（`DiceItem` 付き）を **3個** 配置し、横に並べる
3. `DiceRollRig` 以下すべての Layer を **`DiceRollFX`** に変更（子も含む）

### 6-4. 演出カメラの作成

1. Hierarchy に新しい Camera を作成、名前: `DiceRollCamera`
2. `DiceRollRig` のダイス3個が映る位置・角度に配置する
3. Inspector で以下を設定
   - `Clear Flags`: `Solid Color`、背景色のアルファを 0 に（透過背景）
   - `Culling Mask`: **`DiceRollFX` のみ** にする
   - `Target Texture`: `RT_DiceRoll`
4. **メインカメラ**の `Culling Mask` から `DiceRollFX` を**外す**（演出が二重に映らないように）

### 6-5. 最前面 Canvas の作成

1. Hierarchy に新しい Canvas を作成、名前: `DiceRollCanvas`
2. Inspector で以下を設定
   - `Render Mode`: `Screen Space - Overlay`
   - **`Sort Order`: `100`**（既存UIより大きい値にする → これで一番手前に表示されます）
3. 子に `UI > Raw Image` を作成、名前: `DiceRollImage`
   - `Texture` ← `RT_DiceRoll`
   - 画面中央に適度な大きさで配置（全画面でも可）

### 6-6. BattleDiceRollAnimator の配線

1. Controller の GameObject に **`BattleDiceRollAnimator`** コンポーネントを追加
2. Inspector で以下を割り当てる
   - `演出カメラ` ← `DiceRollCamera`
   - `最前面 Canvas` ← `DiceRollCanvas`
   - `Canvas の RawImage` ← `DiceRollImage`
   - `ダイスリグ（3つ）` ← `DiceRollRig` 配下の `DiceItem` ×3
   - `スピン演出時間` ← `1.5`（お好み）
   - `結果表示時間` ← `1.0`（お好み）
3. `BattleSceneController` の Inspector で
   - **`ダイスロール演出`** ← この `BattleDiceRollAnimator` を割り当てる

> `BattleDiceRollAnimator` 未割り当てでも動作します（演出スキップで即結果表示）。

---

## 7. ボタンSE（ButtonSoundPlayer / ButtonSoundHandler）

ボタンのホバー音・クリック音を一括で設定できます。

1. Hierarchy でメインの Canvas（戦闘UIのルート）を選択
2. **`ButtonSoundPlayer`** コンポーネントを追加（`AudioSource` が自動で付きます）
3. Inspector で以下を設定
   - **`ホバーSE`** ← カーソルが重なった時の AudioClip
   - **`クリックSE`** ← クリックした時の AudioClip
   - **`音量（0～1）`** ← お好み
   - **`起動時に子Buttonへ自動登録`** ← ON のまま（子階層の全 Button に自動適用）

> 非 interactable のボタンでは音は鳴りません。
> 実行中に動的生成したボタンに適用したい場合は `RegisterButtonsInChildren()` を再度呼んでください。

---

## 8. ダイスロールSE / Player行動SE

`BattleSceneController` の Inspector で以下を設定します（いずれも任意、`サウンド再生` に `BattleAudioPlayer` の割り当てが必要）。

- **`ダイスロールSE`** ← Roll 実行時に再生する AudioClip
- **`ダイスロールSE音量（0～1）`** ← お好み
- **`Player攻撃SE`** ← プレイヤーの攻撃値が 1 以上のターンに再生
- **`Player防御SE`** ← プレイヤーの防御（Block獲得）値が 1 以上のターンに再生
- **`Player行動SE音量（0～1）`** ← お好み

> 攻撃と防御が両方発生したターンは両方のSEが再生されます。
> 敵行動のSEは従来どおり `EnemyActionEntry` 側で設定します。

---

## 9. Enemy の複数Visual（アイドルアニメ＋行動時画像）

敵の見た目を複数の Texture で動かせます。

1. `EnemyDefinition` アセットを選択
2. Inspector で以下を設定
   - **`アイドルアニメ用Texture（複数）`** ← 待機中に表示する Texture を順番に追加
     - 例: 3枚設定すると `1→2→3→2→1→2→3→2...` の往復ループで再生されます
   - **`行動時Texture`** ← 攻撃・防御など行動した瞬間に表示する Texture（4枚目の画像）
   - **`アイドルアニメ間隔（秒）`** ← 1コマあたりの表示時間（既定 0.4 秒）

> `アイドルアニメ用Texture` が**空**の場合は、従来どおり `表示Sprite` / `表示Texture` の単一画像表示になります。
> `行動時Texture` 未設定の場合は、行動時もアイドルアニメを継続します。
> 行動時画像は「敵行動の間隔（秒）」の時間だけ表示され、その後アイドルアニメに自動復帰します。

---

## 10. 動作確認チェックリスト

- [ ] 戦闘開始時、Enemy の頭上に行動予測アイコンが表示される（HP帯の行動に応じて変化）
- [ ] ダイス3個選択 → Roll でロール演出が最前面に表示される
- [ ] 演出終了後にダイススロットへ出目が反映され、ダメージが適用される
- [ ] プレイヤーの防御値が `Block: n` として表示される
- [ ] 敵が防御行動をした場合、敵の Block が表示され、次のプレイヤー攻撃で削られる
- [ ] 敵のHPが減ってHP帯が変わると、行動パターン（予測アイコン）が変わる
- [ ] 敵がバフ/デバフ行動をすると、対象側にステータスアイコンが表示される
- [ ] バフ/デバフが持続ターン経過で消える
- [ ] エフェクト/サウンドを設定した行動で再生される
- [ ] ボタンにカーソルを重ねた時／クリックした時にSEが鳴る
- [ ] Roll 実行時にダイスロールSEが鳴る
- [ ] プレイヤーの攻撃／防御時にSEが鳴る
- [ ] 敵が待機中にアイドルアニメ（往復ループ）で動く
- [ ] 敵が行動した瞬間に行動時画像（4枚目）が表示され、その後アイドルに戻る
