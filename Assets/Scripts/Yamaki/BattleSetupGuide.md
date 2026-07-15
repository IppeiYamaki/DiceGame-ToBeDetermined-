# Yamaki 戦闘セットアップガイド

このガイドは、Yamaki 配下に追加した 3ダイス選択・ロール・Enemy Visual 表示を Unity 上で接続する手順です。

## 1. 敵データを作成する

1. Project ビューで右クリックします。
2. `Create > DiceGame > EnemyDefinition` を選択します。
3. 作成した EnemyDefinition に以下を設定します。
   - `敵名`: 戦闘画面に表示したい名前
   - `最大HP`: 敵のHP
   - `基礎攻撃力`: 行動パターン未設定時の敵ダメージ
   - `表示Sprite`: Enemy Image に表示したい Sprite
   - `表示Texture`: Sprite がない場合の Texture2D
   - `行動パターン`: 任意。未設定でも基礎攻撃力で行動します。

## 2. DiceMasterRegistry を設定する

1. Project ビューで `Create > DiceGame > DiceMasterRegistry` から Registry を作成します。
2. `ダイス定義（マスタ）` に使用する DiceDefinition を登録します。
3. `役定義（マスタ）` に DiceRoleDefinition を登録します。
4. 戦闘Controllerの `ダイスマスタ` にこの Registry を設定します。

## 3. Canvas に戦闘UIを作る

Canvas 配下に以下を用意します。

- 敵画像用 `Image`
- Player HP 表示用 `TextMeshPro - Text`
- Enemy HP 表示用 `TextMeshPro - Text`
- Phase 表示用 `TextMeshPro - Text`
- Roll Result 表示用 `TextMeshPro - Text`
- Battle Log 表示用 `TextMeshPro - Text`
- ダイス選択用 Panel
- ダイス選択完了 Button
- ダイス6個分の Slot UI

## 4. ダイスSlot UIを設定する

1. ダイス1個分のUI GameObjectを作ります。
2. その GameObject に `YamakiDiceSlotView` を追加します。
3. Inspector で以下を設定します。
   - `ダイス名テキスト`: ダイス名を表示する TMP_Text
   - `出目テキスト`: ロール結果の数字を表示する TMP_Text
   - `選択中表示`: 選択中だけ表示する GameObject
   - `クリックボタン`: Slot をクリックする Button
4. 同じ構成の Slot を6個用意します。

## 5. ダイス選択Viewを設定する

1. ダイス選択Panelまたは管理用GameObjectに `YamakiDiceSelectionView` を追加します。
2. Inspector で以下を設定します。
   - `選択パネル`: ダイス選択Panel
   - `ダイススロット`: 6個の `YamakiDiceSlotView`
   - `決定ボタン`: 3個選択後に押す Button
   - `選択数テキスト`: `選択数: 0/3` を表示する TMP_Text

## 6. 戦闘Controllerを設定する

1. シーン上に空の GameObject を作り、名前を `YamakiBattleController` などにします。
2. `YamakiBattleController` コンポーネントを追加します。
3. Inspector で以下を設定します。
   - `ダイスマスタ`: DiceMasterRegistry
   - `プレイヤー所持ダイス（6個）`: 戦闘で表示する DiceDefinition を6個
   - `敵データ`: EnemyDefinition
   - `プレイヤー最大HP`: 任意の値
   - `ダイス選択View`: YamakiDiceSelectionView
   - `敵画像`: 敵を表示する Image
   - `HP表示`: Player HP / Enemy HP の TMP_Text
   - `フェーズ表示`: 現在フェーズ用 TMP_Text
   - `ロール結果表示`: 出目・役・Attack・Defense 表示用 TMP_Text
   - `戦闘ログ`: 戦闘ログ用 TMP_Text

## 7. 再生して確認する

1. Play ボタンを押します。
2. 敵画像が表示されることを確認します。
3. ダイス選択Panelに6個のダイスが表示されることを確認します。
4. 3個のダイスをクリックして選択します。
5. 3個選択すると決定ボタンが押せるようになります。
6. 決定ボタンを押すとロールされます。
7. ロール結果に以下が表示されます。
   - 3個の出目
   - 成立役と倍率
   - Attack
   - Defense
8. Attack 分だけ敵HPが減ります。
9. 敵行動でプレイヤーHPが減ります。直前の Defense が敵ダメージを軽減します。
10. 敵行動後、勝敗が未決定なら再びダイス選択フェーズに戻ります。

## 注意

- `プレイヤー所持ダイス（6個）` には必ず DiceDefinition を6個設定してください。
- DiceDefinition は DiceMasterRegistry の `ダイス定義（マスタ）` にも登録してください。
- DiceRoleDefinition を登録しない場合、役は成立せず倍率は 1.0 として扱われます。
- EnemyDefinition の `表示Sprite` がある場合は Sprite が優先されます。
- `表示Sprite` が未設定で `表示Texture` がある場合、実行時に Sprite に変換して表示します。
