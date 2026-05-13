using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DiceTriggerCheck : MonoBehaviour
{
    // Collider型の引数を持つUnityEventを定義
    public UnityEvent<Collider> onColliderStay;
    

    // コライダーが他のオブジェクトと接触した際に呼ばれるメソッド
    private void OnTriggerStay(Collider other)
    {
        //if (Keyboard.current.dKey.wasPressedThisFrame)
        if (other.gameObject.tag == "Field")
        {
            // UnityEventがnullでない場合、イベントを発生させる
            onColliderStay?.Invoke(other);
        }
    }
}
