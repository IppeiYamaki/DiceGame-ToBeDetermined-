using UnityEngine;
using UnityEngine.Rendering;

public class Item_DataMaster : MonoBehaviour
{
    public static Item_DataMaster Instance;

    public struct Item
    {
        public int id;
        public string name;
        public int count;
    }

 
    private static int itemSuu = 5;//アイテムの種類数をここに入れる


    //[SerializeField]
    //public Item[] Items2 = new Item[itemSuu];


    public Item[] Items = new Item[itemSuu];

    


    //アイテム取得時に呼び出す
    public void AddItemCount(int id)
    {
        //無効値なら処理しない
        if (id < 0 || id > Items.Length - 1)
        {
            Debug.LogWarning("無効値です");
            return;
        }

        Items[id].count++;
        //for (int i = 0; i < Items.Length; i++)
        //{
        //    Debug.Log("Items[" + i + "]のアイテム数＝" + Items[i].count);
        //}
    }

    //アイテム使用時に呼び出す
    public void UseItem(int id)
    {
        //無効値なら処理しない
        if (id < 0 || id > Items.Length - 1)
        {
            Debug.LogWarning("無効値です");
            return;
        }

        Items[id].count--;
        switch (id)
        {
            case 0:
                //ここでアイテムの効果処理を行う
                break;
            case 1:
                //ここでアイテムの効果処理を行う
                break;
            case 2:
                //ここでアイテムの効果処理を行う
                break;
            case 3:
                //ここでアイテムの効果処理を行う
                break;
            case 4:
                //ここでアイテムの効果処理を行う
                break;
        }

       //for (int i = 0; i < Items.Length; i++)
       //{
       //    Debug.Log("Items[" + i + "]のアイテム数＝" + Items[i].count);
       //}
    }

    //使えるかどうかのチェック
    public bool CheckUseItem(int id)
    {
        //無効値ならfalseを返す
        if (id < 0 || id > Items.Length -1)
        {
            Debug.LogWarning("無効値です");
            return false;
        }

        //アイテムの個数をチェック。0個ならfalseを返す
        if (Items[id].count <= 0)
        {
            return false;
        }


        bool check = false;
        switch(id)
        { 
            case 0:
                //ここで使えるか確認（使えるならcheckをtrueにする）
                check = true;
                break;
            case 1:
                //ここで使えるか確認
                check = true;
                break;
            case 2:
                //ここで使えるか確認
                check = true;
                break;
            case 3:
                //ここで使えるか確認
                check = true;
                break;
            case 4:
                //ここで使えるか確認
                check = true;
                break;

        }
        return check;
    }



    public int GetItemCount(int id)
    {
        //無効値なら-1を返す
        if (id < 0 || id > Items.Length - 1)
        {
            Debug.LogWarning("無効値です");
            return -1;
        }
        return Items[id].count;
    }



    public void ResetItems()
    {
        for(int i = 0; i < itemSuu; i++)
        {
            Items[i].count = 0;
        }
    }

  

    private void Awake()
    {

        // すでに存在する場合は重複生成を防ぐ
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Debug.Log("Item_DataMasterのAwake()");
        for (int i = 0; i < itemSuu; i++)
        {
            Items[i].id = i;
            Items[i].count = i+1;//テスト。本番時はi+1を0にする

            switch (i)
            {
                case 0:
                    Items[i].name = "テスト１";
                    break;
                case 1:
                    Items[i].name = "テスト２";
                    break;
                case 2:
                    Items[i].name = "テスト３";
                    break;
                case 3:
                    Items[i].name = "テスト４";
                    break;
                case 4:
                    Items[i].name = "テスト５";
                    break;
            }

            Debug.Log("Items[" + i + "]のアイテム数＝" + Items[i].count);
        }

        DontDestroyOnLoad(gameObject);
    }
}
