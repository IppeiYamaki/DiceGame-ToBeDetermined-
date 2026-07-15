using System.Collections;
using UnityEngine;

public class DiceVisualRoller : MonoBehaviour
{
    [Header("回転させるダイス")]
    public Transform dice1;
    public Transform dice2;
    public Transform dice3;

    [Header("回転設定")]
    public float rollTime = 1.0f;
    public float rotateSpeed = 720f;

    [Header("出目ごとの最終角度")]
    public Vector3 face1Rotation;
    public Vector3 face2Rotation;
    public Vector3 face3Rotation;
    public Vector3 face4Rotation;
    public Vector3 face5Rotation;
    public Vector3 face6Rotation;

    [Header("ダイスの面表示")]
    public DiceFaceView diceFaceView1;
    public DiceFaceView diceFaceView2;
    public DiceFaceView diceFaceView3;


    private bool isRolling = false;

    public IEnumerator RollAnimation(int result1, int result2, int result3)
    {
        if (isRolling)
        {
            yield break;
        }

        isRolling = true;

        float timer = 0f;

        while (timer < rollTime)
        {
            RotateDice(dice1);
            RotateDice(dice2);
            RotateDice(dice3);

            timer += Time.deltaTime;
            yield return null;
        }

        ResetDiceRotation(dice1);
        ResetDiceRotation(dice2);
        ResetDiceRotation(dice3);

        if (diceFaceView1 != null)
        {
            diceFaceView1.SetFace(result1);
        }

        if (diceFaceView2 != null)
        {
            diceFaceView2.SetFace(result2);
        }

        if (diceFaceView3 != null)
        {
            diceFaceView3.SetFace(result3);
        }

        isRolling = false;
    }

    void ResetDiceRotation(Transform dice)
    {
        if (dice == null)
        {
            return;
        }

        // ダイスを正面向きに戻す
        dice.localEulerAngles = Vector3.zero;
    }

    void RotateDice(Transform dice)
    {
        if (dice == null)
        {
            return;
        }

        dice.Rotate(
            rotateSpeed * Time.deltaTime,
            rotateSpeed * Time.deltaTime,
            rotateSpeed * Time.deltaTime,
            Space.Self
        );
    }

    void SetDiceResult(Transform dice, int result)
    {
        if (dice == null)
        {
            return;
        }

        dice.localEulerAngles = GetFaceRotation(result);
    }

    Vector3 GetFaceRotation(int result)
    {
        switch (result)
        {
            case 1:
                return face1Rotation;
            case 2:
                return face2Rotation;
            case 3:
                return face3Rotation;
            case 4:
                return face4Rotation;
            case 5:
                return face5Rotation;
            case 6:
                return face6Rotation;
            default:
                return Vector3.zero;
        }
    }
}